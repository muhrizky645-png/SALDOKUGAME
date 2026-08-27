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
        bool linked = MataUang.Instance != null && MataUang.Instance.Terhubung;

        // Latar gelap layar penuh + semburat hijau tipis biar menyatu tema.
        Tema.LatarGelap(new Color(0.03f, 0.09f, 0.04f, 0.55f));

        // ---- Ukuran panel (dibatasi supaya rapi di layar besar) ----
        float pw = Mathf.Min(w * 0.88f, 620f);
        float ph = Mathf.Min(h * (linked ? 0.88f : 0.64f), linked ? 920f : 640f);
        float px = (w - pw) / 2f, py = (h - ph) / 2f;

        // Bayangan halus di belakang panel biar terlihat "mengambang".
        Tema.Kotak(new Rect(px + 7f, py + 9f, pw, ph), new Color(0f, 0f, 0f, 0.38f));

        // Panel utama + garis tepi army.
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Panel, Tema.Garis, 3f);

        // ---- HEADER: bar aksen + judul ----
        float headH = Mathf.Min(ph * 0.14f, h * 0.09f);
        Tema.Kotak(new Rect(px + 3f, py + 3f, pw - 6f, headH), new Color(0.16f, 0.19f, 0.12f, 0.98f));
        Tema.Kotak(new Rect(px, py, pw, 5f), Tema.Army);                          // strip aksen paling atas
        Tema.Kotak(new Rect(px + 3f, py + headH, pw - 6f, 2f), Tema.GarisRedup);  // garis pemisah header
        Tema.Teks(new Rect(px, py, pw, headH), "AKUN SALDOKU",
            Mathf.RoundToInt(headH * 0.42f), Tema.Army, TextAnchor.MiddleCenter, true);

        float cx = px + pw * 0.07f, cw = pw * 0.86f;
        float yy = py + headH + ph * 0.035f;

        if (!linked)
        {
            // ---- Kotak instruksi ----
            float insH = ph * 0.34f;
            Tema.Panel9(new Rect(cx, yy, cw, insH), Tema.Plate, Tema.GarisRedup, 2f);
            int insF = Mathf.RoundToInt(h * 0.022f);
            string[] langkah = {
                "1.   Buka aplikasi SALDOKU",
                "2.   Menu Game  >  Hubungkan",
                "3.   Salin KODE",
                "4.   Masukkan kode di bawah",
            };
            float lineH = insH / (langkah.Length + 0.6f);
            float ix = cx + cw * 0.07f, iw = cw * 0.86f;
            float iy = yy + insH * 0.08f;
            for (int i = 0; i < langkah.Length; i++)
                Tema.Teks(new Rect(ix, iy + lineH * i, iw, lineH), langkah[i], insF,
                    Tema.Tulang, TextAnchor.MiddleLeft, false);
            yy += insH + ph * 0.05f;

            // ---- Label KODE TAUTAN ----
            Tema.Teks(new Rect(cx, yy, cw, h * 0.03f), "KODE TAUTAN", Mathf.RoundToInt(h * 0.02f),
                Tema.Amber, TextAnchor.MiddleLeft, true);
            yy += h * 0.036f;

            // ---- Field kode (plate gelap + border army biar kontras) ----
            float fieldH = h * 0.075f;
            Tema.Panel9(new Rect(cx, yy, cw, fieldH), new Color(0.03f, 0.05f, 0.03f, 0.96f), Tema.Garis, 2f);
            GUIStyle tf = new GUIStyle(GUI.skin.textField);
            tf.font = Tema.FontUtama;
            tf.fontSize = Mathf.RoundToInt(h * 0.036f);
            tf.fontStyle = FontStyle.Bold;
            tf.alignment = TextAnchor.MiddleCenter;
            tf.normal.textColor = Tema.Tulang;
            tf.focused.textColor = Tema.Tulang;
            tf.normal.background = null;
            tf.focused.background = null;
            tf.active.background = null;
            tf.hover.background = null;
            GUI.SetNextControlName("KodeField");
            string typed = GUI.TextField(new Rect(cx + 8f, yy, cw - 16f, fieldH), kode ?? "", 8, tf);
            kode = typed.ToUpperInvariant();
            yy += fieldH + h * 0.018f;

            // ---- Status ----
            if (!string.IsNullOrEmpty(statusPesan))
                Tema.Teks(new Rect(cx, yy, cw, h * 0.03f), statusPesan, Mathf.RoundToInt(h * 0.02f),
                    Tema.Amber, TextAnchor.MiddleCenter, true);
            yy += h * 0.045f;

            // ---- Tombol HUBUNGKAN | TUTUP ----
            float gap = w * 0.025f;
            float bw = (cw - gap) / 2f;
            float bh = h * 0.078f;
            if (GUI.Button(new Rect(cx, yy, bw, bh), sibuk ? "..." : "HUBUNGKAN", Tema.GayaTombol(Mathf.RoundToInt(h * 0.026f))) && !sibuk)
                KirimKode();
            if (GUI.Button(new Rect(cx + bw + gap, yy, bw, bh), "TUTUP", Tema.GayaTombol(Mathf.RoundToInt(h * 0.026f))))
                Tutup();
        }
        else
        {
            // ---- Status terhubung ----
            Tema.Teks(new Rect(cx, yy, cw, h * 0.035f), "Terhubung: " + NamaTampil(),
                Mathf.RoundToInt(h * 0.026f), Tema.Army, TextAnchor.MiddleLeft, true);
            yy += h * 0.055f;

            Tema.Teks(new Rect(cx, yy, cw, h * 0.028f), "Julukan (nama tampilan):", Mathf.RoundToInt(h * 0.019f),
                Tema.Redup, TextAnchor.UpperLeft, false);
            yy += h * 0.034f;
            GUIStyle jf = new GUIStyle(GUI.skin.textField);
            jf.font = Tema.FontUtama;
            jf.fontSize = Mathf.RoundToInt(h * 0.026f);
            jf.alignment = TextAnchor.MiddleLeft;
            jf.normal.textColor = Tema.Tulang;
            jf.focused.textColor = Tema.Tulang;
            float jbw = pw * 0.26f;
            float jtw = cw - jbw - w * 0.02f;
            Tema.Panel9(new Rect(cx, yy, jtw, h * 0.06f), new Color(0.03f, 0.05f, 0.03f, 0.96f), Tema.GarisRedup, 2f);
            GUI.SetNextControlName("JulukanField");
            julukan = GUI.TextField(new Rect(cx + 8f, yy, jtw - 16f, h * 0.06f), julukan ?? "", 16, jf);
            if (GUI.Button(new Rect(cx + jtw + w * 0.02f, yy, jbw, h * 0.06f), "SIMPAN", Tema.GayaTombol(Mathf.RoundToInt(h * 0.022f))))
                SimpanJulukan();
            yy += h * 0.085f;

            // ---- Kartu Koin ----
            long koin = (MataUang.Instance != null) ? MataUang.Instance.Koin : 0;
            bool online = MataUang.Instance != null && MataUang.Instance.Online;
            float koinH = h * 0.09f;
            Tema.Panel9(new Rect(cx, yy, cw, koinH), Tema.Plate, Tema.GarisRedup, 2f);
            Tema.Teks(new Rect(cx + cw * 0.05f, yy + koinH * 0.14f, cw * 0.9f, koinH * 0.34f), "KOIN SALDOKU",
                Mathf.RoundToInt(h * 0.02f), Tema.Redup, TextAnchor.MiddleLeft, true);
            Tema.Teks(new Rect(cx + cw * 0.05f, yy + koinH * 0.44f, cw * 0.9f, koinH * 0.5f),
                MataUang.Ringkas(koin) + (online ? "" : "  (offline)"),
                Mathf.RoundToInt(h * 0.034f), Tema.Amber, TextAnchor.MiddleLeft, true);
            yy += koinH + h * 0.02f;

            Tema.Teks(new Rect(cx, yy, cw, h * 0.03f),
                "Peti: " + petiProgress + "/" + iklanPerPeti + "  (+" + poinPerPeti + " Koin)     Iklan: " + iklanHariIni + "/" + batasHarian,
                Mathf.RoundToInt(h * 0.019f), Tema.Redup, TextAnchor.MiddleLeft, true);
            yy += h * 0.045f;

            if (GUI.Button(new Rect(cx, yy, cw, h * 0.078f),
                    petiSibuk ? "Memuat iklan..." : ("TONTON IKLAN    " + petiProgress + "/" + iklanPerPeti),
                    Tema.GayaTombol(Mathf.RoundToInt(h * 0.026f))) && !petiSibuk)
                TontonIklanPeti();
            yy += h * 0.095f;

            if (!string.IsNullOrEmpty(petiPesan))
            {
                Tema.Teks(new Rect(cx, yy, cw, h * 0.03f), petiPesan, Mathf.RoundToInt(h * 0.019f),
                    Tema.Army, TextAnchor.MiddleLeft, true);
                yy += h * 0.035f;
            }
            if (!string.IsNullOrEmpty(statusPesan))
            {
                Tema.Teks(new Rect(cx, yy, cw, h * 0.03f), statusPesan, Mathf.RoundToInt(h * 0.019f),
                    Tema.Amber, TextAnchor.MiddleLeft, true);
                yy += h * 0.035f;
            }
            yy += h * 0.01f;

            float gap = w * 0.025f;
            float bw = (cw - gap) / 2f;
            if (GUI.Button(new Rect(cx, yy, bw, h * 0.068f), sibuk ? "..." : "SEGARKAN", Tema.GayaTombol(Mathf.RoundToInt(h * 0.024f))) && !sibuk)
                SegarkanSekarang();
            if (GUI.Button(new Rect(cx + bw + gap, yy, bw, h * 0.068f), "PUTUSKAN", Tema.GayaTombol(Mathf.RoundToInt(h * 0.024f))))
                Putuskan();
            yy += h * 0.085f;
            if (GUI.Button(new Rect(cx, yy, cw, h * 0.062f), "TUTUP", Tema.GayaTombol(Mathf.RoundToInt(h * 0.024f))))
                Tutup();
        }

        // Penelan klik di LUAR panel: digambar PALING AKHIR supaya tombol di dalam
        // panel (digambar lebih dulu) tetap menang menerima klik, sedangkan klik di
        // area gelap sekitar panel tidak menembus ke menu di belakang.
        GUI.Button(new Rect(0, 0, w, h), "", GUIStyle.none);
    }
}

