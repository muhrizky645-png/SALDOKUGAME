using UnityEngine;

// Kumpulan IKON/logo yang dibuat lewat KODE (tanpa file gambar).
// Digambar putih (dengan anti-alias), lalu diberi warna saat dipakai.
public static class Ikon
{
    // ====== PRIMITIF BANTU (ruang normal -1..1, y ke atas) ======
    static bool Disc(float x, float y, float cx, float cy, float r)
    {
        float dx = x - cx, dy = y - cy;
        return dx * dx + dy * dy <= r * r;
    }

    static bool Kotak(float x, float y, float x0, float y0, float x1, float y1)
    {
        return x >= x0 && x <= x1 && y >= y0 && y <= y1;
    }

    static bool Cincin(float x, float y, float r0, float r1)
    {
        float d = x * x + y * y;
        return d >= r0 * r0 && d <= r1 * r1;
    }

    static bool Garis(float x, float y, float ax, float ay, float bx, float by, float w)
    {
        float vx = bx - ax, vy = by - ay, wx = x - ax, wy = y - ay;
        float len = vx * vx + vy * vy;
        float t = len > 0f ? Mathf.Clamp01((wx * vx + wy * vy) / len) : 0f;
        float px = ax + vx * t, py = ay + vy * t, dx = x - px, dy = y - py;
        return dx * dx + dy * dy <= w * w;
    }

