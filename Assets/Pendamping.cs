using UnityEngine;

// SENJATA: Pendamping.
// Bola energi yang MELAYANG mengelilingi pemain dan OTOMATIS menyambar musuh
// terdekat dalam jangkauannya (sambaran petir instan via PetirEfek). L1 1 bola &
// lemah; naik tiap level (lebih cepat & kuat); evolusi (lvl 5+): ungu, 2 bola +
// sambarannya meloncat ke musuh kedua.
public class Pendamping : MonoBehaviour
{
    Transform pemain;
    int indeks, total;
    int dmg;
    float jeda, jangkauan;
    Color warna;
    bool evo;
    float timer;
    SpriteRenderer sr;

    public static Pendamping Buat(Transform pemain, int indeks, int total, int dmg, float jeda, float jangkauan, float skala, Color warna, bool evo)
    {
        GameObject go = new GameObject("Pendamping");
        Pendamping p = go.AddComponent<Pendamping>();
        p.pemain = pemain; p.indeks = indeks; p.total = total;
        p.dmg = dmg; p.jeda = jeda; p.jangkauan = jangkauan;
        p.warna = warna; p.evo = evo;
        p.Init(skala);
        return p;
    }

    void Init(float skala)
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatOrb(32);
        sr.color = warna;
        sr.sortingOrder = 6;
        transform.localScale = Vector3.one * skala;
        if (pemain != null) transform.position = pemain.position;
    }

    void Update()
    {
        if (pemain == null) return;

        // melayang mengelilingi pemain
        float sudut = (Time.time * 40f + indeks * (360f / Mathf.Max(1, total))) * Mathf.Deg2Rad;
        float r = 1.3f;
        Vector3 target = pemain.position + new Vector3(Mathf.Cos(sudut), Mathf.Sin(sudut), 0f) * r;
        transform.position = Vector3.Lerp(transform.position, target, 10f * Time.deltaTime);

        if (!GameMenu.SedangMain) return;

        timer += Time.deltaTime;
        if (timer >= jeda)
        {
            EnemyChase e = EnemyRegistry.Terdekat(transform.position, jangkauan, null);
            if (e != null)
            {
                timer = 0f;
                e.KenaSerangan(dmg, false);
                PetirEfek.Sambar(transform.position, e.transform.position, warna, 0.1f);
                if (evo)
                {
                    EnemyChase e2 = EnemyRegistry.Terdekat(e.transform.position, 3f, e);
                    if (e2 != null)
                    {
                        e2.KenaSerangan(dmg, false);
                        PetirEfek.Sambar(e.transform.position, e2.transform.position, warna, 0.1f);
                    }
                }
            }
            else
            {
                timer = jeda; // tetap siap menembak
            }
        }
    }

    static Sprite BuatOrb(int S)
    {
        Texture2D t = new Texture2D(S, S, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        float r = S / 2f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                float a = d <= 0.5f ? 1f : (d <= 1f ? Mathf.Lerp(1f, 0f, (d - 0.5f) / 0.5f) : 0f);
                t.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }
}
