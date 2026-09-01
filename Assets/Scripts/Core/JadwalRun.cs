using UnityEngine;

// Fase yang sedang berjalan dalam satu run.
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
//     (a) ZombieSpawner membuka tier ke-N pada level pemain N. Terlihat
//         bertahap. Tapi dengan kurva XP lama, level 5 tercapai sekitar
//         setengah menit - seluruh daftar musuh terbuka hampir seketika.
//
//     (b) EnemyChase.RollTipe mengacak Cepat/Tank/Peledak/Penembak dengan
//         peluang 12% SEJAK DETIK KE-0, tanpa gerbang apa pun. Ini penyebab
//         yang paling terasa, karena berlaku untuk prefab apa pun.
//
//  2) Bos tiap 45 detik, tanpa struktur. Bos yang terlalu sering justru
//     berhenti terasa istimewa.
//
//  3) Tidak ada gelombang sama sekali. Tekanannya datar dari awal ke akhir.
// ============================================================================
public static class JadwalRun
{
    // =================================================================
    //  IRAMA SATU SIKLUS
    // =================================================================
    //
    //   ...tenang...  ──── GELOMBANG 45s ──── HENING 20s ──── BOS
    //
    // Keheningan itu bukan waktu kosong, dan bukan hiasan. Setelah 45 detik
    // diserbu, 20 detik sunyi membuat pemain sadar sesuatu yang lebih besar
    // sedang datang. Tanpa jeda ini bos cuma jadi musuh besar di tengah
    // keramaian, dan kemunculannya tidak terbaca sebagai peristiwa.

    public const float SiklusDetik  = 300f;  // bos tiap 5 menit
    public const float DetikWave    = 65f;   // gelombang + hening, dihitung mundur dari bos
    public const float DetikHening  = 20f;   // sunyi total tepat sebelum bos
    public const float MajuBosAkhir = 60f;   // bos terakhir dimajukan sekian detik dari akhir run

    // Durasi run diambil dari STAGE yang sedang dimainkan, bukan angka tetap.
    // Dengan begitu jadwal bos ikut menyesuaikan sendiri kalau kamu mengubah
    // durasi sebuah stage - tidak ada dua tempat yang harus diselaraskan manual.
    public static float DurasiRun
    {
        get { return Mathf.Max(60f, StageManager.TargetSekarang); }
    }

    public static int JumlahBosDalamRun
    {
        get { return Mathf.Max(1, Mathf.FloorToInt(DurasiRun / SiklusDetik)); }
    }

    // Kapan bos ke-n muncul (n mulai dari 1).
    //
    // Bos TERAKHIR sengaja dimajukan 60 detik dari akhir run. Kalau tidak,
    // pada run 15 menit bos ketiga muncul tepat di detik ke-900 - persis saat
    // HasilMain memicu layar MENANG. Pemain akan menang sebelum sempat melihat
    // bosnya, dan pertarungan terakhir jadi tidak pernah terjadi.
    public static float WaktuBos(int n)
    {
        float t = n * SiklusDetik;
        if (n >= JumlahBosDalamRun) t = Mathf.Min(t, DurasiRun - MajuBosAkhir);
        return Mathf.Max(30f, t);
    }

    // Berapa bos yang seharusnya SUDAH pernah muncul pada detik ini.
    // Spawner membandingkannya dengan hitungannya sendiri, jadi bos tidak
    // pernah dobel atau terlewat walau satu frame tersendat.
    public static int JumlahBosSeharusnya(float detik)
    {
        int n = 0;
        int maks = JumlahBosDalamRun;
        for (int i = 1; i <= maks; i++)
            if (detik >= WaktuBos(i)) n++;
        return n;
    }

    // Detik menuju bos berikutnya. Mengembalikan angka besar kalau semua bos
    // dalam run ini sudah lewat.
    public static float DetikKeBosBerikut(float detik)
    {
        int maks = JumlahBosDalamRun;
        for (int i = 1; i <= maks; i++)
        {
            float t = WaktuBos(i);
            if (detik < t) return t - detik;
        }
        return 9999f;
    }

