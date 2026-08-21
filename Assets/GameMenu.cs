using UnityEngine;
using UnityEngine.SceneManagement;

// Menu awal + menu jeda (pause). Otomatis dibuat saat game mulai DAN tiap scene
// di-reload (tanpa setting di Editor). Game dibekukan (Time.timeScale = 0) selama
// menu awal / menu jeda tampil.
public class GameMenu : MonoBehaviour
{
    public static GameMenu Instance;
    public static bool SedangMain = false;   // true saat sedang bermain (HUD tampil)
    public static bool SedangJeda = false;   // true saat game di-pause lewat menu jeda

    // dipakai saat restart: kalau true, setelah scene reload langsung main (skip menu awal)
    public static bool langsungMainSetelahLoad = false;

    private bool tampilHome = true; // menu awal tampil

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

        if (langsungMainSetelahLoad)
        {
            // dipanggil setelah "Ulangi" / "Main Lagi": langsung main tanpa menu awal
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
        // paksa game tetap beku selama menu awal atau menu jeda tampil
        if (tampilHome || SedangJeda) Time.timeScale = 0f;
    }

    void Mulai()
    {
        tampilHome = false;
        SedangMain = true;
        SedangJeda = false;
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
        // ====== MENU AWAL ======
        if (tampilHome)
        {
            GambarPanel(new Color(0.04f, 0.12f, 0.06f, 0.92f));
            GambarJudul("SALDOKU\nLAST STAND");
            if (ScoreManager.Instance != null)
                GambarInfo("Rekor Tertinggi: " + ScoreManager.Instance.RekorTertinggi, 0.40f);
            if (Tombol("MAIN", 0.5f)) { SoundManager.Klik(); Mulai(); }
            return;
        }

        if (!SedangMain) return;

        // ====== MENU JEDA (PAUSE) ======
        if (SedangJeda)
        {
            GambarPanel(new Color(0f, 0f, 0f, 0.85f));
            GambarJudul("JEDA");
            if (Tombol("LANJUT", 0.40f)) { SoundManager.Klik(); Lanjut(); }
            if (Tombol("ULANGI", 0.55f)) { SoundManager.Klik(); UlangiDanMain(); }
            if (Tombol("KEMBALI KE HOME", 0.70f)) { SoundManager.Klik(); KeHome(); }
            return;
        }

        // ====== TOMBOL PAUSE SAAT MAIN ======
        // sembunyikan kalau sedang Game Over (PlayerHealth yang pegang layar)
        if (PlayerHealth.GameOver) return;

        float sz = Screen.height * 0.055f;
        float pad = Screen.height * 0.02f;
        float x = (Screen.width - sz) / 2f; // tengah atas
        GUIStyle tp = new GUIStyle(GUI.skin.button);
        tp.fontSize = Mathf.RoundToInt(Screen.height * 0.03f);
        tp.fontStyle = FontStyle.Bold;
        if (GUI.Button(new Rect(x, pad, sz, sz), "II", tp)) { SoundManager.Klik(); Jeda(); }
    }

    // ---------- util gambar ----------
    void GambarPanel(Color c)
    {
        Color simpan = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = simpan;
    }

    void GambarJudul(string teks)
    {
        int f = Mathf.RoundToInt(Screen.height * 0.06f);
        GUIStyle st = new GUIStyle();
        st.fontSize = f;
        st.fontStyle = FontStyle.Bold;
        st.alignment = TextAnchor.MiddleCenter;
        st.wordWrap = true;
        st.normal.textColor = new Color(0f, 0f, 0f, 0.6f);
        GUI.Label(new Rect(3, Screen.height * 0.20f + 3, Screen.width, f * 3f), teks, st);
        st.normal.textColor = new Color(0.5f, 1f, 0.5f, 1f);
        GUI.Label(new Rect(0, Screen.height * 0.20f, Screen.width, f * 3f), teks, st);
    }

    void GambarInfo(string teks, float posY)
    {
        GUIStyle st = new GUIStyle();
        st.fontSize = Mathf.RoundToInt(Screen.height * 0.03f);
        st.fontStyle = FontStyle.Bold;
        st.alignment = TextAnchor.MiddleCenter;
        st.normal.textColor = new Color(1f, 0.95f, 0.4f, 1f);
        GUI.Label(new Rect(0, Screen.height * posY, Screen.width, st.fontSize * 2f), teks, st);
    }

    bool Tombol(string teks, float posY)
    {
        float bw = Screen.width * 0.55f;
        float bh = Screen.height * 0.08f;
        float bx = (Screen.width - bw) / 2f;
        float by = Screen.height * posY;
        GUIStyle st = new GUIStyle(GUI.skin.button);
        st.fontSize = Mathf.RoundToInt(Screen.height * 0.04f);
        st.fontStyle = FontStyle.Bold;
        return GUI.Button(new Rect(bx, by, bw, bh), teks, st);
    }
}
