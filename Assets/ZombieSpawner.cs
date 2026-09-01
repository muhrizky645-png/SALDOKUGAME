using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [System.Serializable]
    public class MusuhTier
    {
        [Tooltip("Prefab musuh (drag dari folder DungeonMonsters2D/Characters). Contoh: Rat, Bat, Spider, dst.")]
        public GameObject prefab;
        [Tooltip("Gerbang TAMBAHAN opsional. Isi 0 = biarkan JadwalRun yang mengatur kapan musuh ini terbuka (disarankan). Kalau diisi, musuh ini juga harus menunggu level pemain segini.")]
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
    [Tooltip("URUTAN PENTING. JadwalRun membuka daftar ini dari atas ke bawah seiring waktu, jadi baris pertama adalah musuh menit pertama.")]
    public MusuhTier[] daftarMusuh;

    [Header("Cadangan (dipakai kalau Daftar Musuh kosong)")]
    public GameObject zombiePrefab;

    public float spawnDistance = 10f;    // jarak spawn dari pemain

    [Header("Ukuran musuh (sisi terpanjang gambar dalam satuan dunia)")]
    [Tooltip("Semua musuh otomatis diskalakan supaya sisi terpanjangnya (lebar atau tinggi) kira-kira segini.")]
    public float ukuranMusuh = 1f;

    [Header("Batas ukuran musuh biasa (mencegah ada yang kebesaran/kekecilan)")]
    public float pengaliMin = 0.7f;
    public float pengaliMax = 1.5f;

    [Header("Kesulitan mengikuti WAKTU bertahan")]
    [Tooltip("Jeda antar gelombang spawn di detik ke-0.")]
    public float spawnAwal = 0.9f;
    [Tooltip("Pengurangan jeda spawn setiap MENIT (dulu: setiap level).")]
    public float penguranganTiapLevel = 0.1f;
    public float spawnTercepat = 0.2f;
    [Tooltip("TIDAK DIPAKAI LAGI. Jumlah musuh kini dihitung Balance.MaxMusuhHidup.")]
    public int maxAwal = 20;
    [Tooltip("TIDAK DIPAKAI LAGI. Jumlah musuh kini dihitung Balance.MaxMusuhHidup.")]
    public int tambahMaxTiapLevel = 5;
    [Tooltip("Langit-langit keras. DIPAKSA di Start() lewat kode, jadi mengubahnya di sini TIDAK berpengaruh - ubah di ZombieSpawner.Start.")]
    public int maxMutlak = 90;
    public int spawnSekaligus = 2;

    [Header("BOSS (dijadwalkan JadwalRun, tiap 5 menit)")]
    [Tooltip("Daftar prefab BOSS gahar (mis. DragonRed, Demon, MagmaGolem, StoneGolem). Tiap boss muncul, dipilih ACAK dari daftar ini. Kalau kosong, otomatis pakai musuh terkuat di daftar musuh.")]
    public GameObject[] bossPrefabs;
    [Tooltip("TIDAK DIPAKAI LAGI. Jadwal bos kini dipegang JadwalRun.SiklusDetik (300 detik).")]
    public float jedaBoss = 45f;
    [Tooltip("Pengali ukuran boss dibanding musuh biasa. Boss sengaja dibuat besar (minimal dipaksa besar lewat kode).")]
    public float skalaBoss = 4.5f;
    [Tooltip("Ukuran boss minimal (sisi terpanjang, satuan dunia) - dipakai walau skalaBoss diisi kecil.")]
    public float ukuranBossMinimal = 4.5f;

    private Transform player;
    private float timer = 0f;
    private int bossKe = 0;
    private int bossTerakhir = -1;

    // Pengali kesulitan dari stage yang sedang dimainkan.
    // CATATAN: StageManager.PengaliMusuhSekarang sudah lama ada tapi TIDAK
    // PERNAH dibaca siapa pun, sehingga keempat stage praktis identik dan
    // hanya berbeda durasi. Sekarang benar-benar dipakai.
    float Pengali
    {
        get { return Mathf.Max(0.1f, StageManager.PengaliMusuhSekarang); }
    }

    void Start()
    {
        // Buang sisa pendaftaran dari sesi permainan sebelumnya.
        EnemyRegistry.Bersihkan();

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        bossKe = 0;

        // ==== KODE JADI SUMBER KEBENARAN (menimpa nilai Inspector) ====
        // Kepadatan & laju spawn dulu tersimpan di dalam komponen di scene,
        // jadi menyetelnya lewat kode tidak pernah terasa - inilah kenapa
        // fase normal tetap sepi walau angka Balance dinaikkan. Sekarang
        // dipaksa dari sini supaya perubahan benar-benar berlaku saat main.
        //
        // KALAU FPS DI HP TURUN saat ramai: kecilkan maxMutlak di baris ini
        // (mis. 120 atau 100), bukan di Inspector.
        maxMutlak = 145;             // langit-langit musuh hidup (naik dari 90)
        spawnSekaligus = 4;          // berapa musuh tiap gelombang spawn
        spawnAwal = 0.55f;           // jeda spawn di detik ke-0 (lebih cepat mengisi layar)
        spawnTercepat = 0.15f;       // jeda spawn tercepat di menit akhir
        penguranganTiapLevel = 0.08f;
    }

    void Update()
    {
        if (player == null) return;

        float detik = GameTimer.Detik;
        int level = (LevelSystem.Instance != null) ? LevelSystem.Instance.Level : 1;

        // ==== BOS MENGIKUTI JADWAL, BUKAN TIMER SENDIRI ====
        // Dulu: bossBerikut += jedaBoss (45 detik). Bos yang datang tiap 45
        // detik berhenti terasa istimewa - ia jadi sekadar musuh besar yang
        // rutin lewat. Sekarang tiap 5 menit, didahului gelombang dan hening.
        //
        // Dibandingkan dengan hitungan (bukan ditambahkan ke timer) supaya
        // bos tidak pernah dobel atau terlewat kalau satu frame tersendat.
        if (JadwalRun.JumlahBosSeharusnya(detik) > bossKe)
            SpawnBos(detik);

        FaseRun fase = JadwalRun.Fase(detik);

        // Saat GELOMBANG, musuh biasa berjalan lebih cepat biar terasa "diserbu".
        // Boss tidak terpengaruh (diproses terpisah di EnemyChase).
        EnemyChase.PengaliLajuFase = (fase == FaseRun.Wave) ? 1.35f : 1f;

        // HENING: spawn berhenti total. Inilah jeda mencekam sebelum bos.
        if (fase == FaseRun.Hening)
        {
            timer = 0f;
            return;
        }

        // ==== KESULITAN MENGIKUTI WAKTU, BUKAN LEVEL PEMAIN ====
        // Dulu jeda spawn dan jumlah musuh dihitung dari level pemain. Itu
        // menciptakan umpan balik: makin banyak bunuh -> makin cepat naik level
        // -> makin banyak musuh -> makin banyak XP -> naik level lagi. Pemain
        // kuat menghadapi ledakan kesulitan, pemain lemah menghadapi layar
        // kosong. Kurva kesulitannya berbeda untuk tiap pemain sehingga
        // mustahil diseimbangkan. Waktu memberi kurva yang sama untuk semua.
        float menit = Mathf.Max(0f, detik) / 60f;
        float jedaSpawn = Mathf.Max(spawnTercepat, spawnAwal - penguranganTiapLevel * menit);
        jedaSpawn *= JadwalRun.PengaliJedaSpawn(fase);

        float pengaliFase = JadwalRun.PengaliJumlah(fase);
        int maxSekarang = Mathf.Min(maxMutlak,
            Mathf.RoundToInt(Balance.MaxMusuhHidup(detik, Pengali) * pengaliFase));
        maxSekarang = Mathf.Max(1, maxSekarang);

        int sekaligus = Mathf.Max(1, Mathf.RoundToInt(spawnSekaligus * pengaliFase));

        timer += Time.deltaTime;
        if (timer >= jedaSpawn)
        {
            timer = 0f;
            for (int i = 0; i < sekaligus; i++)
                Spawn(detik, level, maxSekarang);
        }
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

    void Spawn(float detik, int level, int maxSekarang)
    {
        // DULU: FindGameObjectsWithTag("Enemy").Length menyisir seluruh scene
        // dan mengalokasikan array baru, setiap kali mau spawn.
        // SEKARANG: registry sudah tahu jumlahnya, O(1).
        if (EnemyRegistry.Jumlah >= maxSekarang) return;

        Vector3 posisi = PosisiSpawn();

        int index;
        MusuhTier tier = PilihTier(detik, level, out index);
        GameObject prefab = (tier != null && tier.prefab != null) ? tier.prefab : zombiePrefab;
        if (prefab == null) return;

        GameObject musuh = Instantiate(prefab, posisi, Quaternion.identity);

        if (tier != null && tier.prefab != null)
            SiapkanMusuh(musuh, tier, index);
    }

    // Munculkan musuh MENGELILINGI sebuah titik, bukan mengelilingi pemain.
    // Dipakai BosPola saat bos memanggil bawahan, supaya bawahan muncul dari
    // bos dan memutus jalur mundur pemain - bukan sekadar menambah keramaian.
    public void SpawnDiSekitar(Vector3 pusat, int jumlah)
    {
        float detik = GameTimer.Detik;
        int level = (LevelSystem.Instance != null) ? LevelSystem.Instance.Level : 1;

        for (int i = 0; i < jumlah; i++)
        {
            Vector2 r = Random.insideUnitCircle.normalized * Random.Range(1.8f, 3.2f);
            Vector3 posisi = pusat + new Vector3(r.x, r.y, 0f);

            int index;
            MusuhTier tier = PilihTier(detik, level, out index);
            GameObject prefab = (tier != null && tier.prefab != null) ? tier.prefab : zombiePrefab;
            if (prefab == null) return;

            GameObject musuh = Instantiate(prefab, posisi, Quaternion.identity);
            if (tier != null && tier.prefab != null)
                SiapkanMusuh(musuh, tier, index);
        }
    }

    // Paksa spawn sejumlah musuh, mengabaikan batas maksimum.
    // Dipakai StressTest untuk mengukur FPS pada beban tinggi.
    public void SpawnPaksa(int jumlah)
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
        if (player == null) return;

        float detik = GameTimer.Detik;
        int level = (LevelSystem.Instance != null) ? LevelSystem.Instance.Level : 1;
        for (int i = 0; i < jumlah; i++) Spawn(detik, level, int.MaxValue);
    }

    // Spawn satu BOSS: dipilih ACAK dari daftar bossPrefabs, kalau kosong pakai yang terkuat.
    void SpawnBos(float detik)
    {
        GameObject prefab = PilihBoss();
        if (prefab == null) prefab = PrefabTerkuat();
        if (prefab == null) prefab = zombiePrefab;

        // Naikkan hitungan APA PUN yang terjadi. Kalau tidak, dan prefab-nya
        // kosong, Update akan mencoba spawn bos yang sama tiap frame selamanya.
        bossKe++;
        if (prefab == null) return;

        Vector3 posisi = PosisiSpawn();
        GameObject go = Instantiate(prefab, posisi, Quaternion.identity);

        go.tag = "Enemy";
        go.layer = 0;

        // Boss DIPAKSA besar: ambil yang terbesar antara ukuranMusuh*skalaBoss dan ukuranBossMinimal.
        float ukuranBoss = Mathf.Max(ukuranMusuh * Mathf.Max(1f, skalaBoss), ukuranBossMinimal);
        AturFisikDanUkuran(go, ukuranBoss);

        EnemyChase ec = go.GetComponent<EnemyChase>();
        if (ec == null) ec = go.AddComponent<EnemyChase>();
        ec.moveSpeed = 1.3f;
        ec.nyawa = JadwalRun.NyawaBos(bossKe, detik, Pengali);
        ec.skor = 500;
        ec.xp = 25;
        ec.bos = true; // ditandai boss (diproses di EnemyChase.Start)

        // Pola serangan dipasang lewat kode, jadi kamu TIDAK perlu mengedit
        // satu pun prefab bos di Inspector. Prefab bos apa pun yang kamu drag
        // ke bossPrefabs langsung mendapat tembakan melingkar, terjangan, dan
        // panggil bawahan.
        BosPola pola = go.GetComponent<BosPola>();
        if (pola == null) pola = go.AddComponent<BosPola>();
        pola.tingkat = bossKe;
    }

    // Pilih boss acak dari daftar; hindari mengulang boss yang sama dua kali berturut-turut bila memungkinkan.
    GameObject PilihBoss()
    {
        if (bossPrefabs == null || bossPrefabs.Length == 0) return null;

        int jumlah = 0;
        for (int i = 0; i < bossPrefabs.Length; i++)
            if (bossPrefabs[i] != null) jumlah++;
        if (jumlah == 0) return null;

        int pilih = Random.Range(0, jumlah);
        int hitung = 0;
        int indexTerpilih = -1;
        for (int i = 0; i < bossPrefabs.Length; i++)
        {
            if (bossPrefabs[i] == null) continue;
            if (hitung == pilih) { indexTerpilih = i; break; }
            hitung++;
        }

        // Kalau kebetulan sama dengan boss sebelumnya dan ada lebih dari satu pilihan, geser satu.
        if (indexTerpilih == bossTerakhir && jumlah > 1)
        {
            for (int i = 1; i <= bossPrefabs.Length; i++)
            {
                int cek = (indexTerpilih + i) % bossPrefabs.Length;
                if (bossPrefabs[cek] != null && cek != bossTerakhir) { indexTerpilih = cek; break; }
            }
        }

        bossTerakhir = indexTerpilih;
        return (indexTerpilih >= 0) ? bossPrefabs[indexTerpilih] : null;
    }

    GameObject PrefabTerkuat()
    {
        if (daftarMusuh == null) return null;
        for (int i = daftarMusuh.Length - 1; i >= 0; i--)
            if (daftarMusuh[i] != null && daftarMusuh[i].prefab != null)
                return daftarMusuh[i].prefab;
        return null;
    }

    // Pilih satu tier dari yang SUDAH TERBUKA pada detik ini.
    //
    // Dulu gerbangnya level pemain: tier ke-N terbuka di level N. Terlihat
    // bertahap, tapi dengan kurva XP lama pemain mencapai level 5 dalam
    // sekitar setengah menit - jadi seluruh daftar musuh terbuka hampir
    // seketika. Gerbangnya ada, pemain hanya melewatinya terlalu cepat.
    //
    // Sekarang gerbangnya WAKTU. mulaiLevel tetap dihormati kalau kamu
    // sengaja mengisinya, sebagai syarat tambahan.
    MusuhTier PilihTier(float detik, int level, out int indexTerpilih)
    {
        indexTerpilih = -1;
        if (daftarMusuh == null || daftarMusuh.Length == 0) return null;

        int batas = JadwalRun.JenisTerbuka(detik);

        int jumlah = 0;
        for (int i = 0; i < daftarMusuh.Length; i++)
        {
            if (Terbuka(i, batas, level)) jumlah++;
        }

        // Jaring pengaman: kalau belum ada yang terbuka (misal daftar kosong
        // di baris-baris awal), pakai baris pertama yang punya prefab.
        if (jumlah == 0)
        {
            for (int i = 0; i < daftarMusuh.Length; i++)
            {
                var t0 = daftarMusuh[i];
                if (t0 != null && t0.prefab != null) { indexTerpilih = i; return t0; }
            }
            return null;
        }

        int pilih = Random.Range(0, jumlah);
        int hitung = 0;
        for (int i = 0; i < daftarMusuh.Length; i++)
        {
            if (!Terbuka(i, batas, level)) continue;
            if (hitung == pilih) { indexTerpilih = i; return daftarMusuh[i]; }
            hitung++;
        }
        return null;
    }

    bool Terbuka(int i, int batas, int level)
    {
        var t = daftarMusuh[i];
        if (t == null || t.prefab == null) return false;
        if (i >= batas) return false;
        if (t.mulaiLevel > 0 && level < t.mulaiLevel) return false;
        return true;
    }

    void SiapkanMusuh(GameObject go, MusuhTier tier, int index)
    {
        go.tag = "Enemy";
        go.layer = 0;

        // Batasi pengali biar tidak ada musuh yang kebesaran/kekecilan
        float pengali = (tier.skala > 0f) ? tier.skala : 1f;
        pengali = Mathf.Clamp(pengali, pengaliMin, pengaliMax);
        AturFisikDanUkuran(go, ukuranMusuh * pengali);

        EnemyChase ec = go.GetComponent<EnemyChase>();
        if (ec == null) ec = go.AddComponent<EnemyChase>();
        ec.moveSpeed = tier.kecepatan;
        // Nyawa dikalikan tingkat kesulitan stage.
        ec.nyawa = Mathf.Max(1, Mathf.RoundToInt(NyawaTier(index, tier) * Pengali));
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
