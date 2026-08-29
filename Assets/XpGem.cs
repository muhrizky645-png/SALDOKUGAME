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

    public int nilai = 1; // XP yang diberikan
    public float ukuran = 1.2f; // besar permata
    public float jarakMagnet = 2.5f; // mulai ketarik ke pemain
    public float jarakAmbil = 0.5f; // langsung dipungut
    public float kecepatanTarik = 9f; // kecepatan ketarik ke pemain

    private Transform player;
    private SpriteRenderer sr;
    private float t = 0f;
    private bool pakaiFile = false; // true kalau sprite dari PNG (Assets/Resources/Icons/xp)

    // Sprite dari file (Assets/Resources/Icons/xp.png), dicache sekali. null = tidak ada.
    // CATATAN: PNG HARUS di-import sebagai Texture Type = "Sprite (2D and UI)".
    static Sprite _fileSprite;
    static bool _fileDicek;
    static Sprite FileSprite()
    {
        if (!_fileDicek)
        {
            _fileSprite = Resources.Load<Sprite>("Icons/xpgem");
            if (_fileSprite == null) _fileSprite = Resources.Load<Sprite>("Icons/xp");
            _fileDicek = true;
        }
        return _fileSprite;
    }

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
        Sprite file = FileSprite();
        if (file != null)
        {
            sr.sprite = file;              // pakai PNG asli
            sr.color = Color.white;        // jangan tint biar warna PNG asli
            pakaiFile = true;
            // Normalisasi ukuran: samakan dimensi terbesar sprite ke target world-units,
            // apa pun resolusi/PPU PNG-nya. Mencegah drop jadi kegedean.
            float maxDim = Mathf.Max(file.bounds.size.x, file.bounds.size.y);
            float target = ukuran * 0.32f; // ~0.38 unit, mirip sprite prosedural lama
            float sc = (maxDim > 0.0001f) ? target / maxDim : ukuran;
            transform.localScale = Vector3.one * sc;
        }
        else
        {
            sr.sprite = BuatPermata(32);   // fallback: sprite prosedural
            sr.color = new Color(0.4f, 1f, 1f, 1f); // cyan
            pakaiFile = false;
            transform.localScale = Vector3.one * ukuran;
        }
        sr.sortingOrder = 40;
    }

    void Update()
    {
        // efek berkedip halus biar kelihatan
        t += Time.deltaTime;
        float kilau = 0.75f + 0.25f * Mathf.Sin(t * 6f);
        if (sr != null)
        {
            if (pakaiFile)
                sr.color = new Color(kilau, kilau, kilau, 1f); // shimmer tanpa ubah warna PNG
            else
                sr.color = new Color(0.4f * kilau, 1f * kilau, 1f, 1f);
        }

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
                float m = (dx + dy) / r; // 0 tengah, 1 ujung wajik
                float a = Mathf.Clamp01((1f - m) * 4f); // tepi halus
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
