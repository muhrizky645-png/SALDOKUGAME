using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================================
// SALDOKU LAST STAND - SISTEM DUA MATA UANG (Permata & Koin)
// Mengikuti pola manager lain (bootstrap + sceneLoaded singleton).
//
// * PERMATA : mata uang IN-GAME lokal (PlayerPrefs). Didapat dari drop
//            musuh (mirip XP, tapi lebih jarang). Dipakai beli buff di TOKO.
// * KOIN    : cermin poin SALDOKU (1 Koin = 1 poin). READ-ONLY di game;
//            hanya server yang menambah (via SSV iklan). Terkunci sampai
//            akun SALDOKU dihubungkan.
// =====================================================================
public class MataUang : MonoBehaviour
{
    public static MataUang Instance;

    const string PP_PERMATA = "permata";
    const string PP_KOIN_CACHE = "koin_cache";
    const string PP_LINKED = "saldoku_linked";

    int permata;
    long koin;
    bool linked;
    bool online;

    public int Permata { get { return permata; } }
    public long Koin { get { return koin; } }
    public bool Terhubung { get { return linked; } }
    public bool Online { get { return online; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("MataUang", typeof(MataUang));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        permata = PlayerPrefs.GetInt(PP_PERMATA, 0);
        koin = PlayerPrefs.GetInt(PP_KOIN_CACHE, 0);
        linked = PlayerPrefs.GetInt(PP_LINKED, 0) == 1;
        online = false;
    }

    public void TambahPermata(int n)
    {
        if (n <= 0) return;
        permata += n;
        PlayerPrefs.SetInt(PP_PERMATA, permata);
    }

    public bool PakaiPermata(int n)
    {
        if (n <= 0) return true;
        if (permata < n) return false;
        permata -= n;
        PlayerPrefs.SetInt(PP_PERMATA, permata);
        PlayerPrefs.Save();
        return true;
    }

    // Dipanggil hasil sinkron server (Saldoku). Game TIDAK menambah Koin sendiri.
    public void SetKoinDariServer(long value, bool isOnline, bool isLinked)
    {
        koin = value < 0 ? 0 : value;
        online = isOnline;
        linked = isLinked;
        PlayerPrefs.SetInt(PP_KOIN_CACHE, (int)Mathf.Clamp(koin, 0, int.MaxValue));
        PlayerPrefs.SetInt(PP_LINKED, linked ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnApplicationPause(bool p) { if (p) PlayerPrefs.Save(); }
    void OnApplicationQuit() { PlayerPrefs.Save(); }

    // Angka ringkas (1.2K, 3.4M)
    public static string Ringkas(long v)
    {
        if (v >= 1000000) return (v / 1000000f).ToString("0.#") + "M";
        if (v >= 1000) return (v / 1000f).ToString("0.#") + "K";
        return v.ToString();
    }

    // ---- tekstur koin (lingkaran) prosedural ----
    static Texture2D _koinTex;
    static Texture2D KoinTex
    {
        get
        {
            if (_koinTex == null)
            {
                int size = 48;
                _koinTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                _koinTex.hideFlags = HideFlags.HideAndDontSave;
                _koinTex.wrapMode = TextureWrapMode.Clamp;
                float r = size / 2f;
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = x - r + 0.5f, dy = y - r + 0.5f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(r - d);
                    _koinTex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
                _koinTex.Apply();
            }
            return _koinTex;
        }
    }

    // Chip mata uang bertema (dipakai HUD main & menu Home).
    public void GambarChip(Rect r, bool gem, int font, string teks, Color aksen, bool aktif)
    {
        Tema.Panel9(r, Tema.Plate, Tema.GarisRedup, 2f);
        float ic = r.height - r.height * 0.32f;
        Rect ir = new Rect(r.x + r.height * 0.16f, r.y + (r.height - ic) / 2f, ic, ic);
        if (gem)
        {
            Ikon.Gambar(ir, Ikon.Dari("permata", Ikon.Berlian), aksen);
        }
        else
        {
            Texture2D koinFile = Ikon.Dari("koin", null);
            if (koinFile != null)
            {
                Ikon.Gambar(ir, koinFile, Color.white);
            }
            else
            {
                Color s = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 0.5f);
                GUI.DrawTexture(new Rect(ir.x + 1f, ir.y + 1f, ir.width, ir.height), KoinTex, ScaleMode.ScaleToFit, true);
                GUI.color = aksen;
                GUI.DrawTexture(ir, KoinTex, ScaleMode.ScaleToFit, true);
                GUI.color = s;
            }
        }
        float tx = ir.xMax + r.height * 0.14f;
        Tema.Teks(new Rect(tx, r.y, r.xMax - tx - r.height * 0.14f, r.height), teks, font,
            aktif ? Tema.Tulang : Tema.Amber, TextAnchor.MiddleLeft, true);
    }

    // HUD Permata saat MAIN: chip di tengah, tepat di bawah panel level.
    void OnGUI()
    {
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;

        float w = Screen.width, h = Screen.height;
        int f = Mathf.Min(Tema.Font(0.030f), Mathf.RoundToInt(w * 0.055f));
        float chipH = f * 1.7f;
        string t = Ringkas(permata);
        // Lebar chip HARUS memuat ikon (~1.12x tinggi chip) + teks. Rumus lama
        // memakai 1.6 yang lebih kecil dari ruang ikon -> angka nabrak/ke-wrap.
        // 2.3 menutup ikon+padding, 0.66/char untuk lebar teks font piksel tebal.
        float chipW = Mathf.Max(f * 4.0f, f * (2.3f + 0.66f * t.Length));
        float chipX = (w - chipW) / 2f;
        float chipY = Tema.AmanAtas + Tema.Pad + LevelSystem.TinggiPanel(w) + Tema.Pad * 0.6f;
        GambarChip(new Rect(chipX, chipY, chipW, chipH), true, f, t, new Color(0.78f, 0.5f, 1f), true);
    }
}
