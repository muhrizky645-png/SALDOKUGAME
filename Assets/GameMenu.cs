using UnityEngine;

// Menu awal sebelum main. Otomatis dibuat saat game mulai (tanpa setting di Editor).
// Game dibekukan (Time.timeScale = 0) sampai pemain menekan tombol MAIN.
public class GameMenu : MonoBehaviour
{
    public static GameMenu Instance;
    public static bool SedangMain = false; // true setelah tombol MAIN ditekan

    private bool tampil = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null) new GameObject("GameMenu", typeof(GameMenu));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        tampil = true;
        SedangMain = false;
        Time.timeScale = 0f; // bekukan game sampai tekan MAIN
    }

    void Update()
    {
        // paksa game tetap beku selama menu tampil (walau script lain mengubah timeScale)
        if (tampil) Time.timeScale = 0f;
    }

    void Mulai()
    {
        tampil = false;
        SedangMain = true;
        Time.timeScale = 1f; // jalankan game
    }

    void OnGUI()
    {
        if (!tampil) return;

        // panel gelap menutup seluruh layar (menutupi game & HUD di belakang)
        Color simpan = GUI.color;
        GUI.color = new Color(0.04f, 0.12f, 0.06f, 0.92f); // hijau gelap
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = simpan;

        // ---- judul game ----
        int fontJudul = Mathf.RoundToInt(Screen.height * 0.06f);
        GUIStyle judul = new GUIStyle();
        judul.fontSize = fontJudul;
        judul.fontStyle = FontStyle.Bold;
        judul.alignment = TextAnchor.MiddleCenter;
        judul.wordWrap = true;
        judul.normal.textColor = new Color(0f, 0f, 0f, 0.6f); // bayangan
        GUI.Label(new Rect(3, Screen.height * 0.20f + 3, Screen.width, fontJudul * 3f), "SALDOKU\nLAST STAND", judul);
        judul.normal.textColor = new Color(0.5f, 1f, 0.5f, 1f);
        GUI.Label(new Rect(0, Screen.height * 0.20f, Screen.width, fontJudul * 3f), "SALDOKU\nLAST STAND", judul);

        // ---- rekor tertinggi ----
        if (ScoreManager.Instance != null)
        {
            GUIStyle info = new GUIStyle();
            info.fontSize = Mathf.RoundToInt(Screen.height * 0.03f);
            info.fontStyle = FontStyle.Bold;
            info.alignment = TextAnchor.MiddleCenter;
            info.normal.textColor = new Color(1f, 0.95f, 0.4f, 1f);
            GUI.Label(new Rect(0, Screen.height * 0.40f, Screen.width, info.fontSize * 2f),
                "Rekor Tertinggi: " + ScoreManager.Instance.RekorTertinggi, info);
        }

        // ---- tombol MAIN di tengah ----
        float bw = Screen.width * 0.5f;
        float bh = Screen.height * 0.09f;
        float bx = (Screen.width - bw) / 2f;
        float by = Screen.height * 0.5f;

        GUIStyle tombol = new GUIStyle(GUI.skin.button);
        tombol.fontSize = Mathf.RoundToInt(Screen.height * 0.045f);
        tombol.fontStyle = FontStyle.Bold;

        if (GUI.Button(new Rect(bx, by, bw, bh), "MAIN", tombol))
        {
            Mulai();
        }
    }
}
