using UnityEngine;

// Item yang jatuh di lapangan lalu dipungut pemain saat disentuh.
//  - Bom    : bersihkan (bunuh) semua musuh di layar + kilat
//  - Magnet : tarik semua permata XP ke pemain
//  - Peti   : hadiah XP besar (biasanya menaikkan level -> pilih skill)
//
// Ikon: utamakan FILE dari asset pack (Assets/Resources/Icons/<id>.png) lewat
// Ikon.UntukItem(); kalau belum ada, otomatis pakai ikon KODE (prosedural).
public class ItemLapangan : MonoBehaviour
{
    public enum Jenis { Bom, Magnet, Peti }
    public Jenis jenis;

    public static void Jatuhkan(Vector3 pos, Jenis j)
    {
        GameObject go = new GameObject("Item_" + j);
        go.transform.position = pos;
        ItemLapangan it = go.AddComponent<ItemLapangan>();
        it.jenis = j;
    }

    private SpriteRenderer sr;
    private Transform player;
    private float t = 0f;
    private float jarakAmbil = 0.6f;
    private float jarakMagnet = 2.2f;
    private float kecepatan = 8f;
    private float ukuran = 0.9f;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        sr = gameObject.AddComponent<SpriteRenderer>();

        string id;
        Color warna;
        switch (jenis)
        {
            case Jenis.Bom:    id = "bom";    warna = new Color(1f, 0.5f, 0.3f); ukuran = 1.0f; break;
            case Jenis.Magnet: id = "magnet"; warna = new Color(0.5f, 0.8f, 1f); ukuran = 0.9f; break;
            default:           id = "peti";   warna = new Color(1f, 0.82f, 0.3f); ukuran = 1.1f; break;
        }

        Texture2D tex = Ikon.UntukItem(id);
        // Normalisasi ukuran: sisi terpanjang tekstur = 1 unit, biar konsisten
        // walau resolusi PNG asset berbeda-beda.
        float ppu = Mathf.Max(1, Mathf.Max(tex.width, tex.height));
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
        // ikon dari FILE (asset) -> tampilkan warna ASLI (putih); ikon KODE -> beri warna tema
        sr.color = Ikon.AdalahFile(tex) ? Color.white : warna;
        sr.sortingOrder = 42;
        transform.localScale = Vector3.one * ukuran;
    }

    void Update()
    {
        // denyut biar menarik perhatian
        t += Time.deltaTime;
        float denyut = 1f + 0.1f * Mathf.Sin(t * 5f);
        transform.localScale = Vector3.one * ukuran * denyut;

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform; else return;
        }

        float d = Vector3.Distance(transform.position, player.position);
        if (d <= jarakAmbil)
        {
            Efek();
            Destroy(gameObject);
            return;
        }
        if (d <= jarakMagnet)
            transform.position = Vector3.MoveTowards(transform.position, player.position, kecepatan * Time.deltaTime);
    }

    void Efek()
    {
        switch (jenis)
        {
            case Jenis.Bom:
                GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var m in musuh)
                {
                    if (m == null) continue;
                    EnemyChase ec = m.GetComponentInParent<EnemyChase>();
                    if (ec != null) ec.KenaSerangan(9999);
                }
                if (player != null)
                    Ledakan.Munculkan(player.position, 6f, 0, 0f, new Color(1f, 0.85f, 0.35f, 0.7f));
                break;

            case Jenis.Magnet:
                XpGem[] gems = Object.FindObjectsByType<XpGem>(FindObjectsSortMode.None);
                foreach (var g in gems) if (g != null) g.PaksaTarik();
                SoundManager.AmbilXp();
                break;

            case Jenis.Peti:
                if (LevelSystem.Instance != null)
                    LevelSystem.Instance.AddXp(Mathf.Max(5, LevelSystem.Instance.XpUntukNaik));
                SoundManager.LevelUp();
                break;
        }
    }
}
