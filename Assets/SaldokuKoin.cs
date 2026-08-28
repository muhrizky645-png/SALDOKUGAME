using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
#if SALDOKU_ADMOB
using GoogleMobileAds.Api;
#endif

// =====================================================================
//  SALDOKU LAST STAND x SALDOKU - HUBUNGKAN AKUN, SINKRON KOIN, PETI IKLAN
//  Port dari sistem Tetris3D (Saldoku/PetiKoin), disesuaikan ke gaya UI
//  SALDOKUGAME (Tema) dan mata uang MataUang.
//
//  Alur:
//   1. User login app SALDOKU -> dapat KODE.
//   2. Masukkan KODE di game -> game_link_verify.php -> game_token.
//   3. GET poin_game_status_apk.php (Bearer token) -> koin + progress peti.
//  KOIN READ-ONLY: hanya server yang menambah (via SSV iklan berhadiah).
//
//  CATATAN: aktifkan iklan dengan Scripting Define Symbol: SALDOKU_ADMOB
//  (butuh Google Mobile Ads SDK). Ganti AD_UNIT sesuai SALDOKUGAME.
//  Backend saldoku.site juga harus mengenali game ini (identitas game / SSV).
// =====================================================================
[DefaultExecutionOrder(-30000)]
public class Saldoku : MonoBehaviour
{
    public static Saldoku Instance;

    const string BASE       = "https://saldoku.site";
    const string PP_TOKEN   = "saldoku_game_token";
    const string PP_NAMA    = "saldoku_nama";
    const string PP_JULUKAN = "saldoku_julukan";

    string token, nama, julukan;
    bool   terbuka;
    string kode = "";
    string statusPesan = "";
    bool   sibuk;

    int petiProgress, iklanPerPeti = 5, poinPerPeti = 1000;
    int iklanHariIni, batasHarian = 20, sisaIklan;

    bool petiSibuk;
    string petiPesan = "";
    bool autoDone;

    public bool Terbuka { get { return terbuka; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }
    static void Buat() { if (Instance == null) new GameObject("Saldoku", typeof(Saldoku)); }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        token   = PlayerPrefs.GetString(PP_TOKEN, "");
        nama    = PlayerPrefs.GetString(PP_NAMA, "");
        julukan = PlayerPrefs.GetString(PP_JULUKAN, "");
    }

    void Update()
    {
        if (!autoDone) { autoDone = true; if (!string.IsNullOrEmpty(token)) StartCoroutine(CoRefresh(true)); }
    }

    public void Buka()
    {
        terbuka = true; statusPesan = "";
        if (!(MataUang.Instance != null && MataUang.Instance.Terhubung)) kode = "";
    }
    public void Tutup() { terbuka = false; }

    string NamaTampil()
    {
        if (!string.IsNullOrEmpty(julukan)) return julukan;
        if (!string.IsNullOrEmpty(nama)) return nama;
        return "SALDOKU";
    }

    void KirimKode()
    {
        if (sibuk) return;
        string k = (kode ?? "").Trim().ToUpperInvariant();
        if (k.Length < 4) { statusPesan = "Masukkan kode dulu."; return; }
        StartCoroutine(CoLink(k));
    }

    void SegarkanSekarang()
    {
        if (string.IsNullOrEmpty(token)) { statusPesan = "Belum terhubung."; return; }
        StartCoroutine(CoRefresh(false));
    }

    void SimpanJulukan()
    {
        julukan = (julukan ?? "").Trim();
        PlayerPrefs.SetString(PP_JULUKAN, julukan);
        PlayerPrefs.Save();
        statusPesan = "Julukan disimpan.";
    }

    void Putuskan()
    {
        token = ""; nama = "";
        PlayerPrefs.DeleteKey(PP_TOKEN);
        PlayerPrefs.DeleteKey(PP_NAMA);
        if (MataUang.Instance != null) MataUang.Instance.SetKoinDariServer(0, false, false);
        statusPesan = ""; kode = "";
    }

