using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================================
//  ARENA TAK TERBATAS (LANTAI) - CERAH & BERVARIASI ala Survivor.io
//  Hamparan tanah yang MENGIKUTI kamera dan BERULANG (tiled) tanpa batas.
//
//  CATATAN: pohon, batu, dan semak SEKARANG bukan lagi bagian latar. Mereka
//  dibuat sebagai OBJEK NYATA yang timbul, bisa menabrak, dan menutupi player
//  oleh RintanganArena.cs. File ini hanya menggambar LANTAI: rumput cerah
//  banyak nuansa + bercak TANAH coklat + BUNGA warna-warni + KERIKIL + rumput
//  kecil. Semua di-\"bake\" ke satu tekstur petak BESAR dengan stamp WRAP-AROUND
//  (modulo) supaya menyambung mulus antar-petak.
//
//  Otomatis dibuat saat game mulai & tiap scene di-reload, tanpa setup Editor.
// ============================================================================
public class ArenaTakTerbatas : MonoBehaviour
{
    public static ArenaTakTerbatas Instance;

    const float UKURAN_TILE = 24f;  // petak besar -> pengulangan makin tak kentara
    const int   RES = 384;          // resolusi petak (ppu = RES/UKURAN_TILE = 16)

    Camera cam;
    Transform tanah;
    SpriteRenderer sr;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("ArenaTakTerbatas", typeof(ArenaTakTerbatas));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        BuatTanah();
        SembunyikanTanahLama();
    }

    void BuatTanah()
    {
        Texture2D tex = BuatTeksturArena(RES);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        float ppu = RES / UKURAN_TILE;
        Sprite spr = Sprite.Create(tex, new Rect(0, 0, RES, RES),
            new Vector2(0.5f, 0.5f), ppu, 0, SpriteMeshType.FullRect);

        GameObject go = new GameObject("TanahTakTerbatas");
        tanah = go.transform;
        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.tileMode = SpriteTileMode.Continuous;
        sr.sortingOrder = -9; // di bawah semua objek gameplay
        sr.color = Color.white;
    }

    void SembunyikanTanahLama()
    {
        GameObject lama = GameObject.Find("Paper_4");
        if (lama != null)
        {
            SpriteRenderer s = lama.GetComponent<SpriteRenderer>();
            if (s != null) s.enabled = false;
        }
    }

    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null || tanah == null || sr == null) return;

        float tinggi = cam.orthographicSize * 2f;
        float lebar = tinggi * cam.aspect;
        sr.size = new Vector2(lebar + UKURAN_TILE * 2f, tinggi + UKURAN_TILE * 2f);

        Vector3 c = cam.transform.position;
        float x = Mathf.Floor(c.x / UKURAN_TILE) * UKURAN_TILE;
        float y = Mathf.Floor(c.y / UKURAN_TILE) * UKURAN_TILE;
        tanah.position = new Vector3(x, y, 1f);
    }

    // ===================== GENERATOR TEKSTUR LANTAI =====================
    static Color[] _buf;
    static int _n;
    static System.Random _r;

    static float Clamp01(float v) { return v < 0f ? 0f : (v > 1f ? 1f : v); }
    static float Acak(float a, float b) { return a + (float)_r.NextDouble() * (b - a); }

    static Color Jit(Color c, float amt)
    {
        return new Color(
            Clamp01(c.r + (float)(_r.NextDouble() - 0.5) * 2f * amt),
            Clamp01(c.g + (float)(_r.NextDouble() - 0.5) * 2f * amt),
            Clamp01(c.b + (float)(_r.NextDouble() - 0.5) * 2f * amt), 1f);
    }

    static void Titik(int x, int y, Color c, float a)
    {
        if (a <= 0f) return;
        if (a > 1f) a = 1f;
        x = ((x % _n) + _n) % _n;
        y = ((y % _n) + _n) % _n;
        int idx = y * _n + x;
        Color b = _buf[idx];
        _buf[idx] = new Color(b.r + (c.r - b.r) * a, b.g + (c.g - b.g) * a, b.b + (c.b - b.b) * a, 1f);
    }

    static void BlobLembut(float cx, float cy, float r, Color col, float kuat)
    {
        int x0 = Mathf.FloorToInt(cx - r) - 1, x1 = Mathf.CeilToInt(cx + r) + 1;
        int y0 = Mathf.FloorToInt(cy - r) - 1, y1 = Mathf.CeilToInt(cy + r) + 1;
        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                float dx = (x + 0.5f - cx) / r, dy = (y + 0.5f - cy) / r;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d >= 1f) continue;
                float a = (1f - d); a *= a; a *= kuat;
                Titik(x, y, col, a);
            }
    }

    static void BlobPadat(float cx, float cy, float r, Color isi, Color rim)
    {
        int x0 = Mathf.FloorToInt(cx - r) - 1, x1 = Mathf.CeilToInt(cx + r) + 1;
        int y0 = Mathf.FloorToInt(cy - r) - 1, y1 = Mathf.CeilToInt(cy + r) + 1;
        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                float dx = (x + 0.5f - cx) / r, dy = (y + 0.5f - cy) / r;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d > 1f) continue;
                float a = Clamp01((1f - d) * 7f);
                Color c = d > 0.80f ? rim : isi;
                Titik(x, y, c, a);
            }
    }

    // ---- rumput kecil: beberapa bilah tipis (dekor lantai) ----
    static void RumputKecil(float cx, float cy, float tinggi)
    {
        Color hijau = Jit(new Color(0.34f, 0.60f, 0.26f), 0.06f);
        int bilah = 3 + _r.Next(3);
        for (int b = 0; b < bilah; b++)
        {
            float bx = cx + Acak(-tinggi * 0.6f, tinggi * 0.6f);
            float lean = Acak(-0.35f, 0.35f);
            int h = Mathf.CeilToInt(tinggi);
            for (int i = 0; i < h; i++)
            {
                float t = i / (float)h;
                Titik(Mathf.RoundToInt(bx + lean * i), Mathf.RoundToInt(cy + i), hijau, 0.55f * (1f - t * 0.4f));
            }
        }
    }

    // ---- BUNGA warna-warni (kluster) ----
    static void Bunga(float cx, float cy, float rad)
    {
        Color[] pal = {
            new Color(0.98f, 0.98f, 0.98f), new Color(1f, 0.86f, 0.28f),
            new Color(1f, 0.55f, 0.72f), new Color(0.92f, 0.35f, 0.32f),
            new Color(0.70f, 0.52f, 0.95f), new Color(1f, 0.66f, 0.26f),
            new Color(0.45f, 0.68f, 1f)
        };
        Color wb = pal[_r.Next(pal.Length)];
        Color rim = new Color(wb.r * 0.75f, wb.g * 0.75f, wb.b * 0.75f, 1f);
        int kel = 5;
        float mulai = Acak(0f, 6.28f);
        for (int k = 0; k < kel; k++)
        {
            float a = mulai + k * (6.2832f / kel);
            BlobPadat(cx + Mathf.Cos(a) * rad, cy + Mathf.Sin(a) * rad, rad * 0.7f, wb, rim);
        }
        BlobPadat(cx, cy, rad * 0.7f, new Color(1f, 0.82f, 0.2f), new Color(0.85f, 0.6f, 0.12f));
    }

    static Texture2D BuatTeksturArena(int n)
    {
        _n = n;
        _buf = new Color[n * n];
        _r = new System.Random(2026);

        // ---- 1) RUMPUT DASAR cerah + noise halus ----
        Color g1 = new Color(0.44f, 0.74f, 0.32f);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float v = (float)(_r.NextDouble() - 0.5) * 0.09f;
                _buf[y * n + x] = new Color(Clamp01(g1.r + v * 0.7f), Clamp01(g1.g + v), Clamp01(g1.b + v * 0.7f), 1f);
            }

        // ---- 2) BERCAK rumput banyak NUANSA (segar / lush / kering kuning / gelap) ----
        Color[] nuansa = {
            new Color(0.55f, 0.84f, 0.40f), // terang
            new Color(0.35f, 0.63f, 0.26f), // gelap
            new Color(0.34f, 0.68f, 0.36f), // lush kebiruan
            new Color(0.64f, 0.73f, 0.34f), // kering kekuningan
            new Color(0.50f, 0.70f, 0.28f)  // zaitun
        };
        for (int i = 0; i < 28; i++)
        {
            float cx = Acak(0, n), cy = Acak(0, n);
            float rad = n * Acak(0.05f, 0.16f);
            BlobLembut(cx, cy, rad, Jit(nuansa[_r.Next(nuansa.Length)], 0.04f), Acak(0.30f, 0.55f));
        }

        // ---- 3) BERCAK TANAH COKLAT (lebih JARANG, tidak terlalu penuh) ----
        Color d1 = new Color(0.66f, 0.48f, 0.29f), d2 = new Color(0.53f, 0.37f, 0.22f);
        int jmlTanah = 3 + _r.Next(2);
        for (int i = 0; i < jmlTanah; i++)
        {
            float cx = Acak(0, n), cy = Acak(0, n);
            float rad = n * Acak(0.04f, 0.10f);
            Color td1 = Jit(d1, 0.05f), td2 = Jit(d2, 0.04f);
            int blobs = 4 + _r.Next(5);
            for (int b = 0; b < blobs; b++)
            {
                float ang = Acak(0, 6.2832f), dist = Acak(0, rad * 0.85f);
                float bx = cx + Mathf.Cos(ang) * dist, by = cy + Mathf.Sin(ang) * dist;
                float br = rad * Acak(0.5f, 1.05f);
                BlobPadat(bx, by, br, Color.Lerp(td1, td2, Acak(0f, 0.5f)), td2);
            }
            for (int k = 0; k < 3; k++)
                BlobLembut(cx + Acak(-rad, rad), cy + Acak(-rad, rad), rad * 0.18f, new Color(0.40f, 0.28f, 0.16f), 0.5f);
        }

        // ---- 4) RUMPUT KECIL bertebaran (tekstur lantai) ----
        int jmlRumput = 60 + _r.Next(30);
        for (int i = 0; i < jmlRumput; i++)
            RumputKecil(Acak(0, n), Acak(0, n), n * Acak(0.010f, 0.024f));

        // ---- 5) BUNGA warna-warni (lebih JARANG, kluster kecil) ----
        int jmlBunga = 9 + _r.Next(5);
        for (int i = 0; i < jmlBunga; i++)
        {
            float cx = Acak(0, n), cy = Acak(0, n);
            int grup = 1 + _r.Next(2);
            for (int g = 0; g < grup; g++)
                Bunga(cx + Acak(-n * 0.02f, n * 0.02f), cy + Acak(-n * 0.02f, n * 0.02f), n * Acak(0.006f, 0.012f));
        }

        // ---- 6) KERIKIL kecil ----
        int jmlKerikil = 16 + _r.Next(8);
        for (int i = 0; i < jmlKerikil; i++)
        {
            Color kb = Jit(new Color(0.60f, 0.60f, 0.62f), 0.06f);
            BlobPadat(Acak(0, n), Acak(0, n), n * Acak(0.004f, 0.010f), kb, new Color(kb.r * 0.7f, kb.g * 0.7f, kb.b * 0.72f, 1f));
        }

        Texture2D t = new Texture2D(n, n, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave;
        t.SetPixels(_buf);
        t.Apply();
        _buf = null;
        return t;
    }
}
