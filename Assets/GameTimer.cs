using UnityEngine;
using UnityEngine.SceneManagement;

// Mengukur berapa lama pemain bertahan + menampilkan timer (tengah atas)
// dan bar nyawa BOSS saat boss hidup. Otomatis dibuat & di-reset tiap scene.
public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;
    public static float Detik = 0f; // lama bertahan (detik) - dibaca Spawner & musuh

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("GameTimer", typeof(GameTimer));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Detik = 0f;
        // reset status boss tiap scene baru biar tidak nyangkut
        EnemyChase.JumlahBos = 0;
        EnemyChase.BosSaatIni = null;
    }

    void Update()
    {
        if (GameMenu.SedangMain && !GameMenu.SedangJeda &&
            !SkillManager.AktifMemilih && !PlayerHealth.GameOver)
        {
            Detik += Time.deltaTime;
        }
    }

    void OnGUI()
    {
        // hanya tampil saat benar-benar sedang main
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float w = Screen.width, h = Screen.height;

        // ==== TIMER (tengah atas) ====
        int m = (int)(Detik / 60f);
        int s = (int)(Detik % 60f);
        string txt = string.Format("{0:00}:{1:00}", m, s);
        float tw = w * 0.3f;
        Tema.Teks(new Rect((w - tw) / 2f, h * 0.012f, tw, h * 0.05f), txt,
            Mathf.RoundToInt(h * 0.045f), Tema.Tulang, TextAnchor.MiddleCenter, true);

        // ==== BAR NYAWA BOSS ====
        if (EnemyChase.JumlahBos > 0 && EnemyChase.BosSaatIni != null)
        {
            float bw = w * 0.7f, bh = h * 0.028f, bx = (w - bw) / 2f, by = h * 0.175f;
            Tema.Teks(new Rect(0, by - h * 0.032f, w, h * 0.03f), "! B O S S !",
                Mathf.RoundToInt(h * 0.028f), Tema.Darah, TextAnchor.MiddleCenter, true);
            Tema.Panel9(new Rect(bx, by, bw, bh), new Color(0f, 0f, 0f, 0.55f), Tema.GarisRedup, 1f);
            float r = Mathf.Clamp01((float)EnemyChase.BosSaatIni.NyawaSisa /
                Mathf.Max(1, EnemyChase.BosSaatIni.NyawaMaks));
            Tema.Kotak(new Rect(bx + 1, by + 1, (bw - 2) * r, bh - 2), Tema.Darah);
        }
    }
}
