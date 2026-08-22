using UnityEngine;

// Kumpulan IKON/logo yang dibuat lewat KODE (tanpa file gambar).
// Bentuk dipanggang jadi tekstur dengan GRADASI (atas lebih terang) + anti-alias,
// lalu saat digambar diberi OUTLINE gelap supaya lebih "tergambar" / berdimensi.
//
// ITEM LAPANGAN (bom/magnet/peti) memakai ikon KODE BERWARNA & TRANSPARAN (tanpa
// background kotak). Skill masih bisa pakai FILE dari asset pack kalau tersedia.
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

    // Render bentuk BERWARNA: kelas(nx,my) -> 0 kosong, i>=1 pakai palet[i-1].
    // Anti-alias 2x2 + sedikit gradasi (atas lebih terang) biar berdimensi.
    static Texture2D BuatWarna(System.Func<float, float, int> kelas, Color[] palet, int size)
    {
        Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        int[] hitung = new int[palet.Length + 1];
        for (int y = 0; y < size; y++)
        {
            float ny = ((y + 0.5f) / size) * 2f - 1f;
            float g = 0.84f + 0.16f * ((ny + 1f) * 0.5f);
            for (int x = 0; x < size; x++)
            {
                for (int i = 0; i < hitung.Length; i++) hitung[i] = 0;
                int total = 0;
                for (int sy = 0; sy < 2; sy++)
                    for (int sx = 0; sx < 2; sx++)
                    {
                        float nx = ((x + (sx + 0.5f) / 2f) / size) * 2f - 1f;
                        float my = ((y + (sy + 0.5f) / 2f) / size) * 2f - 1f;
                        int k = kelas(nx, my);
                        if (k > 0) { hitung[k]++; total++; }
                    }
                if (total == 0) { t.SetPixel(x, y, new Color(0f, 0f, 0f, 0f)); continue; }
                int best = 1;
                for (int i = 2; i < hitung.Length; i++) if (hitung[i] > hitung[best]) best = i;
                Color c = palet[best - 1];
                t.SetPixel(x, y, new Color(c.r * g, c.g * g, c.b * g, total / 4f));
            }
        }
        t.Apply();
        return t;
    }

    // ====== IKON (lazy + cache) ======
    static Texture2D _bintang, _petir, _peluru, _target, _chevron, _hati, _berlian, _tengkorak;
    static Texture2D _bom, _magnet, _peti, _aura, _roket, _pisau;

    // ====== IKON DARI FILE (Assets/Resources/Icons), fallback ke ikon KODE ======
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
    public static Texture2D Aura { get { if (_aura == null) _aura = Buat(FAura, 72); return _aura; } }
    public static Texture2D Roket { get { if (_roket == null) _roket = Buat(FRoket, 72); return _roket; } }
    public static Texture2D Pisau { get { if (_pisau == null) _pisau = Buat(FPisau, 72); return _pisau; } }

    // ====== ITEM LAPANGAN: ikon KODE BERWARNA & TRANSPARAN (tanpa background) ======
    static readonly Color[] _paletBom = new Color[] {
        new Color(0.17f, 0.18f, 0.22f), // 1 badan bom (gelap)
        new Color(0.50f, 0.54f, 0.62f), // 2 kilau
        new Color(0.55f, 0.38f, 0.18f), // 3 sumbu
        new Color(1.00f, 0.75f, 0.20f), // 4 percik api
    };
    static readonly Color[] _paletPeti = new Color[] {
        new Color(0.58f, 0.37f, 0.17f), // 1 kayu
        new Color(0.43f, 0.26f, 0.11f), // 2 kayu tutup (lebih gelap)
        new Color(0.95f, 0.75f, 0.22f), // 3 logam emas
        new Color(1.00f, 0.86f, 0.38f), // 4 kunci
    };

    // Magnet tapal kuda BERWARNA (merah + kutub perak).
    public static Texture2D Magnet
    {
        get
        {
            if (_magnet == null) { _magnet = BuatMagnetBerwarna(72); _fileSet.Add(_magnet); }
            return _magnet;
        }
    }

    // Bom BERWARNA (transparan, tanpa background).
    public static Texture2D Bom
    {
        get
        {
            if (_bom == null) { _bom = BuatWarna(BomKelas, _paletBom, 72); _fileSet.Add(_bom); }
            return _bom;
        }
    }

    // Peti BERWARNA (transparan, tanpa background).
    public static Texture2D Peti
    {
        get
        {
            if (_peti == null) { _peti = BuatWarna(PetiKelas, _paletPeti, 72); _fileSet.Add(_peti); }
            return _peti;
        }
    }

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

    static bool FPetir(float x, float y)
    {
        float w = 0.17f;
        return Garis(x, y, 0.15f, 0.92f, -0.38f, 0.08f, w)
            || Garis(x, y, -0.38f, 0.08f, 0.12f, 0.08f, w)
            || Garis(x, y, 0.12f, 0.08f, -0.18f, -0.92f, w);
    }

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

    static bool FTarget(float x, float y)
    {
        return Cincin(x, y, 0.72f, 0.96f)
            || Cincin(x, y, 0.30f, 0.52f)
            || Disc(x, y, 0f, 0f, 0.13f)
            || Garis(x, y, -0.98f, 0f, 0.98f, 0f, 0.05f)
            || Garis(x, y, 0f, -0.98f, 0f, 0.98f, 0.05f);
    }

    static bool FChevron(float x, float y)
    {
        float w = 0.15f;
        return Garis(x, y, -0.35f, 0.6f, 0.15f, 0f, w) || Garis(x, y, 0.15f, 0f, -0.35f, -0.6f, w)
            || Garis(x, y, 0.15f, 0.6f, 0.65f, 0f, w) || Garis(x, y, 0.65f, 0f, 0.15f, -0.6f, w);
    }

    static bool FHati(float x, float y)
    {
        float X = x / 0.92f; float Y = (y - 0.15f) / 0.92f;
        float a = X * X + Y * Y - 1f;
        return a * a * a - X * X * Y * Y * Y <= 0f;
    }

    static bool FBerlian(float x, float y)
    {
        return Mathf.Abs(x) + Mathf.Abs(y) <= 0.92f;
    }

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

    // ====== BOM BERWARNA ====== (1 badan, 2 kilau, 3 sumbu, 4 percik)
    static int BomKelas(float x, float y)
    {
        if (Disc(x, y, 0.5f, 0.90f, 0.14f)) return 4;                               // percik api
        if (Garis(x, y, 0.25f, 0.45f, 0.38f, 0.72f, 0.075f)
            || Garis(x, y, 0.38f, 0.72f, 0.50f, 0.86f, 0.07f)) return 3;           // sumbu
        if (Disc(x, y, 0f, -0.15f, 0.62f))                                          // badan
        {
            if (Disc(x, y, -0.22f, 0.08f, 0.16f)) return 2;                         // kilau kiri-atas
            return 1;
        }
        return 0;
    }

    // ====== MAGNET TAPAL KUDA BERWARNA ====== (1 badan merah, 2 kutub perak)
    static int MagnetKelas(float x, float y)
    {
        float ax = Mathf.Abs(x);
        if (y < -0.52f && y >= -0.84f && ax >= 0.40f && ax <= 0.86f) return 2; // ujung kutub (perak)
        if (Cincin(x, y, 0.44f, 0.82f) && y >= -0.05f) return 1;               // lengkung atas
        if (y < -0.05f && y >= -0.52f && ax >= 0.44f && ax <= 0.82f) return 1; // kaki
        return 0;
    }

    static Texture2D BuatMagnetBerwarna(int size)
    {
        Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        Color merah = new Color(0.90f, 0.17f, 0.16f, 1f);
        Color merahGelap = new Color(0.52f, 0.06f, 0.06f, 1f);
        Color perak = new Color(0.90f, 0.92f, 0.97f, 1f);
        for (int y = 0; y < size; y++)
        {
            float ny = ((y + 0.5f) / size) * 2f - 1f;
            for (int x = 0; x < size; x++)
            {
                int badan = 0, kutub = 0;
                for (int sy = 0; sy < 2; sy++)
                    for (int sx = 0; sx < 2; sx++)
                    {
                        float nx = ((x + (sx + 0.5f) / 2f) / size) * 2f - 1f;
                        float my = ((y + (sy + 0.5f) / 2f) / size) * 2f - 1f;
                        int k = MagnetKelas(nx, my);
                        if (k == 1) badan++; else if (k == 2) kutub++;
                    }
                int hit = badan + kutub;
                if (hit == 0) { t.SetPixel(x, y, new Color(0f, 0f, 0f, 0f)); continue; }
                Color isi = (kutub >= badan)
                    ? perak
                    : Color.Lerp(merahGelap, merah, (ny + 1f) * 0.5f);
                t.SetPixel(x, y, new Color(isi.r, isi.g, isi.b, hit / 4f));
            }
        }
        t.Apply();
        return t;
    }

    // ====== PETI HARTA BERWARNA ====== (1 kayu, 2 tutup, 3 logam emas, 4 kunci)
    static int PetiKelas(float x, float y)
    {
        float ax = Mathf.Abs(x);
        bool basis = Kotak(x, y, -0.75f, -0.65f, 0.75f, 0.24f);
        bool tutup = Kotak(x, y, -0.80f, 0.24f, 0.80f, 0.58f);
        if (!(basis || tutup)) return 0;
        if (Disc(x, y, 0f, 0.20f, 0.12f)) return 4;   // kunci di tengah
        if (ax <= 0.12f) return 3;                    // pita logam vertikal
        if (y >= 0.18f && y <= 0.30f) return 3;       // seam logam horizontal
        if (tutup && y >= 0.50f) return 3;            // trim atas tutup
        if (tutup) return 2;                          // kayu tutup (lebih gelap)
        return 1;                                     // kayu badan
    }

    static bool FAura(float x, float y)
    {
        return Cincin(x, y, 0.78f, 0.96f) || Cincin(x, y, 0.40f, 0.56f) || Disc(x, y, 0f, 0f, 0.14f);
    }

    static bool FRoket(float x, float y)
    {
        float ax = Mathf.Abs(x);
        bool inside = false;
        if (y >= -0.48f && y <= 0.45f && ax <= 0.22f) inside = true;
        if (y > 0.45f && y <= 0.92f)
        {
            float t = (y - 0.45f) / 0.47f;
            if (ax <= Mathf.Lerp(0.22f, 0f, t)) inside = true;
        }
        if (y >= -0.74f && y <= -0.30f)
        {
            float t = (y + 0.74f) / 0.44f;
            float outer = Mathf.Lerp(0.55f, 0.22f, t);
            if (ax >= 0.20f && ax <= outer) inside = true;
        }
        if (y >= -0.98f && y < -0.74f)
        {
            float t = (y + 0.98f) / 0.24f;
            if (ax <= Mathf.Lerp(0.04f, 0.15f, t)) inside = true;
        }
        if (inside && Disc(x, y, 0f, 0.12f, 0.11f)) return false;
        return inside;
    }

    static bool FPisau(float x, float y)
    {
        float r = Mathf.Sqrt(x * x + y * y);
        float ang = Mathf.Atan2(y, x);
        float step = Mathf.PI * 2f / 4f;
        float a = Mathf.Repeat(ang, step);
        float tt = a / (step / 2f); if (tt > 1f) tt = 2f - tt;
        float radius = Mathf.Lerp(0.98f, 0.28f, tt);
        if (Disc(x, y, 0f, 0f, 0.16f)) return false;
        return r <= radius;
    }

    // Ambil ikon berdasarkan id SKILL. Utamakan FILE, fallback ke ikon KODE.
    public static Texture2D UntukSkill(string id)
    {
        switch (id)
        {
            case "petir": return Dari("petir", Petir);
            case "peluru": return Dari("peluru", Peluru);
            case "target": return Dari("target", Target);
            case "chevron": return Chevron;
            case "hati": return Dari("hati", Hati);
            case "berlian": return Dari("berlian", Berlian);
            case "pisau": return Dari("pisau", Pisau);
            case "aura": return Dari("aura", Aura);
            case "roket": return Dari("roket", Roket);
            default: return Dari("bintang", Bintang);
        }
    }

    // Ambil ikon untuk ITEM lapangan: ikon KODE BERWARNA (transparan, tanpa background).
    public static Texture2D UntukItem(string id)
    {
        switch (id)
        {
            case "bom": return Bom;
            case "magnet": return Magnet;
            case "peti": return Peti;
            default: return Peti;
        }
    }

    // true kalau tekstur ini FILE (asset) ATAU sudah berwarna -> pakai warna asli.
    public static bool AdalahFile(Texture2D tex)
    {
        return tex != null && _fileSet.Contains(tex);
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

        // Ikon berwarna / dari FILE: gambar apa adanya (warna isi diabaikan) + outline tipis.
        if (_fileSet.Contains(tex)) { GambarPenuh(r, tex, garis); return; }

        Color simpan = GUI.color;

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

        GUI.color = isi;
        GUI.DrawTexture(r, tex, ScaleMode.StretchToFill, true);

        GUI.color = simpan;
    }

    // Gambar ikon berwarna/FILE apa adanya + outline gelap tipis (4 arah), jaga rasio aspek.
    static void GambarPenuh(Rect r, Texture2D tex, Color garis)
    {
        Color simpan = GUI.color;
        float o = Mathf.Max(1f, r.width * 0.03f);
        GUI.color = garis;
        GUI.DrawTexture(new Rect(r.x - o, r.y, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(r.x + o, r.y, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(r.x, r.y - o, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(r.x, r.y + o, r.width, r.height), tex, ScaleMode.ScaleToFit, true);
        GUI.color = Color.white; // tampilkan warna ASLI
        GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
        GUI.color = simpan;
    }
}
