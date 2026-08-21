using UnityEngine;
using UnityEngine.SceneManagement;

// Menu awal (Home) + menu jeda (Pause) + Pengaturan Suara, tema "SURVIVAL".
// Otomatis dibuat saat game mulai DAN tiap scene di-reload.
public class GameMenu : MonoBehaviour
{
    public static GameMenu Instance;
    public static bool SedangMain = false;   // true saat sedang bermain (HUD tampil)
    public static bool SedangJeda = false;   // true saat game di-pause lewat menu jeda

    // dipakai saat restart: kalau true, setelah scene reload langsung main (skip menu awal)
    public static bool langsungMainSetelahLoad = false;

    private bool tampilHome = true;          // menu awal tampil
    private bool tampilPengaturan = false;   // panel pengaturan suara tampil

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        // PENTING: RuntimeInitialize hanya jalan sekali. Supaya manager tetap ada
        // setiap kali scene di-reload (restart), buat ulang lewat event sceneLoaded.
        SceneManager.sceneLoaded += (scene, mode) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("GameMenu", typeof(GameMenu));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SedangJeda = false;
        tampilPengaturan = false;

        if (langsungMainSetelahLoad)
        {
            // dipanggil setelah \"Ulangi\" / \"Main Lagi\": langsung main tanpa menu awal
            langsungMainSetelahLoad = false;
            tampilHome = false;
            SedangMain = true;
            Time.timeScale = 1f;
        }
        else
        {
            // tampilkan menu awal
            tampilHome = true;
            SedangMain = false;
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        // paksa game tetap beku selama menu awal / menu jeda / pengaturan tampil
        if (tampilHome || SedangJeda || tampilPengaturan) Time.timeScale = 0f;
    }

    void Mulai()
    {
        tampilHome = false;
        SedangMain = true;
        SedangJeda = false;
        tampilPengaturan = false;
        Time.timeScale = 1f;
    }

    void Jeda()
    {
        SedangJeda = true;
        Time.timeScale = 0f;
    }

    void Lanjut()
    {
        SedangJeda = false;
        Time.timeScale = 1f;
    }

