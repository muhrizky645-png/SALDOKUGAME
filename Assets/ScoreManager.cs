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

    // Tata letak UI:
    //  - Level  : pojok KIRI atas   (diatur di LevelSystem.cs)
    //  - Rekor  : pojok KANAN atas  (High Score)
    //  - Skor   : TENGAH, diturunkan sedikit biar tidak tabrakan dengan Level/bar XP
    void OnGUI()
    {
        // sembunyikan HUD selama menu awal / menu jeda tampil
        if (!GameMenu.SedangMain || GameMenu.SedangJeda) return;

        float pad = Screen.height * 0.02f;

        // ---- SKOR BERJALAN (tengah atas, diturunkan sedikit) ----
        int fontSize = Mathf.RoundToInt(Screen.height * 0.045f); // otomatis pas di HP
        float atas = Screen.height * 0.09f;   // diturunkan biar di bawah Level & bar XP
        float tinggi = fontSize * 1.6f;
        float lebar = Screen.width;
        string teks = "Skor: " + score;

        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.UpperCenter; // rata tengah

        // bayangan gelap biar kebaca di background apa pun
        float o = Mathf.Max(2f, fontSize * 0.06f);
        style.normal.textColor = new Color(0f, 0f, 0f, 0.6f);
        GUI.Label(new Rect(o, atas + o, lebar, tinggi), teks, style);

        // teks utama (putih)
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(0, atas, lebar, tinggi), teks, style);

        // ---- REKOR / HIGH SCORE (pojok KANAN atas) ----
        int fontRekor = Mathf.RoundToInt(Screen.height * 0.032f);
        float tinggiRekor = fontRekor * 2f;
        float lebarRekor = Screen.width * 0.5f;
        float kananX = Screen.width - lebarRekor - pad; // rata kanan dengan jarak dari tepi
        string teksRekor = "Rekor: " + rekor;

        GUIStyle styleKanan = new GUIStyle();
        styleKanan.fontSize = fontRekor;
        styleKanan.fontStyle = FontStyle.Bold;
        styleKanan.alignment = TextAnchor.UpperRight; // menempel ke tepi kanan rect
        styleKanan.normal.textColor = new Color(0f, 0f, 0f, 0.6f);
        GUI.Label(new Rect(kananX + 2, pad + 2, lebarRekor, tinggiRekor), teksRekor, styleKanan);
        styleKanan.normal.textColor = new Color(1f, 0.95f, 0.4f, 1f); // kuning emas
        GUI.Label(new Rect(kananX, pad, lebarRekor, tinggiRekor), teksRekor, styleKanan);
    }
}
