using UnityEngine;
using UnityEngine.SceneManagement;

// Mengatur Level & XP pemain. XP didapat dari memungut permata (XpGem).
// Otomatis dibuat saat game mulai DAN tiap scene di-reload.
public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance;

    private int level = 1;
    private int xp = 0;
    private int xpUntukNaik = 5;   // XP yang dibutuhkan untuk naik ke level berikutnya

    public int Level { get { return level; } }
    public int Xp { get { return xp; } }
    public int XpUntukNaik { get { return xpUntukNaik; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (scene, mode) => Buat();
    }

    static void Buat()
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
        bool naik = false;
        while (xp >= xpUntukNaik)
        {
            xp -= xpUntukNaik;
            level++;
            xpUntukNaik = Mathf.RoundToInt(xpUntukNaik * 1.3f) + 2; // tiap level butuh lebih banyak XP
            naik = true;
        }
        if (naik) SoundManager.LevelUp(); // suara naik level (pengumuman \"LEVEL UP\" ada di kartu skill)
    }

    // HUD Level + bar XP, pojok KIRI atas (tema survival)
    void OnGUI()
    {
        // sembunyikan HUD selama menu awal / jeda / saat memilih skill / SAAT GAME OVER
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float h = Screen.height;
        float pad = h * 0.02f;

        // Panel dibuat lebih lebar & font lebih kecil supaya \"LEVEL x\" muat SATU baris (tidak turun & menabrak bar).
        int fLv = Mathf.RoundToInt(h * 0.022f);
        float pW = Screen.width * 0.34f;
        float pH = fLv * 3.2f;
        float pX = pad;
        float pY = pad;

        // panel level
        Tema.Panel9(new Rect(pX, pY, pW, pH), Tema.Plate, Tema.GarisRedup, 2f);

        // teks \"LEVEL x\" di bagian ATAS panel (satu baris)
        Tema.Teks(new Rect(pX + pW * 0.06f, pY + pH * 0.10f, pW * 0.88f, pH * 0.42f),
            "LEVEL " + level, fLv, Tema.Army, TextAnchor.UpperLeft, true);

        // bar XP di bagian BAWAH panel, ada jarak jelas dari teks -> tidak menabrak
        float barX = pX + pW * 0.06f;
        float barW = pW * 0.88f;
        float barH = pH * 0.20f;
        float barY = pY + pH * 0.68f;
        float ratio = (xpUntukNaik > 0) ? Mathf.Clamp01((float)xp / xpUntukNaik) : 0f;
        Tema.Panel9(new Rect(barX, barY, barW, barH), new Color(0f, 0f, 0f, 0.55f), Tema.GarisRedup, 1f);
        Tema.Kotak(new Rect(barX + 1f, barY + 1f, (barW - 2f) * ratio, barH - 2f),
            new Color(0.45f, 0.95f, 1f, 0.95f));
    }
}