    // restart lalu LANGSUNG main
    public static void UlangiDanMain()
    {
        langsungMainSetelahLoad = true;
        SedangJeda = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // restart lalu kembali ke MENU AWAL
    public static void KeHome()
    {
        langsungMainSetelahLoad = false;
        SedangJeda = false;
        Time.timeScale = 1f; // biar reload lancar; Awake akan set 0 lagi
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnGUI()
    {
        float h = Screen.height;
        float w = Screen.width;

        // ====== PANEL PENGATURAN SUARA (menutupi layar) ======
        if (tampilPengaturan)
        {
            GambarPengaturan();
            return;
        }

        // ====== MENU AWAL (HOME) ======
        if (tampilHome)
        {
            Tema.LatarGelap(new Color(0.04f, 0.10f, 0.05f, 0.35f)); // semburat hijau gelap

            Tema.Teks(new Rect(0, h * 0.14f, w, h * 0.12f), "SALDOKU", Mathf.RoundToInt(h * 0.085f),
                Tema.Darah, TextAnchor.MiddleCenter, true);
            Tema.Teks(new Rect(0, h * 0.25f, w, h * 0.09f), "LAST STAND", Mathf.RoundToInt(h * 0.062f),
                Tema.Army, TextAnchor.MiddleCenter, true);
            Tema.Teks(new Rect(0, h * 0.35f, w, h * 0.05f), "BERTAHAN SELAMA MUNGKIN", Mathf.RoundToInt(h * 0.026f),
                Tema.Redup, TextAnchor.MiddleCenter, true);

            // panel rekor (dengan ikon bintang mengapit angka)
            if (ScoreManager.Instance != null)
            {
                float rw = w * 0.7f, rh = h * 0.09f, rx = (w - rw) / 2f, ry = h * 0.43f;
                Tema.Panel9(new Rect(rx, ry, rw, rh), Tema.Plate, Tema.GarisRedup, 2f);
                Tema.Teks(new Rect(rx, ry + rh * 0.10f, rw, rh * 0.45f), "REKOR TERTINGGI",
                    Mathf.RoundToInt(h * 0.024f), Tema.Redup, TextAnchor.MiddleCenter, true);
                Tema.Teks(new Rect(rx, ry + rh * 0.44f, rw, rh * 0.55f), ScoreManager.Instance.RekorTertinggi.ToString(),
                    Mathf.RoundToInt(h * 0.038f), Tema.Amber, TextAnchor.MiddleCenter, true);

                float ik = rh * 0.42f;
                Ikon.Gambar(new Rect(rx + rw * 0.13f, ry + rh * 0.44f, ik, ik), Ikon.Bintang, Tema.Amber);
                Ikon.Gambar(new Rect(rx + rw * 0.87f - ik, ry + rh * 0.44f, ik, ik), Ikon.Bintang, Tema.Amber);
            }

            if (Tombol("MAIN", 0.58f, 0.55f)) { SoundManager.Klik(); Mulai(); }
            if (Tombol("PENGATURAN", 0.69f, 0.55f)) { SoundManager.Klik(); tampilPengaturan = true; }
            return;
        }

        if (!SedangMain) return;

        // ====== MENU JEDA (PAUSE) ======
        if (SedangJeda)
        {
            Tema.LatarGelap();
            Tema.Teks(new Rect(0, h * 0.14f, w, h * 0.1f), "JEDA", Mathf.RoundToInt(h * 0.075f),
                Tema.Army, TextAnchor.MiddleCenter, true);
            if (Tombol("LANJUT", 0.33f, 0.62f)) { SoundManager.Klik(); Lanjut(); }
            if (Tombol("ULANGI", 0.46f, 0.62f)) { SoundManager.Klik(); UlangiDanMain(); }
            if (Tombol("PENGATURAN", 0.59f, 0.62f)) { SoundManager.Klik(); tampilPengaturan = true; }
            if (Tombol("KE HOME", 0.72f, 0.62f)) { SoundManager.Klik(); KeHome(); }
            return;
        }

        // ====== TOMBOL JEDA SAAT MAIN ======
        // sembunyikan saat Game Over atau saat memilih skill
        if (PlayerHealth.GameOver || SkillManager.AktifMemilih) return;

        float sz = h * 0.055f;
        float pad = h * 0.02f;
        float x = w - sz - pad; // POJOK KANAN ATAS (ruang tengah dipakai timer)
        if (GUI.Button(new Rect(x, pad, sz, sz), "II", Tema.GayaTombol(Mathf.RoundToInt(h * 0.028f))))
        {
            SoundManager.Klik();
            Jeda();
        }
    }

    // ====== PANEL PENGATURAN SUARA ======
    void GambarPengaturan()
    {
        float h = Screen.height;
        float w = Screen.width;

        Tema.LatarGelap();
        Tema.Teks(new Rect(0, h * 0.12f, w, h * 0.09f), "PENGATURAN SUARA", Mathf.RoundToInt(h * 0.05f),
            Tema.Army, TextAnchor.MiddleCenter, true);

        float pw = w * 0.78f, px = (w - pw) / 2f;
        float sldW = pw * 0.62f;
        float lblF = Mathf.RoundToInt(h * 0.030f);
        float btnF = Mathf.RoundToInt(h * 0.024f);

        // ---- MUSIK ----
        BarisSuara("MUSIK", h * 0.30f, px, pw, sldW, lblF, btnF,
            SoundManager.VolMusik, SoundManager.MuteMusik,
            (v) => SoundManager.SetVolMusik(v),
            () => { SoundManager.ToggleMuteMusik(); });

        // ---- EFEK ----
        BarisSuara("EFEK", h * 0.46f, px, pw, sldW, lblF, btnF,
            SoundManager.VolEfek, SoundManager.MuteEfek,
            (v) => SoundManager.SetVolEfek(v),
            () => { SoundManager.ToggleMuteEfek(); if (!SoundManager.MuteEfek) SoundManager.Klik(); });

        // ---- TUTUP ----
        if (Tombol("TUTUP", 0.66f, 0.5f)) { SoundManager.Klik(); tampilPengaturan = false; }
    }

    // satu baris pengaturan: label + slider volume + tombol mute
    void BarisSuara(string nama, float y, float px, float pw, float sldW, float lblF, float btnF,
        float nilai, bool mute, System.Action<float> onUbah, System.Action onMute)
    {
        float h = Screen.height;

        // label + persentase
        Tema.Teks(new Rect(px, y - h * 0.05f, pw, h * 0.04f), nama + "   " + Mathf.RoundToInt(nilai * 100f) + "%",
            Mathf.RoundToInt(lblF), Tema.Tulang, TextAnchor.MiddleLeft, true);

        // slider volume
        float baru = GUI.HorizontalSlider(new Rect(px, y, sldW, h * 0.05f), nilai, 0f, 1f);
        if (!Mathf.Approximately(baru, nilai)) onUbah(baru);

        // tombol mute
        float bx = px + sldW + Screen.width * 0.03f;
        float bw = px + pw - bx;
        if (GUI.Button(new Rect(bx, y - h * 0.018f, bw, h * 0.065f), mute ? "BISU" : "AKTIF",
            Tema.GayaTombol(Mathf.RoundToInt(btnF))))
        {
            onMute();
        }
    }

    // tombol menu bertema (lebar cukup supaya teks tidak kepotong)
    bool Tombol(string teks, float posY, float lebarFrac)
    {
        float bw = Screen.width * lebarFrac;
        float bh = Screen.height * 0.085f;
        float bx = (Screen.width - bw) / 2f;
        float by = Screen.height * posY;
        int f = Mathf.RoundToInt(Screen.height * 0.036f);
        return GUI.Button(new Rect(bx, by, bw, bh), teks, Tema.GayaTombol(f));
    }
}