    // Fase dihitung dari JARAK KE BOS BERIKUTNYA, bukan dari sisa bagi waktu.
    // Ini penting: bos terakhir dimajukan, jadi rumus modulo akan meleset dan
    // gelombang penutup tidak akan pernah terjadi.
    public static FaseRun Fase(float detik)
    {
        // Selama bos hidup, dialah acaranya. Fase lain ditunda.
        if (EnemyChase.JumlahBos > 0) return FaseRun.Bos;

        float sisa = DetikKeBosBerikut(detik);
        if (sisa <= DetikHening) return FaseRun.Hening;
        if (sisa <= DetikWave) return FaseRun.Wave;
        return FaseRun.Normal;
    }

    // =================================================================
    //  TEKANAN TIAP FASE
    // =================================================================
    //
    // Normal dinaikkan (0.65 -> 0.85) setelah playtest: fase normal dulu
    // terasa terlalu sepi dibanding Survivor yang langsung ramai. Sekarang
    // fase normal pun padat, dan Wave (1.45) MELONJAK jelas di atasnya supaya
    // serbuan pra-bos benar-benar terbaca sebagai gelombang.
    //
    // CATATAN: langit-langit musuh hidup kini 145 (DIPAKSA lewat kode di
    // ZombieSpawner.Start, bukan 90 di Inspector). Wave akan mendekati langit-
    // langit itu; kalau FPS di HP asli turun, kecilkan maxMutlak di sana.

    public static float PengaliJumlah(FaseRun fase)
    {
        switch (fase)
        {
            case FaseRun.Wave:   return 1.45f;  // GELOMBANG: lonjakan jelas
            case FaseRun.Hening: return 0.40f;
            case FaseRun.Bos:    return 0.55f;  // lapangan dilapangkan untuk bos
            default:             return 0.85f;  // Normal: sekarang sudah padat
        }
    }

    // Pengali jeda antar spawn. Di bawah 1 = lebih sering.
    public static float PengaliJedaSpawn(FaseRun fase)
    {
        switch (fase)
        {
            case FaseRun.Wave:   return 0.30f;
            case FaseRun.Hening: return 999f;   // praktis berhenti
            case FaseRun.Bos:    return 1.60f;
            default:             return 0.55f;  // Normal: layar terisi cepat sejak awal
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
    // Jaraknya sengaja lebar supaya pemain sempat MEMPELAJARI satu ancaman
    // sebelum ancaman berikutnya datang. Tank yang muncul bersamaan dengan
    // Penembak hanya terasa sebagai kekacauan, bukan tantangan.
    public const float DetikBukaCepat    = 60f;   // 1:00 - paling mudah dibaca, diajarkan duluan
    public const float DetikBukaTank     = 150f;  // 2:30 - memaksa berhenti spam tembak
    public const float DetikBukaPenembak = 240f;  // 4:00 - ancaman jarak jauh pertama
    public const float DetikBukaPeledak  = 360f;  // 6:00 - menghukum yang terlalu dekat

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

    // Nyawa bos. Berbasis WAKTU dan nomor bos, bukan level pemain - alasan
    // yang sama seperti jumlah musuh: level pemain menciptakan umpan balik
    // yang membuat kurvanya berbeda untuk tiap orang.
    //
    // ANGKA INI TEBAKAN. Aku tidak tahu DPS pemain di menit ke-5 karena damage
    // senjata masih tertanam di SenjataManager dan belum terukur. Kalau bos
    // mati dalam 5 detik, naikkan NyawaBosDasar. Kalau butuh lebih dari 45
    // detik, turunkan. Targetnya kira-kira 20-35 detik.
    public const int   NyawaBosDasar    = 90;
    public const float NyawaBosPerMenit = 45f;
    public const int   NyawaBosPerNomor = 90;

    public static int NyawaBos(int nomorBos, float detik, float pengaliStage)
    {
        float menit = Mathf.Max(0f, detik) / 60f;
        float n = NyawaBosDasar
                + NyawaBosPerMenit * menit
                + NyawaBosPerNomor * Mathf.Max(0, nomorBos - 1);
        return Mathf.Max(1, Mathf.RoundToInt(n * Mathf.Max(0.1f, pengaliStage)));
    }
}
