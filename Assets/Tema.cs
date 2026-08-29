using UnityEngine;
using System.Collections.Generic;

// Tema visual BERSAMA bergaya "SURVIVOR.IO" (cerah, warna-warni, playful).
// Semua digambar lewat kode (IMGUI) + tekstur dibuat saat runtime, TANPA file gambar.
//
// GAYA "SEMI-3D": panel & tombol pakai SUDUT MEMBULAT + GRADASI + BEVEL + BAYANGAN
// (drop shadow) supaya berdimensi. Latar hangat oranye + vignette lembut.
// Tombol default BIRU, tombol aksi utama (MAIN dll.) HIJAU menyala.
// Teks putih dengan bayangan gelap = kesan outline khas Survivor.io.
//
// PENTING: semua tekstur runtime ditandai HideFlags.HideAndDontSave supaya TIDAK ikut
// dihapus Unity saat scene di-reload (restart / tonton iklan).
public static class Tema
{
    // ====== PALET WARNA (CERAH ala Survivor.io) ======
    public static readonly Color Overlay     = new Color(0.20f, 0.10f, 0.02f, 0.78f); // dim hangat (pause/game over)
    public static readonly Color Panel       = new Color(0.98f, 0.56f, 0.14f, 0.98f); // isi panel oranye cerah
    public static readonly Color PanelTerang = new Color(1.00f, 0.70f, 0.24f, 0.99f); // panel saat disorot
    public static readonly Color Plate       = new Color(1.00f, 0.62f, 0.18f, 0.90f); // plat HUD oranye
    public static readonly Color Garis       = new Color(1.00f, 0.86f, 0.44f, 1f);    // garis tepi emas
    public static readonly Color GarisRedup  = new Color(0.90f, 0.58f, 0.22f, 0.95f); // garis tepi oranye redup
    public static readonly Color Darah        = new Color(0.93f, 0.27f, 0.21f, 1f);    // merah cerah (judul/bahaya)
    public static readonly Color Tulang       = new Color(1.00f, 0.99f, 0.95f, 1f);    // putih (teks utama)
    public static readonly Color Army         = new Color(0.55f, 0.82f, 0.22f, 1f);    // hijau segar
    public static readonly Color Amber        = new Color(1.00f, 0.82f, 0.20f, 1f);    // kuning emas
    public static readonly Color Redup        = new Color(1.00f, 0.95f, 0.84f, 1f);    // krem terang (teks redup)

    // ====== RESPONSIF: SKALA & SAFE AREA (semua device) ======
    public static float Unit { get { return Mathf.Min(Screen.width, Screen.height); } }
    public static int Font(float frac) { return Mathf.Max(1, Mathf.RoundToInt(Unit * frac)); }
    public static float Pad { get { return Unit * 0.03f; } }
    public static float AmanKiri  { get { return Screen.safeArea.x; } }
    public static float AmanKanan { get { return Screen.width  - (Screen.safeArea.x + Screen.safeArea.width); } }
    public static float AmanAtas  { get { return Screen.height - (Screen.safeArea.y + Screen.safeArea.height); } }
    public static float AmanBawah { get { return Screen.safeArea.y; } }

    // ====== FONT PIXEL (Thaleah - versi TTF dinamis) ======
    static Font _font;
    static bool _fontDicari;
    public static Font FontUtama
    {
        get
        {
            if (!_fontDicari)
            {
                                // Utamakan font baru "Square-Black". Taruh TTF-nya di Assets/Resources/.
                // Kalau tak ada, otomatis balik ke ThaleahPixel (fallback aman).
                _font = Resources.Load<Font>("Square-Black");
                if (_font == null) _font = Resources.Load<Font>("Fonts/Square-Black");
                if (_font == null) _font = Resources.Load<Font>("ThaleahPixel");
                _fontDicari = true;
            }
            return _font;
        }
    }

    // ====== TEKSTUR DASAR ======
    static Texture2D _putih;
    public static Texture2D Putih
    {
        get
        {
            if (_putih == null)
            {
                _putih = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                _putih.hideFlags = HideFlags.HideAndDontSave; // bertahan saat scene reload
                _putih.SetPixel(0, 0, Color.white);
                _putih.Apply();
            }
            return _putih;
        }
    }