// =====================================================================
//  Manajer Rewarded Ad (AdMob) - port KubikaAds. Diselubungi SALDOKU_ADMOB.
// =====================================================================
public class IklanKoin : MonoBehaviour
{
    // TODO: ganti dengan Ad Unit milik SALDOKUGAME sebelum aktifkan iklan.
    const string AD_UNIT_PROD = "ca-app-pub-0000000000000000/0000000000";
    const string AD_UNIT_TEST = "ca-app-pub-3940256099942544/5224354917";
    const bool   USE_TEST_ADS = true;

    static IklanKoin _inst;
    public static IklanKoin Instance
    {
        get
        {
            if (_inst == null)
            {
                var go = new GameObject("IklanKoin");
                DontDestroyOnLoad(go);
                _inst = go.AddComponent<IklanKoin>();
            }
            return _inst;
        }
    }

#if SALDOKU_ADMOB
    RewardedAd _ad;
    bool _init, _wantShow;
    Saldoku _game;
    string _customData;

    void EnsureInit() { if (_init) return; _init = true; MobileAds.Initialize(_ => Load()); }
    string Unit() { return USE_TEST_ADS ? AD_UNIT_TEST : AD_UNIT_PROD; }

    void Load()
    {
        if (_ad != null) { _ad.Destroy(); _ad = null; }
        RewardedAd.Load(Unit(), new AdRequest(), (ad, err) =>
        {
            if (err != null || ad == null)
            {
                if (_wantShow) { _wantShow = false; if (_game != null) _game.OnPetiGagal(_game.PesanIklanTakSiap()); }
                return;
            }
            _ad = ad; Hook(_ad);
            if (_wantShow) { _wantShow = false; DoShow(); }
        });
    }

