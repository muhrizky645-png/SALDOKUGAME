using UnityEngine;

// Mengatur Level & XP pemain. XP didapat dari memungut permata (XpGem).
// Otomatis dibuat saat game mulai, tanpa perlu setting di Editor.
public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance;

    private int level = 1;
    private int xp = 0;
    private int xpUntukNaik = 5;   // XP yang dibutuhkan untuk naik ke level berikutnya
    private float levelUpFlash = 0f;

    public int Level { get { return level; } }
    public int Xp { get { return xp; } }
    public int XpUntukNaik { get { return xpUntukNaik; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null) new GameObject("LevelSystem", typeof(LevelSystem));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Dipanggil XpGem saat permata dipungut
    public void AddXp(int jumlah)
    {
        xp += jumlah;
        while (xp >= xpUntukNaik)
        {
            xp -= xpUntukNaik;
            level++;
            xpUntukNaik = Mathf.RoundToInt(xpUntukNaik * 1.3f) + 2; // tiap level butuh lebih banyak XP
            levelUpFlash = 1.5f;
        }
    }

    void Update()
    {
        if (levelUpFlash > 0f) levelUpFlash -= Time.unscaledDeltaTime;
    }

    void OnGUI()
    {
        int fontSize = Mathf.RoundToInt(Screen.height * 0.032f);
        float pad = Screen.height * 0.02f;
        string teks = "Level " + level;

        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = new Color(0f, 0f, 0f, 0.6f);
        GUI.Label(new Rect(pad + 2, pad + 2, 400, fontSize * 2), teks, style);
        style.normal.textColor = new Color(0.5f, 1f, 0.5f, 1f);
        GUI.Label(new Rect(pad, pad, 400, fontSize * 2), teks, style);

        // bar XP di bawah tulisan Level
        float barW = Screen.width * 0.4f;
        float barH = Screen.height * 0.018f;
        float barX = pad;
        float barY = pad + fontSize * 1.5f;
        float ratio = (xpUntukNaik > 0) ? Mathf.Clamp01((float)xp / xpUntukNaik) : 0f;

        Color simpan = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.5f);
        GUI.DrawTexture(new Rect(barX, barY, barW, barH), Texture2D.whiteTexture);
        GUI.color = new Color(0.4f, 1f, 1f, 1f);
        GUI.DrawTexture(new Rect(barX, barY, barW * ratio, barH), Texture2D.whiteTexture);
        GUI.color = simpan;

        if (levelUpFlash > 0f)
        {
            GUIStyle big = new GUIStyle();
            big.fontSize = Mathf.RoundToInt(Screen.height * 0.06f);
            big.fontStyle = FontStyle.Bold;
            big.alignment = TextAnchor.MiddleCenter;
            float a = Mathf.Clamp01(levelUpFlash);
            big.normal.textColor = new Color(1f, 0.9f, 0.2f, a);
            GUI.Label(new Rect(0, Screen.height * 0.28f, Screen.width, big.fontSize * 1.6f), "LEVEL UP!", big);
        }
    }
}