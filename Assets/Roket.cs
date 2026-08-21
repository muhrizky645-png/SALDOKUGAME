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
        sr.sprite = BuatSprite(20);
        sr.color = new Color(1f, 0.6f, 0.2f); // oranye
        sr.sortingOrder = 45;
        transform.localScale = Vector3.one * 0.5f;
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
        transform.rotation = Quaternion.Euler(0, 0, sudut - 90f);

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

    Sprite BuatSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float cx = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // bentuk kapsul roket vertikal
                float dx = Mathf.Abs(x - cx + 0.5f);
                float ny = (float)y / size;
                float lebar = Mathf.Lerp(size * 0.28f, size * 0.05f, ny); // meruncing ke atas
                float a = dx <= lebar ? 1f : 0f;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
