using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int score = 0;
    private int rekor = 0;

    // biar script lain (mis. PlayerHealth) bisa baca
    public int SkorSekarang { get { return score; } }
    public int RekorTertinggi { get { return rekor; } }

    // Otomatis membuat ScoreManager saat game mulai DAN tiap scene di-reload
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (scene, mode) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("ScoreManager", typeof(ScoreManager));
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        rekor = PlayerPrefs.GetInt("rekor", 0); // ambil rekor tersimpan
    }

    // Panggil ini untuk menambah skor
    public void AddScore(int amount)
    {
        score += amount;
        if (score > rekor)
        {
            rekor = score;
            PlayerPrefs.SetInt("rekor", rekor); // simpan rekor baru
            PlayerPrefs.Save();
        }
    }

    // Tata letak HUD (tema survival):
    //  - Timer : tengah atas (diatur GameTimer.cs)
    //  - Skor  : plat gelap di TENGAH atas (di bawah timer)
    //  - Rekor : panel gelap di pojok KANAN atas, DI BAWAH tombol jeda
    //  (Level + bar XP diatur di LevelSystem.cs, pojok KIRI atas)
    void OnGUI()
    {
        // sembunyikan HUD selama menu awal / jeda / saat memilih skill / SAAT GAME OVER
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float h = Screen.height;
        float pad = h * 0.02f;

        // ---- SKOR (plat tengah atas, di bawah timer) ----
        // Sengaja dibuat ramping supaya tidak menabrak panel REKOR di kanan.
        int fSkor = Mathf.RoundToInt(h * 0.045f);
        float plW = Screen.width * 0.30f;
        float plH = fSkor * 1.6f;
        float plX = (Screen.width - plW) / 2f;
        float plY = h * 0.105f; // sedikit turun supaya timer (tengah paling atas) tidak tertimpa
        Tema.Panel9(new Rect(plX, plY, plW, plH), Tema.Plate, Tema.GarisRedup, 2f);
        Tema.Teks(new Rect(plX, plY + plH * 0.16f, plW, plH), score.ToString(), fSkor,
            Tema.Tulang, TextAnchor.UpperCenter, true);

        // ---- REKOR / HIGH SCORE (panel pojok kanan atas, DI BAWAH tombol jeda) ----
        int fRek = Mathf.RoundToInt(h * 0.024f);
        float rW = Screen.width * 0.30f;
        float rH = fRek * 3.1f;
        float rX = Screen.width - rW - pad;
        float rY = pad + h * 0.055f + h * 0.014f; // di bawah tombol jeda (sz 0.055h)
        Tema.Panel9(new Rect(rX, rY, rW, rH), Tema.Plate, Tema.GarisRedup, 2f);

        // ikon bintang di kiri panel rekor
        float ik = rH * 0.5f;
        Ikon.Gambar(new Rect(rX + rW * 0.06f, rY + (rH - ik) / 2f, ik, ik), Ikon.Bintang, Tema.Amber);

        Tema.Teks(new Rect(rX, rY + rH * 0.10f, rW - pad * 0.5f, rH * 0.45f), "REKOR", fRek,
            Tema.Redup, TextAnchor.UpperRight, true);
        Tema.Teks(new Rect(rX, rY + rH * 0.44f, rW - pad * 0.5f, rH * 0.55f), rekor.ToString(),
            Mathf.RoundToInt(fRek * 1.25f), Tema.Amber, TextAnchor.UpperRight, true);
    }
}
