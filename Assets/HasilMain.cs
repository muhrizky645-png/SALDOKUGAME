using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================================
// ZOMBURST - WIN CONDITION + LAYAR HASIL (MENANG)
// Manager auto-bootstrap (pola sama seperti manager lain).
//
// Tugas:
//  1) Deteksi MENANG: bertahan sampai StageManager.TargetSekarang detik.
//  2) Bekukan game, beri hadiah Permata, buka stage berikutnya.
//  3) Gambar layar Hasil MENANG (waktu bertahan, hadiah, tombol lanjut).
//
// Layar KALAH tetap ditangani PlayerHealth (GAME OVER) yang sudah ada.
// =====================================================================
public class HasilMain : MonoBehaviour
{
    public static HasilMain Instance;
    public static bool Menang = false;   // true saat layar Hasil MENANG tampil

    int hadiahPermata = 0;
    int waktuBertahan = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("HasilMain", typeof(HasilMain));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Menang = false;
        hadiahPermata = 0;
        waktuBertahan = 0;
    }

    void Update()
    {
        // hanya cek saat benar-benar sedang main
        if (!GameMenu.SedangMain || GameMenu.SedangJeda || Menang) return;
        if (PlayerHealth.GameOver || SkillManager.AktifMemilih) return;

        if (GameTimer.Detik >= StageManager.TargetSekarang)
            PicuMenang();
    }

    void PicuMenang()
    {
        Menang = true;
        Time.timeScale = 0f;
        waktuBertahan = Mathf.RoundToInt(StageManager.TargetSekarang);

        // ---- HADIAH PERMATA ----
        // Dasar per-stage + bonus dari skor. Modest, biar tetap seimbang.
        int dasar = 50 + StageManager.Dipilih * 40;
        int skor = (ScoreManager.Instance != null) ? ScoreManager.Instance.SkorSekarang : 0;
        int bonusSkor = skor / 20;
        hadiahPermata = dasar + bonusSkor;
        if (MataUang.Instance != null) MataUang.Instance.TambahPermata(hadiahPermata);

        // ---- BUKA STAGE BERIKUTNYA ----
        StageManager.BukaSampai(StageManager.Dipilih + 1);

        SoundManager.Menang();
    }

    static string FormatWaktu(int detik)
    {
        int m = detik / 60;
        int s = detik % 60;
        return m.ToString("00") + ":" + s.ToString("00");
    }

    void OnGUI()
    {
        if (!Menang) return;

        // Gambar PALING ATAS supaya menutupi HUD (timer/level/permata) di baliknya.
        GUI.depth = -1000;

        float h = Screen.height;
        float w = Screen.width;

        // latar gelap tebal (menutupi HUD)
        Tema.Kotak(new Rect(0, 0, w, h), new Color(0.03f, 0.06f, 0.03f, 0.93f));
        Tema.Vignette();

        // ---- JUDUL MENANG ----
        Tema.Teks(new Rect(0, h * 0.10f, w, h * 0.11f), "MENANG!", Mathf.RoundToInt(h * 0.075f),
            Tema.Army, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(0, h * 0.205f, w, h * 0.05f), StageManager.Sekarang.nama,
            Mathf.RoundToInt(h * 0.03f), Tema.Amber, TextAnchor.MiddleCenter, true);

        // ---- PANEL RINGKASAN (BERTAHAN atas, HADIAH bawah) ----
        float pw = w * 0.72f, ph = h * 0.22f, px = (w - pw) / 2f, py = h * 0.29f;
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Plate, Tema.GarisRedup, 2f);

        float rowH = ph / 2f;
        int fLabel = Mathf.RoundToInt(h * 0.024f);
        int fNilai = Mathf.RoundToInt(h * 0.046f);

        // BERTAHAN (baris atas)
        Tema.Teks(new Rect(px, py + rowH * 0.14f, pw, rowH * 0.30f), "BERTAHAN", fLabel,
            Tema.Redup, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(px, py + rowH * 0.44f, pw, rowH * 0.52f), FormatWaktu(waktuBertahan), fNilai,
            Tema.Tulang, TextAnchor.MiddleCenter, true);

        // garis pemisah
        Tema.Panel9(new Rect(px + pw * 0.10f, py + rowH - 1f, pw * 0.80f, 2f),
            Tema.GarisRedup, Tema.GarisRedup, 0f);

        // HADIAH PERMATA (baris bawah)
        float ryBot = py + rowH;
        Tema.Teks(new Rect(px, ryBot + rowH * 0.14f, pw, rowH * 0.30f), "HADIAH PERMATA", fLabel,
            Tema.Redup, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(px, ryBot + rowH * 0.44f, pw, rowH * 0.52f), "+" + hadiahPermata, fNilai,
            new Color(0.78f, 0.5f, 1f), TextAnchor.MiddleCenter, true);

        // ---- TOMBOL ----
        float bw = w * 0.62f, bh = h * 0.082f, bx = (w - bw) / 2f;
        int fb = Mathf.RoundToInt(h * 0.032f);

        float y1 = h * 0.58f;
        float y2 = h * 0.69f;
        float y3 = h * 0.80f;

        // STAGE BERIKUTNYA (kalau ada) - pakai gaya aksen (emas)
        if (StageManager.AdaBerikutnya)
        {
            if (GUI.Button(new Rect(bx, y1, bw, bh), "STAGE BERIKUTNYA", Tema.GayaTombolAksen(fb)))
            {
                SoundManager.Klik();
                StageManager.Dipilih = StageManager.Dipilih + 1;
                GameMenu.UlangiDanMain();
            }
        }
        else
        {
            // stage terakhir sudah beres
            Tema.Teks(new Rect(bx, y1, bw, bh), "SEMUA STAGE SELESAI!", Mathf.RoundToInt(h * 0.028f),
                Tema.Amber, TextAnchor.MiddleCenter, true);
        }

        if (GUI.Button(new Rect(bx, y2, bw, bh), "ULANGI", Tema.GayaTombol(fb)))
        {
            SoundManager.Klik();
            GameMenu.UlangiDanMain();
        }
        if (GUI.Button(new Rect(bx, y3, bw, bh), "KE HOME", Tema.GayaTombol(fb)))
        {
            SoundManager.Klik();
            GameMenu.KeHome();
        }
    }
}
