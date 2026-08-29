using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Menawarkan 3 kartu skill acak setiap kali pemain naik level (mirip Survivor.io).
// Game berhenti sejenak (pause) saat memilih, lalu lanjut setelah dipilih.
// Dibuat otomatis saat game mulai DAN tiap scene di-reload (biar tetap muncul setelah restart).
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    // dibaca HUD & GameMenu supaya sembunyi saat kartu skill tampil
    public static bool AktifMemilih = false;

    private List<Skill> semuaSkill = new List<Skill>();
    private List<Skill> pilihanSekarang = new List<Skill>();
    // menyimpan berapa kali tiap skill sudah diambil (untuk keterangan level di kartu)
    private Dictionary<string, int> tingkat = new Dictionary<string, int>();
    private bool sedangMemilih = false;
    private int levelTerakhir = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        // PENTING: RuntimeInitialize cuma jalan sekali. Supaya SkillManager tetap ada
        // setiap kali scene di-reload (restart / main lagi), buat ulang lewat sceneLoaded.
        SceneManager.sceneLoaded += (scene, mode) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("SkillManager", typeof(SkillManager));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // reset semua state biar benar-benar mulai dari awal setiap game baru
        sedangMemilih = false;
        AktifMemilih = false;
        levelTerakhir = 1;
        tingkat.Clear();
        XpGem.MagnetMult = 1f; // reset pengali magnet
        BuatDaftarSkill();
    }

    void BuatDaftarSkill()
    {
        semuaSkill = new List<Skill>()
        {
            new Skill("Serang Lebih Cepat", "Kecepatan tembak +20%", "petir", 0, () => {
                PlayerShooting ps = FindFirstObjectByType<PlayerShooting>();
                if (ps != null) ps.fireRate *= 0.80f;
            }),
            new Skill("Peluru Tambahan", "+1 peluru tiap tembak", "peluru", 0, () => {
                PlayerShooting ps = FindFirstObjectByType<PlayerShooting>();
                if (ps != null) ps.jumlahPeluru += 1;
            }),
            new Skill("Jangkauan Lebih Jauh", "Jarak tembak +25%", "target", 0, () => {
                PlayerShooting ps = FindFirstObjectByType<PlayerShooting>();
                if (ps != null) ps.range *= 1.25f;
            }),
            new Skill("Kaki Lebih Cepat", "Kecepatan lari +15%", "chevron", 0, () => {
                PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
                if (pm != null) pm.moveSpeed *= 1.15f;
            }),
            new Skill("Badan Lebih Kuat", "Max HP +30 & pulih", "hati", 0, () => {
                PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
                if (ph != null) { ph.maxHealth += 30f; ph.health = Mathf.Min(ph.health + 30f, ph.maxHealth); }
            }),
            new Skill("Magnet Permata", "Jarak tarik permata +40%", "berlian", 0, () => {
                XpGem.MagnetMult *= 1.4f;
            }),

            // ===== SENJATA OTOMATIS (ala Survivor.io) - evolusi otomatis di level 5+ =====
            new Skill("Pisau Berputar", "Bilah berputar melukai musuh. Lv5: evolusi!", "pisau", SenjataManager.MAX, () => {
                if (SenjataManager.Instance != null) SenjataManager.Instance.TambahOrbit();
            }),
            new Skill("Aura Setrum", "Medan setrum di sekitarmu. Lv5: evolusi!", "aura", SenjataManager.MAX, () => {
                if (SenjataManager.Instance != null) SenjataManager.Instance.TambahAura();
            }),
            new Skill("Roket Pelacak", "Jumlah roket = levelnya! Lv5: evolusi", "roket", SenjataManager.MAX, () => {
                if (SenjataManager.Instance != null) SenjataManager.Instance.TambahRoket();
            }),
        };
    }

    void Update()
    {
        // jangan tawarkan skill saat menu awal / jeda / game over
        if (!GameMenu.SedangMain || GameMenu.SedangJeda || PlayerHealth.GameOver) return;
        if (sedangMemilih) return;
        if (LevelSystem.Instance == null) return;

        int lv = LevelSystem.Instance.Level;
        if (lv > levelTerakhir)
        {
            levelTerakhir = lv;
            MulaiPilih();
        }
    }

    void MulaiPilih()
    {
        pilihanSekarang.Clear();
        List<Skill> kolam = new List<Skill>(semuaSkill);
        int jumlah = Mathf.Min(3, kolam.Count);
        for (int i = 0; i < jumlah; i++)
        {
            int idx = Random.Range(0, kolam.Count);
            pilihanSekarang.Add(kolam[idx]);
            kolam.RemoveAt(idx);
        }
        sedangMemilih = true;
        AktifMemilih = true;
        Time.timeScale = 0f; // pause saat memilih
    }

    int LevelSaatIni(Skill s)
    {
        int c;
        return tingkat.TryGetValue(s.nama, out c) ? c : 0;
    }

    void Pilih(Skill s)
    {
        if (s.efek != null) s.efek.Invoke();
        // catat kenaikan level skill (untuk keterangan di kartu)
        int cur = LevelSaatIni(s);
        int baru = cur + 1;
        if (s.maks > 0) baru = Mathf.Min(baru, s.maks);
        tingkat[s.nama] = baru;

        SoundManager.LevelUp(); // suara konfirmasi ambil skill
        sedangMemilih = false;
        AktifMemilih = false;
        Time.timeScale = 1f;
        // kalau naik beberapa level sekaligus, Update akan menawarkan lagi otomatis
    }

    void OnGUI()
    {
        if (!sedangMemilih) return;

        float h = Screen.height;
        float w = Screen.width;

        // latar gelap survival
        Tema.LatarGelap();

        // ==== 3 KARTU VERTIKAL SEJAJAR (gaya Survivor.io) ====
        // Tiap kartu: TAB NAMA kuning di atas, label "Baru!" di pojok,
        // IKON besar dalam kotak inset, deskripsi, lalu BINTANG rating.
        float margin = w * 0.035f;
        float gap = w * 0.022f;
        int jumlah = pilihanSekarang.Count;
        if (jumlah < 1) return;
        float totalW = w - margin * 2f;
        float cardW = (totalW - gap * (jumlah - 1)) / jumlah;
        float cardH = Mathf.Min(cardW * 2.35f, h * 0.55f);
        float y = (h - cardH) / 2f + h * 0.045f;

        // ---- HEADER + banner ----
        int fBig = Mathf.RoundToInt(h * 0.050f);
        int fSub = Mathf.RoundToInt(h * 0.028f);
        Tema.Teks(new Rect(0, y - h * 0.165f, w, fBig * 1.3f), "LEVEL UP!", fBig, Tema.Darah, TextAnchor.MiddleCenter, true);
        float banW = w * 0.62f, banH = h * 0.052f, banX = (w - banW) / 2f, banY = y - h * 0.085f;
        Tema.Panel9(new Rect(banX, banY, banW, banH), Tema.Amber, Tema.Garis, 2f);
        Tema.Teks(new Rect(banX, banY, banW, banH), "PILIH SKILL", fSub, new Color(0.15f, 0.09f, 0.02f, 1f), TextAnchor.MiddleCenter, true);

        // font relatif LEBAR kartu
        int fNama = Mathf.RoundToInt(cardW * 0.120f);
        int fDesk = Mathf.RoundToInt(cardW * 0.090f);
        int fBaru = Mathf.RoundToInt(cardW * 0.100f);

        // warna kartu slate + tab kuning (mirip referensi)
        Color bodi = new Color(0.24f, 0.28f, 0.34f, 0.98f);
        Color bodiHover = new Color(0.31f, 0.37f, 0.45f, 1f);
        Color inset = new Color(0.14f, 0.17f, 0.22f, 1f);
        Color txtGelap = new Color(0.13f, 0.08f, 0.02f, 1f);

        Texture2D starTex = Ikon.Dari("bintang", Ikon.Bintang);

        for (int i = 0; i < jumlah; i++)
        {
            float x = margin + i * (cardW + gap);
            Rect cr = new Rect(x, y, cardW, cardH);
            Skill s = pilihanSekarang[i];
            bool hover = cr.Contains(Event.current.mousePosition);

            // ===== BODY KARTU =====
            Tema.Panel9(cr, hover ? bodiHover : bodi, Tema.Garis, Mathf.Max(2f, cardW * 0.02f));

            // ===== TAB NAMA (header kuning di atas) =====
            float tabH = cardH * 0.17f;
            Rect tab = new Rect(cr.x + cardW * 0.05f, cr.y + cardH * 0.02f, cardW * 0.90f, tabH);
            Tema.Panel9(tab, hover ? Tema.PanelTerang : Tema.Amber, Tema.Garis, 1.5f);
            Tema.Teks(new Rect(tab.x + 3, tab.y, tab.width - 6, tab.height), s.nama, fNama, txtGelap, TextAnchor.MiddleCenter, true);

            // ===== LABEL "Baru!" / "Lv x" / "MAX!" di POJOK KANAN ATAS =====
            int cur = LevelSaatIni(s);
            string lbl; Color lblCol;
            if (s.maks > 0 && cur >= s.maks) { lbl = "MAX!"; lblCol = Tema.Darah; }
            else if (cur == 0) { lbl = "Baru!"; lblCol = Tema.Army; }
            else { lbl = "Lv " + (cur + 1); lblCol = Tema.Amber; }
            float lblW = lbl.Length * fBaru * 0.60f + fBaru * 1.1f;
            float lblH = fBaru * 1.7f;
            Rect lblR = new Rect(cr.xMax - lblW - cardW * 0.03f, cr.y - lblH * 0.55f, lblW, lblH);
            Tema.Panel9(lblR, lblCol, Tema.Garis, 1.5f);
            Tema.Teks(lblR, lbl, fBaru, txtGelap, TextAnchor.MiddleCenter, true);

            // ===== IKON dalam INSET gelap =====
            float insz = cardW * 0.72f;
            float insx = cr.x + (cardW - insz) / 2f;
            float insy = cr.y + tabH + cardH * 0.045f;
            Tema.Panel9(new Rect(insx, insy, insz, insz), inset, Tema.GarisRedup, 1.5f);
            float ik = insz * 0.76f;
            Texture2D ikTex = Ikon.Dari(s.ikon, Ikon.UntukSkill(s.ikon));
            Ikon.Gambar(new Rect(insx + (insz - ik) / 2f, insy + (insz - ik) / 2f, ik, ik),
                ikTex, hover ? Tema.Amber : Tema.Army);

            // ===== DESKRIPSI =====
            float dy = insy + insz + cardH * 0.03f;
            Tema.Teks(new Rect(cr.x + cardW * 0.07f, dy, cardW * 0.86f, cardH * 0.26f),
                s.deskripsi, fDesk, Tema.Tulang, TextAnchor.UpperCenter, false);

            // ===== BINTANG RATING (bawah) =====
            int totalStar = (s.maks > 0) ? s.maks : 5;
            totalStar = Mathf.Clamp(totalStar, 1, 5);
            int filled;
            if (s.maks > 0 && cur >= s.maks) filled = totalStar;
            else filled = Mathf.Clamp(cur + 1, 1, totalStar);

            float ssz = cardW * 0.145f;
            float sgap = ssz * 0.16f;
            float rowW = totalStar * ssz + (totalStar - 1) * sgap;
            float sx = cr.x + (cardW - rowW) / 2f;
            float sy = cr.yMax - cardH * 0.105f;
            Color gc = GUI.color;
            for (int k = 0; k < totalStar; k++)
            {
                Rect sr = new Rect(sx + k * (ssz + sgap), sy, ssz, ssz);
                GUI.color = (k < filled) ? Color.white : new Color(1f, 1f, 1f, 0.20f);
                GUI.DrawTexture(sr, starTex);
            }
            GUI.color = gc;

            // ===== KLIK / SENTUH untuk pilih =====
            if (GUI.Button(cr, "", GUIStyle.none))
            {
                Pilih(s);
            }
        }
    }

    // struktur data satu skill
    private class Skill
    {
        public string nama;
        public string deskripsi;
        public string ikon;
        public int maks; // 0 = tak terbatas
        public System.Action efek;
        public Skill(string n, string d, string ik, int mk, System.Action e)
        { nama = n; deskripsi = d; ikon = ik; maks = mk; efek = e; }
    }
}
