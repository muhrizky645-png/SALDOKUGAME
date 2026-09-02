using UnityEngine;

// =====================================================================
//  BALANCE - SATU SUMBER KEBENARAN UNTUK SEMUA ANGKA KESEIMBANGAN
// =====================================================================
//  Sebelum file ini ada, angka keseimbangan tersebar di belasan file:
//  kurva XP di LevelSystem, jumlah musuh di ZombieSpawner, batas skill di
//  SkillManager, damage senjata di SenjataManager. Menyetel kesulitan
//  berarti berburu angka di seluruh project.
//
//  ATURAN MULAI SEKARANG: kalau sebuah angka mempengaruhi rasa main,
//  tempatnya di sini. Bukan di dalam metode, bukan di dalam if.
//
//  Cara menyetel: ubah angka di file ini saja, lalu main. Tidak perlu
//  menyentuh file lain.
// =====================================================================
public static class Balance
{
    // =================================================================
    //  STAT DASAR PEMAIN
    // =================================================================

    // Jeda antar tembakan di awal permainan, dalam detik.
    //
    // Survivor.io memakai sekitar 1 detik untuk senjata dasar di Lv.1.
    // Kode ini dulu menulis 1.2f, TAPI angka itu hanya berlaku untuk
    // komponen yang baru dibuat. Nilai yang benar-benar dipakai tersimpan
    // di dalam prefab pemain, dan bisa saja berbeda tanpa terlihat di kode.
    // Itulah sebabnya sulit tahu kenapa tembakan terasa terlalu cepat.
    //
    // Sekarang PlayerShooting mengambil nilainya dari sini saat Awake,
    // jadi yang tertulis di file inilah yang benar-benar terjadi.
    public const float JedaTembakAwal = 1.0f;

    public const int JumlahPeluruAwal = 1;

    // Jarak pemain mulai menembak musuh, dalam satuan dunia.
    // Kamera memakai orthographicSize 10, jadi layar potret kira-kira
    // 11 satuan lebar dan 20 satuan tinggi. Musuh muncul di radius 10.
    // Nilai 8 berarti pemain menembak musuh yang sudah terlihat di layar.
    // PERIKSA ANGKA INI DULU saat pertama main - kode lama menulis 1f yang
    // hampir pasti bukan nilai sebenarnya di prefab.
    public const float JangkauanTembakAwal = 8f;

    // =================================================================
    //  RUN
    // =================================================================

    // Target durasi satu run penuh (PRD: 15 menit).
    // CATATAN: StageManager saat ini memakai 180/240/300/360 detik.
    // Angka ini belum tersambung ke sana - itu pekerjaan Fase 1.
    public const float DurasiRunDetik = 900f;

    // =================================================================
    //  XP & LEVEL
    // =================================================================

    // Kalau true  -> kurva PRD (kuadratik): floor(5 + 8n + 0.55n^2)
    // Kalau false -> kurva lama (eksponensial x1.3), untuk perbandingan.
    //
    // Kenapa diganti: kurva lama tumbuh x1.3 tiap level, jadi menyenangkan
    // di awal tapi mandek total di akhir. Di level 20 kurva lama menuntut
    // sekitar 1500 XP untuk satu level, sedangkan PRD 385. Padahal justru
    // di menit-menit akhir Survivor.io mempercepat, bukan memperlambat.
    public static bool GunakanKurvaXpPrd = true;

    public static int XpUntukLevel(int level)
    {
        if (level < 1) level = 1;

        if (GunakanKurvaXpPrd)
            return Mathf.FloorToInt(5f + 8f * level + 0.55f * level * level);

        // Kurva lama, direkonstruksi supaya bisa dibandingkan adil.
        float x = 5f;
        for (int i = 1; i < level; i++) x = Mathf.Round(x * 1.3f) + 2f;
        return Mathf.Max(1, Mathf.RoundToInt(x));
    }

    // =================================================================
    //  JUMLAH MUSUH
    // =================================================================

