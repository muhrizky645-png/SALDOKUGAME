using UnityEngine;

// Permata XP yang jatuh saat musuh mati. Panggil: XpGem.Munculkan(posisi);
// Kalau pemain mendekat, permata otomatis ketarik lalu terpungut (nambah XP).
public class XpGem : MonoBehaviour
{
    // pengali jarak magnet dari skill (diatur SkillManager). 1 = normal.
    public static float MagnetMult = 1f;

    public static void Munculkan(Vector3 pos, int nilai = 1)
    {
        GameObject go = new GameObject("XpGem");
        go.transform.position = pos;
        XpGem g = go.AddComponent<XpGem>();
        g.nilai = nilai;
    }

    public int nilai = 1;             // XP yang diberikan
    public float ukuran = 1.2f;       // besar permata
    public float jarakMagnet = 2.5f;  // mulai ketarik ke pemain
    public float jarakAmbil = 0.5f;   // langsung dipungut
    public float kecepatanTarik = 9f; // kecepatan ketarik ke pemain

    private Transform player;
    private SpriteRenderer sr;
    private float t = 0f;

    // Dipanggil item Magnet: paksa permata langsung terbang ke pemain
    public void PaksaTarik()
    {
        jarakMagnet = 9999f;
        kecepatanTarik = Mathf.Max(kecepatanTarik, 16f);
    }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        jarakMagnet *= MagnetMult; // terapkan skill Magnet (kalau ada)

        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatPermata(32);
        sr.color = new Color(0.4f, 1f, 1f, 1f); // cyan
        sr.sortingOrder = 40;
        transform.localScale = Vector3.one * ukuran;
    }

    void Update()
    {
        // efek berkedip halus biar kelihatan
        t += Time.deltaTime;
        float kilau = 0.75f + 0.25f * Mathf.Sin(t * 6f);
        if (sr != null) sr.color = new Color(0.4f * kilau, 1f * kilau, 1f, 1f);

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform; else return;
        }

        float d = Vector3.Distance(transform.position, player.position);
        if (d <= jarakAmbil)
        {
            if (LevelSystem.Instance != null) LevelSystem.Instance.AddXp(nilai);
            SoundManager.AmbilXp(); // suara ambil XP
            Destroy(gameObject);
            return;
        }

        // MODE DEWA: MAGNET SEMESTA -> semua permata langsung tersedot
        float jm = jarakMagnet;
        float speed = kecepatanTarik;
        if (ModeDewa.Aktif) { jm = 99999f; speed = Mathf.Max(speed, 22f); }

        if (d <= jm)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    // Membuat sprite permata (bentuk wajik) lewat kode, tanpa file gambar
    Sprite BuatPermata(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = Mathf.Abs(x - r + 0.5f);
                float dy = Mathf.Abs(y - r + 0.5f);
                float m = (dx + dy) / r;               // 0 tengah, 1 ujung wajik
                float a = Mathf.Clamp01((1f - m) * 4f); // tepi halus
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
