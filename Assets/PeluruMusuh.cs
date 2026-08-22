using UnityEngine;

// Proyektil yang ditembakkan musuh tipe "Penembak" (EnemyChase.Tipe.Penembak) ke arah
// pemain dari jarak jauh. Tampilannya kini BOLA API bertema (inti terang -> oranye ->
// merah gelap + glow) supaya jelas terbaca sebagai serangan musuh, bukan sekadar titik.
public class PeluruMusuh : MonoBehaviour
{
    public static void Tembak(Vector3 pos, Vector3 arah, float speed, float dmg)
    {
        GameObject go = new GameObject("PeluruMusuh");
        go.transform.position = pos;
        PeluruMusuh p = go.AddComponent<PeluruMusuh>();
        p.arah = arah.normalized;
        p.speed = speed;
        p.dmg = dmg;
    }

    public Vector3 arah = Vector3.right;
    public float speed = 4.5f;
    public float dmg = 7f;
    public float lifeTime = 4f;

    private SpriteRenderer sr;
    private Transform player;
    private float t = 0f;

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatBolaApi(24);
        sr.color = Color.white; // warna asli bola api
        sr.sortingOrder = 45;
        transform.localScale = Vector3.one * 0.55f;

        GameObject pl = GameObject.FindWithTag("Player");
        if (pl != null) player = pl.transform;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += arah * speed * Time.deltaTime;

        // denyut kecil biar terlihat "membara"
        t += Time.deltaTime;
        float k = 1f + 0.12f * Mathf.Sin(t * 18f);
        transform.localScale = Vector3.one * 0.55f * k;

        if (player != null && Vector3.Distance(transform.position, player.position) <= 0.45f)
        {
            if (PlayerHealth.Instance != null) PlayerHealth.Instance.Kurangi(dmg);
            Destroy(gameObject);
        }
    }

    // Bola api: inti kuning-terang -> oranye -> merah gelap, dengan halo/glow lembut di tepi.
    Sprite BuatBolaApi(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.wrapMode = TextureWrapMode.Clamp;

        float c = (size - 1) / 2f;
        float rSolid = size * 0.40f;   // bola padat
        float rGlow = size * 0.50f;    // halo
        Color inti = new Color(1f, 0.96f, 0.70f);
        Color tengah = new Color(1f, 0.55f, 0.16f);
        Color tepi = new Color(0.75f, 0.12f, 0.05f);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - c, dy = y - c;
                float d = Mathf.Sqrt(dx * dx + dy * dy);

                if (d <= rSolid)
                {
                    float tt = d / rSolid; // 0 pusat .. 1 tepi
                    Color col = (tt < 0.4f)
                        ? Color.Lerp(inti, tengah, tt / 0.4f)
                        : Color.Lerp(tengah, tepi, (tt - 0.4f) / 0.6f);
                    float a = (d < rSolid - 1f) ? 1f : Mathf.Clamp01(rSolid - d); // tepi halus
                    tex.SetPixel(x, y, new Color(col.r, col.g, col.b, a));
                }
                else if (d <= rGlow)
                {
                    float g = 1f - (d - rSolid) / (rGlow - rSolid); // 1 di tepi bola -> 0
                    tex.SetPixel(x, y, new Color(tepi.r, tepi.g * 0.6f, tepi.b, g * 0.35f));
                }
                else
                {
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                }
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
