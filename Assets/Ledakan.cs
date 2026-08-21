using UnityEngine;

// Ledakan area: memberi damage sekali ke musuh dan/atau pemain di dalam radius,
// lalu menampilkan lingkaran yang membesar & memudar. Semua lewat kode.
public class Ledakan : MonoBehaviour
{
    // pos: pusat ledakan; radius: jangkauan; dmgMusuh: damage ke musuh; dmgPemain: damage ke pemain
    public static void Munculkan(Vector3 pos, float radius, int dmgMusuh, float dmgPemain, Color warna)
    {
        // ---- damage musuh ----
        if (dmgMusuh > 0)
        {
            GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var m in musuh)
            {
                if (m == null) continue;
                if (Vector3.Distance(m.transform.position, pos) <= radius)
                {
                    EnemyChase ec = m.GetComponentInParent<EnemyChase>();
                    if (ec != null) ec.KenaSerangan(dmgMusuh);
                }
            }
        }

        // ---- damage pemain ----
        if (dmgPemain > 0f && PlayerHealth.Instance != null)
        {
            if (Vector3.Distance(PlayerHealth.Instance.transform.position, pos) <= radius)
                PlayerHealth.Instance.Kurangi(dmgPemain);
        }

        // ---- visual ----
        GameObject go = new GameObject("Ledakan");
        go.transform.position = pos;
        Ledakan e = go.AddComponent<Ledakan>();
        e.radius = radius;
        e.warna = warna;
    }

    public float radius = 1.5f;
    public Color warna = new Color(1f, 0.6f, 0.2f, 0.85f);
    private SpriteRenderer sr;
    private float t = 0f;
    private float durasi = 0.35f;

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatLingkaran(64);
        sr.color = warna;
        sr.sortingOrder = 55;
        transform.localScale = Vector3.one * (radius * 1.0f);
        SoundManager.MusuhMati();
    }

    void Update()
    {
        t += Time.deltaTime;
        float p = Mathf.Clamp01(t / durasi);
        // sprite 64px ~ 0.64 unit diameter di scale 1; skalakan supaya menutupi radius
        float sMul = (2f * radius) / 0.64f;
        transform.localScale = Vector3.one * Mathf.Lerp(sMul * 0.4f, sMul, p);
        Color c = sr.color;
        c.a = Mathf.Lerp(warna.a, 0f, p);
        sr.color = c;
        if (p >= 1f) Destroy(gameObject);
    }

    Sprite BuatLingkaran(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(r - d);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
