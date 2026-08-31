using UnityEngine;
using UnityEngine.SceneManagement;

// Mengatur Level & XP pemain. XP didapat dari memungut permata (XpGem).
// Otomatis dibuat saat game mulai DAN tiap scene di-reload.
public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance;

    private int level = 1;
    private int xp = 0;
    private int xpUntukNaik = 5; // XP yang dibutuhkan untuk naik ke level berikutnya

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

        // Kurva XP kini berasal dari Balance, bukan dari rumus yang tertanam
        // di dalam AddXp. Lihat Balance.XpUntukLevel untuk alasan penggantian.
        level = 1;
        xp = 0;
        xpUntukNaik = Balance.XpUntukLevel(level);
    }

    public void AddXp(int jumlah)
    {
        xp += jumlah;
        bool naik = false;
        while (xp >= xpUntukNaik)
        {
            xp -= xpUntukNaik;
            level++;
            xpUntukNaik = Balance.XpUntukLevel(level);
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
    // BARIS ATAS panel = "LEVEL x" (TERPISAH di kiri, DIPERBESAR, ruang pas 2 digit)
    // + ikon HATI merah + BAR NYAWA (HP) yang mengisi sisa lebar ke kanan.
    // Tulisan level TIDAK menimpa bar. BARIS BAWAH tetap bar XP biru.
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

        // ============ BARIS ATAS: LEVEL (terpisah) + HATI + BAR NYAWA ============
        float rowX = pX + pW * 0.03f;
        float rowW = pW * 0.94f;
        float hpH = pH * 0.36f;
        float hpY = pY + pH * 0.09f;

        // 1) tulisan "LEVEL x" TERPISAH di kiri, DIPERBESAR, ruang PAS untuk 2 digit
        // (level 10, 88, dst). Karena ruang disesuaikan lebar teks, hati & bar
        // merapat, tidak ada celah kosong lebar seperti sebelumnya.
        int fLvTeks = Mathf.Min(Mathf.RoundToInt(fLv * 1.28f), Mathf.RoundToInt(hpH * 0.92f));
        float charW = fLvTeks * 0.62f; // estimasi lebar 1 huruf (font piksel tebal)
        float lvW = "LEVEL 88".Length * charW; // sediakan ruang untuk level 2 digit
        Tema.Teks(new Rect(rowX, hpY, lvW, hpH), "LEVEL " + level, fLvTeks,
            Tema.Army, TextAnchor.MiddleLeft, true);

        // 2) ikon HATI merah tepat setelah tulisan level (jarak rapat)
        //    Utamakan FILE (Assets/Resources/Icons/hati.png), fallback ke hati-kode.
        float ik = hpH * 1.5f;
        float heartX = rowX + lvW + hpH * 0.06f;
        Ikon.Gambar(new Rect(heartX, hpY + hpH / 2f - ik / 2f, ik, ik), Ikon.Dari("hati", Ikon.Hati), Tema.Darah);

        // 3) BAR NYAWA mengisi sisa lebar setelah hati sampai ujung kanan panel
        float barNX = heartX + ik * 0.95f;
        float barNW = (rowX + rowW) - barNX;

        PlayerHealth ph = PlayerHealth.Instance;
        float hpRatio = (ph != null && ph.maxHealth > 0f)
            ? Mathf.Clamp01(ph.health / ph.maxHealth) : 1f;

        // backing rounded gelap (senada bar XP di bawah)
        Tema.Panel9(new Rect(barNX, hpY, barNW, hpH), new Color(0f, 0f, 0f, 0.55f), Tema.GarisRedup, 1f);

        // isi bar warna DINAMIS (hijau penuh -> kuning -> merah saat kritis)
        Color warnaHp = (hpRatio > 0.5f)
            ? Color.Lerp(Tema.Amber, Tema.Army, (hpRatio - 0.5f) * 2f)
            : Color.Lerp(Tema.Darah, Tema.Amber, hpRatio * 2f);
        if (hpRatio > 0f)
            Tema.BarIsi(new Rect(barNX + 2f, hpY + 2f, (barNW - 4f) * hpRatio, hpH - 4f), warnaHp);

        // angka HP di tengah bar
        if (ph != null)
        {
            int hpNow = Mathf.CeilToInt(ph.health);
            int hpMax = Mathf.RoundToInt(ph.maxHealth);
            int fHp = Mathf.Min(Tema.Font(0.020f), Mathf.RoundToInt(hpH * 0.85f));
            Tema.Teks(new Rect(barNX, hpY, barNW, hpH), hpNow + " / " + hpMax,
                fHp, Tema.Tulang, TextAnchor.MiddleCenter, true);
        }

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
