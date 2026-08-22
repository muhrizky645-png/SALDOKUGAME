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

        // ukuran kartu: 3 kartu persegi sejajar di tengah
        float margin = w * 0.05f;
        float gap = w * 0.03f;
        float totalW = w - margin * 2f;
        float cardW = (totalW - gap * 2f) / 3f;
        float cardH = cardW * 1.32f;
        float y = (h - cardH) / 2f;

        // ---- HEADER GABUNGAN (tidak lagi tabrakan) ----
        int fBig = Mathf.RoundToInt(h * 0.055f);
        int fSub = Mathf.RoundToInt(h * 0.032f);
        float headY = y - h * 0.22f;
        Tema.Teks(new Rect(0, headY, w, fBig * 1.4f), "LEVEL UP!", fBig, Tema.Darah, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(0, headY + fBig * 1.25f, w, fSub * 1.6f), "PILIH SKILL", fSub, Tema.Army, TextAnchor.MiddleCenter, true);

        int fNama = Mathf.RoundToInt(cardW * 0.11f);
        int fDesk = Mathf.RoundToInt(cardW * 0.088f);
        int fLevel = Mathf.RoundToInt(cardW * 0.095f);

        for (int i = 0; i < pilihanSekarang.Count; i++)
        {
            float x = margin + i * (cardW + gap);
            Rect cr = new Rect(x, y, cardW, cardH);
            Skill s = pilihanSekarang[i];

            bool hover = cr.Contains(Event.current.mousePosition);

            // kartu bertema
            Tema.Panel9(cr, hover ? Tema.PanelTerang : Tema.Panel, hover ? Tema.Army : Tema.Garis, Mathf.Max(2f, cardW * 0.02f));
            Tema.StripAtas(cr, Tema.Army, cardH * 0.045f); // strip aksen di atas

            // IKON skill (dibuat lewat kode)
            float isz = cardH * 0.26f;
            Ikon.Gambar(new Rect(cr.x + (cardW - isz) / 2f, cr.y + cardH * 0.07f, isz, isz),
                Ikon.UntukSkill(s.ikon), hover ? Tema.Amber : Tema.Army);

            // KETERANGAN LEVEL skill (posisi level saat ini)
            int cur = LevelSaatIni(s);
            string tk;
            if (s.maks > 0 && cur >= s.maks) tk = "Lv MAKS (" + s.maks + ")";
            else if (cur == 0) tk = "BARU  >  Lv 1";
            else tk = "Lv " + cur + "  >  " + (cur + 1);
            Tema.Teks(new Rect(cr.x + 4, cr.y + cardH * 0.35f, cr.width - 8, cardH * 0.09f),
                tk, fLevel, Tema.Amber, TextAnchor.MiddleCenter, true);

            // nama skill (hijau army) + deskripsi (putih tulang)
            Tema.Teks(new Rect(cr.x + 6, cr.y + cardH * 0.45f, cr.width - 12, cardH * 0.22f),
                s.nama, fNama, Tema.Army, TextAnchor.MiddleCenter, true);
            Tema.Teks(new Rect(cr.x + 6, cr.y + cardH * 0.68f, cr.width - 12, cardH * 0.30f),
                s.deskripsi, fDesk, Tema.Tulang, TextAnchor.MiddleCenter, false);

            // tombol transparan di atas kartu untuk deteksi klik/sentuh
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
