using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Mengelola senjata otomatis ala Survivor.io: Pisau Berputar (orbit), Aura Setrum,
// Roket Pelacak, Kilat Rantai, Bola Api. Tiap senjata bisa naik sampai level MAX.
// Di level 5+ senjata OTOMATIS berevolusi (lebih kuat, berubah UNGU + petir).
//
// KURVA KEKUATAN (disetel ulang setelah playtest):
// Level 1 sengaja KECIL & LEMAH (tapi tetap membunuh gerombolan terlemah), lalu
// UKURAN dan DAMAGE naik jelas tiap level. Evolusi (lvl 5+) bukan sekadar
// membesar: berubah UNGU dan menyambar PETIR (lihat PetirEfek.cs).
//
// CATATAN FASE 0:
// Semua pencarian musuh sekarang lewat EnemyRegistry, bukan
// GameObject.FindGameObjectsWithTag("Enemy").
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
    public int lvRantai = 0;
    public int lvBolaApi = 0;

    private Transform player;

    // orbit (pisau berputar)
    private List<Transform> bilah = new List<Transform>();
    private float sudutOrbit = 0f;

    // aura (medan setrum)
    private GameObject auraVisual;
    private SpriteRenderer auraSR;
    private float auraTimer = 0f;

    // roket
    private float roketTimer = 0f;

    // kilat rantai
    private float rantaiTimer = 0f;

    // bola api
    private float bolaApiTimer = 0f;

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
        lvOrbit = 0; lvAura = 0; lvRoket = 0; lvRantai = 0; lvBolaApi = 0;
        bilah.Clear();
        sudutOrbit = 0f; auraTimer = 0f; roketTimer = 0f; rantaiTimer = 0f; bolaApiTimer = 0f;
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
    public void TambahRantai() { lvRantai = Mathf.Min(MAX, lvRantai + 1); }
    public void TambahBolaApi() { lvBolaApi = Mathf.Min(MAX, lvBolaApi + 1); }

    // ====== ORBIT ======
    void BangunOrbit()
    {
        foreach (var b in bilah) if (b != null) Destroy(b.gameObject);
        bilah.Clear();

        bool evo = lvOrbit >= 5;
        int jumlah = lvOrbit + 1 + (evo ? 2 : 0); // evolusi: +2 bilah

        // Level 1 sengaja lemah & pisau kecil, naik jelas tiap level. Evolusi
        // melonjak besar, berubah ungu, dan memercikkan petir (di PisauOrbit).
        //  dmg   : L1=3, L2=5, L3=7, L4=9, L5(evo)=17, L6=19
        //  skala : L1=0.53, L2=0.61, ... L5(evo)=1.10 (2x lipat lebih dari L1)
        int dmg = 1 + lvOrbit * 2 + (evo ? 6 : 0);
        float skala = 0.45f + lvOrbit * 0.08f + (evo ? 0.25f : 0f);
        Color warna = evo ? new Color(0.78f, 0.45f, 1f, 1f) : Color.white;

        for (int i = 0; i < jumlah; i++)
        {
            GameObject go = new GameObject("Bilah");
            go.transform.SetParent(transform);
            PisauOrbit po = go.AddComponent<PisauOrbit>();
            po.dmg = dmg;
            po.skala = skala;
            po.warna = warna;
            po.evo = evo;
            bilah.Add(go.transform);
        }
    }

    // ====== AURA ======
    void BangunAura()
    {
        if (auraVisual == null)
        {
            auraVisual = new GameObject("AuraVisual");
            auraSR = auraVisual.AddComponent<SpriteRenderer>();
            auraSR.sprite = BuatLingkaran(64);
            auraSR.color = new Color(0.4f, 0.8f, 1f, 0.16f);
            auraSR.sortingOrder = 5;
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
            //  radius : L1=1.35, L2=1.70, L3=2.05, L4=2.40, L5(evo)=3.55, L6=3.90
            //  dmg    : L1=5,   L2=8,   L3=11,  L4=14,  L5(evo)=29,   L6=32
            float radius = 1.0f + lvAura * 0.35f + (evo ? 0.8f : 0f);
            int dmg = 2 + lvAura * 3 + (evo ? 12 : 0);

            if (auraSR != null)
                auraSR.color = evo
                    ? new Color(0.62f, 0.28f, 1f, 0.24f)
                    : new Color(0.4f, 0.8f, 1f, 0.16f);

            auraVisual.transform.position = pl.position;
            auraVisual.transform.localScale = Vector3.one * radius * 2f;
            auraTimer += Time.deltaTime;
            if (auraTimer >= 0.28f)
            {
                auraTimer = 0f;
                int n = EnemyRegistry.DalamRadius(pl.position, radius, EnemyRegistry.Buffer);
                for (int i = 0; i < n; i++)
                {
                    EnemyChase ec = EnemyRegistry.Buffer[i];
                    if (ec == null) continue;
                    ec.KenaSerangan(dmg, false);
                }
                if (n > 0) SoundManager.AuraZap();

                // EVOLUSI: sambar petir ungu ke beberapa musuh terdekat yang kena.
                if (evo && n > 0)
                {
                    int petir = Mathf.Min(n, 3);
                    Color ungu = new Color(0.8f, 0.5f, 1f, 1f);
                    for (int i = 0; i < petir; i++)
                    {
                        EnemyChase ec2 = EnemyRegistry.Buffer[i];
                        if (ec2 != null)
                            PetirEfek.Sambar(pl.position, ec2.transform.position, ungu, 0.12f);
                    }
                }
            }
        }

        // ---- ROKET ----
        //  jumlah : L1=1, L2=2, L3=3, L4=4, L5(evo)=7 roket per gelombang
        //  dmg    : L1=5, L2=8, L3=11, L4=14, L5(evo)=27
        //  ukuran : L1=0.53 -> L5(evo)=1.10 ; radius ledak L1=1.32 -> L5(evo)=2.90
        if (lvRoket > 0)
        {
            bool evo = lvRoket >= 5;
            float jeda = Mathf.Max(0.7f, 2.0f - lvRoket * 0.2f);
            int dmg = 2 + lvRoket * 3 + (evo ? 10 : 0);
            float radius = 1.1f + lvRoket * 0.22f + (evo ? 0.7f : 0f);
            float skala = 0.45f + lvRoket * 0.08f + (evo ? 0.25f : 0f);
            int jumlahRoket = lvRoket + (evo ? 2 : 0);
            roketTimer += Time.deltaTime;
            if (roketTimer >= jeda)
            {
                roketTimer = 0f;
                int n = EnemyRegistry.NTerdekat(pl.position, JangkauanCariTarget, jumlahRoket, EnemyRegistry.Buffer);
                if (n > 0)
                {
                    for (int i = 0; i < jumlahRoket; i++)
                    {
                        EnemyChase ec = EnemyRegistry.Buffer[i % n];
                        if (ec == null) continue;
                        Roket.Tembak(pl.position, ec.transform, 8f, dmg, radius, skala, evo);
                    }
                }
            }
        }

        // ---- KILAT RANTAI ----
        //  dmg      : L1=6, L2=9, L3=12, L4=15, L5(evo)=28
        //  lompatan : L1=3, L2=4, L3=5, L4=6, L5(evo)=10 musuh
        if (lvRantai > 0)
        {
            bool evo = lvRantai >= 5;
            float jeda = Mathf.Max(0.5f, 1.6f - lvRantai * 0.18f);
            int dmg = 3 + lvRantai * 3 + (evo ? 10 : 0);
            int lompatan = 2 + lvRantai + (evo ? 3 : 0);
            float radiusLompat = 3.6f;
            rantaiTimer += Time.deltaTime;
            if (rantaiTimer >= jeda)
            {
                rantaiTimer = 0f;
                Color warna = evo ? new Color(0.8f, 0.5f, 1f, 1f) : new Color(0.5f, 0.85f, 1f, 1f);
                KilatRantai.Sambar(pl.position, dmg, lompatan, radiusLompat, warna);
            }
        }

        // ---- BOLA API ----
        // Dilempar ke musuh terdekat, meledak (area) + meninggalkan genangan api
        // yang terus membakar (lihat BolaApi.cs). L1 lemah & kecil; naik tiap
        // level; evolusi (lvl 5+): ungu + ledakannya menyambar petir.
        //  dmg ledak : L1=8, L2=12, L3=16, L4=20, L5(evo)=36
        //  genangan  : L1=2/tick ~2.3s -> makin lama & sakit tiap level
        if (lvBolaApi > 0)
        {
            bool evo = lvBolaApi >= 5;
            float jeda = Mathf.Max(0.8f, 2.2f - lvBolaApi * 0.2f);
            int dmg = 4 + lvBolaApi * 4 + (evo ? 12 : 0);
            float radius = 1.1f + lvBolaApi * 0.18f + (evo ? 0.6f : 0f);
            float skala = 0.4f + lvBolaApi * 0.07f + (evo ? 0.2f : 0f);
            int dmgGenangan = 1 + lvBolaApi + (evo ? 4 : 0);
            float durasiGenangan = 2.0f + lvBolaApi * 0.3f;
            bolaApiTimer += Time.deltaTime;
            if (bolaApiTimer >= jeda)
            {
                bolaApiTimer = 0f;
                EnemyChase t = EnemyRegistry.Terdekat(pl.position, JangkauanCariTarget, null);
                if (t != null)
                    BolaApi.Tembak(pl.position, t.transform, dmg, radius, skala, evo, durasiGenangan, dmgGenangan);
            }
        }
    }

    Transform MusuhTerdekat(Vector3 pos, Transform kecuali)
    {
        EnemyChase e = EnemyRegistry.Terdekat(pos, JangkauanCariTarget, null);
        if (e == null) return null;

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
                float a = d <= 1f ? Mathf.Lerp(0.45f, 0f, d) : 0f;
                if (d > 0.9f && d <= 1f) a = 0.8f;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
