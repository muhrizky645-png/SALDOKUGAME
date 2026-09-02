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
    float pemicuRadius = 0.7f;
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
        warna = evo ? new Color(0.85f, 0.55f, 1f, 1f) : new Color(1f, 0.55f, 0.2f, 1f);
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatRanjau(48);
        sr.color = warna;
        sr.sortingOrder = 3;
        // JAUH lebih besar dari sebelumnya biar tidak seperti titik kecil.
        transform.localScale = Vector3.one * (1.4f + radius * 0.5f);
    }

    void Update()
    {
        if (!GameMenu.SedangMain) return;
        umur -= Time.deltaTime;
        arm -= Time.deltaTime;
        if (umur <= 0f) { Destroy(gameObject); return; }

        // kedip menandakan ranjau aktif (makin kentara)
        float a = 0.55f + 0.45f * Mathf.Sin(Time.time * 10f);
        Color c = warna; c.a = a; sr.color = c;

        if (arm > 0f) return;

        EnemyChase dekat = EnemyRegistry.Terdekat(transform.position, pemicuRadius, null);
        if (dekat != null) Meledak();
    }

    void Meledak()
    {
        // getar=false: ranjau bisa sering meledak, jadi tak usah menggetarkan layar
        // terus-menerus (getar besar disimpan untuk Meteor).
        Color w = evo ? new Color(0.72f, 0.40f, 1f, 0.85f) : new Color(1f, 0.55f, 0.18f, 0.9f);
        Ledakan.Munculkan(transform.position, radius, dmg, 0f, w, false);
        if (evo)
        {
            EnemyChase e = EnemyRegistry.Terdekat(transform.position, radius * 1.5f, null);
            if (e != null) PetirEfek.Sambar(transform.position, e.transform.position, new Color(0.85f, 0.55f, 1f, 1f), 0.1f);
        }
        Destroy(gameObject);
    }

    // Ranjau laut: badan bulat + 8 paku meruncing keluar. Digambar PUTIH lalu
    // diwarnai lewat SpriteRenderer.color (biar kedip alpha tetap jalan).
    static Sprite BuatRanjau(int S)
    {
        Texture2D t = new Texture2D(S, S, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        float r = S / 2f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float nx = (x - r + 0.5f) / r, ny = (y - r + 0.5f) / r;
                float d = Mathf.Sqrt(nx * nx + ny * ny);
                bool isi = d <= 0.5f; // badan
                if (!isi)
                {
                    // 8 paku: cek jarak titik ke segmen dari pusat ke ujung paku
                    for (int k = 0; k < 8; k++)
                    {
                        float ang = k * Mathf.PI / 4f;
                        float ex = Mathf.Cos(ang) * 0.96f, ey = Mathf.Sin(ang) * 0.96f;
                        float len = ex * ex + ey * ey;
                        float tt = len > 0f ? Mathf.Clamp01((nx * ex + ny * ey) / len) : 0f;
                        float px = ex * tt, py = ey * tt, ddx = nx - px, ddy = ny - py;
                        // paku meruncing: makin ke ujung makin tipis
                        float lebar = Mathf.Lerp(0.14f, 0.03f, tt);
                        if (ddx * ddx + ddy * ddy <= lebar * lebar) { isi = true; break; }
                    }
                }
                // lubang gelap kecil di tengah biar terlihat seperti mata ranjau
                if (isi && d <= 0.16f) isi = false;
                t.SetPixel(x, y, new Color(1f, 1f, 1f, isi ? 1f : 0f));
            }
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }
}
