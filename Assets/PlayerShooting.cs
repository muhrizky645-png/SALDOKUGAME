using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1.2f;   // jeda antar tembakan (makin besar = makin pelan)
    public float range = 1f;        // jarak deteksi zombie
    public int jumlahPeluru = 1;    // berapa peluru sekali tembak (naik lewat skill)
    public float sudutSebar = 12f;  // sebaran sudut antar peluru (derajat)

    private float timer = 0.5f;      // sedikit jeda sebelum tembakan pertama

    // ====== SPRITE PROYEKTIL PER KARAKTER ======
    // Sebagian karakter memakai proyektil KHUSUS yang lebih masuk akal:
    //   TENTARA (idx 6) -> peluru        PEMANAH (idx 0) -> anak panah
    //   KESATRIA (idx 7) -> pedang (dilempar berputar)
    // Karakter lain memakai sprite SENJATA-nya (ninja=shuriken, dst).

    static readonly Dictionary<int, Sprite> _cacheSenjata = new Dictionary<int, Sprite>();
    static Sprite _sprPanah, _sprPedang, _sprPeluru;

    static Sprite SpriteSenjata(int idx)
    {
        Sprite s;
        if (_cacheSenjata.TryGetValue(idx, out s)) return s;
        Texture2D t = KarakterManager.Tekstur(idx, "Weapon");
        s = (t != null)
            ? Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100f)
            : null;
        _cacheSenjata[idx] = s;
        return s;
    }

    // orient = true -> proyektil menghadap arah terbang & tidak berputar (peluru/panah)
    static Sprite SpriteProyektil(int idx, out bool orient)
    {
        switch (idx)
        {
            case 6: orient = true; return SprPeluru();   // TENTARA -> peluru
            case 0: orient = true; return SprPanah();    // PEMANAH -> anak panah
            case 7: orient = false; return SprPedang();  // KESATRIA -> pedang (berputar saat dilempar)
            default: orient = false; return SpriteSenjata(idx);
        }
    }

    static Sprite SprPanah()
    {
        if (_sprPanah == null)
        {
            Texture2D t = Bake(BentukPanah, 72, new Color(0.80f, 0.62f, 0.36f));
            _sprPanah = Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100f);
        }
        return _sprPanah;
    }

    static Sprite SprPedang()
    {
        if (_sprPedang == null)
        {
            Texture2D t = Bake(BentukPedang, 72, new Color(0.86f, 0.90f, 0.98f));
            _sprPedang = Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100f);
        }
        return _sprPedang;
    }

    static Sprite SprPeluru()
    {
        if (_sprPeluru == null)
        {
            Texture2D t = Bake(BentukPeluru, 72, new Color(1f, 0.85f, 0.30f));
            _sprPeluru = Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100f);
        }
        return _sprPeluru;
    }

    // ---- rasterizer kecil + primitif bentuk (ruang -1..1, y ke atas) ----
    static Texture2D Bake(System.Func<float, float, bool> f, int size, Color warna)
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
                t.SetPixel(x, y, new Color(warna.r, warna.g, warna.b, hit / 4f));
            }
        t.Apply();
        return t;
    }

    static bool Seg(float x, float y, float ax, float ay, float bx, float by, float w)
    {
        float vx = bx - ax, vy = by - ay, wx = x - ax, wy = y - ay;
        float len = vx * vx + vy * vy;
        float tt = len > 0f ? Mathf.Clamp01((wx * vx + wy * vy) / len) : 0f;
        float px = ax + vx * tt, py = ay + vy * tt, dx = x - px, dy = y - py;
        return dx * dx + dy * dy <= w * w;
    }

    static bool Box(float x, float y, float x0, float y0, float x1, float y1)
    {
        return x >= x0 && x <= x1 && y >= y0 && y <= y1;
    }

    static bool Circ(float x, float y, float cx, float cy, float r)
    {
        float dx = x - cx, dy = y - cy;
        return dx * dx + dy * dy <= r * r;
    }

    // panah: menghadap ATAS (+Y)
    static bool BentukPanah(float x, float y)
    {
        float w = 0.08f;
        bool batang = Seg(x, y, 0f, -0.78f, 0f, 0.55f, w);
        bool kepala = Seg(x, y, 0f, 0.92f, -0.40f, 0.42f, 0.11f)
                   || Seg(x, y, 0f, 0.92f, 0.40f, 0.42f, 0.11f);
        bool ekor = Seg(x, y, 0f, -0.55f, -0.32f, -0.92f, w)
                 || Seg(x, y, 0f, -0.55f, 0.32f, -0.92f, w);
        return batang || kepala || ekor;
    }

    // pedang: menghadap ATAS (+Y)
    static bool BentukPedang(float x, float y)
    {
        bool bilah = false;
        if (y >= -0.08f && y <= 0.72f && Mathf.Abs(x) <= 0.10f) bilah = true;
        if (y > 0.72f && y <= 0.95f)
        {
            float t = (y - 0.72f) / 0.23f;
            if (Mathf.Abs(x) <= Mathf.Lerp(0.10f, 0f, t)) bilah = true;
        }
        bool pelindung = Box(x, y, -0.42f, -0.22f, 0.42f, -0.08f);
        bool gagang = Box(x, y, -0.07f, -0.52f, 0.07f, -0.22f);
        bool pangkal = Circ(x, y, 0f, -0.56f, 0.11f);
        return bilah || pelindung || gagang || pangkal;
    }

    // peluru: menghadap ATAS (+Y)
    static bool BentukPeluru(float x, float y)
    {
        if (Box(x, y, -0.15f, -0.60f, 0.15f, 0.40f)) return true;
        if (Circ(x, y, 0f, 0.40f, 0.15f)) return true;
        return false;
    }

    void Update()
    {
        // pakai timer manual (bukan InvokeRepeating) supaya perubahan fireRate
        // dari skill langsung terasa saat itu juga
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Shoot();
            // MODE DEWA: BADAI PELURU -> tembak jauh lebih cepat
            timer = fireRate * (ModeDewa.Aktif ? 0.12f : 1f);
        }
    }

    void Shoot()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
        if (zombies.Length == 0) return;

        // MODE DEWA: jangkauan tembak jauh lebih luas
        float rangeEfektif = range * (ModeDewa.Aktif ? 4f : 1f);

        // cari zombie terdekat
        GameObject terdekat = null;
        float jarakTerdekat = rangeEfektif;
        foreach (GameObject z in zombies)
        {
            float jarak = Vector3.Distance(transform.position, z.transform.position);
            if (jarak < jarakTerdekat)
            {
                jarakTerdekat = jarak;
                terdekat = z;
            }
        }

        if (terdekat == null) return;

        Vector3 arah = (terdekat.transform.position - transform.position).normalized;

        // proyektil sesuai karakter terpilih
        bool orient;
        Sprite peluruSpr = SpriteProyektil(KarakterManager.Dipilih, out orient);

        // tembak beberapa peluru sekaligus dengan sedikit sebaran (kalau punya skill)
        // MODE DEWA: BADAI PELURU -> peluru jauh lebih banyak
        int n = Mathf.Max(1, jumlahPeluru + (ModeDewa.Aktif ? 10 : 0));
        float total = (n - 1) * sudutSebar;
        float mulai = -total / 2f;
        for (int i = 0; i < n; i++)
        {
            float sudut = mulai + i * sudutSebar;
            Vector3 arahPeluru = Quaternion.Euler(0f, 0f, sudut) * arah;
            GameObject peluru = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            if (peluruSpr != null)
            {
                SpriteRenderer psr = peluru.GetComponent<SpriteRenderer>();
                if (psr != null) { psr.sprite = peluruSpr; psr.color = Color.white; }
            }
            Bullet b = peluru.GetComponent<Bullet>();
            b.direction = arahPeluru;
            b.orientKeArah = orient;
        }

        SoundManager.Tembak(); // suara tembak
    }
}
