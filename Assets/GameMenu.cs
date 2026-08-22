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
        // pastikan karakter yang dipilih di Home dipakai walau scene tidak di-reload
        KarakterPemain.TerapkanPilihan();

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

            // ---- JUDUL ----
            Tema.Teks(new Rect(0, h * 0.05f, w, h * 0.10f), "SALDOKU", Mathf.RoundToInt(h * 0.072f),
                Tema.Darah, TextAnchor.MiddleCenter, true);
            Tema.Teks(new Rect(0, h * 0.145f, w, h * 0.075f), "LAST STAND", Mathf.RoundToInt(h * 0.05f),
                Tema.Army, TextAnchor.MiddleCenter, true);
            Tema.Teks(new Rect(0, h * 0.222f, w, h * 0.04f), "BERTAHAN SELAMA MUNGKIN", Mathf.RoundToInt(h * 0.023f),
                Tema.Redup, TextAnchor.MiddleCenter, true);

            // ---- PANEL REKOR (ikon bintang mengapit angka) ----
            if (ScoreManager.Instance != null)
            {
                float rw = w * 0.66f, rh = h * 0.075f, rx = (w - rw) / 2f, ry = h * 0.275f;
                Tema.Panel9(new Rect(rx, ry, rw, rh), Tema.Plate, Tema.GarisRedup, 2f);
                Tema.Teks(new Rect(rx, ry + rh * 0.10f, rw, rh * 0.42f), "REKOR TERTINGGI",
                    Mathf.RoundToInt(h * 0.022f), Tema.Redup, TextAnchor.MiddleCenter, true);
                Tema.Teks(new Rect(rx, ry + rh * 0.46f, rw, rh * 0.50f), ScoreManager.Instance.RekorTertinggi.ToString(),
                    Mathf.RoundToInt(h * 0.034f), Tema.Amber, TextAnchor.MiddleCenter, true);

                float ik = rh * 0.40f;
                Ikon.Gambar(new Rect(rx + rw * 0.13f, ry + rh * 0.46f, ik, ik), Ikon.Bintang, Tema.Amber);
                Ikon.Gambar(new Rect(rx + rw * 0.87f - ik, ry + rh * 0.46f, ik, ik), Ikon.Bintang, Tema.Amber);
            }

            // ---- PEMILIH KARAKTER ----
            GambarPilihKarakter(w, h);

            // ---- TOMBOL (diturunkan supaya ada ruang untuk pilih karakter) ----
            if (Tombol("MAIN", 0.665f, 0.62f)) { SoundManager.Klik(); Mulai(); }
            if (Tombol("PENGATURAN", 0.785f, 0.62f)) { SoundManager.Klik(); tampilPengaturan = true; }
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

        // Pojok KANAN atas, ukuran responsif (sisi terpendek) + hormati safe area
        float sz = Tema.Unit * 0.09f;
        float pad = Tema.Pad;
        float x = w - sz - pad - Tema.AmanKanan;
        float y = Tema.AmanAtas + pad;
        Rect rJeda = new Rect(x, y, sz, sz);
        if (GUI.Button(rJeda, "", Tema.GayaTombol(1)))
        {
            SoundManager.Klik();
            Jeda();
        }
        // ikon jeda digambar MANUAL: dua batang vertikal LURUS (bukan teks \"II\" yang terlihat miring)
        float pbW = sz * 0.13f;                 // lebar tiap batang
        float pbH = sz * 0.42f;                 // tinggi batang
        float pbGap = sz * 0.14f;               // jarak antar batang
        float pbY = y + (sz - pbH) / 2f;
        float pbCx = x + sz / 2f;
        Tema.Kotak(new Rect(pbCx - pbGap / 2f - pbW, pbY, pbW, pbH), Tema.Tulang);
        Tema.Kotak(new Rect(pbCx + pbGap / 2f, pbY, pbW, pbH), Tema.Tulang);
    }

    // ====== PEMILIH KARAKTER (Home) ======
    void GambarPilihKarakter(float w, float h)
    {
        float pw = w * 0.82f, ph = h * 0.245f;
        float px = (w - pw) / 2f, py = h * 0.37f;
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Plate, Tema.GarisRedup, 2f);

        // judul kecil
        Tema.Teks(new Rect(px, py + ph * 0.04f, pw, ph * 0.15f), "PILIH KARAKTER",
            Mathf.RoundToInt(h * 0.022f), Tema.Redup, TextAnchor.MiddleCenter, true);

        int idx = KarakterManager.Dipilih;

        // area tengah untuk portrait
        float aY = py + ph * 0.20f;
        float aH = ph * 0.54f;

        // tombol panah kiri / kanan
        float ab = Mathf.Min(pw * 0.15f, aH * 0.85f);
        float ay = aY + (aH - ab) / 2f;
        int af = Mathf.RoundToInt(h * 0.045f);
        if (GUI.Button(new Rect(px + pw * 0.02f, ay, ab, ab), "<", Tema.GayaTombol(af)))
        {
            SoundManager.Klik();
            KarakterManager.Sebelumnya();
            KarakterPemain.TerapkanPilihan();
        }
        if (GUI.Button(new Rect(px + pw * 0.98f - ab, ay, ab, ab), ">", Tema.GayaTombol(af)))
        {
            SoundManager.Klik();
            KarakterManager.Berikutnya();
            KarakterPemain.TerapkanPilihan();
        }

        // portrait (kepala) karakter di tengah
        float potH = aH;
        float potW = aH;
        float potX = (w - potW) / 2f;
        Texture2D kepala = KarakterManager.Kepala(idx);
        if (kepala != null)
        {
            GUI.DrawTexture(new Rect(potX, aY, potW, potH), kepala, ScaleMode.ScaleToFit, true);
        }
        else
        {
            // fallback kalau tekstur belum tersalin: ikon bintang
            Ikon.Gambar(new Rect(potX + potW * 0.2f, aY + potH * 0.2f, potW * 0.6f, potH * 0.6f), Ikon.Bintang, Tema.Amber);
        }

        // nama karakter
        Tema.Teks(new Rect(px, py + ph * 0.80f, pw, ph * 0.17f), KarakterManager.Nama[idx],
            Mathf.RoundToInt(h * 0.026f), Tema.Tulang, TextAnchor.MiddleCenter, true);
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
        int f = Mathf.RoundToInt(Screen.height * 0.034f);
        return GUI.Button(new Rect(bx, by, bw, bh), teks, Tema.GayaTombol(f));
    }
}
