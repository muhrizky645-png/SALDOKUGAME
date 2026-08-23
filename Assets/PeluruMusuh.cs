using UnityEngine;

// Proyektil yang ditembakkan musuh tipe "Penembak" (EnemyChase.Tipe.Penembak) ke arah
// pemain dari jarak jauh. Bentuknya ANAK PANAH (menghadap arah terbang) supaya cocok
// dengan musuh pemanah/busur dan mudah terbaca sebagai serangan musuh (bukan titik kecil).
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

    static Sprite _panah;

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = SpritePanah();
        sr.color = Color.white;
        sr.sortingOrder = 45;
        transform.localScale = Vector3.one * 1.25f; // lebih besar dari titik lama

        // hadapkan panah ke arah terbang (sprite digambar menghadap ATAS/+Y)
        float a = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, a);

        GameObject pl = GameObject.FindWithTag("Player");
        if (pl != null) player = pl.transform;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += arah * speed * Time.deltaTime;

        if (player != null && Vector3.Distance(transform.position, player.position) <= 0.45f)
        {
            if (PlayerHealth.Instance != null) PlayerHealth.Instance.Kurangi(dmg);
            Destroy(gameObject);
        }
    }

    // ====== sprite anak panah (di-cache) ======
    static Sprite SpritePanah()
    {
        if (_panah == null)
        {
            Texture2D t = BakeArrow(64);
            _panah = Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100f);
        }
        return _panah;
    }

    static Texture2D BakeArrow(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color badan = new Color(0.95f, 0.35f, 0.18f);  // oranye-merah (panah api musuh)
        Color garis = new Color(0.35f, 0.08f, 0.05f);  // outline gelap
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int hit = 0, hitLuar = 0;
                for (int sy = 0; sy < 2; sy++)
                    for (int sx = 0; sx < 2; sx++)
                    {
                        float nx = ((x + (sx + 0.5f) / 2f) / size) * 2f - 1f;
                        float ny = ((y + (sy + 0.5f) / 2f) / size) * 2f - 1f;
                        if (Panah(nx, ny, 0f)) hit++;
                        if (Panah(nx, ny, 0.06f)) hitLuar++;
                    }
                if (hit > 0)
                    tex.SetPixel(x, y, new Color(badan.r, badan.g, badan.b, hit / 4f));
                else if (hitLuar > 0)
                    tex.SetPixel(x, y, new Color(garis.r, garis.g, garis.b, hitLuar / 4f));
                else
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
            }
        tex.Apply();
        return tex;
    }

    // bentuk panah menghadap ATAS (+Y); pad = pelebaran untuk outline
    static bool Panah(float x, float y, float pad)
    {
        float w = 0.09f + pad;
        bool batang = Seg(x, y, 0f, -0.78f, 0f, 0.55f, w);
        bool kepala = Seg(x, y, 0f, 0.92f, -0.42f, 0.42f, 0.12f + pad)
                   || Seg(x, y, 0f, 0.92f, 0.42f, 0.42f, 0.12f + pad);
        bool ekor = Seg(x, y, 0f, -0.55f, -0.34f, -0.92f, w)
                 || Seg(x, y, 0f, -0.55f, 0.34f, -0.92f, w);
        return batang || kepala || ekor;
    }

    static bool Seg(float x, float y, float ax, float ay, float bx, float by, float w)
    {
        float vx = bx - ax, vy = by - ay, wx = x - ax, wy = y - ay;
        float len = vx * vx + vy * vy;
        float tt = len > 0f ? Mathf.Clamp01((wx * vx + wy * vy) / len) : 0f;
        float px = ax + vx * tt, py = ay + vy * tt, dx = x - px, dy = y - py;
        return dx * dx + dy * dy <= w * w;
    }
}
