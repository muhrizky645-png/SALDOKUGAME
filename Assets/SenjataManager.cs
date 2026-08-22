using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Mengelola senjata otomatis ala Survivor.io: Pisau Berputar (orbit), Aura Setrum, Roket Pelacak.
// Tiap senjata bisa naik sampai level MAX. Di level 5+ senjata OTOMATIS berevolusi (lebih kuat).
public class SenjataManager : MonoBehaviour
{
    public static SenjataManager Instance;
    public const int MAX = 6;

    public int lvOrbit = 0;
    public int lvAura = 0;
    public int lvRoket = 0;

    private Transform player;

    // orbit (pisau berputar)
    private List<Transform> bilah = new List<Transform>();
    private float sudutOrbit = 0f;

    // aura (medan setrum)
    private GameObject auraVisual;
    private float auraTimer = 0f;

    // roket
    private float roketTimer = 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("SenjataManager", typeof(SenjataManager));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // reset semua senjata tiap game baru / restart
        lvOrbit = 0; lvAura = 0; lvRoket = 0;
        bilah.Clear();
        sudutOrbit = 0f; auraTimer = 0f; roketTimer = 0f;
    }

    Transform Player()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
        return player;
    }

    // ====== DIPANGGIL DARI SkillManager ======
    public void TambahOrbit() { lvOrbit = Mathf.Min(MAX, lvOrbit + 1); BangunOrbit(); }
    public void TambahAura() { lvAura = Mathf.Min(MAX, lvAura + 1); BangunAura(); }
    public void TambahRoket() { lvRoket = Mathf.Min(MAX, lvRoket + 1); }

    // ====== ORBIT ======
    void BangunOrbit()
    {
        foreach (var b in bilah) if (b != null) Destroy(b.gameObject);
        bilah.Clear();

        bool evo = lvOrbit >= 5;
        int jumlah = lvOrbit + 1 + (evo ? 2 : 0); // evolusi: +2 bilah
        int dmg = 3 + lvOrbit * 2 + (evo ? 5 : 0); // damage dinaikkan

        for (int i = 0; i < jumlah; i++)
        {
            GameObject go = new GameObject("Bilah");
            go.transform.SetParent(transform);
            PisauOrbit po = go.AddComponent<PisauOrbit>();
            po.dmg = dmg;
            bilah.Add(go.transform);
        }
    }

    // ====== AURA ======
    void BangunAura()
    {
        if (auraVisual == null)
        {
            auraVisual = new GameObject("AuraVisual");
            SpriteRenderer sr = auraVisual.AddComponent<SpriteRenderer>();
            sr.sprite = BuatLingkaran(64);
            sr.color = new Color(0.4f, 0.8f, 1f, 0.16f);
            sr.sortingOrder = 5;
        }
    }

    void Update()
    {
        Transform pl = Player();
        if (pl == null) return;
        if (!GameMenu.SedangMain) return;

        // ---- ORBIT ----
        if (bilah.Count > 0)
        {
            bool evo = lvOrbit >= 5;
            float radius = evo ? 2.1f : 1.6f;
            float kecepatan = evo ? 200f : 140f;
            sudutOrbit += kecepatan * Time.deltaTime;
            int n = bilah.Count;
            for (int i = 0; i < n; i++)
            {
                if (bilah[i] == null) continue;
                float a = (sudutOrbit + i * (360f / n)) * Mathf.Deg2Rad;
                bilah[i].position = pl.position + new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * radius;
            }
        }

        // ---- AURA ----
        if (lvAura > 0 && auraVisual != null)
        {
            bool evo = lvAura >= 5;
            float radius = (evo ? 2.6f : 1.8f) + lvAura * 0.18f;
            int dmg = 2 + lvAura * 2 + (evo ? 5 : 0); // damage dinaikkan
            auraVisual.transform.position = pl.position;
            auraVisual.transform.localScale = Vector3.one * radius * 2f;
            auraTimer += Time.deltaTime;
            if (auraTimer >= 0.4f) // sedikit lebih sering
            {
                auraTimer = 0f;
                GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var m in musuh)
                {
                    if (m == null) continue;
                    if (Vector3.Distance(pl.position, m.transform.position) > radius) continue;
                    EnemyChase ec = m.GetComponentInParent<EnemyChase>();
                    if (ec != null) ec.KenaSerangan(dmg);
                }
            }
        }

        // ---- ROKET ----
        // JUMLAH roket per gelombang = level roket (evolusi +2). Tiap roket mengejar musuh berbeda.
        if (lvRoket > 0)
        {
            bool evo = lvRoket >= 5;
            float jeda = Mathf.Max(0.7f, 2.0f - lvRoket * 0.2f);
            int dmg = 8 + lvRoket * 3 + (evo ? 8 : 0); // damage dinaikkan
            float radius = evo ? 2.4f : 1.8f;
            int jumlahRoket = lvRoket + (evo ? 2 : 0);
            roketTimer += Time.deltaTime;
            if (roketTimer >= jeda)
            {
                roketTimer = 0f;
                // kumpulkan musuh, urutkan dari yang terdekat, lalu bagikan target ke tiap roket
                List<Transform> ts = new List<Transform>();
                GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var m in musuh) if (m != null) ts.Add(m.transform);
                if (ts.Count > 0)
                {
                    ts.Sort((a, b) => (a.position - pl.position).sqrMagnitude
                        .CompareTo((b.position - pl.position).sqrMagnitude));
                    for (int i = 0; i < jumlahRoket; i++)
                    {
                        Transform t = ts[i % ts.Count]; // kalau musuh sedikit, target dipakai ulang
                        Roket.Tembak(pl.position, t, 8f, dmg, radius);
                    }
                }
            }
        }
    }

    Transform MusuhTerdekat(Vector3 pos, Transform kecuali)
    {
        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
        Transform t = null; float min = Mathf.Infinity;
        foreach (var m in musuh)
        {
            if (m == null) continue;
            if (kecuali != null && m.transform == kecuali) continue;
            float d = Vector3.Distance(pos, m.transform.position);
            if (d < min) { min = d; t = m.transform; }
        }
        return t;
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
                float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                float a = d <= 1f ? Mathf.Lerp(0.45f, 0f, d) : 0f; // isi lembut
                if (d > 0.9f && d <= 1f) a = 0.8f;                 // cincin tepi
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
