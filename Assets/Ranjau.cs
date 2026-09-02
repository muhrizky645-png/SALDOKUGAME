using UnityEngine;

// SENJATA: Ranjau.
// Dipasang di dekat pemain, diam sebentar untuk 'arming', lalu MELEDAK begitu
// ada musuh menyentuh radius pemicunya (atau hilang sendiri setelah 'umur' detik
// biar tidak menumpuk). L1 kecil & 1 ranjau; naik tiap level; evolusi (lvl 5+):
// ungu, 3 ranjau sekaligus + menyambar petir saat meledak.
public class Ranjau : MonoBehaviour
{
    int dmg = 8;
    float radius = 1.2f;
    bool evo = false;
    float pemicuRadius = 0.6f;
    float umur = 8f;
    float arm = 0.4f; // jeda aktif sebelum bisa meledak
    SpriteRenderer sr;
    Color warna;

    public static void Pasang(Vector3 pos, int dmg, float radius, bool evo)
    {
        GameObject go = new GameObject("Ranjau");
        go.transform.position = pos;
        Ranjau r = go.AddComponent<Ranjau>();
        r.dmg = dmg; r.radius = radius; r.evo = evo;
    }

    void Start()
    {
        warna = evo ? new Color(0.8f, 0.5f, 1f, 1f) : new Color(1f, 0.5f, 0.2f, 1f);
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatTitik(32);
        sr.color = warna;
        sr.sortingOrder = 3;
        transform.localScale = Vector3.one * (0.35f + radius * 0.1f);
    }

    void Update()
    {
        if (!GameMenu.SedangMain) return;
        umur -= Time.deltaTime;
        arm -= Time.deltaTime;
        if (umur <= 0f) { Destroy(gameObject); return; }

        // kedip menandakan ranjau aktif
        float a = 0.7f + 0.3f * Mathf.Sin(Time.time * 10f);
        Color c = sr.color; c.a = a; sr.color = c;

        if (arm > 0f) return;

        EnemyChase dekat = EnemyRegistry.Terdekat(transform.position, pemicuRadius, null);
        if (dekat != null) Meledak();
    }

    void Meledak()
    {
        // getar=false: ranjau bisa sering meledak, jadi tak usah menggetarkan layar
        // terus-menerus (getar besar disimpan untuk Meteor).
        Color w = evo ? new Color(0.72f, 0.40f, 1f, 0.8f) : new Color(1f, 0.55f, 0.18f, 0.85f);
        Ledakan.Munculkan(transform.position, radius, dmg, 0f, w, false);
        if (evo)
        {
            EnemyChase e = EnemyRegistry.Terdekat(transform.position, radius * 1.5f, null);
            if (e != null) PetirEfek.Sambar(transform.position, e.transform.position, new Color(0.8f, 0.5f, 1f, 1f), 0.1f);
        }
        Destroy(gameObject);
    }

    static Sprite BuatTitik(int S)
    {
        Texture2D t = new Texture2D(S, S, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        float r = S / 2f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                float a = d <= 0.8f ? 1f : (d <= 1f ? Mathf.Lerp(1f, 0f, (d - 0.8f) / 0.2f) : 0f);
                // titik tengah putih (lampu), badan berwarna
                Color cc = (d <= 0.35f) ? Color.white : Color.white;
                t.SetPixel(x, y, new Color(cc.r, cc.g, cc.b, a));
            }
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }
}
