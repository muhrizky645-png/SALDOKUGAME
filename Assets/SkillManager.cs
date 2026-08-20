using UnityEngine;
using System.Collections.Generic;

// Menawarkan 3 kartu skill acak setiap kali pemain naik level (mirip Survivor.io).
// Game berhenti sejenak (pause) saat memilih, lalu lanjut setelah dipilih.
// Dibuat otomatis saat game mulai, tanpa perlu setting di Editor.
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    private List<Skill> semuaSkill = new List<Skill>();
    private List<Skill> pilihanSekarang = new List<Skill>();
    private bool sedangMemilih = false;
    private int levelTerakhir = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null) new GameObject("SkillManager", typeof(SkillManager));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        XpGem.MagnetMult = 1f; // reset pengali magnet saat mulai
        BuatDaftarSkill();
    }

    void BuatDaftarSkill()
    {
        semuaSkill = new List<Skill>()
        {
            new Skill("Serang Lebih Cepat", "Kecepatan tembak +15%", () => {
                PlayerShooting ps = FindFirstObjectByType<PlayerShooting>();
                if (ps != null) ps.fireRate *= 0.85f;
            }),
            new Skill("Peluru Tambahan", "+1 peluru tiap tembak", () => {
                PlayerShooting ps = FindFirstObjectByType<PlayerShooting>();
                if (ps != null) ps.jumlahPeluru += 1;
            }),
            new Skill("Jangkauan Lebih Jauh", "Jarak tembak +20%", () => {
                PlayerShooting ps = FindFirstObjectByType<PlayerShooting>();
                if (ps != null) ps.range *= 1.2f;
            }),
            new Skill("Kaki Lebih Cepat", "Kecepatan lari +12%", () => {
                PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
                if (pm != null) pm.moveSpeed *= 1.12f;
            }),
            new Skill("Badan Lebih Kuat", "Max HP +20 & pulih", () => {
                PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
                if (ph != null) { ph.maxHealth += 20f; ph.health = Mathf.Min(ph.health + 20f, ph.maxHealth); }
            }),
            new Skill("Magnet Permata", "Jarak tarik permata +30%", () => {
                XpGem.MagnetMult *= 1.3f;
            }),
        };
    }

    void Update()
    {
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
        Time.timeScale = 0f; // pause saat memilih
    }

    void Pilih(Skill s)
    {
        if (s.efek != null) s.efek.Invoke();
        sedangMemilih = false;
        Time.timeScale = 1f;
        // kalau naik beberapa level sekaligus, Update akan menawarkan lagi otomatis
    }

    void OnGUI()
    {
        if (!sedangMemilih) return;

        // latar gelap transparan
        Color simpan = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.75f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = simpan;

        // judul
        GUIStyle judul = new GUIStyle();
        judul.fontSize = Mathf.RoundToInt(Screen.height * 0.045f);
        judul.fontStyle = FontStyle.Bold;
        judul.alignment = TextAnchor.MiddleCenter;
        judul.normal.textColor = new Color(1f, 0.9f, 0.2f, 1f);
        GUI.Label(new Rect(0, Screen.height * 0.12f, Screen.width, judul.fontSize * 2f), "PILIH SKILL!", judul);

        // kartu-kartu
        float cardW = Screen.width * 0.8f;
        float cardH = Screen.height * 0.13f;
        float startY = Screen.height * 0.28f;
        float gap = Screen.height * 0.04f;
        float x = (Screen.width - cardW) / 2f;

        GUIStyle kartu = new GUIStyle(GUI.skin.button);
        kartu.fontSize = Mathf.RoundToInt(Screen.height * 0.028f);
        kartu.fontStyle = FontStyle.Bold;
        kartu.alignment = TextAnchor.MiddleCenter;
        kartu.wordWrap = true;

        for (int i = 0; i < pilihanSekarang.Count; i++)
        {
            float y = startY + i * (cardH + gap);
            Skill s = pilihanSekarang[i];
            string label = s.nama + "\n" + s.deskripsi;
            if (GUI.Button(new Rect(x, y, cardW, cardH), label, kartu))
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
        public System.Action efek;
        public Skill(string n, string d, System.Action e) { nama = n; deskripsi = d; efek = e; }
    }
}