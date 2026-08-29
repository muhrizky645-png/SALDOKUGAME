using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================================
// ZOMBURST - UPGRADE PERMANEN (pakai PERMATA)
// Boost TETAP yang tersimpan antar-run (PlayerPrefs). Diterapkan ke
// komponen pemain SETIAP run mulai, TANPA mengedit script pemain
// (aman dari bug generic): manager ini mencari komponen pemain lalu
// menambah nilainya. Panel toko dibuka dari layar Peta (tombol UPGRADE).
// =====================================================================
public class UpgradePermanen : MonoBehaviour
{
    public static UpgradePermanen Instance;

    public const int MAKS = 4;
    static readonly string[] NAMA = { "MAX HP", "KECEPATAN GERAK", "KECEPATAN TEMBAK", "PELURU EKSTRA" };
    static readonly string[] DESK =
    {
        "+20 HP maksimum per level",
        "+0.4 kecepatan gerak per level",
        "-8% jeda tembak per level",
        "+1 peluru sekali tembak per level"
    };
    static readonly int[] LEVEL_MAKS  = { 8, 6, 6, 3 };
    static readonly int[] HARGA_DASAR = { 60, 60, 80, 150 };
    static readonly int[] HARGA_NAIK  = { 40, 45, 60, 120 };

    const string PP = "upg_perm"; // simpan "l0,l1,l2,l3"
    int[] lvl = new int[MAKS];

    bool terbuka = false;
    string status = "";

    bool sebelumnyaMain = false;
    int terapkanTunda = -1;       // countdown frame sebelum menerapkan bonus

    public bool Terbuka { get { return terbuka; } }
    public void Buka() { terbuka = true; status = ""; }
    public void Tutup() { terbuka = false; status = ""; }

