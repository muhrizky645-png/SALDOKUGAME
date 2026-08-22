using UnityEngine;
using UnityEngine.SceneManagement;

// Mengukur berapa lama pemain bertahan + menampilkan timer (sejajar kotak skor,
// mepet kanan) dan bar nyawa BOSS saat boss hidup. Otomatis dibuat & di-reset tiap scene.
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
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float w = Screen.width, h = Screen.height;
        float atas = Tema.AmanAtas; // hormati poni/notch
        float pad = Tema.Pad;

        // ==== TIMER (mepet KANAN, SEJAJAR dgn kotak skor, ukuran ~= skor) ====
        int m = (int)(Detik / 60f);
        int s = (int)(Detik % 60f);
        string txt = string.Format("{0:00}:{1:00}", m, s);

        // hitung posisi vertikal kotak SKOR (harus identik dgn ScoreManager)
        float levelBawah = atas + pad + LevelSystem.TinggiPanel(w);
        int fSkor = Mathf.Min(Tema.Font(0.05f), Mathf.RoundToInt(w * 0.10f));
        float skorH = fSkor * 1.7f;
        float skorY = levelBawah + pad * 0.6f;
        float skorCenter = skorY + skorH / 2f;

        // timer: sedikit lebih besar dari skor, tengahnya disamakan dgn skor
        int fTimer = Mathf.Min(Tema.Font(0.052f), Mathf.RoundToInt(w * 0.105f));
        float tH = fTimer * 1.7f;
        float tW = fTimer * 3.6f; // cukup utk "00:00"
        float tX = w - pad - Tema.AmanKanan - tW;
        float tY = skorCenter - tH / 2f;
        Tema.Panel9(new Rect(tX, tY, tW, tH), Tema.Plate, Tema.GarisRedup, 2f);
        Tema.Teks(new Rect(tX, tY, tW, tH), txt, fTimer, Tema.Tulang, TextAnchor.MiddleCenter, true);

        // ==== BAR NYAWA BOSS (tengah, di bawah baris HUD atas) ====
        // Disembunyikan saat overlay Peti Dewa terbuka biar tidak menimpa panelnya.
        if (EnemyChase.JumlahBos > 0 && EnemyChase.BosSaatIni != null && !ModeDewa.MenuTerbuka)
        {
            float bossY = atas + h * 0.150f;
            Tema.Teks(new Rect(0, bossY, w, h * 0.035f), "! B O S S !",
                Mathf.Min(Tema.Font(0.030f), Mathf.RoundToInt(w * 0.06f)), Tema.Darah, TextAnchor.MiddleCenter, true);
            float bw = w * 0.7f, bh = h * 0.026f, bx = (w - bw) / 2f, by = bossY + h * 0.040f;
            Tema.Panel9(new Rect(bx, by, bw, bh), new Color(0f, 0f, 0f, 0.55f), Tema.GarisRedup, 1f);
            float r = Mathf.Clamp01((float)EnemyChase.BosSaatIni.NyawaSisa /
                Mathf.Max(1, EnemyChase.BosSaatIni.NyawaMaks));
            Tema.Kotak(new Rect(bx + 1, by + 1, (bw - 2) * r, bh - 2), Tema.Darah);
        }
    }
}
