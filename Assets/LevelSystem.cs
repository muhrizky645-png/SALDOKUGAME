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

    public void AddXp(int jumlah)
    {
        xp += jumlah;
        bool naik = false;
        while (xp >= xpUntukNaik)
        {
            xp -= xpUntukNaik;
            level++;
            xpUntukNaik = Mathf.RoundToInt(xpUntukNaik * 1.3f) + 2;
            naik = true;
        }
        if (naik) SoundManager.LevelUp();
    }

    // Tinggi panel level (dipakai juga ScoreManager utk menaruh skor tepat di bawahnya).
    public static float TinggiPanel(float w)
    {
        int fLv = Mathf.Min(Tema.Font(0.030f), Mathf.RoundToInt(w * 0.055f));
        return fLv * 4.6f;
    }

    // HUD Level + bar XP, pojok KIRI atas, MEMANJANG ke arah tombol jeda (kanan).
    // BARIS ATAS panel kini = BAR NYAWA (HP) yang menyatu: bar penuh selebar panel,
    // tulisan "LEVEL x" MENIMPA (kepotong) di kiri, ikon HATI merah di ujung kiri,
    // angka HP di kanan. BARIS BAWAH tetap bar XP biru.
    void OnGUI()
    {
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float w = Screen.width, h = Screen.height;

        int fLv = Mathf.Min(Tema.Font(0.030f), Mathf.RoundToInt(w * 0.055f));
        float pX = Tema.AmanKiri + Tema.Pad;
        float pY = Tema.AmanAtas + Tema.Pad;

        // panjangkan panel level sampai DEKAT tombol jeda (kanan atas)
        float jedaSz = Tema.Unit * 0.09f;
        float jedaKiri = w - jedaSz - Tema.Pad - Tema.AmanKanan;
        float pW = Mathf.Max(w * 0.34f, (jedaKiri - Tema.Pad) - pX);
        float pH = fLv * 4.6f; // lebih tinggi ke bawah

        Tema.Panel9(new Rect(pX, pY, pW, pH), Tema.Plate, Tema.GarisRedup, 2f);

        // ================= BARIS ATAS: BAR NYAWA (HP) =================
        float rowX = pX + pW * 0.03f;
        float rowW = pW * 0.94f;
        float hpH  = pH * 0.36f;
        float hpY  = pY + pH * 0.09f;

        PlayerHealth ph = PlayerHealth.Instance;
        float hpRatio = (ph != null && ph.maxHealth > 0f)
            ? Mathf.Clamp01(ph.health / ph.maxHealth) : 1f;

        // backing rounded gelap (senada bar XP di bawah)
        Tema.Panel9(new Rect(rowX, hpY, rowW, hpH), new Color(0f, 0f, 0f, 0.55f), Tema.GarisRedup, 1f);

        // isi bar warna DINAMIS (hijau penuh -> kuning -> merah saat kritis)
        Color warnaHp = (hpRatio > 0.5f)
            ? Color.Lerp(Tema.Amber, Tema.Army, (hpRatio - 0.5f) * 2f)
            : Color.Lerp(Tema.Darah, Tema.Amber, hpRatio * 2f);
        if (hpRatio > 0f)
            Tema.BarIsi(new Rect(rowX + 2f, hpY + 2f, (rowW - 4f) * hpRatio, hpH - 4f), warnaHp);

        // ikon HATI merah menonjol di ujung kiri
        float ik = hpH * 1.7f;
        float heartX = rowX - ik * 0.28f;

        // angka HP di KANAN bar (tidak bentrok dgn tulisan LEVEL di kiri)
        if (ph != null)
        {
            int hpNow = Mathf.CeilToInt(ph.health);
            int hpMax = Mathf.RoundToInt(ph.maxHealth);
            int fHp = Mathf.Min(Tema.Font(0.020f), Mathf.RoundToInt(hpH * 0.85f));
            Tema.Teks(new Rect(rowX, hpY, rowW - hpH * 0.35f, hpH), hpNow + " / " + hpMax,
                fHp, Tema.Tulang, TextAnchor.MiddleRight, true);
        }

        // tulisan "LEVEL x" MENIMPA bar (terlihat kepotong) di KIRI, setelah ikon hati
        float lvX = rowX + ik * 0.80f;
        Tema.Teks(new Rect(lvX, hpY, rowW - ik * 0.80f, hpH),
            "LEVEL " + level, fLv, Tema.Tulang, TextAnchor.MiddleLeft, true);

        // gambar ikon HATI paling akhir supaya di atas bar & teks
        Ikon.Gambar(new Rect(heartX, hpY + hpH / 2f - ik / 2f, ik, ik), Ikon.Hati, Tema.Darah);

        // ================= BARIS BAWAH: BAR XP =================
        float barX = pX + pW * 0.03f;
        float barW = pW * 0.94f;
        float barH = pH * 0.30f;
        float barY = pY + pH * 0.58f;
        float ratio = (xpUntukNaik > 0) ? Mathf.Clamp01((float)xp / xpUntukNaik) : 0f;
        Tema.Panel9(new Rect(barX, barY, barW, barH), new Color(0f, 0f, 0f, 0.55f), Tema.GarisRedup, 1f);
        Tema.BarIsi(new Rect(barX + 1f, barY + 1f, (barW - 2f) * ratio, barH - 2f),
            new Color(0.40f, 0.90f, 1f, 0.98f));
    }
}
