using UnityEngine;

// Item yang jatuh di lapangan lalu dipungut pemain saat disentuh.
//  - Bom    : bersihkan (bunuh) semua musuh di layar + kilat
//  - Magnet : tarik semua permata XP ke pemain
//  - Peti   : hadiah XP besar (biasanya menaikkan level -> pilih skill)
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
        Texture2D tex;
        Color warna;
        switch (jenis)
        {
            case Jenis.Bom: tex = Ikon.Bom; warna = new Color(1f, 0.5f, 0.3f); ukuran = 1.0f; break;
            case Jenis.Magnet: tex = Ikon.Magnet; warna = new Color(0.5f, 0.8f, 1f); ukuran = 0.9f; break;
            default: tex = Ikon.Peti; warna = new Color(1f, 0.82f, 0.3f); ukuran = 1.1f; break;
        }
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sr.color = warna;
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
                // bunuh semua musuh di layar + kilat besar
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
                // hadiah XP besar -> biasanya langsung naik level (muncul pilih skill)
                if (LevelSystem.Instance != null)
                    LevelSystem.Instance.AddXp(Mathf.Max(5, LevelSystem.Instance.XpUntukNaik));
                SoundManager.LevelUp();
                break;
        }
    }
}
