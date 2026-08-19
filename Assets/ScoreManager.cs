using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int score = 0;

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
    }

    // Panggil ini untuk menambah skor
    public void AddScore(int amount)
    {
        score += amount;
    }

    // Tampilkan skor di layar (cara cepat tanpa setup UI)
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 36;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 15, 500, 60), "Skor: " + score, style);
    }
}