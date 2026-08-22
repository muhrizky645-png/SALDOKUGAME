using UnityEngine;

// Kumpulan IKON/logo yang dibuat lewat KODE (tanpa file gambar).
// Bentuk dipanggang jadi tekstur dengan GRADASI (atas lebih terang) + anti-alias,
// lalu saat digambar diberi OUTLINE gelap supaya lebih "tergambar" / berdimensi.
//
// TAMBAHAN: kalau ada file PNG di Assets/Resources/Icons/<id>.png (disalin otomatis
// oleh Editor script PasangIkon dari asset pack), ikon skill akan memakai FILE itu.
// Kalau file belum ada, otomatis JATUH KEMBALI ke ikon kode di bawah.
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

    // Render fungsi bentuk (true = di dalam) jadi tekstur + anti-alias 2x2.
    // RGB dipanggang GRADASI vertikal (atas terang, bawah gelap) supaya saat
    // diberi warna, ikon terlihat punya bayangan/dimensi (bukan flat 1 warna).
    static Texture2D Buat(System.Func<float, float, bool> f, int size)
    {
        Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++)
        {
            float ny = ((y + 0.5f) / size) * 2f - 1f;        // -1 bawah .. 1 atas
            float g = 0.70f + 0.30f * ((ny + 1f) * 0.5f);     // 0.70 bawah .. 1.0 atas
            for (int x = 0; x < size; x++)
            {
                int hit = 0;
                for (int sy = 0; sy < 2; sy++)
                    for (int sx = 0; sx < 2; sx++)
                    {
                        float nx = ((x + (sx + 0.5f) / 2f) / size) * 2f - 1f;
                        float my = ((y + (sy + 0.5f) / 2f) / size) * 2f - 1f;
                        if (f(nx, my)) hit++;
                    }
                t.SetPixel(x, y, new Color(g, g, g, hit / 4f));
            }
        }
        t.Apply();
        return t;
    }

    // ====== IKON (lazy + cache) ======
    static Texture2D _bintang, _petir, _peluru, _target, _chevron, _hati, _berlian, _tengkorak;
    static Texture2D _bom, _magnet, _peti, _aura, _roket, _pisau;

    // ====== IKON DARI FILE (Assets/Resources/Icons), fallback ke ikon KODE ======
    // Diisi otomatis oleh Editor script PasangIkon (menyalin PNG dari asset pack).
    static readonly System.Collections.Generic.Dictionary<string, Texture2D> _fileCache
        = new System.Collections.Generic.Dictionary<string, Texture2D>();
    static readonly System.Collections.Generic.HashSet<Texture2D> _fileSet
        = new System.Collections.Generic.HashSet<Texture2D>();

    // Muat "Icons/<nama>" dari Resources. Kalau file belum ada, pakai ikon KODE (bawaan).
    static Texture2D Dari(string nama, Texture2D bawaan)
    {
        Texture2D t;
        if (!_fileCache.TryGetValue(nama, out t))
        {
            t = Resources.Load<Texture2D>("Icons/" + nama);
            _fileCache[nama] = t;
            if (t != null) _fileSet.Add(t);
        }
        return (t != null) ? t : bawaan;
    }

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
    public static Texture2D Roket { get { if (_roket == null) _roket = Buat(FRoket, 72); return _roket; } }
    public static Texture2D Pisau { get { if (_pisau == null) _pisau = Buat(FPisau, 72); return _pisau; } }

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

    // Petir (serang lebih cepat)
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

    // Target / crosshair (jangkauan lebih jauh)
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

    // Roket (skill Roket Pelacak): badan silinder + hidung kerucut + sirip + nyala + jendela
    static bool FRoket(float x, float y)
    {
        float ax = Mathf.Abs(x);
        bool inside = false;

        // badan silinder
        if (y >= -0.48f && y <= 0.45f && ax <= 0.22f) inside = true;
        // hidung kerucut
        if (y > 0.45f && y <= 0.92f)
        {
            float t = (y - 0.45f) / 0.47f;
            if (ax <= Mathf.Lerp(0.22f, 0f, t)) inside = true;
        }
        // sirip kiri-kanan
        if (y >= -0.74f && y <= -0.30f)
        {
            float t = (y + 0.74f) / 0.44f;
            float outer = Mathf.Lerp(0.55f, 0.22f, t);
            if (ax >= 0.20f && ax <= outer) inside = true;
        }
        // nyala api di bawah
        if (y >= -0.98f && y < -0.74f)
        {
            float t = (y + 0.98f) / 0.24f;
            if (ax <= Mathf.Lerp(0.04f, 0.15f, t)) inside = true;
        }
        // jendela (lubang) di badan
        if (inside && Disc(x, y, 0f, 0.12f, 0.11f)) return false;
        return inside;
    }

    // Pisau berputar (shuriken 4 sudut + lubang tengah) - ganti bintang biar tidak kembar
    static bool FPisau(float x, float y)
    {
        float r = Mathf.Sqrt(x * x + y * y);
        float ang = Mathf.Atan2(y, x);
        float step = Mathf.PI * 2f / 4f;
        float a = Mathf.Repeat(ang, step);
        float tt = a / (step / 2f); if (tt > 1f) tt = 2f - tt;
        float radius = Mathf.Lerp(0.98f, 0.28f, tt); // sudut tajam
        if (Disc(x, y, 0f, 0f, 0.16f)) return false; // lubang tengah
        return r <= radius;
    }

    // Ambil ikon berdasarkan id skill.
    // Utamakan FILE (Assets/Resources/Icons/<id>.png), fallback ke ikon KODE.
    public static Texture2D UntukSkill(string id)
    {
        switch (id)
        {
            case "petir": return Dari("petir", Petir);
            case "peluru": return Dari("peluru", Peluru);
            case "target": return Dari("target", Target);
            case "chevron": return Chevron; // tidak ada padanan di pack -> tetap ikon kode
            case "hati": return Dari("hati", Hati);
            case "berlian": return Dari("berlian", Berlian);
            case "pisau": return Dari("pisau", Pisau);
            case "aura": return Dari("aura", Aura);
            case "roket": return Dari("roket", Roket);
            default: return Dari("bintang", Bintang);
        }
    }

    // Gambar ikon (versi ringkas: outline gelap otomatis)
    public static void Gambar(Rect r, Texture2D tex, Color c)
    {
        Gambar(r, tex, c, new Color(0f, 0f, 0f, 0.55f));
    }

    // Gambar ikon dengan warna isi + warna outline (biar lebih "tergambar")
    public static void Gambar(Rect r, Texture2D tex, Color isi, Color garis)
    {
        if (tex == null) return;

        // Ikon dari FILE (berwarna): gambar apa adanya (warna isi diabaikan) + outline tipis,
        // jaga rasio aspek supaya sprite senjata/permata tidak gepeng.
        if (_fileSet.Contains(tex)) { GambarPenuh(r, tex, garis); return; }

        Color simpan = GUI.color;

        // OUTLINE: gambar tekstur gelap di 8 arah offset kecil
        float o = Mathf.Max(1f, r.width * 0.03f);
        GUI.color = garis;
        GUI.DrawTexture(new Rect(r.x - o, r.y, r.width, r.height), tex, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(r.x + o, r.y, r.width, r.height), tex, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(r.x, r.y - o, r.width, r.height), tex, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(r.x, r.y + o, r.width, r.height), tex, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(r.x - o, r.y - o, r.width, r.height), tex, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(r.x + o, r.y - o, r.width, r.height), tex, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(r.x - o, r.y + o, r.width, r.height), tex, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(r.x + o, r.y + o, r.width, r.height), tex, ScaleMode.StretchToFill, true);

        // ISI di atas outline
        GUI.color = isi;
        GUI.DrawTexture(r, tex, ScaleMode.StretchToFill, true);

        GUI.color = simpan;
    }

    // Gambar ikon FILE (berwarna) apa adanya + outline gelap tipis (4 arah), jaga rasio aspek.
    static void GambarPenuh(Rect r, Texture2D tex, Color garis)
    {
        Color simpan = GUI.color;
        float o = Mathf.Max(1f, r.width * 0.03f);
        GUI.color = garis;
        GUI.DrawTexture(new Rect(r.x - o, r.y, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(r.x + o, r.y, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(r.x, r.y - o, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(r.x, r.y + o, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.color = Color.white; // tampilkan warna ASLI sprite
        GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
        GUI.color = simpan;
    }
}
