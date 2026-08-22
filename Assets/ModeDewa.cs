using UnityEngine;
using UnityEngine.SceneManagement;

// ====== MODE DEWA (Peti Dewa) ======
// Peti muncul saat pemain sudah bertahan cukup lama, lalu HANYA TAMPIL beberapa detik:
// kalau tidak diklik, peti HILANG dan hitung mundur kemunculan berikutnya dimulai lagi.
// Dibuka dengan "tonton iklan" (sekarang PLACEHOLDER -> langsung buka; untuk iklan ASLI
// cukup panggil Aktifkan() pada callback reward Unity Ads/AdMob). Memberi 4 SKILL DI LUAR
// NALAR selama 30 DETIK:
//   1) KEBAL          - tidak bisa mati
//   2) BADAI PELURU   - tembakan super cepat + banyak + jauh
//   3) MAGNET SEMESTA - semua permata langsung tersedot
//   4) AURA MAUT      - musuh di sekitar musnah terus-menerus
// Dibuat otomatis tiap scene (seperti manager lain) & reset tiap game baru.
public class ModeDewa : MonoBehaviour
{
    public static ModeDewa Instance;

    // dibaca PlayerShooting / PlayerHealth / XpGem
    public static bool Aktif = false;
    public static float SisaDetik = 0f;

    // true saat overlay konfirmasi peti tampil -> HUD lain (mis. bar boss) sembunyi dulu.
    public static bool MenuTerbuka = false;

    const float DURASI = 30f;         // durasi skill dewa = 30 detik
    const float JedaIsiUlang = 90f;   // peti muncul lagi tiap 90 detik bermain (biar spesial)
    const float DurasiTampil = 10f;   // peti hanya tampil 10 detik; kalau tak diklik -> hilang

    private bool tersedia = false;    // peti siap dibuka
    private bool konfirmasi = false;  // overlay "tonton iklan" tampil
    private float isiUlang = 0f;      // hitung menuju peti berikutnya
    private float tampilSisa = 0f;    // hitung mundur selama peti tampil
    private float pulseT = 0f;        // timer aura maut

