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
    [Tooltip("Semua musuh otomatis diskalakan supaya sisi terpanjangnya (lebar atau tinggi) kira-kira segini. Kalau musuh terasa masih kegedean, kecilkan angka ini (mis. 0.8). Kalau kekecilan, besarkan (mis. 1.5).")]
    public float ukuranMusuh = 1f;

    [Header("Kesulitan mengikuti Level pemain")]
    public float spawnAwal = 0.9f;            // jeda spawn di Level 1 (detik) - lebih kecil = lebih ramai
    public float penguranganTiapLevel = 0.1f; // jeda spawn berkurang tiap naik level
    public float spawnTercepat = 0.2f;        // batas jeda spawn tercepat
    public int maxAwal = 20;                  // batas musuh di layar Level 1
    public int tambahMaxTiapLevel = 5;        // batas musuh nambah tiap level
    public int maxMutlak = 90;                // batas musuh paling banyak
    public int spawnSekaligus = 2;            // berapa musuh muncul tiap spawn

    private Transform player;
    private float timer = 0f;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // level diambil dari LevelSystem (naik dari XP permata)
        int level = (LevelSystem.Instance != null) ? LevelSystem.Instance.Level : 1;

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

    // Level di mana musuh ini mulai muncul (otomatis dari urutan kalau mulaiLevel <= 0)
    int LevelBuka(int index, MusuhTier t)
    {
        return (t.mulaiLevel > 0) ? t.mulaiLevel : (index + 1);
    }

    // Nyawa musuh (otomatis makin tebal tiap tingkat kalau nyawa <= 0)
    int NyawaTier(int index, MusuhTier t)
    {
        return (t.nyawa > 0) ? t.nyawa : (index + 1);
    }

    void Spawn(int level, int maxSekarang)
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxSekarang) return;

        Vector2 arahAcak = Random.insideUnitCircle.normalized;
        Vector3 posisi = player.position + new Vector3(arahAcak.x, arahAcak.y, 0f) * spawnDistance;

        int index;
        MusuhTier tier = PilihTier(level, out index);
        GameObject prefab = (tier != null && tier.prefab != null) ? tier.prefab : zombiePrefab;
        if (prefab == null) return;

        GameObject musuh = Instantiate(prefab, posisi, Quaternion.identity);

        // kalau dari daftar tier -> lengkapi komponennya otomatis
        if (tier != null && tier.prefab != null)
            SiapkanMusuh(musuh, tier, index);
    }

    // Pilih acak salah satu musuh yang sudah terbuka di level ini
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

    // Melengkapi prefab musuh "mentah" jadi musuh yang berfungsi (tag, ukuran, fisika, collider, EnemyChase)
    void SiapkanMusuh(GameObject go, MusuhTier tier, int index)
    {
        // tag & layer biar bisa kena peluru (samakan dengan ZOMBIE yang sudah jalan)
        go.tag = "Enemy";
        go.layer = 0;

        // === ATUR UKURAN OTOMATIS ===
        // Ukur sisi TERPANJANG gambar (lebar atau tinggi), lalu skalakan supaya seragam.
        // Pakai sisi terpanjang biar monster lebar (mis. tikus) tidak jadi melar kegedean.
        float pengali = (tier.skala > 0f) ? tier.skala : 1f;
        float ukuranTarget = ukuranMusuh * pengali;
        if (ukuranTarget > 0f)
        {
            Renderer[] rendUkur = go.GetComponentsInChildren<Renderer>();
            if (rendUkur.Length > 0)
            {
                Bounds b = rendUkur[0].bounds;
                for (int i = 1; i < rendUkur.Length; i++) b.Encapsulate(rendUkur[i].bounds);
                float dimSekarang = Mathf.Max(b.size.x, b.size.y); // sisi terpanjang
                if (dimSekarang > 0.0001f)
                    go.transform.localScale *= (ukuranTarget / dimSekarang);
            }
        }

        // Rigidbody2D (biar bisa digerakkan tanpa gravitasi & tidak berputar)
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb == null) rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Collider2D (untuk deteksi peluru). Dihitung SETELAH ukuran diatur.
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

        // EnemyChase (otak musuh: kejar + serang + nyawa)
        EnemyChase ec = go.GetComponent<EnemyChase>();
        if (ec == null) ec = go.AddComponent<EnemyChase>();
        ec.moveSpeed = tier.kecepatan;
        ec.nyawa = NyawaTier(index, tier);
        ec.skor = tier.skor;
        ec.xp = tier.xp;
    }
}