    void Hook(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => { if (_game != null) _game.SetPetiSibuk(false); Load(); };
        ad.OnAdFullScreenContentFailed += (AdError e) =>
        {
            if (_game != null) { _game.SetPetiSibuk(false); _game.OnPetiGagal(_game.PesanIklanTakSiap()); }
            Load();
        };
    }

    void DoShow()
    {
        if (_ad == null || !_ad.CanShowAd()) { _wantShow = true; Load(); return; }
        if (!string.IsNullOrEmpty(_customData))
        {
            var ssv = new ServerSideVerificationOptions.Builder().SetCustomData(_customData).Build();
            _ad.SetServerSideVerificationOptions(ssv);
        }
        _ad.Show(reward => { if (_game != null) _game.OnPetiReward(); });
    }

    public void TampilkanPeti(Saldoku game, string customData)
    {
        _game = game; _customData = customData;
        game.SetPetiSibuk(true);
        EnsureInit();
        if (_ad != null && _ad.CanShowAd()) DoShow();
        else { _wantShow = true; Load(); }
    }
#else
    public void TampilkanPeti(Saldoku game, string customData)
    {
        game.OnPetiGagal(game.PesanIklanMati());
    }
#endif
}

// ---- DTO JSON ----
[System.Serializable] public class SalVerifyReq  { public string kode; public string device; }
[System.Serializable] public class SalVerifyResp { public bool status; public string message; public SalVerifyData data; }
[System.Serializable] public class SalVerifyData { public string game_token; public int user_id; public string nama; public string referral_code; }
[System.Serializable] public class SalStatusResp { public bool status; public string message; public SalStatusData data; }
[System.Serializable]
public class SalStatusData
{
    public long koin; public long poin; public long rupiah; public int kurs;
    public int iklan_per_peti; public int poin_per_peti; public int peti_progress; public int sisa_ke_peti;
    public int iklan_hari_ini; public int batas_harian; public int sisa_iklan; public int peti_hari_ini;
    public long poin_hari_ini; public string nama;
}
