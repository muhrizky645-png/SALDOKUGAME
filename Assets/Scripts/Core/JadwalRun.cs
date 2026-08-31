using UnityEngine;

// Fase yang sedang berjalan dalam satu siklus 5 menit.
public enum FaseRun
{
    Normal,   // tekanan tetap, musuh bertambah pelan
    Wave,     // GELOMBANG: serbuan padat sebelum bos
    Hening,   // jeda mencekam, spawn berhenti total
    Bos       // bos sedang hidup di lapangan
}

// ============================================================================
//  JADWAL RUN - SUTRADARA WAKTU
// ----------------------------------------------------------------------------
//  Pasangan Balance.cs, khusus untuk hal yang bergantung WAKTU.
//  Balance.cs menjawab "seberapa kuat". File ini menjawab "kapan".
//
//  MASALAH YANG DIPERBAIKI:
//
//  1) Variasi musuh muncul semua sekaligus.
//     Ada DUA penyebab yang menumpuk, dan yang kedua tersembunyi:
//
//     (a) ZombieSpawner.LevelBuka membuka tier ke-N pada level pemain N.
//         Terlihat bertahap. Tapi dengan kurva XP lama, level 2 tercapai
//         pada 5 XP dan level 5 pada sekitar 68 XP total - artinya seluruh
//         daftar musuh terbuka dalam kira-kira setengah menit pertama.
//         Gerbangnya ada, tapi pemain melewatinya terlalu cepat.
//
//     (b) EnemyChase.RollTipe mengacak Cepat/Tank/Peledak/Penembak dengan
//         peluang 12% SEJAK DETIK KE-0, tanpa gerbang apa pun. Jadi walau
//         tier prefab-nya dibatasi, keempat perilaku khusus tetap tampil
//         di menit pertama. Inilah yang paling terasa sebagai
//         "musuh langsung muncul semua".
//
//     Sekarang keduanya dijadwalkan lewat WAKTU di file ini.
//
//  2) Bos tiap 45 detik, tanpa struktur.
//     Bos yang sering justru berhenti terasa istimewa. Sekarang tiap 5 menit,
//     dengan gelombang dan keheningan sebagai pembuka.
//
//  3) Tidak ada gelombang sama sekali.
//     Tekanannya datar dari awal sampai akhir. Sekarang ada irama:
//     tenang -> serbuan -> hening -> bos.
// ============================================================================
public static class JadwalRun
{
    // =================================================================
    //  SIKLUS 5 MENIT
    // =================================================================
    //
    //   0:00 ─────────────── 3:55      tenang, musuh bertambah pelan
    //   3:55 ─────── 4:40              GELOMBANG, serbuan padat
    //   4:40 ─── 5:00                  HENING, spawn berhenti total
    //   5:00                           BOS MUNCUL
    //
    // Keheningan itu bukan waktu kosong. Setelah 45 detik diserbu,
    // 20 detik sunyi membuat pemain sadar sesuatu yang lebih besar
    // sedang datang. Tanpa jeda ini, bos hanya jadi musuh besar
    // di tengah keramaian dan kemunculannya tidak terbaca.

    public const float SiklusDetik = 300f;   // bos tiap 5 menit
    public const float WaveMulai   = 235f;   // gelombang mulai (detik ke-N dalam siklus)
    public const float HeningMulai = 280f;   // spawn berhenti

    public static FaseRun Fase(float detik)
    {
        // Selama bos hidup, dialah acaranya. Fase lain ditunda.
        if (EnemyChase.JumlahBos > 0) return FaseRun.Bos;

        float t = Mathf.Max(0f, detik) % SiklusDetik;
        if (t >= HeningMulai) return FaseRun.Hening;
        if (t >= WaveMulai) return FaseRun.Wave;
        return FaseRun.Normal;
    }

    // Berapa bos yang seharusnya SUDAH pernah muncul pada detik ini.
    // Spawner membandingkannya dengan hitungannya sendiri, jadi bos tidak
    // pernah dobel walau frame sempat tersendat.
    public static int JumlahBosSeharusnya(float detik)
    {
        return Mathf.FloorToInt(Mathf.Max(0f, detik) / SiklusDetik);
    }

    public static float DetikKeBosBerikut(float detik)
    {
        float t = Mathf.Max(0f, detik) % SiklusDetik;
        return SiklusDetik - t;
    }

    // =================================================================
    //  TEKANAN TIAP FASE
    // =================================================================
    //
    // PENTING - kenapa Normal justru DI BAWAH 1.0:
    // Batas jumlah musuh dijaga maxMutlak (90) sampai stress test selesai.
    // Kalau Normal bernilai 1.0, kurva Balance.MaxMusuhHidup sudah menabrak
    // langit-langit itu sekitar menit ke-2,8 - sehingga gelombang tidak punya
    // ruang untuk terasa lebih padat. Jadi Normal diturunkan supaya
    // GELOMBANG punya tempat naik. Kontras itu yang menciptakan irama;
    // angka mutlaknya tidak sepenting selisihnya.

