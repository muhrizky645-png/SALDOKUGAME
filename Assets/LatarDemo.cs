using UnityEngine;
using System.Collections.Generic;

// ============================================================================
//  LATAR MENU "DEMO BATTLE" ala mini-video BLUR (versi CERAH Survivor.io).
//  Simulasi pertempuran SILUET (hero + gerombolan musuh + tracer tembakan +
//  ledakan + bara api) yang berjalan MULUS walau game di-pause
//  (Time.timeScale = 0), karena memakai waktu UNSCALED.
//
//  Semua digambar dengan tekstur LEMBUT (radial falloff) sehingga terlihat
//  BLUR seperti rekaman di balik kaca buram, di atas latar ORANYE CERAH,
//  lalu ditimpa scrim hangat TIPIS + vignette lembut (dari Tema) supaya teks
//  & panel menu tetap terbaca tanpa membuat layar jadi gelap.
//
//  Sepenuhnya MANDIRI: tidak menyentuh Player/Spawner/kamera game asli, jadi
//  aman & tidak mempengaruhi jalannya permainan. Tekstur runtime memakai
//  HideAndDontSave supaya tidak ikut terhapus saat scene reload.
// ============================================================================
public static class LatarDemo
{
    // ---- tekstur bulat LEMBUT (radial gradient) untuk kesan blur ----
    static Texture2D _blob;
    static Texture2D Blob
    {
        get
        {
            if (_blob == null)
            {
                int s = 64;
                _blob = new Texture2D(s, s, TextureFormat.RGBA32, false);
                _blob.hideFlags = HideFlags.HideAndDontSave;
                _blob.wrapMode = TextureWrapMode.Clamp;
                _blob.filterMode = FilterMode.Bilinear;
                float r = s / 2f;
                for (int y = 0; y < s; y++)
                    for (int x = 0; x < s; x++)
                    {
                        float dx = (x + 0.5f - r) / r;
                        float dy = (y + 0.5f - r) / r;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        float a = Mathf.Clamp01(1f - d);
                        a = a * a; // falloff lembut -> tepi kabur (blur)
                        _blob.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                    }
                _blob.Apply();
            }
            return _blob;
        }
    }

    class Musuh { public float sudut; public float jarak; public float spd; public float sz; public int hp; public bool hidup; public float mati; }
    class Tracer { public Vector2 a, b; public float umur; }
    class Kilat { public Vector2 pos; public float umur; public float besar; }
    class Bara  { public float x, y, spd, sz, goyang, fase; }

    static Musuh[] _musuh;
    static readonly List<Tracer> _tracer = new List<Tracer>();
    static readonly List<Kilat>  _kilat  = new List<Kilat>();
    static Bara[] _bara;
    static float _lastT = -1f;
    static float _tembakT = 0.3f;
    static float _muzzle = 0f;
    static float _fase = 0f; // untuk bob hero & goyang halus

    static void Init()
    {
        _musuh = new Musuh[14];
        for (int i = 0; i < _musuh.Length; i++)
        {
            _musuh[i] = new Musuh();
            Reset(_musuh[i], Random.Range(0.35f, 1.05f));
        }
        _bara = new Bara[26];
        for (int i = 0; i < _bara.Length; i++)
            _bara[i] = new Bara
            {
                x = Random.value,
                y = Random.value,
                spd = Random.Range(0.02f, 0.06f),
                sz = Random.Range(0.006f, 0.018f),
                goyang = Random.Range(0.01f, 0.03f),
                fase = Random.Range(0f, 10f)
            };
        _tracer.Clear();
        _kilat.Clear();
    }

    static void Reset(Musuh m, float jarak)
    {
        m.sudut = Random.Range(0f, Mathf.PI * 2f);
        m.jarak = jarak;
        m.spd = Random.Range(0.05f, 0.11f);
        m.sz = Random.Range(0.05f, 0.09f);
        m.hp = Random.Range(2, 5);
        m.hidup = true;
        m.mati = 0f;
    }

    static void Langkah(float dt, float w, float h)
    {
        float u = Mathf.Min(w, h);
        Vector2 hero = new Vector2(w * 0.5f, h * 0.5f);
        _fase += dt;

        // gerak musuh mendekat / respawn setelah mati
        foreach (var m in _musuh)
        {
            if (m.hidup)
            {
                m.jarak -= m.spd * dt;
                if (m.jarak < 0.12f) m.jarak = 0.12f;
            }
            else
            {
                m.mati += dt;
                if (m.mati > 0.45f) Reset(m, Random.Range(0.85f, 1.15f));
            }
        }

        // hero menembak musuh TERDEKAT
        _tembakT -= dt;
        if (_muzzle > 0f) _muzzle -= dt;
        if (_tembakT <= 0f)
        {
            Musuh t = null; float best = 999f;
            foreach (var m in _musuh)
                if (m.hidup && m.jarak < best) { best = m.jarak; t = m; }
            if (t != null)
            {
                _tembakT = Random.Range(0.16f, 0.30f);
                _muzzle = 0.09f;
                Vector2 ep = hero + new Vector2(Mathf.Cos(t.sudut), Mathf.Sin(t.sudut)) * (t.jarak * u);
                _tracer.Add(new Tracer { a = hero, b = ep, umur = 1f });
                t.hp -= 1;
                if (t.hp <= 0)
                {
                    t.hidup = false; t.mati = 0f;
                    _kilat.Add(new Kilat { pos = ep, umur = 1f, besar = u * Random.Range(0.10f, 0.16f) });
                }
            }
            else _tembakT = 0.2f;
        }

        for (int i = _tracer.Count - 1; i >= 0; i--)
        {
            _tracer[i].umur -= dt * 3.4f;
            if (_tracer[i].umur <= 0f) _tracer.RemoveAt(i);
        }
        for (int i = _kilat.Count - 1; i >= 0; i--)
        {
            _kilat[i].umur -= dt * 2.2f;
            if (_kilat[i].umur <= 0f) _kilat.RemoveAt(i);
        }

        foreach (var b in _bara)
        {
            b.y -= b.spd * dt;
            if (b.y < -0.05f) { b.y = 1.05f; b.x = Random.value; }
        }
    }