    // PRD: MaxAliveEnemies = min(320, 40 + 18 * menit)
    //
    // PENTING - kenapa berbasis WAKTU, bukan level pemain:
    // Versi lama memakai level pemain. Itu menciptakan umpan balik:
    // makin banyak bunuh -> makin cepat naik level -> makin banyak musuh
    // -> makin banyak XP -> naik level lagi. Pemain kuat menghadapi
    // ledakan kesulitan, pemain lemah menghadapi layar kosong. Kurva
    // kesulitannya jadi berbeda untuk tiap pemain sehingga mustahil
    // diseimbangkan. Waktu memberi kurva yang sama untuk semua orang.
    //
    // MusuhDasar dinaikkan bertahap dari 40 (angka PRD) -> 80 -> 95 setelah
    // beberapa playtest: fase normal masih terasa sepi dibanding Survivor yang
    // langsung penuh. Langit-langit musuh hidup sekarang DIPAKSA 145 lewat kode
    // di ZombieSpawner.Start (bukan lagi 90 di Inspector), jadi angka ini
    // benar-benar terpakai saat main.
    public const int   MusuhDasar     = 95;   // jumlah di detik ke-0
    public const float MusuhPerMenit  = 18f;  // pertambahan tiap menit
    public const int   MaxMusuhMutlak = 320;  // target PRD; WAJIB diuji beban dulu

    public static int MaxMusuhHidup(float detik, float pengaliStage)
    {
        float menit = Mathf.Max(0f, detik) / 60f;
        float n = (MusuhDasar + MusuhPerMenit * menit) * Mathf.Max(0.1f, pengaliStage);
        return Mathf.Clamp(Mathf.RoundToInt(n), 1, MaxMusuhMutlak);
    }

    // =================================================================
    //  PEMANASAN AWAL RUN
    // =================================================================
    //
    // Begitu pemain klik Mulai, jangan langsung sesak. Mulai dari kepadatan
    // sangat rendah lalu naik bertahap sampai penuh, supaya pemain sempat
    // kenalan dengan kendali sebelum lapangan ramai.
    //
    //   0 dtk       : 10%   (sangat lega di awal)
    //   0 - 90 dtk  : naik bertahap 10% -> 100% (naik terus tiap detik)
    //   > 90 dtk    : 100%  (kepadatan normal seperti biasa)
    //
    // Pengali ini dikalikan ke langit-langit musuh hidup DAN jumlah musuh per
    // gelombang spawn di ZombieSpawner, jadi awal run benar-benar merangkak,
    // bukan langsung nyembur. Gelombang pra-bos (jauh setelah menit ke-1,5)
    // tidak terpengaruh - tetap ramai seperti yang diinginkan.
    //
    // MENYETEL:
    //   Mau awal lebih sepi         -> kecilkan PemanasanAwal (mis. 0.05).
    //   Mau ada jeda tahan di awal  -> besarkan PemanasanDetikTahan (mis. 5).
    //   Mau naik penuh lebih cepat  -> kecilkan PemanasanDetikPenuh (mis. 60).
    public const float PemanasanAwal       = 0.1f;  // kepadatan di detik ke-0 (10%)
    public const float PemanasanDetikTahan = 0f;    // lama ditahan di kepadatan awal (0 = langsung naik)
    public const float PemanasanDetikPenuh = 90f;   // kapan mencapai kepadatan penuh (1,5 menit)

    public static float PengaliPemanasan(float detik)
    {
        detik = Mathf.Max(0f, detik);
        if (detik <= PemanasanDetikTahan) return PemanasanAwal;
        if (detik >= PemanasanDetikPenuh) return 1f;
        float t = (detik - PemanasanDetikTahan) / (PemanasanDetikPenuh - PemanasanDetikTahan);
        return Mathf.Lerp(PemanasanAwal, 1f, t);
    }

