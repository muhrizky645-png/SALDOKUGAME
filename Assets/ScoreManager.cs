using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int score = 0;
    private int rekor = 0;

    public int SkorSekarang { get { return score; } }
    public int RekorTertinggi { get { return rekor; } }

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
        rekor = PlayerPrefs.GetInt("rekor", 0);
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score > rekor)
        {
            rekor = score;
            PlayerPrefs.SetInt("rekor", rekor);
            PlayerPrefs.Save();
        }
    }

    // Tata letak HUD (tema survival):
    //  - Level + bar XP : pojok KIRI atas, memanjang ke kanan (LevelSystem.cs)
    //  - Skor  : di BAWAH panel level, mepet KIRI (diatur di sini)
    //  - Tombol jeda : pojok KANAN atas (GameMenu.cs)
    //  - Timer : di BAWAH tombol jeda, mepet KANAN (GameTimer.cs)
    //  Semua posisi responsif + hormati safe area.
    void OnGUI()
    {
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float w = Screen.width, h = Screen.height;

        // posisi bawah panel level (konsisten dgn LevelSystem)
        float levelBawah = Tema.AmanAtas + Tema.Pad + LevelSystem.TinggiPanel(w);

        // SKOR: di BAWAH panel level, mepet KIRI (plat menyesuaikan jumlah digit)
        int fSkor = Mathf.Min(Tema.Font(0.05f), Mathf.RoundToInt(w * 0.10f));
        string s = score.ToString();
        float plH = fSkor * 1.7f;
        float plW = Mathf.Max(fSkor * 2.6f, fSkor * (1.1f + 0.62f * s.Length));
        float plX = Tema.AmanKiri + Tema.Pad;
        float plY = levelBawah + Tema.Pad * 0.6f;
        Tema.Panel9(new Rect(plX, plY, plW, plH), Tema.Plate, Tema.GarisRedup, 2f);
        Tema.Teks(new Rect(plX, plY, plW, plH), s, fSkor, Tema.Tulang, TextAnchor.MiddleCenter, true);
    }
}
