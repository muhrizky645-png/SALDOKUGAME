using UnityEngine;
using UnityEngine.SceneManagement;

// TOKO: beli buff pakai PERMATA + inventaris buff yang bisa dipakai saat MAIN.
// Buff: 0=Bom (musnahkan semua musuh), 1=Pulih HP, 2=Perlambat musuh.
// Pola manager sama seperti yang lain (bootstrap + sceneLoaded singleton).
[DefaultExecutionOrder(-26000)]
public class Toko : MonoBehaviour
{
    public static Toko Instance;

    const string PP_INV = "toko_buff_inv";

    static readonly string[] NAMA  = { "BOM", "PULIH HP", "PERLAMBAT" };
    static readonly string[] DESK  = { "Musnahkan semua musuh",
                                       "Pulihkan 40 HP seketika",
                                       "Perlambat musuh 5 detik" };
    static readonly int[]    HARGA = { 40, 25, 30 };

    int[] inv = new int[3];
    bool  terbuka = false;
    string status = "";

    public bool Terbuka { get { return terbuka; } }
    public void Buka() { terbuka = true; status = ""; }
    public void Tutup() { terbuka = false; status = ""; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }
    static void Buat() { if (Instance == null) new GameObject("Toko", typeof(Toko)); }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Muat();
        EnemyChase.ResetPerlambat(); // buff slow tidak kebawa antar game
    }

    void Muat()
    {
        inv = new int[3];
        string s = PlayerPrefs.GetString(PP_INV, "");
        if (!string.IsNullOrEmpty(s))
        {
            string[] p = s.Split(',');
            for (int i = 0; i < 3 && i < p.Length; i++) int.TryParse(p[i], out inv[i]);
        }
    }

    void Simpan()
    {
        PlayerPrefs.SetString(PP_INV, inv[0] + "," + inv[1] + "," + inv[2]);
        PlayerPrefs.Save();
    }

    Texture2D IkonBuff(int i)
    {
        switch (i)
        {
            case 0:  return Ikon.UntukItem("bom");
            case 1:  return Ikon.UntukSkill("hati");
            default: return Ikon.UntukSkill("aura");
        }
    }

    void OnGUI()
    {
        if (terbuka) { GambarPanel(); return; }

        if (GameMenu.SedangMain && !GameMenu.SedangJeda &&
            !SkillManager.AktifMemilih && !PlayerHealth.GameOver &&
            (Saldoku.Instance == null || !Saldoku.Instance.Terbuka))
        {
            GambarInv();
        }
    }

    void GambarInv()
    {
        float slot = Mathf.Min(Screen.width * 0.135f, 88f);
        float gap = slot * 0.20f;
        float totalH = 3f * slot + 2f * gap;
        float sx = Screen.width - slot - Tema.AmanKanan - Tema.Pad;
        float sy = Screen.height * 0.5f - totalH * 0.5f;

        for (int i = 0; i < 3; i++)
        {
            Rect rr = new Rect(sx, sy + i * (slot + gap), slot, slot);
            bool ada = inv[i] > 0;
            Tema.Panel9(rr, ada ? Tema.Plate : new Color(0.05f, 0.06f, 0.04f, 0.5f),
                ada ? Tema.GarisRedup : new Color(0.3f, 0.33f, 0.2f, 0.5f), 2f);

            Color prev = GUI.color;
            if (!ada) GUI.color = new Color(1f, 1f, 1f, 0.35f);
            Ikon.Gambar(new Rect(rr.x + slot * 0.16f, rr.y + slot * 0.10f, slot * 0.68f, slot * 0.68f),
                IkonBuff(i), ada ? Tema.Army : Tema.Redup);
            GUI.color = prev;

            Rect badge = new Rect(rr.xMax - slot * 0.42f, rr.yMax - slot * 0.36f, slot * 0.40f, slot * 0.30f);
            Tema.Panel9(badge, new Color(0.05f, 0.06f, 0.04f, 0.95f), Tema.GarisRedup, 1f);
            Tema.Teks(badge, "x" + inv[i], Mathf.RoundToInt(slot * 0.24f),
                ada ? Tema.Amber : Tema.Redup, TextAnchor.MiddleCenter, true);

            if (ada && GUI.Button(rr, "", GUIStyle.none)) Pakai(i);
        }
    }

    void GambarPanel()
    {
        // === Panel Toko HARUS paling DEPAN (menutup HUD game). ===
        // Toko pakai DefaultExecutionOrder -26000 -> OnGUI-nya jalan LEBIH DULU, jadi
        // HUD (permata, level, pause) yang digambar belakangan menimpanya. Di IMGUI,
        // depth lebih KECIL = digambar paling depan. Overlay gelap di bawah ini lalu
        // menutup seluruh HUD.
        GUI.depth = -1000;

        float w = Screen.width, h = Screen.height;
        // Basis font = sisi TERPENDEK layar biar teks tidak meluber di HP potrait.
        float u = Tema.Unit;

        Tema.LatarGelap(new Color(0.03f, 0.09f, 0.04f, 0.55f));

        float pw = Mathf.Min(w * 0.92f, 900f);
        float ph = Mathf.Min(h * 0.92f, u * 1.18f);
        float px = (w - pw) / 2f, py = (h - ph) / 2f;

        // bayangan + panel
        Tema.Kotak(new Rect(px + 7f, py + 9f, pw, ph), new Color(0f, 0f, 0f, 0.38f));
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Panel, Tema.Garis, 3f);

        // ---- HEADER ----
        float headH = u * 0.13f;
        Tema.Kotak(new Rect(px + 3f, py + 3f, pw - 6f, headH), new Color(0.16f, 0.19f, 0.12f, 0.98f));
        Tema.Kotak(new Rect(px, py, pw, 5f), Tema.Army);
        Tema.Kotak(new Rect(px + 3f, py + headH, pw - 6f, 2f), Tema.GarisRedup);
        Tema.Teks(new Rect(px, py, pw, headH), "TOKO", Mathf.RoundToInt(u * 0.055f),
            Tema.Army, TextAnchor.MiddleCenter, true);

        float cx = px + pw * 0.06f, cw = pw * 0.88f;
        float yy = py + headH + u * 0.035f;

        // ---- SALDO PERMATA ----
        int permata = (MataUang.Instance != null) ? MataUang.Instance.Permata : 0;
        float saldoH = u * 0.10f;
        Tema.Panel9(new Rect(cx, yy, cw, saldoH), Tema.Plate, Tema.GarisRedup, 2f);
        float ikn = saldoH * 0.55f;
        Ikon.Gambar(new Rect(cx + saldoH * 0.24f, yy + (saldoH - ikn) / 2f, ikn, ikn),
            Ikon.Berlian, new Color(0.78f, 0.5f, 1f));
        Tema.Teks(new Rect(cx + saldoH * 0.95f, yy, cw - saldoH, saldoH),
            "PERMATA: " + permata, Mathf.RoundToInt(u * 0.036f), Tema.Tulang, TextAnchor.MiddleLeft, true);
        yy += saldoH + u * 0.035f;

        // ---- DAFTAR ITEM ----
        float rowH = u * 0.17f;
        float rowGap = u * 0.025f;
        for (int i = 0; i < 3; i++)
        {
            Rect rr = new Rect(cx, yy, cw, rowH);
            Tema.Panel9(rr, Tema.Plate, Tema.GarisRedup, 2f);

            // ikon buff
            float isz = rowH * 0.5f;
            Ikon.Gambar(new Rect(rr.x + rowH * 0.16f, rr.y + (rowH - isz) / 2f, isz, isz), IkonBuff(i), Tema.Army);

            // tombol harga (kanan) - dihitung DULU agar area teks tidak nabrak tombol
            float bw = u * 0.20f;
            float bh = rowH * 0.58f;
            Rect br = new Rect(rr.xMax - bw - rowH * 0.14f, rr.y + (rowH - bh) / 2f, bw, bh);

            // area teks (nama + deskripsi) di antara ikon & tombol
            float tx = rr.x + rowH * 0.16f + isz + rowH * 0.16f;
            float infoW = br.x - tx - rowH * 0.10f;
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.14f, infoW, rowH * 0.40f), NAMA[i],
                Mathf.RoundToInt(u * 0.034f), Tema.Tulang, TextAnchor.LowerLeft, true);
            // jumlah dimiliki: rata-kanan di baris nama
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.14f, infoW, rowH * 0.40f), "x" + inv[i],
                Mathf.RoundToInt(u * 0.028f), Tema.Amber, TextAnchor.LowerRight, true);
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.56f, infoW, rowH * 0.36f),
                DESK[i], Mathf.RoundToInt(u * 0.022f), Tema.Redup, TextAnchor.UpperLeft, false);

            if (GUI.Button(br, HARGA[i].ToString(), Tema.GayaTombol(Mathf.RoundToInt(u * 0.032f))))
            {
                if (MataUang.Instance != null && MataUang.Instance.PakaiPermata(HARGA[i]))
                {
                    inv[i]++; Simpan(); SoundManager.Klik();
                    status = "Dibeli: " + NAMA[i] + " (x" + inv[i] + ")";
                    permata = MataUang.Instance.Permata;
                }
                else status = "Permata kurang.";
            }
            yy += rowH + rowGap;
        }

        // ---- STATUS + HINT + TUTUP (dijangkar dari bawah panel) ----
        if (!string.IsNullOrEmpty(status))
            Tema.Teks(new Rect(cx, py + ph - u * 0.225f, cw, u * 0.04f), status,
                Mathf.RoundToInt(u * 0.024f), Tema.Amber, TextAnchor.MiddleCenter, true);

        Tema.Teks(new Rect(cx, py + ph - u * 0.175f, cw, u * 0.04f),
            "Pakai buff dengan tap ikonnya saat main.", Mathf.RoundToInt(u * 0.022f),
            Tema.Redup, TextAnchor.MiddleCenter, false);

        float clw = Mathf.Min(cw * 0.6f, u * 0.5f);
        if (GUI.Button(new Rect(px + pw / 2f - clw / 2f, py + ph - u * 0.115f, clw, u * 0.085f),
                "TUTUP", Tema.GayaTombol(Mathf.RoundToInt(u * 0.034f))))
        {
            SoundManager.Klik(); Tutup();
        }

        // penelan klik di luar panel (digambar terakhir): tombol di dalam panel tetap
        // menang klik, area gelap sekitar tidak menembus ke HUD/menu di belakang.
        GUI.Button(new Rect(0, 0, w, h), "", GUIStyle.none);
    }

    void Pakai(int i)
    {
        if (i < 0 || i > 2 || inv[i] <= 0) return;
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            PlayerHealth.GameOver || SkillManager.AktifMemilih) return;
        inv[i]--; Simpan();
        Terapkan(i);
    }

    void Terapkan(int i)
    {
        switch (i)
        {
            case 0: // Bom
                GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var m in musuh)
                {
                    if (m == null) continue;
                    EnemyChase ec = m.GetComponentInParent<EnemyChase>();
                    if (ec != null) ec.KenaSerangan(9999);
                }
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null)
                    Ledakan.Munculkan(p.transform.position, 6f, 0, 0f, new Color(1f, 0.85f, 0.35f, 0.7f));
                break;
            case 1: // Pulih HP
                if (PlayerHealth.Instance != null) PlayerHealth.Instance.Pulih(40f);
                SoundManager.LevelUp();
                break;
            case 2: // Perlambat
                EnemyChase.Perlambat(5f, 0.4f);
                SoundManager.AmbilXp();
                break;
        }
    }
}
