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
    //  - Skor  : plat gelap di TENGAH atas, tepat di bawah timer
    //  (Level + bar XP diatur di LevelSystem.cs, pojok KIRI atas)
    //  Rekor SENGAJA tidak ditampilkan di HUD (biar rapi). Tetap disimpan & dipakai di layar Game Over.
    void OnGUI()
    {
        // sembunyikan HUD selama menu awal / jeda / saat memilih skill / SAAT GAME OVER
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float h = Screen.height;

        // ---- SKOR (plat ramping tengah atas, tepat di bawah timer, angka di tengah plat) ----
        int fSkor = Mathf.RoundToInt(h * 0.05f);
        float plW = Screen.width * 0.26f;
        float plH = fSkor * 1.7f;
        float plX = (Screen.width - plW) / 2f;
        float plY = h * 0.105f; // di bawah timer (timer paling atas tengah)
        Tema.Panel9(new Rect(plX, plY, plW, plH), Tema.Plate, Tema.GarisRedup, 2f);
        Tema.Teks(new Rect(plX, plY, plW, plH), score.ToString(), fSkor,
            Tema.Tulang, TextAnchor.MiddleCenter, true);
    }
}
