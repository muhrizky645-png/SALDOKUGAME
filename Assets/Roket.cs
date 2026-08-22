using UnityEngine;

// Roket pelacak: terbang menuju target, saat dekat musuh -> meledak (area damage).
// Panggil: Roket.Tembak(posisi, targetTransform, speed, dmg, radius);
public class Roket : MonoBehaviour
{
    private Transform target;
    private float speed = 6f;
    private int dmg = 8;
    private float radius = 1.6f;
    private float umur = 4f;
    private Vector3 arah = Vector3.right;
    private SpriteRenderer sr;

    public static void Tembak(Vector3 pos, Transform target, float speed, int dmg, float radius)
    {
        GameObject go = new GameObject("Roket");
        go.transform.position = pos;
        Roket r = go.AddComponent<Roket>();
        r.target = target;
        r.speed = speed;
        r.dmg = dmg;
        r.radius = radius;
        if (target != null) r.arah = (target.position - pos).normalized;
    }

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatSprite(64);
        sr.color = Color.white;          // warna sudah dipanggang di tekstur
        sr.sortingOrder = 45;
        transform.localScale = Vector3.one * 0.85f; // sedikit lebih besar dari sebelumnya
    }

    void Update()
    {
        umur -= Time.deltaTime;
        if (umur <= 0f) { Meledak(); return; }

        // pelacakan: kalau target hilang, cari musuh terdekat
        if (target == null) target = MusuhTerdekat();
        if (target != null)
        {
            Vector3 mau = (target.position - transform.position).normalized;
            arah = Vector3.Lerp(arah, mau, 6f * Time.deltaTime).normalized;
        }

        transform.position += arah * speed * Time.deltaTime;
        float sudut = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, sudut - 90f); // hidung roket (+y) mengarah ke gerak

        // kena musuh?
        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var m in musuh)
        {
            if (m == null) continue;
            if (Vector3.Distance(transform.position, m.transform.position) <= 0.5f)
            {
                Meledak();
                return;
            }
        }
    }

    void Meledak()
    {
        Ledakan.Munculkan(transform.position, radius, dmg, 0f, new Color(1f, 0.6f, 0.2f, 0.7f));
        Destroy(gameObject);
    }

    Transform MusuhTerdekat()
    {
        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
        Transform t = null;
        float min = Mathf.Infinity;
        foreach (var m in musuh)
        {
            if (m == null) continue;
            float d = Vector3.Distance(transform.position, m.transform.position);
            if (d < min) { min = d; t = m.transform; }
        }
        return t;
    }

    // Bentuk ROKET beneran (hidung menghadap ATAS), warna dipanggang ke tekstur:
    //  - badan silinder metalik  - hidung kerucut merah  - sirip merah
    //  - jendela biru            - nyala api kuning/oranye di bawah
    Sprite BuatSprite(int S)
    {
        Texture2D tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        float cx = (S - 1) * 0.5f;

        for (int y = 0; y < S; y++)
        {
            float ny = (y + 0.5f) / S;                 // 0 bawah .. 1 atas
            for (int x = 0; x < S; x++)
            {
                float dx = (x + 0.5f) - (cx + 0.5f);
                float adx = Mathf.Abs(dx);
                Color col = new Color(0f, 0f, 0f, 0f);  // transparan

                // NYALA API (paling bawah)
                if (ny < 0.20f)
                {
                    float t = ny / 0.20f;               // 0 bawah .. 1 atas
                    float w = Mathf.Lerp(S * 0.02f, S * 0.13f, t);
                    if (adx <= w)
                        col = (adx <= w * 0.5f)
                            ? new Color(1f, 0.93f, 0.40f, 1f)  // inti kuning
                            : new Color(1f, 0.50f, 0.12f, 1f); // tepi oranye
                }

                // SIRIP kiri-kanan
                if (ny >= 0.16f && ny <= 0.36f)
                {
                    float t = (ny - 0.16f) / 0.20f;
                    float outer = Mathf.Lerp(S * 0.34f, S * 0.17f, t);
                    if (adx >= S * 0.15f && adx <= outer)
                        col = new Color(0.80f, 0.18f, 0.15f, 1f); // merah
                }

                // BADAN silinder (metalik, ada shading kiri terang -> kanan gelap)
                if (ny >= 0.20f && ny <= 0.74f && adx <= S * 0.17f)
                {
                    float shade = Mathf.Lerp(1f, 0.72f, (dx / (S * 0.17f)) * 0.5f + 0.5f);
                    col = new Color(0.92f * shade, 0.92f * shade, 0.96f * shade, 1f);
                }

                // HIDUNG kerucut merah
                if (ny > 0.74f && ny <= 0.95f)
                {
                    float t = (ny - 0.74f) / 0.21f;
                    if (adx <= Mathf.Lerp(S * 0.17f, 0f, t))
                        col = new Color(0.86f, 0.20f, 0.16f, 1f);
                }

                // JENDELA biru di badan
                float ddx = (x + 0.5f) - (cx + 0.5f);
                float ddy = (y + 0.5f) - 0.55f * S;
                if (ddx * ddx + ddy * ddy <= (S * 0.075f) * (S * 0.075f))
                    col = new Color(0.45f, 0.82f, 1f, 1f);

                tex.SetPixel(x, y, col);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }
}
