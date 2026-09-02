using UnityEngine;

// SENJATA: Meteor.
// Menandai lokasi musuh (penanda berkedip sebentar) lalu MENGHANTAM dari langit
// dengan ledakan AoE besar + getar layar. Cocok untuk melibas kerumunan. L1
// kecil & 1 meteor; naik tiap level; evolusi (lvl 5+): ungu, lebih besar +
// menyambar petir ke musuh di area hantaman.
public class Meteor : MonoBehaviour
{
    Vector3 sasaran;
    int dmg = 10;
    float radius = 1.6f;
    bool evo = false;
    float delay = 0.55f; // jeda peringatan sebelum menghantam
    SpriteRenderer tanda;
    Color warna;

    public static void Panggil(Vector3 sasaran, int dmg, float radius, bool evo)
    {
        GameObject go = new GameObject("Meteor");
        go.transform.position = sasaran;
        Meteor m = go.AddComponent<Meteor>();
        m.sasaran = sasaran; m.dmg = dmg; m.radius = radius; m.evo = evo;
    }

    void Start()
    {
        warna = evo ? new Color(0.8f, 0.5f, 1f, 1f) : new Color(1f, 0.45f, 0.2f, 1f);
        tanda = gameObject.AddComponent<SpriteRenderer>();
        tanda.sprite = BuatTanda(48);
        tanda.color = new Color(warna.r, warna.g, warna.b, 0.55f);
        tanda.sortingOrder = 3;
        transform.localScale = Vector3.one * (2f * radius) / 0.48f;
    }

    void Update()
    {
        if (!GameMenu.SedangMain) return;
        delay -= Time.deltaTime;
        // penanda berkedip makin cepat
        float a = 0.35f + 0.35f * Mathf.Sin(Time.time * 18f);
        Color c = tanda.color; c.a = a; tanda.color = c;
        if (delay <= 0f) Jatuh();
    }

    void Jatuh()
    {
        // getar=false di Ledakan supaya kita atur getar sendiri (lebih besar utk meteor)
        Color w = evo ? new Color(0.72f, 0.4f, 1f, 0.85f) : new Color(1f, 0.5f, 0.15f, 0.9f);
        Ledakan.Munculkan(sasaran, radius, dmg, 0f, w, false);
        ScreenShake.Getar(evo ? 0.9f : 0.6f, 0.3f);
        if (evo)
        {
            int n = EnemyRegistry.DalamRadius(sasaran, radius, EnemyRegistry.Buffer);
            int p = Mathf.Min(n, 4);
            Color ungu = new Color(0.8f, 0.5f, 1f, 1f);
            for (int i = 0; i < p; i++)
            {
                EnemyChase e = EnemyRegistry.Buffer[i];
                if (e != null) PetirEfek.Sambar(sasaran, e.transform.position, ungu, 0.1f);
            }
        }
        Destroy(gameObject);
    }

    static Sprite BuatTanda(int S)
    {
        Texture2D t = new Texture2D(S, S, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        float r = S / 2f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float nx = (x - r + 0.5f) / r, ny = (y - r + 0.5f) / r;
                float d = Mathf.Sqrt(nx * nx + ny * ny);
                // cincin sasaran + garis silang (crosshair)
                float cincin = (d >= 0.78f && d <= 1f) ? 1f : 0f;
                float silang = (Mathf.Abs(nx) < 0.06f || Mathf.Abs(ny) < 0.06f) && d <= 1f ? 0.9f : 0f;
                float a = Mathf.Max(cincin, silang);
                t.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }
}