    // Gambar kotak isi warna tunggal
    public static void Kotak(Rect r, Color c)
    {
        Color s = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Putih);
        GUI.color = s;
    }

    // Latar dim layar penuh (buat pause / game over)
    public static void LatarGelap()
    {
        Kotak(new Rect(0, 0, Screen.width, Screen.height), Overlay);
    }

    // Latar dim dengan sedikit semburat warna (mis. merah untuk Game Over)
    public static void LatarGelap(Color semburat)
    {
        Kotak(new Rect(0, 0, Screen.width, Screen.height), Overlay);
        Kotak(new Rect(0, 0, Screen.width, Screen.height), semburat);
    }

    // ====== UTIL WARNA ======
    public static Color Terangkan(Color c, float f)
    {
        return new Color(Mathf.Clamp01(c.r + f), Mathf.Clamp01(c.g + f), Mathf.Clamp01(c.b + f), c.a);
    }
    public static Color Gelapkan(Color c, float f)
    {
        return new Color(Mathf.Max(0f, c.r - f), Mathf.Max(0f, c.g - f), Mathf.Max(0f, c.b - f), c.a);
    }

    // ====== GRADASI (cache biar tidak bikin tekstur tiap frame) ======
    static readonly Dictionary<int, Texture2D> _gradVCache = new Dictionary<int, Texture2D>();
    static readonly Dictionary<int, Texture2D> _gradHCache = new Dictionary<int, Texture2D>();
    static int KunciWarna(Color c)
    {
        int r = Mathf.RoundToInt(c.r * 255f), g = Mathf.RoundToInt(c.g * 255f);
        int b = Mathf.RoundToInt(c.b * 255f), a = Mathf.RoundToInt(c.a * 255f);
        return (r << 24) ^ (g << 16) ^ (b << 8) ^ a;
    }
    static Texture2D GradienV(Color atas, Color bawah)
    {
        int key = (KunciWarna(atas) * 397) ^ KunciWarna(bawah);
        Texture2D t;
        if (_gradVCache.TryGetValue(key, out t) && t != null) return t;
        int tinggi = 64;
        t = new Texture2D(1, tinggi, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave;
        t.wrapMode = TextureWrapMode.Clamp;
        t.filterMode = FilterMode.Bilinear;
        for (int y = 0; y < tinggi; y++)
        {
            float f = (float)y / (tinggi - 1); // 0 bawah .. 1 atas
            t.SetPixel(0, y, Color.Lerp(bawah, atas, f));
        }
        t.Apply();
        _gradVCache[key] = t;
        return t;
    }
    static Texture2D GradienHTex(Color kiri, Color kanan)
    {
        int key = (KunciWarna(kiri) * 397) ^ KunciWarna(kanan);
        Texture2D t;
        if (_gradHCache.TryGetValue(key, out t) && t != null) return t;
        int lebar = 64;
        t = new Texture2D(lebar, 1, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave;
        t.wrapMode = TextureWrapMode.Clamp;
        t.filterMode = FilterMode.Bilinear;
        for (int x = 0; x < lebar; x++)
        {
            float f = (float)x / (lebar - 1);
            t.SetPixel(x, 0, Color.Lerp(kiri, kanan, f));
        }
        t.Apply();
        _gradHCache[key] = t;
        return t;
    }

    // Isi rect dengan gradasi vertikal (atas -> bawah).
    public static void KotakGradien(Rect r, Color atas, Color bawah)
    {
        Color s = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(r, GradienV(atas, bawah));
        GUI.color = s;
    }
    // Isi rect dengan gradasi horizontal (kiri -> kanan).
    public static void KotakGradienH(Rect r, Color kiri, Color kanan)
    {
        Color s = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(r, GradienHTex(kiri, kanan));
        GUI.color = s;
    }

    // ====== VIGNETTE: pinggir layar digelapkan lembut biar ada kedalaman ======
    public static void Vignette()
    {
        float w = Screen.width, h = Screen.height;
        Color gelap = new Color(0f, 0f, 0f, 0.34f);
        Color kosong = new Color(0f, 0f, 0f, 0f);
        float vx = w * 0.26f;
        float vy = h * 0.16f;
        KotakGradienH(new Rect(0, 0, vx, h), gelap, kosong);            // kiri
        KotakGradienH(new Rect(w - vx, 0, vx, h), kosong, gelap);       // kanan
        KotakGradien(new Rect(0, 0, w, vy), gelap, kosong);            // atas
        KotakGradien(new Rect(0, h - vy, w, vy), kosong, gelap);       // bawah
    }

    // ====== BAR ISI (gradasi + kilau) buat XP / boss / nyawa ======
    public static void BarIsi(Rect r, Color warna)
    {
        if (r.width <= 0f || r.height <= 0f) return;
        KotakGradien(r, Terangkan(warna, 0.18f), Gelapkan(warna, 0.16f));
        float kh = Mathf.Max(1f, r.height * 0.30f);
        Kotak(new Rect(r.x, r.y, r.width, kh), new Color(1f, 1f, 1f, 0.28f));            // kilau atas
        float sh = Mathf.Max(1f, r.height * 0.18f);
        Kotak(new Rect(r.x, r.yMax - sh, r.width, sh), new Color(0f, 0f, 0f, 0.16f));    // bayangan bawah
    }

    // ====== TEKSTUR ROUNDED + BEVEL (dipakai panel, tombol, bayangan) ======
    static Texture2D BuatTexRounded(int size, float rad, float ring,
        Color grAtas, Color grBawah, Color garis, float gloss, bool ditekan)
    {
        Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave; // bertahan saat scene reload
        t.wrapMode = TextureWrapMode.Clamp;
        t.filterMode = FilterMode.Bilinear;
        float hw = size / 2f, hh = size / 2f;
        for (int yy = 0; yy < size; yy++)
        {
            float fy = (float)yy / (size - 1); // 0 bawah .. 1 atas
            for (int xx = 0; xx < size; xx++)
            {
                float x = xx + 0.5f, y = yy + 0.5f;
                float qx = Mathf.Abs(x - hw) - (hw - rad);
                float qy = Mathf.Abs(y - hh) - (hh - rad);
                float luar = Mathf.Sqrt(Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f) + Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f));
                float dalam = Mathf.Min(Mathf.Max(qx, qy), 0f);
                float sd = luar + dalam - rad;                 // <0 di dalam, 0 di tepi
                float a = Mathf.Clamp01(-sd + 0.75f);          // anti-alias tepi
                if (a <= 0f) { t.SetPixel(xx, yy, new Color(0f, 0f, 0f, 0f)); continue; }

                Color c = Color.Lerp(grBawah, grAtas, fy);
                if (!ditekan)
                {
                    if (fy > 0.52f) { float g = (fy - 0.52f) / 0.48f; c = Color.Lerp(c, Color.white, g * gloss); }
                    if (fy < 0.22f) { float g = (0.22f - fy) / 0.22f; c = Color.Lerp(c, Color.black, g * 0.15f); }
                }
                else
                {
                    if (fy > 0.60f) { float g = (fy - 0.60f) / 0.40f; c = Color.Lerp(c, Color.black, g * 0.20f); }
                }

                float alpha = c.a * a;
                if (ring > 0f && sd > -ring) { c = Color.Lerp(c, garis, 0.9f); alpha = garis.a * a; } // cincin tepi
                c.a = alpha;
                t.SetPixel(xx, yy, c);
            }
        }
        t.Apply();
        return t;
    }

    // ====== BAYANGAN (drop shadow) rounded gelap tanpa tepi ======
    static GUIStyle _bayang;
    static GUIStyle GayaBayang()
    {
        if (_bayang == null || _bayang.normal.background == null)
        {
            _bayang = new GUIStyle();
            _bayang.normal.background = BuatTexRounded(56, 16f, 0f,
                new Color(0f, 0f, 0f, 0.4f), new Color(0f, 0f, 0f, 0.4f), new Color(0f, 0f, 0f, 0f), 0f, false);
            _bayang.border = new RectOffset(18, 18, 18, 18);
        }
        return _bayang;
    }

    // ====== PANEL: bayangan + sudut membulat + gradasi + bevel (cache GUIStyle per warna) ======
    static readonly Dictionary<int, GUIStyle> _panelCache = new Dictionary<int, GUIStyle>();
    public static void Panel9(Rect r, Color isi, Color garis, float t)
    {
        int key = (KunciWarna(isi) * 397) ^ (KunciWarna(garis) * 131) ^ Mathf.RoundToInt(t * 7f);
        GUIStyle st;
        if (!_panelCache.TryGetValue(key, out st) || st == null || st.normal.background == null)
        {
            st = new GUIStyle();
            st.normal.background = BuatTexRounded(56, 14f, Mathf.Max(2f, t + 1f),
                Terangkan(isi, 0.10f), Gelapkan(isi, 0.10f), garis, 0.12f, false);
            st.border = new RectOffset(16, 16, 16, 16);
            _panelCache[key] = st;
        }
        Color s = GUI.color;
        GUI.color = Color.white;
        // drop shadow buat panel yang cukup besar (bukan bar tipis)
        if (r.height >= Unit * 0.05f)
        {
            float o = Mathf.Clamp(r.height * 0.05f, 3f, 10f);
            GUI.Box(new Rect(r.x + o * 0.4f, r.y + o, r.width, r.height), GUIContent.none, GayaBayang());
        }
        GUI.Box(r, GUIContent.none, st);
        GUI.color = s;
    }

    // Strip aksen warna di sisi atas panel (buat kartu skill dsb.)
    public static void StripAtas(Rect r, Color c, float tinggi)
    {
        Kotak(new Rect(r.x, r.y, r.width, tinggi), c);
    }

    // Teks dengan bayangan biar kebaca di background apa pun (kesan outline)
    public static void Teks(Rect r, string teks, int ukuran, Color warna, TextAnchor anchor, bool tebal)
    {
        GUIStyle st = new GUIStyle();
        st.font = FontUtama;
        st.fontSize = ukuran;
        st.fontStyle = tebal ? FontStyle.Bold : FontStyle.Normal;
        st.alignment = anchor;
        st.wordWrap = true;
        float o = Mathf.Max(1.5f, ukuran * 0.07f);
        st.normal.textColor = new Color(0.12f, 0.06f, 0.02f, 0.85f); // bayangan hangat gelap
        GUI.Label(new Rect(r.x + o, r.y + o, r.width, r.height), teks, st);
        st.normal.textColor = warna;
        GUI.Label(r, teks, st);
    }

    // ====== TOMBOL BERTEMA DEFAULT (BIRU: rounded + gradasi + bevel + gloss) ======
    static GUIStyle _tombol;
    public static GUIStyle GayaTombol(int ukuran)
    {
        if (_tombol == null || _tombol.normal.background == null)
        {
            _tombol = new GUIStyle();
            _tombol.font = FontUtama;
            _tombol.normal.background = BuatTexRounded(56, 14f, 3f,
                new Color(0.34f, 0.66f, 0.97f, 0.99f), new Color(0.16f, 0.42f, 0.82f, 0.99f),
                new Color(0.70f, 0.88f, 1f, 1f), 0.28f, false);
            _tombol.hover.background = BuatTexRounded(56, 14f, 3f,
                new Color(0.44f, 0.74f, 1f, 1f), new Color(0.24f, 0.52f, 0.92f, 1f),
                new Color(0.82f, 0.94f, 1f, 1f), 0.32f, false);
            _tombol.active.background = BuatTexRounded(56, 14f, 3f,
                new Color(0.12f, 0.34f, 0.66f, 0.99f), new Color(0.20f, 0.48f, 0.86f, 0.99f),
                new Color(0.66f, 0.86f, 1f, 1f), 0f, true);
            _tombol.border = new RectOffset(16, 16, 16, 16);
            _tombol.alignment = TextAnchor.MiddleCenter;
            _tombol.fontStyle = FontStyle.Bold;
            _tombol.wordWrap = false;
            _tombol.clipping = TextClipping.Clip;
            _tombol.padding = new RectOffset(8, 8, 6, 6);
            _tombol.normal.textColor = Tulang;
            _tombol.hover.textColor = Color.white;
            _tombol.active.textColor = Tulang;
        }
        _tombol.fontSize = ukuran;
        return _tombol;
    }

    // Tombol AKSEN (HIJAU) buat aksi utama spt MAIN -> jadi pusat perhatian.
    static GUIStyle _tombolAksen;
    public static GUIStyle GayaTombolAksen(int ukuran)
    {
        if (_tombolAksen == null || _tombolAksen.normal.background == null)
        {
            _tombolAksen = new GUIStyle();
            _tombolAksen.font = FontUtama;
            _tombolAksen.normal.background = BuatTexRounded(56, 14f, 3f,
                new Color(0.62f, 0.87f, 0.32f, 0.99f), new Color(0.34f, 0.63f, 0.13f, 0.99f),
                new Color(0.84f, 0.99f, 0.52f, 1f), 0.30f, false);
            _tombolAksen.hover.background = BuatTexRounded(56, 14f, 3f,
                new Color(0.72f, 0.95f, 0.40f, 1f), new Color(0.44f, 0.72f, 0.18f, 1f),
                new Color(0.92f, 1f, 0.62f, 1f), 0.34f, false);
            _tombolAksen.active.background = BuatTexRounded(56, 14f, 3f,
                new Color(0.28f, 0.50f, 0.10f, 0.99f), new Color(0.42f, 0.68f, 0.16f, 0.99f),
                new Color(0.80f, 0.96f, 0.46f, 1f), 0f, true);
            _tombolAksen.border = new RectOffset(16, 16, 16, 16);
            _tombolAksen.alignment = TextAnchor.MiddleCenter;
            _tombolAksen.fontStyle = FontStyle.Bold;
            _tombolAksen.wordWrap = false;
            _tombolAksen.clipping = TextClipping.Clip;
            _tombolAksen.padding = new RectOffset(8, 8, 6, 6);
            _tombolAksen.normal.textColor = Tulang;
            _tombolAksen.hover.textColor = Color.white;
            _tombolAksen.active.textColor = Tulang;
        }
        _tombolAksen.fontSize = ukuran;
        return _tombolAksen;
    }
}
