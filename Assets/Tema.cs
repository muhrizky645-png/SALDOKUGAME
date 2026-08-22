using UnityEngine;

// Tema visual BERSAMA bergaya "SURVIVAL" (gelap, gritty, aksen merah darah + hijau army).
// Semua digambar lewat kode (IMGUI) + tekstur dibuat saat runtime, TANPA file gambar.
// Dipakai oleh HUD, menu Home, menu Jeda, Game Over, dan kartu Skill.
//
// PENTING: semua tekstur runtime ditandai HideFlags.HideAndDontSave supaya TIDAK ikut
// dihapus Unity saat scene di-reload (restart / tonton iklan). Tanpa ini, background
// tombol (GayaTombol) hilang setelah main ulang karena GUIStyle di-cache tapi tekstur
// di dalamnya sudah dihancurkan -> border/kotak tombol jadi lenyap.
public static class Tema
{
    // ====== PALET WARNA ======
    public static readonly Color Overlay     = new Color(0.02f, 0.03f, 0.02f, 0.86f); // latar gelap layar penuh
    public static readonly Color Panel       = new Color(0.11f, 0.12f, 0.09f, 0.96f); // isi panel
    public static readonly Color PanelTerang = new Color(0.19f, 0.21f, 0.15f, 0.98f); // panel saat disorot
    public static readonly Color Plate       = new Color(0.05f, 0.06f, 0.04f, 0.78f); // plat HUD tipis
    public static readonly Color Garis       = new Color(0.52f, 0.60f, 0.28f, 1f);    // garis tepi army
    public static readonly Color GarisRedup  = new Color(0.38f, 0.42f, 0.24f, 0.9f);  // garis tepi redup
    public static readonly Color Darah        = new Color(0.82f, 0.17f, 0.13f, 1f);    // merah darah
    public static readonly Color Tulang       = new Color(0.95f, 0.94f, 0.87f, 1f);    // putih tulang
    public static readonly Color Army         = new Color(0.66f, 0.85f, 0.38f, 1f);    // hijau army terang
    public static readonly Color Amber        = new Color(1f, 0.80f, 0.22f, 1f);       // amber (rekor)
    public static readonly Color Redup        = new Color(0.72f, 0.74f, 0.64f, 1f);    // teks redup

    // ====== RESPONSIF: SKALA & SAFE AREA (semua device) ======
    // Sisi TERPENDEK layar dipakai sebagai satuan dasar font/elemen persegi, supaya
    // ukuran konsisten & teks tidak meluber di rasio ekstrem (HP tinggi/lebar/tablet).
    public static float Unit { get { return Mathf.Min(Screen.width, Screen.height); } }

    // Ukuran font responsif: frac dikali sisi terpendek (minimal 1px).
    public static int Font(float frac) { return Mathf.Max(1, Mathf.RoundToInt(Unit * frac)); }

    // Jarak / padding standar HUD (relatif sisi terpendek).
    public static float Pad { get { return Unit * 0.03f; } }

    // Safe area: hindari poni/kamera & sudut melengkung. Origin GUI = kiri-atas.
    public static float AmanKiri  { get { return Screen.safeArea.x; } }
    public static float AmanKanan { get { return Screen.width  - (Screen.safeArea.x + Screen.safeArea.width); } }
    public static float AmanAtas  { get { return Screen.height - (Screen.safeArea.y + Screen.safeArea.height); } }
    public static float AmanBawah { get { return Screen.safeArea.y; } }

    // ====== FONT PIXEL (Thaleah - versi TTF dinamis) ======
    // File TTF disalin otomatis ke Assets/Resources/ThaleahPixel.ttf oleh
    // Assets/Editor/PasangIkon.cs supaya bisa dimuat runtime & DISKALAKAN dgn benar
    // (font .fontsettings bitmap lama mengabaikan fontSize -> teks jadi kecil).
    // Kalau tidak ditemukan, otomatis pakai font bawaan Unity.
    static Font _font;
    static bool _fontDicari;
    public static Font FontUtama
    {
        get
        {
            if (!_fontDicari)
            {
                _font = Resources.Load<Font>("ThaleahPixel");
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

    // Latar gelap layar penuh (buat semua menu)
    public static void LatarGelap()
    {
        Kotak(new Rect(0, 0, Screen.width, Screen.height), Overlay);
    }

    // Latar gelap dengan sedikit semburat warna (mis. merah untuk Game Over)
    public static void LatarGelap(Color semburat)
    {
        Kotak(new Rect(0, 0, Screen.width, Screen.height), Overlay);
        Kotak(new Rect(0, 0, Screen.width, Screen.height), semburat);
    }

    // Panel: isi + garis tepi (border) setebal t piksel
    public static void Panel9(Rect r, Color isi, Color garis, float t)
    {
        Kotak(r, garis);
        Kotak(new Rect(r.x + t, r.y + t, r.width - 2 * t, r.height - 2 * t), isi);
    }

    // Strip aksen warna di sisi atas panel (buat kartu skill dsb.)
    public static void StripAtas(Rect r, Color c, float tinggi)
    {
        Kotak(new Rect(r.x, r.y, r.width, tinggi), c);
    }

    // Teks dengan bayangan biar kebaca di background apa pun
    public static void Teks(Rect r, string teks, int ukuran, Color warna, TextAnchor anchor, bool tebal)
    {
        GUIStyle st = new GUIStyle();
        st.font = FontUtama;
        st.fontSize = ukuran;
        st.fontStyle = tebal ? FontStyle.Bold : FontStyle.Normal;
        st.alignment = anchor;
        st.wordWrap = true;
        float o = Mathf.Max(1.5f, ukuran * 0.06f);
        st.normal.textColor = new Color(0f, 0f, 0f, 0.7f);
        GUI.Label(new Rect(r.x + o, r.y + o, r.width, r.height), teks, st);
        st.normal.textColor = warna;
        GUI.Label(r, teks, st);
    }

    // ====== TOMBOL BERTEMA (pakai GUI.Button biar sentuhan HP responsif) ======
    static GUIStyle _tombol;

    static Texture2D BuatTexKotak(Color isi, Color garis, int b)
    {
        int size = 20;
        Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave; // bertahan saat scene reload (border tombol tak hilang)
        t.wrapMode = TextureWrapMode.Clamp;
        t.filterMode = FilterMode.Point;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                bool tepi = x < b || y < b || x >= size - b || y >= size - b;
                t.SetPixel(x, y, tepi ? garis : isi);
            }
        t.Apply();
        return t;
    }

    // Ambil gaya tombol bertema (tekstur di-cache, cukup ganti fontSize)
    public static GUIStyle GayaTombol(int ukuran)
    {
        if (_tombol == null)
        {
            _tombol = new GUIStyle();
            _tombol.font = FontUtama;
            _tombol.normal.background = BuatTexKotak(Panel, Garis, 3);
            _tombol.hover.background = BuatTexKotak(PanelTerang, Army, 3);
            _tombol.active.background = BuatTexKotak(new Color(0.30f, 0.10f, 0.09f, 0.98f), Darah, 3);
            _tombol.border = new RectOffset(4, 4, 4, 4);
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
}
