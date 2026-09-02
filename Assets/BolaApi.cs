using UnityEngine;

// SENJATA: Bola Api.
// Melesat ke musuh terdekat; saat mengenai musuh / habis umur -> MELEDAK (area
// damage sekali) lalu meninggalkan GENANGAN API yang terus membakar musuh di
// dalamnya selama beberapa detik. Saat evolusi: bola & genangan berwarna ungu,
// dan ledakannya menyambar petir (PetirEfek).
//
// Dua kelas di satu file: BolaApi (proyektil) + GenanganApi (genangan bakar).
public class BolaApi : MonoBehaviour
{
    Transform target;
    Vector3 arah = Vector3.right;
    float speed = 7f;
    int dmg = 8;
    float radius = 1.3f;
    float skala = 0.5f;
    bool evo = false;
    float durasiGenangan = 2f;
    int dmgGenangan = 2;
    float umur = 3f;
    SpriteRenderer sr;

    public static void Tembak(Vector3 pos, Transform target, int dmg, float radius, float skala, bool evo, float durasiGenangan, int dmgGenangan)
    {
        GameObject go = new GameObject("BolaApi");
        go.transform.position = pos;
        BolaApi b = go.AddComponent<BolaApi>();
        b.target = target;
        b.dmg = dmg;
        b.radius = radius;
        b.skala = skala;
        b.evo = evo;
        b.durasiGenangan = durasiGenangan;
        b.dmgGenangan = dmgGenangan;
        if (target != null) b.arah = (target.position - pos).normalized;
    }

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatBulatan(48, evo ? new Color(0.75f, 0.45f, 1f) : new Color(1f, 0.55f, 0.15f));
        sr.sortingOrder = 46;
        transform.localScale = Vector3.one * skala;
    }

    void Update()
    {
        umur -= Time.deltaTime;
        if (umur <= 0f) { Meledak(); return; }

        if (target == null) target = Cari();
        if (target != null)
            arah = Vector3.Lerp(arah, (target.position - transform.position).normalized, 5f * Time.deltaTime).normalized;

        transform.position += arah * speed * Time.deltaTime;
        transform.Rotate(0, 0, 360f * Time.deltaTime); // sedikit berputar

        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var m in musuh)
        {
            if (m == null) continue;
            if (Vector3.Distance(transform.position, m.transform.position) <= 0.5f) { Meledak(); return; }
        }
    }

    void Meledak()
    {
        Color w = evo ? new Color(0.72f, 0.40f, 1f, 0.75f) : new Color(1f, 0.55f, 0.18f, 0.75f);
        Ledakan.Munculkan(transform.position, radius, dmg, 0f, w, false);

        GenanganApi.Buat(transform.position, radius * 1.05f, durasiGenangan, dmgGenangan, evo);

        if (evo)
        {
            GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
            Color ungu = new Color(0.8f, 0.5f, 1f, 1f);
            int dibuat = 0;
            foreach (var m in musuh)
            {
                if (m == null) continue;
                if (Vector3.Distance(transform.position, m.transform.position) > radius * 1.6f) continue;
                PetirEfek.Sambar(transform.position, m.transform.position, ungu, 0.10f);
                if (++dibuat >= 3) break;
            }
        }

        Destroy(gameObject);
    }

    Transform Cari()
    {
        EnemyChase e = EnemyRegistry.Terdekat(transform.position, 30f, null);
        return e != null ? e.transform : null;
    }

    static Sprite BuatBulatan(int S, Color inti)
    {
        Texture2D t = new Texture2D(S, S, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        float r = S / 2f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                Color c = (d <= 0.5f) ? Color.Lerp(Color.white, inti, d / 0.5f) : inti;
                float a = d <= 1f ? Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0.7f, 1f, d)) : 0f;
                t.SetPixel(x, y, new Color(c.r, c.g, c.b, a));
            }
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }
}

// Genangan api: menetap sejenak di tanah dan membakar musuh di dalamnya tiap
// ~0.4 dtk. Sortir di bawah musuh (sortingOrder kecil) supaya terlihat seperti
// di lantai. Berkedip lembut lalu hilang saat waktunya habis.
public class GenanganApi : MonoBehaviour
{
    float radius = 1.3f;
    float sisa = 2f;
    int dmg = 2;
    bool evo = false;
    float tick = 0f;
    SpriteRenderer sr;

    public static void Buat(Vector3 pos, float radius, float durasi, int dmg, bool evo)
    {
        GameObject go = new GameObject("GenanganApi");
        go.transform.position = pos;
        GenanganApi g = go.AddComponent<GenanganApi>();
        g.radius = radius;
        g.sisa = durasi;
        g.dmg = dmg;
        g.evo = evo;
    }

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatLingkaran(48);
        sr.color = evo ? new Color(0.7f, 0.4f, 1f, 0.35f) : new Color(1f, 0.5f, 0.15f, 0.35f);
        sr.sortingOrder = 4;
        transform.localScale = Vector3.one * (2f * radius) / 0.48f;
    }

    void Update()
    {
        if (!GameMenu.SedangMain) return;
        sisa -= Time.deltaTime;
        if (sisa <= 0f) { Destroy(gameObject); return; }

        // kedip lembut biar terlihat menyala
        float a = 0.28f + 0.10f * Mathf.Sin(Time.time * 12f);
        Color c = sr.color; c.a = a; sr.color = c;

        tick -= Time.deltaTime;
        if (tick <= 0f)
        {
            tick = 0.4f;
            int n = EnemyRegistry.DalamRadius(transform.position, radius, EnemyRegistry.Buffer);
            for (int i = 0; i < n; i++)
            {
                EnemyChase ec = EnemyRegistry.Buffer[i];
                if (ec != null && !ec.SudahMati) ec.KenaSerangan(dmg, false);
            }
        }
    }

    static Sprite BuatLingkaran(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                float a = d <= 1f ? Mathf.Lerp(0.6f, 0f, d) : 0f;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