    IEnumerator CoLink(string k)
    {
        sibuk = true; statusPesan = "Menghubungkan...";
        string url = BASE + "/game_link_verify.php";
        string json = JsonUtility.ToJson(new SalVerifyReq { kode = k, device = SystemInfo.deviceUniqueIdentifier });
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        using (var req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = 20;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                statusPesan = "Gagal terhubung ke server."; sibuk = false; yield break;
            }

            SalVerifyResp resp = null;
            try { resp = JsonUtility.FromJson<SalVerifyResp>(req.downloadHandler.text); } catch { resp = null; }

            if (resp == null || !resp.status || resp.data == null || string.IsNullOrEmpty(resp.data.game_token))
            {
                statusPesan = (resp != null && !string.IsNullOrEmpty(resp.message)) ? resp.message : "Kode tidak valid / kadaluarsa.";
                sibuk = false; yield break;
            }

            token = resp.data.game_token;
            nama = resp.data.nama ?? "";
            PlayerPrefs.SetString(PP_TOKEN, token);
            PlayerPrefs.SetString(PP_NAMA, nama);
            PlayerPrefs.Save();
            statusPesan = "Berhasil! Memuat saldo...";
        }

        yield return StartCoroutine(CoRefresh(false));
        sibuk = false;
        if (MataUang.Instance != null && MataUang.Instance.Terhubung) { statusPesan = "Akun terhubung."; terbuka = false; }
    }

    IEnumerator CoRefresh(bool silent)
    {
        if (string.IsNullOrEmpty(token)) yield break;
        if (!silent) statusPesan = "Memuat saldo...";

        string url = BASE + "/poin_game_status_apk.php";
        using (var req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Authorization", "Bearer " + token);
            req.timeout = 20;
            yield return req.SendWebRequest();

            long httpCode = req.responseCode;
            string text = (req.downloadHandler != null) ? req.downloadHandler.text : "";

            if (req.result != UnityWebRequest.Result.Success)
            {
                if (httpCode == 401) { Putuskan(); if (!silent) statusPesan = "Sesi berakhir. Hubungkan ulang."; }
                else
                {
                    long simpan = (MataUang.Instance != null) ? MataUang.Instance.Koin : 0;
                    if (MataUang.Instance != null) MataUang.Instance.SetKoinDariServer(simpan, false, true);
                    if (!silent) statusPesan = "Offline. Pakai saldo tersimpan.";
                }
                yield break;
            }

            SalStatusResp resp = null;
            try { resp = JsonUtility.FromJson<SalStatusResp>(text); } catch { resp = null; }

            if (resp == null || !resp.status || resp.data == null)
            {
                if (httpCode == 401) { Putuskan(); if (!silent) statusPesan = "Sesi berakhir. Hubungkan ulang."; }
                yield break;
            }

            SalStatusData d = resp.data;
            petiProgress = d.peti_progress;
            iklanHariIni = d.iklan_hari_ini;
            sisaIklan    = d.sisa_iklan;
            if (d.iklan_per_peti > 0) iklanPerPeti = d.iklan_per_peti;
            if (d.poin_per_peti  > 0) poinPerPeti  = d.poin_per_peti;
            if (d.batas_harian   > 0) batasHarian  = d.batas_harian;
            if (!string.IsNullOrEmpty(d.nama)) { nama = d.nama; PlayerPrefs.SetString(PP_NAMA, nama); }

            if (MataUang.Instance != null) MataUang.Instance.SetKoinDariServer(d.koin, true, true);
            if (!silent) statusPesan = "";
        }
    }

    void TontonIklanPeti()
    {
        if (petiSibuk) return;
        if (string.IsNullOrEmpty(token) || MataUang.Instance == null || !MataUang.Instance.Terhubung) { petiPesan = "Belum terhubung."; return; }
        if (batasHarian > 0 && sisaIklan <= 0) { petiPesan = "Batas iklan harian tercapai."; return; }
        petiPesan = "";
        IklanKoin.Instance.TampilkanPeti(this, token);
    }

    public void SetPetiSibuk(bool b) { petiSibuk = b; }

    public void OnPetiReward()
    {
        petiPesan = "Iklan selesai. Menambahkan Koin...";
        StartCoroutine(CoSetelahPeti());
    }

    IEnumerator CoSetelahPeti()
    {
        yield return new WaitForSeconds(2.5f);
        yield return StartCoroutine(CoRefresh(false));
        petiPesan = "Koin & peti diperbarui!";
    }

    public void OnPetiGagal(string msg) { petiSibuk = false; petiPesan = msg; }
    public string PesanIklanTakSiap() { return "Iklan belum siap. Coba lagi."; }
    public string PesanIklanMati()   { return "Fitur iklan belum aktif di build ini."; }

    void OnGUI()
    {
        if (!terbuka) return;

        // === PENTING: panel akun HARUS tampil paling DEPAN. ===
        // Menu utama (GameMenu) menggambar di depth default 0 dan dieksekusi SETELAH
        // skrip ini (Saldoku pakai DefaultExecutionOrder -30000), jadi tanpa ini panel
        // akun tertimpa menu utama. Di IMGUI, depth lebih KECIL digambar paling depan.
        GUI.depth = -1000;

        float w = Screen.width, h = Screen.height;
        // Basis ukuran font = SISI TERPENDEK layar. Di HP potrait itu = lebar, jadi teks
        // ikut lebar panel dan TIDAK meluber/kepotong (dulu pakai tinggi -> kegedean).
        float u = Tema.Unit;
        bool linked = MataUang.Instance != null && MataUang.Instance.Terhubung;

        // Latar gelap layar penuh + semburat hijau tipis biar menyatu tema.
        Tema.LatarGelap(new Color(0.03f, 0.09f, 0.04f, 0.55f));

        // ---- Ukuran panel (lebih lebar supaya teks muat 1 baris) ----
        float pw = Mathf.Min(w * 0.92f, 900f);
        float ph = linked ? Mathf.Min(h * 0.92f, u * 1.12f) : Mathf.Min(h * 0.90f, u * 0.98f);
        float px = (w - pw) / 2f, py = (h - ph) / 2f;

        // Bayangan halus di belakang panel biar terlihat "mengambang".
        Tema.Kotak(new Rect(px + 7f, py + 9f, pw, ph), new Color(0f, 0f, 0f, 0.38f));

        // Panel utama + garis tepi army.
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Panel, Tema.Garis, 3f);

        // ---- HEADER: bar aksen + judul ----
        float headH = u * 0.13f;
        Tema.Kotak(new Rect(px + 3f, py + 3f, pw - 6f, headH), new Color(0.16f, 0.19f, 0.12f, 0.98f));
        Tema.Kotak(new Rect(px, py, pw, 5f), Tema.Army);                          // strip aksen paling atas
        Tema.Kotak(new Rect(px + 3f, py + headH, pw - 6f, 2f), Tema.GarisRedup);  // garis pemisah header
        Tema.Teks(new Rect(px, py, pw, headH), "AKUN SALDOKU",
            Mathf.RoundToInt(u * 0.052f), Tema.Army, TextAnchor.MiddleCenter, true);

        float cx = px + pw * 0.07f, cw = pw * 0.86f;
        float yy = py + headH + u * 0.04f;

        if (!linked)
        {
            // ---- Kotak instruksi ----
            float insH = u * 0.30f;
            Tema.Panel9(new Rect(cx, yy, cw, insH), Tema.Plate, Tema.GarisRedup, 2f);
            int insF = Mathf.RoundToInt(u * 0.032f);
            string[] langkah = {
                "1.   Buka aplikasi SALDOKU",
                "2.   Menu Game  >  Hubungkan",
                "3.   Salin KODE",
                "4.   Masukkan kode di bawah",
            };
            float lineH = u * 0.062f;
            float ix = cx + cw *