    // ---- util gambar ----
    static void GambarBlob(float cx, float cy, float r, Color c)
    {
        Color s = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(new Rect(cx - r, cy - r, r * 2f, r * 2f), Blob);
        GUI.color = s;
    }

    static void GambarGaris(Vector2 a, Vector2 b, float lebar, Color c)
    {
        int n = 9;
        for (int i = 0; i < n; i++)
        {
            float f = (i + 0.5f) / n;
            Vector2 p = Vector2.Lerp(a, b, f);
            GambarBlob(p.x, p.y, lebar, c);
        }
    }

    // ====== DIPANGGIL DARI GameMenu (Home) ======
    public static void Gambar(float w, float h)
    {
        if (_musuh == null) { Init(); _lastT = Time.unscaledTime; }

        if (Event.current != null && Event.current.type == EventType.Repaint)
        {
            float now = Time.unscaledTime;
            float dt = Mathf.Clamp(now - _lastT, 0f, 0.05f);
            _lastT = now;
            Langkah(dt, w, h);
        }

        float u = Mathf.Min(w, h);
        Vector2 hero = new Vector2(w * 0.5f, h * 0.5f);

        // 1) dasar SIANG CERAH (gradasi oranye hangat ala Survivor.io)
        Tema.KotakGradien(new Rect(0, 0, w, h),
            new Color(1f, 0.74f, 0.34f, 1f), new Color(0.96f, 0.50f, 0.14f, 1f));

        // 2) bara/percik cahaya melayang (putih hangat, lembut)
        foreach (var b in _bara)
        {
            float bx = (b.x + Mathf.Sin(_fase * 0.6f + b.fase) * b.goyang) * w;
            float by = b.y * h;
            GambarBlob(bx, by, b.sz * u, new Color(1f, 0.95f, 0.7f, 0.30f));
        }

        // 3) musuh (siluet gelap + inti merah samar -> kontras di latar terang)
        foreach (var m in _musuh)
        {
            if (!m.hidup) continue;
            Vector2 p = hero + new Vector2(Mathf.Cos(m.sudut), Mathf.Sin(m.sudut)) * (m.jarak * u);
            float r = m.sz * u;
            GambarBlob(p.x, p.y, r, new Color(0.16f, 0.08f, 0.04f, 0.72f));
            GambarBlob(p.x, p.y, r * 0.45f, new Color(0.80f, 0.16f, 0.12f, 0.5f));
        }

        // 4) tracer tembakan (kuning-putih menyala)
        foreach (var t in _tracer)
        {
            float a = Mathf.Clamp01(t.umur);
            GambarGaris(t.a, t.b, u * 0.010f, new Color(1f, 0.95f, 0.55f, 0.7f * a));
        }

        // 5) kilatan moncong di hero
        if (_muzzle > 0f)
            GambarBlob(hero.x, hero.y, u * 0.06f, new Color(1f, 0.97f, 0.7f, 0.6f));

        // 6) ledakan kematian musuh
        foreach (var k in _kilat)
        {
            float a = Mathf.Clamp01(k.umur);
            float r = k.besar * (1.2f - a * 0.5f);
            GambarBlob(k.pos.x, k.pos.y, r, new Color(1f, 0.7f, 0.32f, 0.6f * a));
            GambarBlob(k.pos.x, k.pos.y, r * 0.5f, new Color(1f, 0.97f, 0.72f, 0.6f * a));
        }

        // 7) hero (siluet dengan rim hijau + bob halus)
        float bob = Mathf.Sin(_fase * 3.2f) * u * 0.006f;
        GambarBlob(hero.x, hero.y + bob, u * 0.14f, new Color(0.14f, 0.09f, 0.05f, 0.82f));
        GambarBlob(hero.x, hero.y + bob, u * 0.10f, new Color(0.55f, 0.82f, 0.22f, 0.18f));

        // 8) scrim hangat TIPIS + vignette lembut biar UI kebaca tapi tetap CERAH
        Tema.Kotak(new Rect(0, 0, w, h), new Color(1f, 0.55f, 0.15f, 0.10f));
        Tema.Vignette();
    }
}
