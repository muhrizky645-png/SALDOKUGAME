using UnityEngine;

// Combo / kill-streak counter. Auto-bootstrap, digambar via OnGUI. Nol asset.
[DefaultExecutionOrder(9000)]
public class ComboMeter : MonoBehaviour
{
    static ComboMeter Instance;

    int combo = 0;
    int comboTertinggi = 0;
    float tKillTerakhir = -999f;
    const float JENDELA = 2.5f; // detik tanpa kill -> combo reset

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("ComboMeter");
        Instance = go.AddComponent<ComboMeter>();
        DontDestroyOnLoad(go);
    }

    // dipanggil tiap musuh mati
    public static void Tambah()
    {
        if (Instance == null) return;
        Instance.combo++;
        Instance.tKillTerakhir = Time.time;
        if (Instance.combo > Instance.comboTertinggi) Instance.comboTertinggi = Instance.combo;
    }

    public static void Reset()
    {
        if (Instance != null) Instance.combo = 0;
    }

    public static int Sekarang { get { return Instance != null ? Instance.combo : 0; } }
    public static int Tertinggi { get { return Instance != null ? Instance.comboTertinggi : 0; } }

    void Update()
    {
        // combo hangus kalau tak ada kill dalam JENDELA detik
        if (combo > 0 && Time.time - tKillTerakhir > JENDELA) combo = 0;
    }

    void OnGUI()
    {
        if (combo < 2) return;
        if (!GameMenu.SedangMain) return;

        float sisa = Mathf.Clamp01(1f - (Time.time - tKillTerakhir) / JENDELA);
        int fontBesar = Mathf.RoundToInt(Screen.height * 0.045f);

        // makin banyak combo, warna makin "panas"
        Color c = combo >= 30 ? new Color(1f, 0.35f, 0.2f)
                : combo >= 15 ? new Color(1f, 0.7f, 0.15f)
                : new Color(1f, 0.95f, 0.4f);

        string teks = "COMBO x" + combo;
        GUIStyle style = new GUIStyle();
        style.fontSize = fontBesar;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;

        float w = Screen.width * 0.6f;
        float x = (Screen.width - w) * 0.5f;
        float y = Screen.height * 0.13f;
        Rect r = new Rect(x, y, w, fontBesar * 1.4f);

        // bayangan
        style.normal.textColor = new Color(0f, 0f, 0f, 0.6f);
        GUI.Label(new Rect(r.x + 2f, r.y + 2f, r.width, r.height), teks, style);
        // teks utama
        style.normal.textColor = c;
        GUI.Label(r, teks, style);

        // bar sisa waktu combo
        float barW = w * 0.45f;
        float bx = (Screen.width - barW) * 0.5f;
        float by = y + fontBesar * 1.4f;
        Color simpan = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.4f);
        GUI.DrawTexture(new Rect(bx, by, barW, 5f), Texture2D.whiteTexture);
        GUI.color = c;
        GUI.DrawTexture(new Rect(bx, by, barW * sisa, 5f), Texture2D.whiteTexture);
        GUI.color = simpan;
    }
}
