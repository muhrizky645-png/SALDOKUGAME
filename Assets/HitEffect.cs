using UnityEngine;

// Efek kecil (lingkaran membesar lalu memudar) saat musuh mati.
// Panggil: HitEffect.Munculkan(posisi);
public class HitEffect : MonoBehaviour
{
    public static void Munculkan(Vector3 pos, float ukuran = 1f)
    {
        GameObject go = new GameObject("HitEffect");
        go.transform.position = pos;
        HitEffect e = go.AddComponent<HitEffect>();
        e.ukuranMax = ukuran;
    }

    public float ukuranMax = 1f;   // besar efek (1 = normal)
    private SpriteRenderer sr;
    private float t = 0f;
    private float durasi = 0.25f;

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatLingkaran(64);
        sr.color = new Color(1f, 0.9f, 0.4f, 0.85f); // kuning terang
        sr.sortingOrder = 50;
        transform.localScale = Vector3.one * (ukuranMax * 0.2f);
    }

    void Update()
    {
        t += Time.deltaTime;
        float p = Mathf.Clamp01(t / durasi);
        transform.localScale = Vector3.one * Mathf.Lerp(ukuranMax * 0.2f, ukuranMax * 1.2f, p);
        Color c = sr.color;
        c.a = Mathf.Lerp(0.85f, 0f, p); // makin pudar
        sr.color = c;
        if (p >= 1f) Destroy(gameObject);
    }

    // Membuat sprite lingkaran lewat kode (tanpa perlu file gambar)
    Sprite BuatLingkaran(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f;
                float dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(r - d);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}