    // =================================================================
    //  KEKUATAN MUSUH MENGIKUTI WAKTU  (paket "seimbang")
    // =================================================================
    //
    // MASALAH YANG DIPERBAIKI: musuh biasa dulu TIDAK pernah bertambah kuat
    // seiring waktu. Nyawa & kecepatannya sama persis di menit ke-0 dan menit
    // ke-10 (ZombieSpawner hanya mengalikan Pengali stage, bukan waktu).
    // Akibatnya begitu senjata pemain berkembang, cincin musuh tersapu di tepi
    // jangkauan dan NYARIS TAK PERNAH menyentuh pemain - run terasa terlalu
    // mudah walau layar penuh.
    //
    // SOLUSI: nyawa musuh tumbuh pelan mengikuti WAKTU (bukan level pemain,
    // alasan sama seperti jumlah musuh). Musuh menit akhir jadi cukup tebal
    // untuk bertahan menembus cincin dan menekan pemain.
    //
    // MENYETEL:
    //   Terlalu SUSAH  -> kecilkan NyawaMusuhPerMenit dan/atau LajuMusuhDasar.
    //   Terlalu MUDAH  -> naikkan keduanya sedikit.
    // Damage kontak musuh (20/detik) ada di PlayerHealth.damagePerSecond, dan
    // HP pemain (100) di PlayerHealth.maxHealth kalau perlu disetel juga.

    // Pengali kecepatan SEMUA musuh biasa (base). 1.0 = kecepatan asli tier.
    // Riwayat setelan: 1.15 (+15%, kelewat cepat) -> 1.0 (normal, masih cepat)
    // -> 0.5 (separuh, sesuai permintaan playtest: musuh terasa terlalu ngebut).
    // Kalau nanti mau lebih cepat sedikit, naikkan pelan (mis. 0.6 / 0.7).
    public const float LajuMusuhDasar        = 0.5f;
    public const float NyawaMusuhPerMenit    = 0.14f;  // +14% nyawa musuh tiap menit bertahan
    public const float NyawaMusuhMaksPengali = 3.0f;   // batas atas pengali nyawa (menit ~14)

    public static float PengaliNyawaMusuh(float detik)
    {
        float menit = Mathf.Max(0f, detik) / 60f;
        return Mathf.Min(NyawaMusuhMaksPengali, 1f + NyawaMusuhPerMenit * menit);
    }

    // =================================================================
    //  SLOT & BATAS LEVEL  (PRD Bab 6)
    // =================================================================

    public const int SlotSenjata = 6;
    public const int SlotPasif   = 6;

    // Batas level tiap senjata / pasif.
    //
    // INI PERBAIKAN TERPENTING DI SELURUH FILE.
    // Sebelumnya semua skill pasif punya maks = 0 yang berarti TAK TERBATAS.
    // "Serang Lebih Cepat" mengalikan fireRate dengan 0.80 tiap diambil, jadi
    // 10x ambil = fireRate 1.2 -> 0.129, 20x = 0.0138. Praktis tembakan tanpa
    // henti. Hal yang sama berlaku untuk jangkauan, kecepatan lari, dan magnet.
    public const int LevelMaksSenjata = 5;
    public const int LevelMaksPasif   = 5;

    // =================================================================
    //  EVOLUSI  (PRD Bab 7)
    // =================================================================

    public const int   LevelSenjataUntukEvolusi = 5;
    public const int   LevelPasifUntukEvolusi   = 3;
    public const float MenitMinimalEvolusi      = 5f;

    public static bool BolehEvolusi(int levelSenjata, int levelPasif, float detik)
    {
        return levelSenjata >= LevelSenjataUntukEvolusi
            && levelPasif   >= LevelPasifUntukEvolusi
            && detik        >= MenitMinimalEvolusi * 60f;
    }

    // =================================================================
    //  BOS
    // =================================================================

    public const float JedaBosDetik = 45f;

    // =================================================================
    //  KESULITAN CHAPTER  (PRD Bab 9)
    // =================================================================

    // ChapterMultiplier = 1.35 ^ (chapter - 1)
    public static float PengaliChapter(int chapter)
    {
        if (chapter < 1) chapter = 1;
        return Mathf.Pow(1.35f, chapter - 1);
    }

    // =================================================================
    //  KARTU LEVEL UP
    // =================================================================

    public const int JumlahKartuDitawarkan = 3;
}
