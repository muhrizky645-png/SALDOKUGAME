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
    //  - Skor  : plat gelap di TENGAH atas, tepat di bawah timer (diatur di sini)
    //  - Level + bar XP : pojok KIRI atas (diatur LevelSystem.cs)
    //  - Tombol jeda    : pojok KANAN atas (diatur GameMenu.cs)
    //  Semua posisi responsif + hormati safe area. Rekor sengaja tidak di HUD.
    void OnGUI()
    {
        // sembunyikan HUD selama menu awal / jeda / saat memilih skill / SAAT GAME OVER
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float w = Screen.width, h = Screen.height;
        float atas = Tema.AmanAtas;

        // ---- SKOR (plat ramping tengah atas, TEPAT di bawah timer, angka di tengah) ----
        int fSkor = Mathf.Min(Tema.Font(0.05f), Mathf.RoundToInt(w * 0.10f));
        float plW = Mathf.Max(w * 0.26f, fSkor * 4f);
        float plH = fSkor * 1.7f;
        float plX = (w - plW) / 2f;
        float plY = atas + h * 0.090f; // di bawah timer (timer di baris paling atas tengah)
        Tema.Panel9(new Rect(plX, plY, plW, plH), Tema.Plate, Tema.GarisRedup, 2f);
        Tema.Teks(new Rect(plX, plY, plW, plH), score.ToString(), fSkor,
            Tema.Tulang, TextAnchor.MiddleCenter, true);
    }
}
