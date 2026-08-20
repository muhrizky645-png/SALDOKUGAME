using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int score = 0;
    private int rekor = 0;

    // biar script lain (mis. PlayerHealth) bisa baca
    public int SkorSekarang { get { return score; } }
    public int RekorTertinggi { get { return rekor; } }

    // Otomatis membuat ScoreManager saat game mulai (tanpa perlu setting di Editor)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null)
        {
            new GameObject("ScoreManager", typeof(ScoreManager));
        }
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

    // Tampilkan skor di TENGAH ATAS layar (ukuran menyesuaikan tinggi layar)
    void OnGUI()
    {
        int fontSize = Mathf.RoundToInt(Screen.height * 0.045f); // otomatis pas di HP
        float atas = Screen.height * 0.02f;
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

        // rekor (lebih kecil, di bawah skor)
        int fontKecil = Mathf.RoundToInt(fontSize * 0.55f);
        float atasRekor = atas + tinggi * 0.85f;
        string teksRekor = "Rekor: " + rekor;
        GUIStyle styleKecil = new GUIStyle();
        styleKecil.fontSize = fontKecil;
        styleKecil.fontStyle = FontStyle.Bold;
        styleKecil.alignment = TextAnchor.UpperCenter;
        styleKecil.normal.textColor = new Color(0f, 0f, 0f, 0.6f);
        GUI.Label(new Rect(o, atasRekor + o, lebar, tinggi), teksRekor, styleKecil);
        styleKecil.normal.textColor = new Color(1f, 0.95f, 0.4f, 1f); // kuning emas
        GUI.Label(new Rect(0, atasRekor, lebar, tinggi), teksRekor, styleKecil);
    }
}