    public static float PengaliJumlah(FaseRun fase)
    {
        switch (fase)
        {
            case FaseRun.Wave:   return 1.00f;
            case FaseRun.Hening: return 0.45f;
            case FaseRun.Bos:    return 0.55f;  // lapangan dilapangkan untuk bos
            default:             return 0.65f;
        }
    }

    // Pengali jeda antar spawn. Di bawah 1 = lebih sering.
    public static float PengaliJedaSpawn(FaseRun fase)
    {
        switch (fase)
        {
            case FaseRun.Wave:   return 0.35f;
            case FaseRun.Hening: return 999f;   // praktis berhenti
            case FaseRun.Bos:    return 1.60f;
            default:             return 1.00f;
        }
    }

    // =================================================================
    //  VARIASI MUSUH BERTAHAP
    // =================================================================

    public const int   JenisAwal          = 2;    // berapa tier terbuka di detik ke-0
    public const float DetikTiapJenisBaru = 55f;  // satu tier baru tiap sekian detik

    // Berapa banyak baris dari daftarMusuh yang sudah boleh muncul.
    public static int JenisTerbuka(float detik)
    {
        return JenisAwal + Mathf.FloorToInt(Mathf.Max(0f, detik) / DetikTiapJenisBaru);
    }

    // Kapan tiap perilaku khusus diperkenalkan.
    // Dipisah supaya pemain punya waktu MEMPELAJARI satu ancaman sebelum
    // ancaman berikutnya datang. Tank yang muncul bersamaan dengan Penembak
    // di menit pertama hanya terasa sebagai kekacauan, bukan tantangan.
    public const float DetikBukaCepat    = 60f;   // 1:00 - dipelajari duluan, paling mudah dibaca
    public const float DetikBukaTank     = 150f;  // 2:30 - memaksa pemain berhenti spam tembak
    public const float DetikBukaPenembak = 240f;  // 4:00 - ancaman jarak jauh pertama
    public const float DetikBukaPeledak  = 360f;  // 6:00 - menghukum pemain yang terlalu dekat

    static readonly EnemyChase.Tipe[] _buf = new EnemyChase.Tipe[4];

    public static EnemyChase.Tipe RollTipe(float detik)
    {
        detik = Mathf.Max(0f, detik);

        // Peluang munculnya tipe khusus naik pelan seiring waktu.
        float peluang = Mathf.Clamp(0.10f + (detik / 60f) * 0.045f, 0f, 0.45f);
        if (Random.value >= peluang) return EnemyChase.Tipe.Biasa;

        int n = 0;
        if (detik >= DetikBukaCepat)    _buf[n++] = EnemyChase.Tipe.Cepat;
        if (detik >= DetikBukaTank)     _buf[n++] = EnemyChase.Tipe.Tank;
        if (detik >= DetikBukaPenembak) _buf[n++] = EnemyChase.Tipe.Penembak;
        if (detik >= DetikBukaPeledak)  _buf[n++] = EnemyChase.Tipe.Peledak;

        if (n == 0) return EnemyChase.Tipe.Biasa;
        return _buf[Random.Range(0, n)];
    }

    // =================================================================
    //  BOS
    // =================================================================

    // Nyawa bos. Berbasis WAKTU dan nomor bos, bukan level pemain -
    // alasan yang sama seperti jumlah musuh: level pemain menciptakan
    // umpan balik yang membuat kurvanya berbeda tiap orang.
    //
    // ANGKA INI TEBAKAN. Aku tidak tahu DPS pemain di menit ke-5 karena
    // damage senjata masih tertanam di SenjataManager dan belum terukur.
    // Kalau bos mati dalam 5 detik, naikkan NyawaBosDasar. Kalau butuh
    // lebih dari 45 detik, turunkan. Targetnya kira-kira 20-35 detik.
    public const int   NyawaBosDasar     = 90;
    public const float NyawaBosPerMenit  = 45f;
    public const int   NyawaBosPerNomor  = 90;

    public static int NyawaBos(int nomorBos, float detik, float pengaliStage)
    {
        float menit = Mathf.Max(0f, detik) / 60f;
        float n = NyawaBosDasar
                + NyawaBosPerMenit * menit
                + NyawaBosPerNomor * Mathf.Max(0, nomorBos - 1);
        return Mathf.Max(1, Mathf.RoundToInt(n * Mathf.Max(0.1f, pengaliStage)));
    }
}