    public int Level(int i) { return (i >= 0 && i < MAKS) ? lvl[i] : 0; }
    int HargaBerikut(int i) { return HARGA_DASAR[i] + HARGA_NAIK[i] * lvl[i]; }
    bool Maks(int i) { return lvl[i] >= LEVEL_MAKS[i]; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }
    static void Buat() { if (Instance == null) new GameObject("UpgradePermanen", typeof(UpgradePermanen)); }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Muat();
        sebelumnyaMain = false;
        terapkanTunda = -1;
    }

    void Muat()
    {
        lvl = new int[MAKS];
        string s = PlayerPrefs.GetString(PP, "");
        if (!string.IsNullOrEmpty(s))
        {
            string[] p = s.Split(',');
            for (int i = 0; i < MAKS && i < p.Length; i++) int.TryParse(p[i], out lvl[i]);
        }
    }

    void Simpan()
    {
        PlayerPrefs.SetString(PP, lvl[0] + "," + lvl[1] + "," + lvl[2] + "," + lvl[3]);
        PlayerPrefs.Save();
    }

    void Update()
    {
        // Deteksi MULAI run (rising edge SedangMain) -> jadwalkan penerapan bonus
        // beberapa frame kemudian supaya Start() pemain & pilihan karakter
        // sudah settle (nilai dasar per-karakter tidak ketimpa).
        bool main = GameMenu.SedangMain;
        if (main && !sebelumnyaMain) terapkanTunda = 3;
        sebelumnyaMain = main;

        if (terapkanTunda >= 0)
        {
            terapkanTunda--;
            if (terapkanTunda < 0) TerapkanBonus();
        }
    }

    // Terapkan bonus ADITIF sekali per run (di atas nilai dasar karakter).
    void TerapkanBonus()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p == null) return;

        if (lvl[0] > 0)
        {
            PlayerHealth ph = p.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.maxHealth += 20f * lvl[0];
                ph.health = ph.maxHealth; // mulai run HP penuh
            }
        }
        if (lvl[1] > 0)
        {
            PlayerMovement pm = p.GetComponent<PlayerMovement>();
            if (pm != null) pm.moveSpeed += 0.4f * lvl[1];
        }
        PlayerShooting ps = p.GetComponent<PlayerShooting>();
        if (ps != null)
        {
            if (lvl[2] > 0)
                ps.fireRate = Mathf.Max(0.3f, ps.fireRate * (1f - 0.08f * lvl[2]));
            if (lvl[3] > 0)
                ps.jumlahPeluru += lvl[3];
        }
    }

    // ====== UI ======
    void OnGUI()
    {
        if (!terbuka) return;
        GambarPanel();
    }

    void GambarPanel()
    {
        GUI.depth = -1000; // paling depan (menutup HUD/menu di belakang)
        float w = Screen.width, h = Screen.height;
        float u = Tema.Unit;

        Tema.LatarGelap(new Color(0.03f, 0.05f, 0.09f, 0.60f));

        float pw = Mathf.Min(w * 0.92f, 900f);
        float ph = Mathf.Min(h * 0.94f, u * 1.35f);
        float px = (w - pw) / 2f, py = (h - ph) / 2f;

        Tema.Kotak(new Rect(px + 7f, py + 9f, pw, ph), new Color(0f, 0f, 0f, 0.38f));
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Panel, Tema.Garis, 3f);

        // ---- HEADER ----
        float headH = u * 0.13f;
        Tema.Kotak(new Rect(px + 3f, py + 3f, pw - 6f, headH), new Color(0.12f, 0.15f, 0.20f, 0.98f));
        Tema.Kotak(new Rect(px, py, pw, 5f), Tema.Army);
        Tema.Kotak(new Rect(px + 3f, py + headH, pw - 6f, 2f), Tema.GarisRedup);
        Tema.Teks(new Rect(px, py, pw, headH), "UPGRADE PERMANEN", Mathf.RoundToInt(u * 0.048f),
            Tema.Army, TextAnchor.MiddleCenter, true);

        float cx = px + pw * 0.06f, cw = pw * 0.88f;
        float yy = py + headH + u * 0.03f;

        // ---- SALDO PERMATA ----
        int permata = (MataUang.Instance != null) ? MataUang.Instance.Permata : 0;
        float saldoH = u * 0.09f;
        Tema.Panel9(new Rect(cx, yy, cw, saldoH), Tema.Plate, Tema.GarisRedup, 2f);
        float ikn = saldoH * 0.55f;
        Ikon.Gambar(new Rect(cx + saldoH * 0.24f, yy + (saldoH - ikn) / 2f, ikn, ikn),
            Ikon.Berlian, new Color(0.78f, 0.5f, 1f));
        Tema.Teks(new Rect(cx + saldoH * 0.95f, yy, cw - saldoH, saldoH),
            "PERMATA: " + permata, Mathf.RoundToInt(u * 0.034f), Tema.Tulang, TextAnchor.MiddleLeft, true);
        yy += saldoH + u * 0.028f;

        // ---- DAFTAR UPGRADE ----
        float rowH = u * 0.155f;
        float rowGap = u * 0.022f;
        for (int i = 0; i < MAKS; i++)
        {
            Rect rr = new Rect(cx, yy, cw, rowH);
            Tema.Panel9(rr, Tema.Plate, Tema.GarisRedup, 2f);

            // tombol beli/harga (kanan) - dihitung dulu
            float bw = u * 0.22f;
            float bh = rowH * 0.6f;
            Rect br = new Rect(rr.xMax - bw - rowH * 0.14f, rr.y + (rowH - bh) / 2f, bw, bh);

            float tx = rr.x + rowH * 0.18f;
            float infoW = br.x - tx - rowH * 0.10f;

            // nama + level (rata-kiri & rata-kanan di baris atas)
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.12f, infoW, rowH * 0.40f), NAMA[i],
                Mathf.RoundToInt(u * 0.032f), Tema.Tulang, TextAnchor.LowerLeft, true);
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.12f, infoW, rowH * 0.40f),
                "Lv " + lvl[i] + "/" + LEVEL_MAKS[i], Mathf.RoundToInt(u * 0.026f),
                Tema.Amber, TextAnchor.LowerRight, true);

            // deskripsi
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.54f, infoW, rowH * 0.34f), DESK[i],
                Mathf.RoundToInt(u * 0.021f), Tema.Redup, TextAnchor.UpperLeft, false);

            if (Maks(i))
            {
                Tema.Teks(br, "MAKS", Mathf.RoundToInt(u * 0.028f), Tema.Army, TextAnchor.MiddleCenter, true);
            }
            else
            {
                int harga = HargaBerikut(i);
                if (GUI.Button(br, harga.ToString(), Tema.GayaTombol(Mathf.RoundToInt(u * 0.030f))))
                {
                    if (MataUang.Instance != null && MataUang.Instance.PakaiPermata(harga))
                    {
                        lvl[i]++; Simpan(); SoundManager.Klik();
                        status = NAMA[i] + " -> Lv " + lvl[i];
                        SoundManager.LevelUp();
                        permata = MataUang.Instance.Permata;
                    }
                    else status = "Permata kurang.";
                }
            }
            yy += rowH + rowGap;
        }

        // ---- STATUS + HINT + TUTUP ----
        if (!string.IsNullOrEmpty(status))
            Tema.Teks(new Rect(cx, py + ph - u * 0.205f, cw, u * 0.04f), status,
                Mathf.RoundToInt(u * 0.024f), Tema.Amber, TextAnchor.MiddleCenter, true);

        Tema.Teks(new Rect(cx, py + ph - u * 0.16f, cw, u * 0.04f),
            "Bonus otomatis aktif tiap mulai run.", Mathf.RoundToInt(u * 0.021f),
            Tema.Redup, TextAnchor.MiddleCenter, false);

        float clw = Mathf.Min(cw * 0.6f, u * 0.5f);
        if (GUI.Button(new Rect(px + pw / 2f - clw / 2f, py + ph - u * 0.10f, clw, u * 0.08f),
            "TUTUP", Tema.GayaTombol(Mathf.RoundToInt(u * 0.032f))))
        {
            SoundManager.Klik(); Tutup();
        }

        // penelan klik di luar panel (digambar terakhir)
        GUI.Button(new Rect(0, 0, w, h), "", GUIStyle.none);
    }
}
