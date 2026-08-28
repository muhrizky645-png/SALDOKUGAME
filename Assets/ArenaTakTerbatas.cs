using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================================
//  ARENA TAK TERBATAS (versi CERAH & BERVARIASI ala Survivor.io)
//  Membuat hamparan tanah yang MENGIKUTI kamera dan BERULANG (tiled) tanpa
//  batas. Pemain tidak pernah bisa "keluar arena".
//
//  Latarnya TIDAK lagi flat hijau: rumput cerah dua-nada + bercak TANAH coklat
//  + SEMAK bulat + BATU + POHON, semuanya di-"bake" ke dalam satu tekstur
//  petak besar. Semua stamp memakai WRAP-AROUND (modulo) sehingga dekorasi yang
//  melewati tepi petak muncul lagi di sisi seberang -> pola menyambung mulus.
//
//  Otomatis dibuat saat game mulai & tiap scene di-reload, jadi TIDAK perlu
//  setup apa pun di Editor. Tekstur runtime pakai HideAndDontSave.
// ============================================================================
public class ArenaTakTerbatas : MonoBehaviour
{
    public static ArenaTakTerbatas Instance;

    const float UKURAN_TILE = 16f;  // besar satu petak (unit dunia) - besar biar pengulangan tak kentara
    const int   RES = 256;          // resolusi tekstur petak (ppu = RES/UKURAN_TILE = 16)

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

        // ppu diatur supaya satu ulangan sprite = UKURAN_TILE unit dunia
        float ppu = RES / UKURAN_TILE;
        Sprite spr = Sprite.Create(tex, new Rect(0, 0, RES, RES),
            new Vector2(0.5f, 0.5f), ppu, 0, SpriteMeshType.FullRect);

