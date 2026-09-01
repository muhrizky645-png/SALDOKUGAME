using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Mengelola senjata otomatis ala Survivor.io: Pisau Berputar (orbit), Aura Setrum, Roket Pelacak.
// Tiap senjata bisa naik sampai level MAX. Di level 5+ senjata OTOMATIS berevolusi (lebih kuat).
//
// CATATAN FASE 0:
// Semua pencarian musuh sekarang lewat EnemyRegistry, bukan
// GameObject.FindGameObjectsWithTag("Enemy"). Lihat komentar di tiap blok.
// Angka balancing di file ini masih hardcoded; pemindahannya ke SenjataSO
// adalah pekerjaan Fase 1.
public class SenjataManager : MonoBehaviour
{
    public static SenjataManager Instance;
    public const int MAX = 6;

    // Sejauh apa roket mau mencari sasaran (satuan dunia).
    const float JangkauanCariTarget = 30f;

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
            // Aura DIPERKUAT setelah playtest: dulu terasa kurang ngefek.
            // Radius lebih lebar + tick lebih sering + damage hampir dua kali
            // lipat, supaya cincin di sekeliling pemain benar-benar membersihkan
            // gerombolan lemah yang mendekat - ini identitas senjata aura.
            float radius = (evo ? 2.9f : 2.1f) + lvAura * 0.22f;
            int dmg = 5 + lvAura * 4 + (evo ? 10 : 0); // damage dinaikkan
            auraVisual.transform.position = pl.position;
            auraVisual.transform.localScale = Vector3.one * radius * 2f;
            auraTimer += Time.deltaTime;
            if (auraTimer >= 0.28f) // ngetik lebih sering = DPS aura naik
            {
                auraTimer = 0f;

                // DULU: FindGameObjectsWithTag menyisir seluruh scene lalu
                // menghitung Vector3.Distance (pakai akar kuadrat) ke SETIAP
                // musuh, 2.5x per detik.
                // SEKARANG: registry hanya memeriksa sel grid yang bersinggungan
                // dengan radius aura, dan membandingkan kuadrat jarak.
                int n = EnemyRegistry.DalamRadius(pl.position, radius, EnemyRegistry.Buffer);
                for (int i = 0; i < n; i++)
                {
                    EnemyChase ec = EnemyRegistry.Buffer[i];
                    if (ec == null) continue;
                    // bunyi=false: aura JANGAN membunyikan "kena" per musuh, karena
                    // memukul puluhan musuh tiap denyut -> jadi dengungan brisik.
                    ec.KenaSerangan(dmg, false);
                }
                // Sebagai gantinya, satu bunyi setrum "zzap" per denyut aura:
                // berirama & garang, bukan brisik. Hanya jika ada yang kena.
                if (n > 0) SoundManager.AuraZap();
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

                // DULU: alokasi List<Transform> baru + Sort penuh atas SEMUA
                // musuh di scene, setiap gelombang roket.
                // SEKARANG: NTerdekat menyaring lewat grid dulu, lalu hanya
                // mengurutkan kandidat yang benar-benar dekat, ke buffer
                // bersama. Nol alokasi per gelombang.
                int n = EnemyRegistry.NTerdekat(pl.position, JangkauanCariTarget, jumlahRoket, EnemyRegistry.Buffer);
                if (n > 0)
                {
                    for (int i = 0; i < jumlahRoket; i++)
                    {
                        EnemyChase ec = EnemyRegistry.Buffer[i % n]; // kalau musuh sedikit, target dipakai ulang
                        if (ec == null) continue;
                        Roket.Tembak(pl.position, ec.transform, 8f, dmg, radius);
                    }
                }
            }
        }
    }

    Transform MusuhTerdekat(Vector3 pos, Transform kecuali)
    {
        EnemyChase e = EnemyRegistry.Terdekat(pos, JangkauanCariTarget, null);
        if (e == null) return null;

        // Kalau yang terdekat justru yang mau dihindari, cari lagi tanpa dia.
        if (kecuali != null && e.transform == kecuali)
            e = EnemyRegistry.Terdekat(pos, JangkauanCariTarget, e);

        return (e != null) ? e.transform : null;
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
