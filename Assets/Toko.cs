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
    static readonly string[] DESK  = { "Musnahkan semua musuh di layar",
                                       "Pulihkan 40 HP seketika",
                                       "Perlambat semua musuh 5 detik" };
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
        float w = Screen.width, h = Screen.height;
        Tema.LatarGelap();

        float pw = Mathf.Min(w * 0.9f, 620f);
        float ph = Mathf.Min(h * 0.86f, 760f);
        float px = (w - pw) / 2f, py = (h - ph) / 2f;
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Panel, Tema.Garis, 3f);

        float cx = px + pw * 0.05f, cw = pw * 0.9f, yy = py + ph * 0.03f;
        Tema.Teks(new Rect(cx, yy, cw, h * 0.06f), "TOKO", Mathf.RoundToInt(h * 0.045f),
            Tema.Army, TextAnchor.MiddleCenter, true);
        yy += h * 0.075f;

        int permata = (MataUang.Instance != null) ? MataUang.Instance.Permata : 0;
        Rect saldo = new Rect(cx, yy, cw, h * 0.06f);
        Tema.Panel9(saldo, Tema.Plate, Tema.GarisRedup, 2f);
        float ikn = saldo.height * 0.6f;
        Ikon.Gambar(new Rect(saldo.x + saldo.height * 0.2f, saldo.y + saldo.height * 0.2f, ikn, ikn),
            Ikon.Berlian, new Color(0.78f, 0.5f, 1f));
        Tema.Teks(new Rect(saldo.x + saldo.height * 1.0f, saldo.y, saldo.width - saldo.height, saldo.height),
            "PERMATA: " + permata, Mathf.RoundToInt(h * 0.028f), Tema.Tulang, TextAnchor.MiddleLeft, true);
        yy += h * 0.08f;

        float rowH = ph * 0.15f;
        for (int i = 0; i < 3; i++)
        {
            Rect rr = new Rect(cx, yy, cw, rowH);
            Tema.Panel9(rr, Tema.Plate, Tema.GarisRedup, 2f);
            float isz = rowH * 0.6f;
            Ikon.Gambar(new Rect(rr.x + rowH * 0.2f, rr.y + rowH * 0.2f, isz, isz), IkonBuff(i), Tema.Army);
            float tx = rr.x + rowH;
            float infoW = cw - rowH - pw * 0.30f;
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.12f, infoW, rowH * 0.4f), NAMA[i],
                Mathf.RoundToInt(h * 0.026f), Tema.Tulang, TextAnchor.LowerLeft, true);
            Tema.Teks(new Rect(tx, rr.y + rowH * 0.52f, infoW, rowH * 0.42f),
                DESK[i] + "  (x" + inv[i] + ")", Mathf.RoundToInt(h * 0.017f),
                Tema.Redup, TextAnchor.UpperLeft, false);

            float bw = pw * 0.26f;
            Rect br = new Rect(rr.xMax - bw - rowH * 0.15f, rr.y + (rowH - rowH * 0.6f) / 2f, bw, rowH * 0.6f);
            if (GUI.Button(br, HARGA[i] + "", Tema.GayaTombol(Mathf.RoundToInt(h * 0.024f))))
            {
                if (MataUang.Instance != null && MataUang.Instance.PakaiPermata(HARGA[i]))
                {
                    inv[i]++; Simpan(); SoundManager.Klik();
                    status = "Dibeli: " + NAMA[i] + " (x" + inv[i] + ")";
                    permata = MataUang.Instance.Permata;
                }
                else status = "Permata kurang.";
            }
            yy += rowH + ph * 0.015f;
        }

        if (!string.IsNullOrEmpty(status))
            Tema.Teks(new Rect(cx, py + ph - h * 0.14f, cw, h * 0.035f), status,
                Mathf.RoundToInt(h * 0.020f), Tema.Amber, TextAnchor.MiddleCenter, true);

        Tema.Teks(new Rect(cx, py + ph - h * 0.105f, cw, h * 0.030f),
            "Pakai buff dengan tap ikonnya saat main.", Mathf.RoundToInt(h * 0.017f),
            Tema.Redup, TextAnchor.MiddleCenter, false);

        float clw = Mathf.Min(cw * 0.6f, w * 0.4f);
        if (GUI.Button(new Rect(px + pw / 2f - clw / 2f, py + ph - h * 0.075f, clw, h * 0.055f),
                "TUTUP", Tema.GayaTombol(Mathf.RoundToInt(h * 0.026f))))
        {
            SoundManager.Klik(); Tutup();
        }

        GUI.Button(new Rect(0, 0, w, h), "", GUIStyle.none); // penelan klik luar
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
