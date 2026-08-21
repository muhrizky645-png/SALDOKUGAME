using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [System.Serializable]
    public class MusuhTier
    {
        [Tooltip("Prefab musuh (drag dari folder DungeonMonsters2D/Characters). Contoh: Rat, Bat, Spider, dst.")]
        public GameObject prefab;
        [Tooltip("Musuh mulai muncul di Level ini. Isi 0 = otomatis dari urutan (baris ke-1 = Level 1, baris ke-2 = Level 2, dst).")]
        public int mulaiLevel = 0;
        [Tooltip("Nyawa: berapa kali kena tembak untuk mati. Isi 0 = otomatis (makin ke bawah makin tebal).")]
        public int nyawa = 0;
        [Tooltip("Kecepatan mengejar pemain.")]
        public float kecepatan = 2f;
        public int skor = 10;
        public int xp = 1;
        [Tooltip("Pengali ukuran. 1 = ukuran normal seragam. Lebih dari 1 = lebih besar, kurang dari 1 = lebih kecil.")]
        public float skala = 1f;
    }

    [Header("Daftar musuh (URUTKAN dari paling lemah ke paling kuat)")]
    public MusuhTier[] daftarMusuh;

    [Header("Cadangan (dipakai kalau Daftar Musuh kosong)")]
    public GameObject zombiePrefab;

    public float spawnDistance = 10f;    // jarak spawn dari pemain

    [Header("Ukuran musuh (sisi terpanjang gambar dalam satuan dunia)")]
    [Tooltip("Semua musuh otomatis diskalakan supaya sisi terpanjangnya (lebar atau tinggi) kira-kira segini.")]
    public float ukuranMusuh = 1f;

    [Header("Kesulitan mengikuti Level pemain")]
    public float spawnAwal = 0.9f;
    public float penguranganTiapLevel = 0.1f;
    public float spawnTercepat = 0.2f;
    public int maxAwal = 20;
    public int tambahMaxTiapLevel = 5;
    public int maxMutlak = 90;
    public int spawnSekaligus = 2;

    [Header("BOSS (muncul mengikuti waktu bertahan)")]
    [Tooltip("Prefab KHUSUS boss (mis. DragonRed dari DungeonMonsters2D/Characters). Kalau kosong, otomatis pakai musuh terkuat di daftar.")]
    public GameObject bossPrefab;
    [Tooltip("Jeda kemunculan boss dalam detik.")]
    public float jedaBoss = 45f;
    [Tooltip("Pengali ukuran boss dibanding musuh biasa. Boss sengaja dibuat besar.")]
    public float skalaBoss = 3.2f;

    private Transform player;
    private float timer = 0f;
    private float bossBerikut = 0f;
    private int bossKe = 0;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        bossBerikut = jedaBoss; // boss pertama setelah 'jedaBoss' detik
        bossKe = 0;
    }

    void Update()
    {
        if (player == null) return;

        int level = (LevelSystem.Instance != null) ? LevelSystem.Instance.Level : 1;

        // ==== BOSS mengikuti waktu ====
        if (GameTimer.Detik >= bossBerikut)
        {
            bossBerikut += jedaBoss;
            SpawnBos(level);
        }

        float jedaSpawn = Mathf.Max(spawnTercepat, spawnAwal - penguranganTiapLevel * (level - 1));
        int maxSekarang = Mathf.Min(maxMutlak, maxAwal + tambahMaxTiapLevel * (level - 1));

        timer += Time.deltaTime;
        if (timer >= jedaSpawn)
        {
            timer = 0f;
            for (int i = 0; i < spawnSekaligus; i++)
                Spawn(level, maxSekarang);
        }
    }

    int LevelBuka(int index, MusuhTier t)
    {
        return (t.mulaiLevel > 0) ? t.mulaiLevel : (index + 1);
    }

    int NyawaTier(int index, MusuhTier t)
    {
        return (t.nyawa > 0) ? t.nyawa : (index + 1);
    }

    Vector3 PosisiSpawn()
    {
        Vector2 arahAcak = Random.insideUnitCircle.normalized;
        return player.position + new Vector3(arahAcak.x, arahAcak.y, 0f) * spawnDistance;
    }

    void Spawn(int level, int maxSekarang)
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxSekarang) return;

        Vector3 posisi = PosisiSpawn();

        int index;
        MusuhTier tier = PilihTier(level, out index);
        GameObject prefab = (tier != null && tier.prefab != null) ? tier.prefab : zombiePrefab;
        if (prefab == null) return;

        GameObject musuh = Instantiate(prefab, posisi, Quaternion.identity);

        if (tier != null && tier.prefab != null)
            SiapkanMusuh(musuh, tier, index);
    }

    // Spawn satu BOSS: prefab khusus (bossPrefab) kalau ada, kalau tidak pakai yang terkuat.
    void SpawnBos(int level)
    {
        GameObject prefab = (bossPrefab != null) ? bossPrefab : PrefabTerkuat();
        if (prefab == null) prefab = zombiePrefab;
        if (prefab == null) return;

        bossKe++;
        Vector3 posisi = PosisiSpawn();
        GameObject go = Instantiate(prefab, posisi, Quaternion.identity);

        go.tag = "Enemy";
        go.layer = 0;
        AturFisikDanUkuran(go, ukuranMusuh * Mathf.Max(1f, skalaBoss));

        EnemyChase ec = go.GetComponent<EnemyChase>();
        if (ec == null) ec = go.AddComponent<EnemyChase>();
        ec.moveSpeed = 1.3f;
        ec.nyawa = 60 + level * 8 + (bossKe - 1) * 40;
        ec.skor = 500;
        ec.xp = 25;
        ec.bos = true; // ditandai boss (diproses di EnemyChase.Start)
    }

    GameObject PrefabTerkuat()
    {
        if (daftarMusuh == null) return null;
        for (int i = daftarMusuh.Length - 1; i >= 0; i--)
            if (daftarMusuh[i] != null && daftarMusuh[i].prefab != null)
                return daftarMusuh[i].prefab;
        return null;
    }

    MusuhTier PilihTier(int level, out int indexTerpilih)
    {
        indexTerpilih = -1;
        if (daftarMusuh == null || daftarMusuh.Length == 0) return null;

        int jumlah = 0;
        for (int i = 0; i < daftarMusuh.Length; i++)
        {
            var t = daftarMusuh[i];
            if (t != null && t.prefab != null && LevelBuka(i, t) <= level) jumlah++;
        }
        if (jumlah == 0) return null;

        int pilih = Random.Range(0, jumlah);
        int hitung = 0;
        for (int i = 0; i < daftarMusuh.Length; i++)
        {
            var t = daftarMusuh[i];
            if (t == null || t.prefab == null) continue;
            if (LevelBuka(i, t) > level) continue;
            if (hitung == pilih) { indexTerpilih = i; return t; }
            hitung++;
        }
        return null;
    }

    void SiapkanMusuh(GameObject go, MusuhTier tier, int index)
    {
        go.tag = "Enemy";
        go.layer = 0;

        float pengali = (tier.skala > 0f) ? tier.skala : 1f;
        AturFisikDanUkuran(go, ukuranMusuh * pengali);

        EnemyChase ec = go.GetComponent<EnemyChase>();
        if (ec == null) ec = go.AddComponent<EnemyChase>();
        ec.moveSpeed = tier.kecepatan;
        ec.nyawa = NyawaTier(index, tier);
        ec.skor = tier.skor;
        ec.xp = tier.xp;
    }

    // Atur ukuran seragam + Rigidbody2D + Collider2D untuk sebuah musuh
    void AturFisikDanUkuran(GameObject go, float ukuranTarget)
    {
        // === UKURAN OTOMATIS (pakai sisi terpanjang) ===
        if (ukuranTarget > 0f)
        {
            Renderer[] rendUkur = go.GetComponentsInChildren<Renderer>();
            if (rendUkur.Length > 0)
            {
                Bounds b = rendUkur[0].bounds;
                for (int i = 1; i < rendUkur.Length; i++) b.Encapsulate(rendUkur[i].bounds);
                float dimSekarang = Mathf.Max(b.size.x, b.size.y);
                if (dimSekarang > 0.0001f)
                    go.transform.localScale *= (ukuranTarget / dimSekarang);
            }
        }

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb == null) rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D col = go.GetComponent<Collider2D>();
        if (col == null)
        {
            CircleCollider2D cc = go.AddComponent<CircleCollider2D>();
            Renderer[] rends = go.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                Bounds b = rends[0].bounds;
                for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
                float sx = Mathf.Abs(go.transform.lossyScale.x);
                if (sx < 0.0001f) sx = 1f;
                cc.radius = Mathf.Max(b.extents.x, b.extents.y) / sx;
                cc.offset = (Vector2)go.transform.InverseTransformPoint(b.center);
            }
            else
            {
                cc.radius = 0.5f;
            }
        }
    }
}