        GameObject go = new GameObject("TanahTakTerbatas");
        tanah = go.transform;
        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.tileMode = SpriteTileMode.Continuous;
        // di ATAS latar lama (-10) tapi di BAWAH semua objek gameplay (>=0)
        sr.sortingOrder = -9;
        sr.color = Color.white;
    }

    // Sembunyikan hamparan rumput lama yang berukuran tetap (objek "Paper_4").
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

        // ukuran area yang terlihat kamera
        float tinggi = cam.orthographicSize * 2f;
        float lebar = tinggi * cam.aspect;

        // sprite selalu dibuat lebih besar dari layar + margin 2 petak
        sr.size = new Vector2(lebar + UKURAN_TILE * 2f, tinggi + UKURAN_TILE * 2f);

        // ikuti kamera tapi SNAP ke kelipatan petak -> pola menyambung mulus
        Vector3 c = cam.transform.position;
        float x = Mathf.Floor(c.x / UKURAN_TILE) * UKURAN_TILE;
        float y = Mathf.Floor(c.y / UKURAN_TILE) * UKURAN_TILE;
        tanah.position = new Vector3(x, y, 1f);
    }

    // ===================== GENERATOR TEKSTUR ARENA =====================
    static Color[] _buf;
    static int _n;

    static float Clamp01(float v) { return v < 0f ? 0f : (v > 1f ? 1f : v); }

    // Blend satu titik dengan wrap-around (dekorasi menyambung antar petak)
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

    // Lingkaran LEMBUT (buat bercak halus / bayangan)
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

    // Lingkaran PADAT dengan tepi (rim) lebih gelap + sedikit anti-alias.
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
                float a = Clamp01((1f - d) * 7f);            // AA tepi tipis
                Color c = d > 0.80f ? rim : isi;
                Titik(x, y, c, a);
            }
    }

    static Texture2D BuatTeksturArena(int n)
    {
        _n = n;
        _buf = new Color[n * n];
        System.Random r = new System.Random(2026);

        // ---- 1) RUMPUT DASAR cerah + noise halus ----
        Color g1 = new Color(0.44f, 0.74f, 0.32f); // hijau cerah utama
        Color g2 = new Color(0.55f, 0.84f, 0.40f); // lebih terang
        Color g3 = new Color(0.35f, 0.63f, 0.26f); // lebih gelap
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float v = (float)(r.NextDouble() - 0.5) * 0.09f;
                _buf[y * n + x] = new Color(Clamp01(g1.r + v * 0.7f), Clamp01(g1.g + v), Clamp01(g1.b + v * 0.7f), 1f);
            }

        // ---- 2) BERCAK rumput terang/gelap (biar tidak rata) ----
        for (int i = 0; i < 16; i++)
        {
            float cx = (float)r.NextDouble() * n, cy = (float)r.NextDouble() * n;
            float rad = n * (0.07f + (float)r.NextDouble() * 0.10f);
            BlobLembut(cx, cy, rad, (r.NextDouble() < 0.5 ? g2 : g3), 0.45f);
        }

        // ---- 3) BERCAK TANAH COKLAT (gugusan blob supaya bentuknya organik) ----
        Color d1 = new Color(0.66f, 0.48f, 0.29f); // coklat tanah
        Color d2 = new Color(0.53f, 0.37f, 0.22f); // coklat lebih gelap (tepi)
        for (int i = 0; i < 5; i++)
        {
            float cx = (float)r.NextDouble() * n, cy = (float)r.NextDouble() * n;
            float rad = n * (0.05f + (float)r.NextDouble() * 0.06f);
            int blobs = 4 + r.Next(4);
            for (int b = 0; b < blobs; b++)
            {
                float ang = (float)r.NextDouble() * 6.2832f;
                float dist = (float)r.NextDouble() * rad * 0.8f;
                float bx = cx + Mathf.Cos(ang) * dist, by = cy + Mathf.Sin(ang) * dist;
                float br = rad * (0.55f + (float)r.NextDouble() * 0.5f);
                BlobPadat(bx, by, br, Color.Lerp(d1, d2, (float)r.NextDouble() * 0.5f), d2);
            }
            // sedikit bintik gelap kecil di tanah
            for (int k = 0; k < 3; k++)
                BlobLembut(cx + (float)(r.NextDouble() - 0.5) * rad, cy + (float)(r.NextDouble() - 0.5) * rad,
                    rad * 0.18f, new Color(0.40f, 0.28f, 0.16f), 0.5f);
        }

        // ---- 4) SEMAK bulat (round shrub) ----
        Color sIsi = new Color(0.30f, 0.57f, 0.24f);   // hijau semak
        Color sRim = new Color(0.20f, 0.42f, 0.16f);   // tepi lebih gelap
        Color sHi  = new Color(0.48f, 0.76f, 0.34f);   // sorot atas
        for (int i = 0; i < 12; i++)
        {
            float cx = (float)r.NextDouble() * n, cy = (float)r.NextDouble() * n;
            float rad = n * (0.028f + (float)r.NextDouble() * 0.028f);
            BlobLembut(cx, cy + rad * 0.35f, rad * 1.15f, new Color(0f, 0f, 0f), 0.22f); // bayangan
            // badan semak: gabungan beberapa gumpalan biar bergerigi
            BlobPadat(cx, cy, rad, sIsi, sRim);
            BlobPadat(cx - rad * 0.5f, cy + rad * 0.2f, rad * 0.6f, sIsi, sRim);
            BlobPadat(cx + rad * 0.5f, cy + rad * 0.15f, rad * 0.6f, sIsi, sRim);
            BlobLembut(cx - rad * 0.3f, cy - rad * 0.35f, rad * 0.6f, sHi, 0.55f);        // sorot
        }

        // ---- 5) BATU ----
        Color bIsi = new Color(0.64f, 0.64f, 0.68f);
        Color bRim = new Color(0.44f, 0.44f, 0.50f);
        Color bHi  = new Color(0.82f, 0.82f, 0.86f);
        for (int i = 0; i < 8; i++)
        {
            float cx = (float)r.NextDouble() * n, cy = (float)r.NextDouble() * n;
            float rad = n * (0.018f + (float)r.NextDouble() * 0.022f);
            BlobLembut(cx, cy + rad * 0.4f, rad * 1.2f, new Color(0f, 0f, 0f), 0.22f);   // bayangan
            BlobPadat(cx, cy, rad, bIsi, bRim);
            BlobPadat(cx + rad * 0.35f, cy - rad * 0.1f, rad * 0.55f, bIsi, bRim);       // gumpalan kedua
            BlobLembut(cx - rad * 0.3f, cy - rad * 0.3f, rad * 0.55f, bHi, 0.6f);        // sorot
        }

        // ---- 6) POHON (kanopi bergumpal + batang + bayangan) ----
        Color pKan = new Color(0.26f, 0.52f, 0.21f);   // kanopi
        Color pRim = new Color(0.17f, 0.37f, 0.14f);   // tepi kanopi
        Color pHi  = new Color(0.42f, 0.68f, 0.28f);   // sorot kanopi
        Color pBat = new Color(0.44f, 0.29f, 0.17f);   // batang
        Color pBatD = new Color(0.32f, 0.20f, 0.11f);
        for (int i = 0; i < 3; i++)
        {
            float cx = (float)r.NextDouble() * n, cy = (float)r.NextDouble() * n;
            float rad = n * (0.06f + (float)r.NextDouble() * 0.03f);
            // bayangan besar di tanah
            BlobLembut(cx + rad * 0.15f, cy + rad * 0.55f, rad * 1.25f, new Color(0f, 0f, 0f), 0.24f);
            // batang (sedikit terlihat di bawah kanopi)
            BlobPadat(cx, cy + rad * 0.7f, rad * 0.22f, pBat, pBatD);
            // kanopi bergumpal (beberapa lingkaran)
            BlobPadat(cx, cy, rad, pKan, pRim);
            BlobPadat(cx - rad * 0.6f, cy + rad * 0.15f, rad * 0.62f, pKan, pRim);
            BlobPadat(cx + rad * 0.6f, cy + rad * 0.1f, rad * 0.62f, pKan, pRim);
            BlobPadat(cx, cy - rad * 0.55f, rad * 0.62f, pKan, pRim);
            // sorot cahaya di kiri-atas kanopi
            BlobLembut(cx - rad * 0.35f, cy - rad * 0.4f, rad * 0.7f, pHi, 0.55f);
        }

        Texture2D t = new Texture2D(n, n, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave;
        t.SetPixels(_buf);
        t.Apply();
        _buf = null;
        return t;
    }
}
