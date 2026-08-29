using UnityEngine;

// Permata (mata uang) yang jatuh saat musuh mati. Mirip XpGem tapi menambah
// PERMATA (bukan XP) dan warnanya ungu. Panggil: PermataGem.Munculkan(pos, nilai);
public class PermataGem : MonoBehaviour
{
    public static void Munculkan(Vector3 pos, int nilai = 1)
    {
        GameObject go = new GameObject("PermataGem");
        go.transform.position = pos;
        PermataGem g = go.AddComponent<PermataGem>();
        g.nilai = nilai;
    }

    public int nilai = 1;
    public float ukuran = 1.15f;
    public float jarakMagnet = 2.3f;
    public float jarakAmbil = 0.5f;
    public float kecepatanTarik = 9f;

    private Transform player;
    private SpriteRenderer sr;
    private float t = 0f;
    private bool pakaiFile = false; // true kalau sprite dari PNG (Assets/Resources/Icons/permata)

    // Sprite dari file (Assets/Resources/Icons/permata.png), dicache sekali. null = tidak ada.
    // CATATAN: PNG HARUS di-import sebagai Texture Type = "Sprite (2D and UI)".
    static Sprite _fileSprite;
    static bool _fileDicek;
    static Sprite FileSprite()
    {
        if (!_fileDicek)
        {
            _fileSprite = Resources.Load<Sprite>("Icons/permatagem");
            if (_fileSprite == null) _fileSprite = Resources.Load<Sprite>("Icons/permata");
            _fileDicek = true;
        }
        return _fileSprite;
    }

    public void PaksaTarik()
    {
        jarakMagnet = 9999f;
        kecepatanTarik = Mathf.Max(kecepatanTarik, 16f);
    }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        jarakMagnet *= XpGem.MagnetMult; // ikut skill Magnet

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
            float target = ukuran * 0.32f; // ~0.37 unit, mirip sprite prosedural lama
            float sc = (maxDim > 0.0001f) ? target / maxDim : ukuran;
            transform.localScale = Vector3.one * sc;
        }
        else
        {
            sr.sprite = BuatPermata(32);   // fallback: sprite prosedural
            sr.color = new Color(0.8f, 0.4f, 1f, 1f); // ungu
            pakaiFile = false;
            transform.localScale = Vector3.one * ukuran;
        }
        sr.sortingOrder = 41;
    }

    void Update()
    {
        t += Time.deltaTime;
        float kilau = 0.75f + 0.25f * Mathf.Sin(t * 6f);
        if (sr != null)
        {
            if (pakaiFile)
                sr.color = new Color(kilau, kilau, kilau, 1f); // shimmer tanpa ubah warna PNG
            else
                sr.color = new Color(0.8f * kilau, 0.4f * kilau, 1f, 1f);
        }

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform; else return;
        }

        float d = Vector3.Distance(transform.position, player.position);
        if (d <= jarakAmbil)
        {
            if (MataUang.Instance != null) MataUang.Instance.TambahPermata(nilai);
            SoundManager.AmbilXp();
            Destroy(gameObject);
            return;
        }

        float jm = jarakMagnet;
        float speed = kecepatanTarik;
        if (ModeDewa.Aktif) { jm = 99999f; speed = Mathf.Max(speed, 22f); }

        if (d <= jm)
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    Sprite BuatPermata(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = Mathf.Abs(x - r + 0.5f);
            float dy = Mathf.Abs(y - r + 0.5f);
            float m = (dx + dy) / r;
            float a = Mathf.Clamp01((1f - m) * 4f);
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