    // Render fungsi bentuk (true = di dalam) jadi tekstur putih + anti-alias 2x2
    static Texture2D Buat(System.Func<float, float, bool> f, int size)
    {
        Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int hit = 0;
                for (int sy = 0; sy < 2; sy++)
                    for (int sx = 0; sx < 2; sx++)
                    {
                        float nx = ((x + (sx + 0.5f) / 2f) / size) * 2f - 1f;
                        float ny = ((y + (sy + 0.5f) / 2f) / size) * 2f - 1f;
                        if (f(nx, ny)) hit++;
                    }
                t.SetPixel(x, y, new Color(1f, 1f, 1f, hit / 4f));
            }
        t.Apply();
        return t;
    }

    // ====== IKON (lazy + cache) ======
    static Texture2D _bintang, _petir, _peluru, _target, _chevron, _hati, _berlian, _tengkorak;
    static Texture2D _bom, _magnet, _peti, _aura;

    public static Texture2D Bintang { get { if (_bintang == null) _bintang = Buat(FBintang, 72); return _bintang; } }
    public static Texture2D Petir { get { if (_petir == null) _petir = Buat(FPetir, 72); return _petir; } }
    public static Texture2D Peluru { get { if (_peluru == null) _peluru = Buat(FPeluru, 72); return _peluru; } }
    public static Texture2D Target { get { if (_target == null) _target = Buat(FTarget, 72); return _target; } }
    public static Texture2D Chevron { get { if (_chevron == null) _chevron = Buat(FChevron, 72); return _chevron; } }
    public static Texture2D Hati { get { if (_hati == null) _hati = Buat(FHati, 72); return _hati; } }
    public static Texture2D Berlian { get { if (_berlian == null) _berlian = Buat(FBerlian, 72); return _berlian; } }
    public static Texture2D Tengkorak { get { if (_tengkorak == null) _tengkorak = Buat(FTengkorak, 72); return _tengkorak; } }
    public static Texture2D Bom { get { if (_bom == null) _bom = Buat(FBom, 72); return _bom; } }
    public static Texture2D Magnet { get { if (_magnet == null) _magnet = Buat(FMagnet, 72); return _magnet; } }
    public static Texture2D Peti { get { if (_peti == null) _peti = Buat(FPeti, 72); return _peti; } }
    public static Texture2D Aura { get { if (_aura == null) _aura = Buat(FAura, 72); return _aura; } }

    // Bintang 5 sudut (rekor / high score)
    static bool FBintang(float x, float y)
    {
        float r = Mathf.Sqrt(x * x + y * y);
        float ang = Mathf.Atan2(y, x);
        float step = Mathf.PI * 2f / 5f;
        float a = Mathf.Repeat(ang - Mathf.PI / 2f, step);
        float tt = a / (step / 2f); if (tt > 1f) tt = 2f - tt;
        float radius = Mathf.Lerp(0.96f, 0.42f, tt);
        return r <= radius;
    }

    // Petir (serang lebih cepat / roket)
    static bool FPetir(float x, float y)
    {
        float w = 0.17f;
        return Garis(x, y, 0.15f, 0.92f, -0.38f, 0.08f, w)
            || Garis(x, y, -0.38f, 0.08f, 0.12f, 0.08f, w)
            || Garis(x, y, 0.12f, 0.08f, -0.18f, -0.92f, w);
    }

    // Tiga peluru (peluru tambahan)
    static bool FPeluru(float x, float y)
    {
        for (int i = -1; i <= 1; i++)
        {
            float cx = i * 0.5f;
            if (Kotak(x, y, cx - 0.14f, -0.55f, cx + 0.14f, 0.35f)) return true;
            if (Disc(x, y, cx, 0.35f, 0.14f)) return true;
        }
        return false;
    }

    // Target / crosshair (jangkauan lebih jauh / aura)
    static bool FTarget(float x, float y)
    {
        return Cincin(x, y, 0.72f, 0.96f)
            || Cincin(x, y, 0.30f, 0.52f)
            || Disc(x, y, 0f, 0f, 0.13f)
            || Garis(x, y, -0.98f, 0f, 0.98f, 0f, 0.05f)
            || Garis(x, y, 0f, -0.98f, 0f, 0.98f, 0.05f);
    }

    // Double chevron (kaki lebih cepat)
    static bool FChevron(float x, float y)
    {
        float w = 0.15f;
        return Garis(x, y, -0.35f, 0.6f, 0.15f, 0f, w) || Garis(x, y, 0.15f, 0f, -0.35f, -0.6f, w)
            || Garis(x, y, 0.15f, 0.6f, 0.65f, 0f, w) || Garis(x, y, 0.65f, 0f, 0.15f, -0.6f, w);
    }

    // Hati (badan lebih kuat)
    static bool FHati(float x, float y)
    {
        float X = x / 0.92f; float Y = (y - 0.15f) / 0.92f;
        float a = X * X + Y * Y - 1f;
        return a * a * a - X * X * Y * Y * Y <= 0f;
    }

    // Berlian / permata (magnet permata)
    static bool FBerlian(float x, float y)
    {
        return Mathf.Abs(x) + Mathf.Abs(y) <= 0.92f;
    }

    // Tengkorak sederhana
    static bool FTengkorak(float x, float y)
    {
        bool kepala = Disc(x, y, 0f, 0.15f, 0.72f);
        bool rahang = Kotak(x, y, -0.35f, -0.75f, 0.35f, 0.05f);
        bool badan = kepala || rahang;
        if (!badan) return false;
        if (Disc(x, y, -0.28f, 0.20f, 0.20f)) return false;
        if (Disc(x, y, 0.28f, 0.20f, 0.20f)) return false;
        if (Disc(x, y, 0f, -0.05f, 0.10f)) return false;
        if (Kotak(x, y, -0.08f, -0.75f, 0.08f, -0.20f)) return false;
        return true;
    }

    // Bom (item bersihkan layar): badan bulat + sumbu + percik
    static bool FBom(float x, float y)
    {
        if (Disc(x, y, 0f, -0.15f, 0.62f)) return true;               // badan bom
        if (Garis(x, y, 0.25f, 0.45f, 0.45f, 0.85f, 0.09f)) return true; // sumbu
        if (Disc(x, y, 0.5f, 0.9f, 0.13f)) return true;                // percik api
        return false;
    }

    // Magnet (tapal kuda U)
    static bool FMagnet(float x, float y)
    {
        bool arc = Cincin(x, y, 0.42f, 0.8f) && y <= 0.15f;
        bool legs = (y > 0.15f) && ((x >= -0.8f && x <= -0.42f) || (x >= 0.42f && x <= 0.8f));
        return arc || legs;
    }

    // Peti / chest (badan + tutup)
    static bool FPeti(float x, float y)
    {
        bool badan = Kotak(x, y, -0.75f, -0.65f, 0.75f, 0.28f);
        bool tutup = Kotak(x, y, -0.8f, 0.28f, 0.8f, 0.62f);
        return badan || tutup;
    }

    // Aura / medan setrum (cincin ganda) - untuk skill aura
    static bool FAura(float x, float y)
    {
        return Cincin(x, y, 0.78f, 0.96f) || Cincin(x, y, 0.40f, 0.56f) || Disc(x, y, 0f, 0f, 0.14f);
    }

    // Ambil ikon berdasarkan id skill
    public static Texture2D UntukSkill(string id)
    {
        switch (id)
        {
            case "petir": return Petir;
            case "peluru": return Peluru;
            case "target": return Target;
            case "chevron": return Chevron;
            case "hati": return Hati;
            case "berlian": return Berlian;
            case "pisau": return Bintang;
            case "aura": return Aura;
            case "roket": return Petir;
            default: return Bintang;
        }
    }

    // Gambar ikon di layar dengan warna tertentu
    public static void Gambar(Rect r, Texture2D tex, Color c)
    {
        if (tex == null) return;
        Color s = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, tex, ScaleMode.StretchToFill, true);
        GUI.color = s;
    }
}