    private Texture2D _chest;
    private bool _chestDicari = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("ModeDewa", typeof(ModeDewa));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // reset penuh tiap game baru
        Aktif = false;
        SisaDetik = 0f;
        MenuTerbuka = false;
        tersedia = false;
        konfirmasi = false;
        isiUlang = 0f;
        tampilSisa = 0f;
        pulseT = 0f;
    }

    Texture2D Chest
    {
        get
        {
            if (!_chestDicari) { _chest = Resources.Load<Texture2D>("Icons/petidewa"); _chestDicari = true; }
            return _chest;
        }
    }

    bool SedangMain()
    {
        return GameMenu.SedangMain && !GameMenu.SedangJeda &&
               !SkillManager.AktifMemilih && !PlayerHealth.GameOver;
    }

    void Update()
    {
        MenuTerbuka = konfirmasi; // sinkron tiap frame
        if (konfirmasi) return;   // dijeda saat overlay tampil

        if (Aktif)
        {
            if (SedangMain())
            {
                SisaDetik -= Time.deltaTime;
                pulseT -= Time.deltaTime;
                if (pulseT <= 0f) { pulseT = 0.45f; AuraMaut(); }
            }
            if (SisaDetik <= 0f) { Aktif = false; SisaDetik = 0f; isiUlang = 0f; }
        }
        else if (tersedia)
        {
            // peti sedang tampil -> hitung mundur; hilang kalau habis
            if (SedangMain())
            {
                tampilSisa -= Time.deltaTime;
                if (tampilSisa <= 0f)
                {
                    tersedia = false;
                    tampilSisa = 0f;
                    isiUlang = 0f; // mulai lagi hitung menuju peti berikutnya
                }
            }
        }
        else
        {
            if (SedangMain())
            {
                isiUlang += Time.deltaTime;
                if (isiUlang >= JedaIsiUlang)
                {
                    tersedia = true;
                    tampilSisa = DurasiTampil;
                    isiUlang = 0f;
                }
            }
        }
    }

    // AURA MAUT: musnahkan semua musuh di sekitar pemain tiap pulsa
    void AuraMaut()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p == null) return;
        Vector3 pos = p.transform.position;
        float radius = 3.6f;

        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var m in musuh)
        {
            if (m == null) continue;
            if (Vector3.Distance(m.transform.position, pos) <= radius)
            {
                EnemyChase ec = m.GetComponentInParent<EnemyChase>();
                if (ec != null) ec.KenaSerangan(9999);
            }
        }
        HitEffect.Munculkan(pos, radius * 3f); // cincin aura kuning
    }

    void Buka()
    {
        // IKLAN ASLI: di sini tampilkan rewarded ad (Unity Ads/AdMob),
        // lalu panggil Aktifkan() pada callback "reward diberikan".
        konfirmasi = false;
        MenuTerbuka = false;
        Time.timeScale = 1f;
        Aktifkan();
    }

    void Aktifkan()
    {
        Aktif = true;
        SisaDetik = DURASI;
        tersedia = false;
        tampilSisa = 0f;
        isiUlang = 0f;
        pulseT = 0f;
        SoundManager.LevelUp();
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) HitEffect.Munculkan(p.transform.position, 8f);
    }

    void OnGUI()
    {
        float w = Screen.width, h = Screen.height;

        // overlay konfirmasi (membekukan game)
        if (konfirmasi)
        {
            if (!SedangMain()) { konfirmasi = false; MenuTerbuka = false; Time.timeScale = 1f; return; }
            Time.timeScale = 0f;
            GambarKonfirmasi(w, h);
            return;
        }

        if (!SedangMain()) return;

        if (Aktif) { GambarStatus(w, h); return; }
        if (tersedia) GambarTombolPeti(w, h);
    }

    // Tombol peti berdenyut di sisi kanan (di bawah timer) + hitung mundur tampil
    void GambarTombolPeti(float w, float h)
    {
        float sz = Tema.Unit * 0.14f;
        float pad = Tema.Pad;
        float x = w - sz - pad - Tema.AmanKanan;
        float y = h * 0.34f;
        float k = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 4f);

        Rect r = new Rect(x, y, sz, sz);
        Tema.Panel9(r, Tema.Panel, Color.Lerp(Tema.Garis, Tema.Amber, k), Mathf.Max(2f, sz * 0.06f));

        Rect ir = new Rect(x + sz * 0.12f, y + sz * 0.06f, sz * 0.76f, sz * 0.76f);
        if (Chest != null) GUI.DrawTexture(ir, Chest, ScaleMode.ScaleToFit, true);
        else Ikon.Gambar(ir, Ikon.Peti, Tema.Amber);

        Tema.Teks(new Rect(x - sz * 0.3f, y + sz * 0.82f, sz * 1.6f, sz * 0.28f),
            "PETI DEWA", Mathf.RoundToInt(sz * 0.16f), Tema.Amber, TextAnchor.MiddleCenter, true);

        // ===== hitung mundur waktu tampil (bar + angka) =====
        int sisa = Mathf.CeilToInt(Mathf.Max(0f, tampilSisa));
        Color warnaSisa = tampilSisa <= 3f ? new Color(0.95f, 0.30f, 0.20f) : Tema.Amber;

        float barY = y + sz * 1.12f, barH = sz * 0.11f;
        Tema.Kotak(new Rect(x, barY, sz, barH), Tema.Plate);
        float frac = Mathf.Clamp01(tampilSisa / DurasiTampil);
        Tema.Kotak(new Rect(x, barY, sz * frac, barH), warnaSisa);

        Tema.Teks(new Rect(x - sz * 0.3f, barY + barH * 1.1f, sz * 1.6f, sz * 0.28f),
            sisa + "s", Mathf.RoundToInt(sz * 0.16f), warnaSisa, TextAnchor.MiddleCenter, true);

        if (GUI.Button(r, "", GUIStyle.none)) { SoundManager.Klik(); konfirmasi = true; MenuTerbuka = true; }
    }

    // Status Mode Dewa aktif: vignette emas + countdown bawah-tengah
    void GambarStatus(float w, float h)
    {
        float a = 0.10f + 0.06f * Mathf.Sin(Time.unscaledTime * 3f);
        Color g = new Color(1f, 0.8f, 0.2f, a);
        float th = Mathf.Min(w, h) * 0.03f;
        Tema.Kotak(new Rect(0, 0, w, th), g);
        Tema.Kotak(new Rect(0, h - th, w, th), g);
        Tema.Kotak(new Rect(0, 0, th, h), g);
        Tema.Kotak(new Rect(w - th, 0, th, h), g);

        int m = (int)(SisaDetik / 60f);
        int s = (int)(SisaDetik % 60f);
        string sisa = string.Format("{0}:{1:00}", m, s);

        float bw = w * 0.56f, bh = h * 0.065f, bx = (w - bw) / 2f, by = h * 0.90f;
        Tema.Panel9(new Rect(bx, by, bw, bh), Tema.Plate, Tema.Amber, 2f);

        float isz = bh * 0.8f;
        Rect ir = new Rect(bx + bh * 0.2f, by + bh * 0.1f, isz, isz);
        if (Chest != null) GUI.DrawTexture(ir, Chest, ScaleMode.ScaleToFit, true);

        Tema.Teks(new Rect(bx + bh, by, bw - bh, bh), "MODE DEWA  " + sisa,
            Mathf.RoundToInt(bh * 0.5f), Tema.Amber, TextAnchor.MiddleCenter, true);
    }

    // Overlay konfirmasi buka peti (tonton iklan)
    void GambarKonfirmasi(float w, float h)
    {
        Tema.LatarGelap(new Color(0.15f, 0.10f, 0.0f, 0.35f));

        float pw = w * 0.84f, ph = h * 0.62f, px = (w - pw) / 2f, py = h * 0.19f;
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Panel, Tema.Amber, 3f);

        // judul
        Tema.Teks(new Rect(px, py + ph * 0.03f, pw, ph * 0.10f), "PETI DEWA",
            Mathf.RoundToInt(ph * 0.085f), Tema.Amber, TextAnchor.MiddleCenter, true);

        // ikon peti
        float isz = ph * 0.15f;
        Rect ir = new Rect(px + (pw - isz) / 2f, py + ph * 0.14f, isz, isz);
        if (Chest != null) GUI.DrawTexture(ir, Chest, ScaleMode.ScaleToFit, true);
        else Ikon.Gambar(ir, Ikon.Peti, Tema.Amber);

        // daftar skill: satu baris tiap poin biar rapi (tidak turun baris)
        string[] baris = new string[] {
            "KEBAL - tak bisa mati",
            "BADAI PELURU - tembak brutal",
            "MAGNET - tarik semua permata",
            "AURA MAUT - musnah sekitar",
        };
        float ly = py + ph * 0.32f;
        float lh = ph * 0.072f;
        int fl = Mathf.RoundToInt(ph * 0.032f);
        for (int i = 0; i < baris.Length; i++)
            Tema.Teks(new Rect(px + pw * 0.09f, ly + i * lh, pw * 0.82f, lh),
                baris[i], fl, Tema.Tulang, TextAnchor.MiddleLeft, false);

        Tema.Teks(new Rect(px, py + ph * 0.63f, pw, ph * 0.07f), "AKTIF SELAMA 30 DETIK",
            Mathf.RoundToInt(ph * 0.05f), Tema.Army, TextAnchor.MiddleCenter, true);

        float bw = pw * 0.82f, bx = px + (pw - bw) / 2f;
        float b1y = py + ph * 0.73f, b1h = ph * 0.13f;
        Rect rIklan = new Rect(bx, b1y, bw, b1h);
        if (GUI.Button(rIklan, "", Tema.GayaTombol(1)))
        {
            SoundManager.Klik();
            Buka();
            return;
        }
        Tema.Teks(new Rect(rIklan.x, rIklan.y + b1h * 0.12f, rIklan.width, b1h * 0.44f),
            "BUKA SEKARANG", Mathf.RoundToInt(ph * 0.055f), Tema.Army, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(rIklan.x, rIklan.y + b1h * 0.54f, rIklan.width, b1h * 0.40f),
            "> TONTON IKLAN", Mathf.RoundToInt(ph * 0.035f), Tema.Amber, TextAnchor.MiddleCenter, true);

        float b2y = py + ph * 0.885f, b2h = ph * 0.09f;
        if (GUI.Button(new Rect(bx, b2y, bw, b2h), "NANTI", Tema.GayaTombol(Mathf.RoundToInt(ph * 0.045f))))
        {
            SoundManager.Klik();
            konfirmasi = false;
            MenuTerbuka = false;
            Time.timeScale = 1f;
        }
    }
}
