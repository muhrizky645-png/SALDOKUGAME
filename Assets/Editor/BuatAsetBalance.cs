using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

// ============================================================================
//  GENERATOR ASET BALANCE
// ----------------------------------------------------------------------------
//  Membuat SEMUA aset ScriptableObject sekali klik, lengkap dengan angka awal
//  dan sambungan evolusinya. Tanpa ini kamu harus klik kanan -> Create -> ...
//  sebanyak 20+ kali lalu mengisi ratusan field satu per satu.
//
//  CARA PAKAI:
//    Menu Unity -> Zomburst -> Buat Semua Aset Balance
//
//  AMAN DIJALANKAN BERULANG KALI.
//  Aset yang sudah ada TIDAK PERNAH ditimpa. Jadi kalau kamu sudah menyetel
//  damage Pisau Berputar lalu menjalankan ini lagi, setelanmu tetap utuh -
//  yang dibuat hanya aset yang benar-benar belum ada. Ini disengaja: menimpa
//  diam-diam adalah cara tercepat menghapus kerja balancing berjam-jam.
// ============================================================================
public static class BuatAsetBalance
{
    const string Akar     = "Assets/Data";
    const string FSenjata = Akar + "/Senjata";
    const string FPasif   = Akar + "/Pasif";
    const string FMusuh   = Akar + "/Musuh";
    const string FStage   = Akar + "/Stage";

    static int dibuat;
    static int dilewati;

    // =====================================================================
    //  MENU UTAMA
    // =====================================================================
    [MenuItem("Zomburst/Buat Semua Aset Balance", false, 0)]
    public static void BuatSemua()
    {
        dibuat = 0;
        dilewati = 0;

        PastikanFolder(FSenjata);
        PastikanFolder(FPasif);
        PastikanFolder(FMusuh);
        PastikanFolder(FStage);

        // Urutannya penting: pasif dulu (dipakai sebagai syarat evolusi),
        // lalu bentuk evolusi (dipakai sebagai hasil), baru senjata dasar.
        Dictionary<string, PasifSO>  pasif = BuatPasif();
        Dictionary<string, SenjataSO> evo  = BuatBentukEvolusi();
        BuatSenjataDasar(pasif, evo);

        Dictionary<string, MusuhSO> musuh = BuatMusuh();
        BuatStage(musuh);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string pesan =
            dibuat + " aset baru dibuat.\n" +
            dilewati + " aset dilewati karena sudah ada.\n\n" +
            "Semua tersimpan di " + Akar + "\n\n" +
            "LANGKAH BERIKUTNYA:\n" +
            "Aset Musuh belum punya prefab. Jalankan\n" +
            "Zomburst > Periksa Aset Balance untuk melihat\n" +
            "apa saja yang masih perlu kamu isi.";

        Debug.Log("[BuatAsetBalance] " + dibuat + " dibuat, " + dilewati + " dilewati.");
        EditorUtility.DisplayDialog("Aset Balance", pesan, "Mengerti");
    }

    // =====================================================================
    //  MENU PEMERIKSA
    // =====================================================================
    [MenuItem("Zomburst/Periksa Aset Balance", false, 1)]
    public static void Periksa()
    {
        List<string> masalah = new List<string>();

        foreach (MusuhSO m in SemuaAset<MusuhSO>(FMusuh))
            if (m.prefab == null)
                masalah.Add("MUSUH " + m.id + " (" + m.namaTampil + ") belum punya prefab.");

        foreach (SenjataSO s in SemuaAset<SenjataSO>(FSenjata))
        {
            if (s.ikon == null)
                masalah.Add("SENJATA " + s.id + " (" + s.namaTampil + ") belum punya ikon.");
            if (!s.bentukEvolusi && s.hasilEvolusi == null)
                masalah.Add("SENJATA " + s.id + " belum punya hasil evolusi.");
        }

        foreach (PasifSO p in SemuaAset<PasifSO>(FPasif))
            if (p.ikon == null)
                masalah.Add("PASIF " + p.id + " (" + p.namaTampil + ") belum punya ikon.");

        foreach (StageSO st in SemuaAset<StageSO>(FStage))
        {
            if (st.musuhTersedia == null || st.musuhTersedia.Length == 0)
                masalah.Add("STAGE " + st.KodeStage + " belum punya daftar musuh.");
            if (st.bosTersedia == null || st.bosTersedia.Length == 0)
                masalah.Add("STAGE " + st.KodeStage + " belum punya daftar bos.");
        }

        if (masalah.Count == 0)
        {
            EditorUtility.DisplayDialog("Periksa Aset Balance",
                "Semua aset lengkap. Tidak ada yang perlu diisi.", "Bagus");
            return;
        }

        // Ikon boleh kosong untuk sementara - game tetap jalan memakai ikon
        // prosedural yang sudah ada di Ikon.UntukSkill. Prefab musuh TIDAK boleh
        // kosong kalau aset itu sudah dipakai spawner.
        string teks = "Masih perlu diisi (" + masalah.Count + "):\n\n";
        for (int i = 0; i < masalah.Count && i < 25; i++) teks += "- " + masalah[i] + "\n";
        if (masalah.Count > 25) teks += "...dan " + (masalah.Count - 25) + " lainnya. Lihat Console.";

        foreach (string m in masalah) Debug.LogWarning("[Periksa Aset] " + m);
        EditorUtility.DisplayDialog("Periksa Aset Balance", teks, "Oke");
    }

    // =====================================================================
    //  PASIF  (P01-P06, menyalin keenam skill yang sudah ada)
    // =====================================================================
    static Dictionary<string, PasifSO> BuatPasif()
    {
        Dictionary<string, PasifSO> hasil = new Dictionary<string, PasifSO>();

        // Nilai di bawah KUMULATIF, bukan per-level.
        // Contoh P01: kode lama mengalikan fireRate dengan 0.80 tiap ambil.
        // Setelah 3x, totalnya 0.8^3 = 0.512, artinya 49% lebih cepat.
        // Angka kumulatif membuat efeknya bisa dibaca langsung tanpa menghitung.

        hasil["P01"] = Pasif("P01_SerangLebihCepat", "P01", "Serang Lebih Cepat",
            "Jeda antar tembakan berkurang.",
            EfekPasif.KecepatanTembak, true,
            new float[] { 0.20f, 0.36f, 0.49f, 0.59f, 0.67f });

        hasil["P02"] = Pasif("P02_PeluruTambahan", "P02", "Peluru Tambahan",
            "Menembakkan peluru tambahan tiap serangan.",
            EfekPasif.JumlahPeluru, false,
            new float[] { 1f, 2f, 3f, 4f, 5f });

        hasil["P03"] = Pasif("P03_JangkauanLebihJauh", "P03", "Jangkauan Lebih Jauh",
            "Menembak musuh dari jarak lebih jauh.",
            EfekPasif.Jangkauan, true,
            new float[] { 0.15f, 0.30f, 0.45f, 0.60f, 0.75f });

        hasil["P04"] = Pasif("P04_KakiLebihCepat", "P04", "Kaki Lebih Cepat",
            "Bergerak lebih gesit menghindari kerumunan.",
            EfekPasif.KecepatanGerak, true,
            new float[] { 0.10f, 0.20f, 0.30f, 0.40f, 0.50f });

        hasil["P05"] = Pasif("P05_BadanLebihKuat", "P05", "Badan Lebih Kuat",
            "Menambah nyawa maksimum dan langsung memulihkan.",
            EfekPasif.MaxHP, false,
            new float[] { 30f, 60f, 90f, 120f, 150f });

        hasil["P06"] = Pasif("P06_MagnetPermata", "P06", "Magnet Permata",
            "Menarik permata dari jarak lebih jauh.",
            EfekPasif.Magnet, true,
            new float[] { 0.30f, 0.60f, 0.90f, 1.20f, 1.50f });

        return hasil;
    }

    static PasifSO Pasif(string file, string id, string nama, string desk,
        EfekPasif efek, bool persen, float[] nilai)
    {
        bool baru;
        PasifSO p = Ambil<PasifSO>(FPasif, file, out baru);
        if (!baru) return p;

        p.id = id;
        p.namaTampil = nama;
        p.deskripsi = desk;
        p.efek = efek;
        p.levelMaks = Balance.LevelMaksPasif;
        p.nilaiPerLevel = nilai;
        p.persentase = persen;
        p.bobotTawaran = 1f;
        EditorUtility.SetDirty(p);
        return p;
    }

    // =====================================================================
    //  BENTUK EVOLUSI  (dibuat lebih dulu supaya bisa disambungkan)
    // =====================================================================
    static Dictionary<string, SenjataSO> BuatBentukEvolusi()
    {
        Dictionary<string, SenjataSO> hasil = new Dictionary<string, SenjataSO>();

        hasil["W01E"] = Evolusi("W01E_BadaiBilah", "W01E", "Badai Bilah",
            "Delapan bilah berputar kencang mengelilingimu.",
            ElemenSenjata.Kinetik, PerilakuSenjata.Orbit,
            26, 0.30f, 2.6f, 8, 260f);

        hasil["W02E"] = Evolusi("W02E_BadaiPetir", "W02E", "Badai Petir",
            "Medan setrum meluas dan menyambar jauh lebih sering.",
            ElemenSenjata.Listrik, PerilakuSenjata.Aura,
            22, 0.45f, 4.2f, 1, 0f);

        hasil["W03E"] = Evolusi("W03E_HujanMeriam", "W03E", "Hujan Meriam",
            "Enam roket sekaligus dengan ledakan lebih lebar.",
            ElemenSenjata.Api, PerilakuSenjata.Pelacak,
            34, 0.90f, 2.2f, 6, 10f);

        return hasil;
    }

    static SenjataSO Evolusi(string file, string id, string nama, string desk,
        ElemenSenjata elemen, PerilakuSenjata perilaku,
        int dmg, float jeda, float rad, int proyektil, float kec)
    {
        bool baru;
        SenjataSO s = Ambil<SenjataSO>(FSenjata, file, out baru);
        if (!baru) return s;

        s.id = id;
        s.namaTampil = nama;
        s.deskripsi = desk;
        s.elemen = elemen;
        s.perilaku = perilaku;
        s.levelMaks = 1;

        // Bentuk evolusi hanya punya satu tingkat, jadi tiap array cukup 1 nilai.
        s.damage          = new int[]   { dmg };
        s.jedaSerang      = new float[] { jeda };
        s.radius          = new float[] { rad };
        s.jumlahProyektil = new int[]   { proyektil };
        s.kecepatan       = new float[] { kec };

        // Penting: bentuk evolusi TIDAK boleh muncul sebagai kartu level-up biasa.
        s.bentukEvolusi = true;
        s.bobotTawaran = 0f;
        s.hasilEvolusi = null;

        EditorUtility.SetDirty(s);
        return s;
    }

    // =====================================================================
    //  SENJATA DASAR  (W01-W03, menyalin ketiga senjata yang sudah ada)
    // =====================================================================
    static void BuatSenjataDasar(Dictionary<string, PasifSO> pasif,
                                 Dictionary<string, SenjataSO> evo)
    {
        // W01 - angkanya sengaja menyamai rumus lama di SenjataManager:
        //       dmg = 3 + lvOrbit * 2  ->  5, 7, 9, 11, 13
        //       Jadi perpindahan ke ScriptableObject tidak mengubah rasa main.
        Senjata("W01_PisauBerputar", "W01", "Pisau Berputar",
            "Bilah berputar mengelilingimu dan melukai musuh yang tersentuh.",
            ElemenSenjata.Kinetik, PerilakuSenjata.Orbit,
            new int[]   { 5, 7, 9, 11, 13 },
            new float[] { 0.50f, 0.47f, 0.44f, 0.41f, 0.38f },
            new float[] { 1.6f, 1.7f, 1.8f, 1.9f, 2.0f },
            new int[]   { 2, 3, 4, 5, 6 },
            new float[] { 140f, 155f, 170f, 185f, 200f },
            Cari(evo, "W01E"), Cari(pasif, "P04"));

        Senjata("W02_AuraSetrum", "W02", "Aura Setrum",
            "Medan setrum di sekitarmu melukai musuh secara berkala.",
            ElemenSenjata.Listrik, PerilakuSenjata.Aura,
            new int[]   { 3, 4, 6, 8, 10 },
            new float[] { 1.00f, 0.90f, 0.80f, 0.70f, 0.60f },
            new float[] { 2.2f, 2.5f, 2.8f, 3.1f, 3.5f },
            new int[]   { 1 },
            new float[] { 0f },
            Cari(evo, "W02E"), Cari(pasif, "P01"));

        // W03 - jumlah roket = level senjata, sesuai deskripsi kartu yang sudah ada.
        Senjata("W03_RoketPelacak", "W03", "Roket Pelacak",
            "Roket mengejar musuh terdekat dan meledak saat kena.",
            ElemenSenjata.Api, PerilakuSenjata.Pelacak,
            new int[]   { 6, 8, 10, 13, 16 },
            new float[] { 2.00f, 1.80f, 1.60f, 1.40f, 1.20f },
            new float[] { 1.2f, 1.3f, 1.4f, 1.5f, 1.6f },
            new int[]   { 1, 2, 3, 4, 5 },
            new float[] { 8f },
            Cari(evo, "W03E"), Cari(pasif, "P03"));
    }

    static void Senjata(string file, string id, string nama, string desk,
        ElemenSenjata elemen, PerilakuSenjata perilaku,
        int[] dmg, float[] jeda, float[] rad, int[] proyektil, float[] kec,
        SenjataSO hasilEvolusi, PasifSO pasifSyarat)
    {
        bool baru;
        SenjataSO s = Ambil<SenjataSO>(FSenjata, file, out baru);
        if (!baru) return;

        s.id = id;
        s.namaTampil = nama;
        s.deskripsi = desk;
        s.elemen = elemen;
        s.perilaku = perilaku;
        s.levelMaks = Balance.LevelMaksSenjata;

        s.damage          = dmg;
        s.jedaSerang      = jeda;
        s.radius          = rad;
        s.jumlahProyektil = proyektil;
        s.kecepatan       = kec;

        // Syarat evolusi diambil dari Balance, bukan diketik ulang di sini.
        s.hasilEvolusi        = hasilEvolusi;
        s.pasifSyarat         = pasifSyarat;
        s.levelSenjataSyarat  = Balance.LevelSenjataUntukEvolusi;
        s.levelPasifSyarat    = Balance.LevelPasifUntukEvolusi;
        s.menitMinimalEvolusi = Balance.MenitMinimalEvolusi;
        s.bentukEvolusi       = false;
        s.bobotTawaran        = 1f;

        EditorUtility.SetDirty(s);
    }

    // =====================================================================
    //  MUSUH  (arketipe PRD; prefab sengaja dikosongkan)
    // =====================================================================
    static Dictionary<string, MusuhSO> BuatMusuh()
    {
        Dictionary<string, MusuhSO> hasil = new Dictionary<string, MusuhSO>();

        // Prefab TIDAK diisi otomatis. Aku tidak tahu prefab mana di
        // DungeonMonsters2D yang kamu maksud untuk tiap peran, dan menebak
        // akan membuat Perayap jadi naga. Isi sendiri lewat Inspector, lalu
        // jalankan Zomburst > Periksa Aset Balance untuk memastikan tidak ada
        // yang terlewat.

        hasil["E01"] = Musuh("E01_Perayap", "E01", "Perayap", ArketipeMusuh.Biasa,
            1, 2.0f, 10, 10, 1, 1.00f, 1, 1.0f, false);

        hasil["E02"] = Musuh("E02_Pelari", "E02", "Pelari", ArketipeMusuh.Cepat,
            1, 3.4f, 8, 15, 1, 0.85f, 2, 0.8f, false);

        hasil["E03"] = Musuh("E03_Perusak", "E03", "Perusak", ArketipeMusuh.Tank,
            6, 1.2f, 18, 30, 3, 1.40f, 4, 0.5f, false);

        hasil["E04"] = Musuh("E04_Peledak", "E04", "Peledak", ArketipeMusuh.Peledak,
            3, 2.2f, 25, 25, 2, 1.10f, 6, 0.4f, false);

        hasil["E05"] = Musuh("E05_Peludah", "E05", "Peludah", ArketipeMusuh.Penembak,
            3, 1.6f, 12, 25, 2, 1.00f, 8, 0.35f, false);

        hasil["E11"] = Musuh("E11_EliteTerinfeksi", "E11", "Elite Terinfeksi", ArketipeMusuh.Elite,
            40, 1.8f, 30, 150, 10, 2.00f, 10, 0.10f, true);

        hasil["B01"] = Musuh("B01_RajaZombi", "B01", "Raja Zombi", ArketipeMusuh.Bos,
            200, 1.3f, 40, 500, 25, 4.50f, 1, 0f, true);

        return hasil;
    }

    static MusuhSO Musuh(string file, string id, string nama, ArketipeMusuh arketipe,
        int nyawa, float kecepatan, int damageSentuh, int skor, int xp,
        float skala, int mulaiLevel, float bobot, bool peti)
    {
        bool baru;
        MusuhSO m = Ambil<MusuhSO>(FMusuh, file, out baru);
        if (!baru) return m;

        m.id = id;
        m.namaTampil = nama;
        m.arketipe = arketipe;
        m.prefab = null;
        m.nyawa = nyawa;
        m.kecepatan = kecepatan;
        m.damageSentuh = damageSentuh;
        m.skor = skor;
        m.xp = xp;
        m.skala = skala;
        m.tint = Color.white;
        m.mulaiLevel = mulaiLevel;
        m.bobot = bobot;
        m.peluangDropPermata = (arketipe == ArketipeMusuh.Bos) ? 1f : 0.5f;
        m.peluangDropBom = 0.02f;
        m.peluangDropMagnet = 0.03f;
        m.jatuhkanPeti = peti;

        EditorUtility.SetDirty(m);
        return m;
    }

    // =====================================================================
    //  STAGE  (menyalin keempat stage dari StageManager)
    // =====================================================================
    static void BuatStage(Dictionary<string, MusuhSO> musuh)
    {
        MusuhSO[] biasa = new MusuhSO[]
        {
            Cari(musuh, "E01"), Cari(musuh, "E02"), Cari(musuh, "E03"),
            Cari(musuh, "E04"), Cari(musuh, "E05"), Cari(musuh, "E11"),
        };
        MusuhSO[] bos = new MusuhSO[] { Cari(musuh, "B01") };

        // Angka diambil PERSIS dari StageManager.Daftar supaya tidak ada
        // perubahan rasa main yang tidak disengaja saat migrasi nanti.
        Stage("Stage_1-1_HutanTerkontaminasi", 1, 1, "HUTAN TERKONTAMINASI",
            "Bertahan 3 menit", 180f, 1.00f,
            new Color(0.10f, 0.13f, 0.11f), new Color(0.15f, 0.25f, 0.18f, 0.35f),
            biasa, bos);

        Stage("Stage_1-2_KotaRuntuh", 1, 2, "KOTA RUNTUH",
            "Bertahan 4 menit", 240f, 1.15f,
            new Color(0.12f, 0.12f, 0.14f), new Color(0.30f, 0.30f, 0.34f, 0.35f),
            biasa, bos);

        Stage("Stage_1-3_GurunReruntuhan", 1, 3, "GURUN RERUNTUHAN",
            "Bertahan 5 menit", 300f, 1.30f,
            new Color(0.20f, 0.16f, 0.10f), new Color(0.55f, 0.44f, 0.24f, 0.30f),
            biasa, bos);

        Stage("Stage_1-4_KutubBeku", 1, 4, "KUTUB BEKU",
            "Bertahan 6 menit", 360f, 1.50f,
            new Color(0.12f, 0.16f, 0.22f), new Color(0.60f, 0.75f, 0.90f, 0.30f),
            biasa, bos);
    }

    static void Stage(string file, int chapter, int nomor, string nama, string tagline,
        float targetDetik, float pengali, Color latar, Color kabut,
        MusuhSO[] musuhTersedia, MusuhSO[] bosTersedia)
    {
        bool baru;
        StageSO s = Ambil<StageSO>(FStage, file, out baru);
        if (!baru) return;

        s.chapter = chapter;
        s.nomorStage = nomor;
        s.namaTampil = nama;
        s.tagline = tagline;
        s.targetDetik = targetDetik;

        // StageManager lama hanya punya SATU pengali untuk semuanya.
        // Di sini dipecah tiga supaya kamu bisa membuat stage yang musuhnya
        // banyak tapi tipis, atau sedikit tapi tebal - variasi yang selama ini
        // tidak mungkin dibuat.
        s.pengaliNyawaMusuh = pengali;
        s.pengaliJumlahMusuh = pengali;
        s.pengaliKecepatanMusuh = 1f;

        s.musuhTersedia = musuhTersedia;
        s.bosTersedia = bosTersedia;
        s.jedaBosDetik = Balance.JedaBosDetik;

        s.warnaLatar = latar;
        s.warnaKabut = kabut;

        s.permataMenangPertamaKali = 50;
        s.permataMenangUlang = 10;

        EditorUtility.SetDirty(s);
    }

    // =====================================================================
    //  UTILITAS
    // =====================================================================

    static T Ambil<T>(string folder, string namaFile, out bool baru) where T : ScriptableObject
    {
        string path = folder + "/" + namaFile + ".asset";
        T ada = AssetDatabase.LoadAssetAtPath<T>(path);
        if (ada != null)
        {
            baru = false;
            dilewati++;
            return ada;
        }

        T obj = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(obj, path);
        baru = true;
        dibuat++;
        return obj;
    }

    static TVal Cari<TVal>(Dictionary<string, TVal> peta, string kunci) where TVal : class
    {
        TVal v;
        return peta.TryGetValue(kunci, out v) ? v : null;
    }

    static List<T> SemuaAset<T>(string folder) where T : ScriptableObject
    {
        List<T> hasil = new List<T>();
        if (!AssetDatabase.IsValidFolder(folder)) return hasil;

        string[] guid = AssetDatabase.FindAssets("t:" + typeof(T).Name, new string[] { folder });
        for (int i = 0; i < guid.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid[i]);
            T a = AssetDatabase.LoadAssetAtPath<T>(path);
            if (a != null) hasil.Add(a);
        }
        return hasil;
    }

    static void PastikanFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string induk = Path.GetDirectoryName(path).Replace("\\", "/");
        string nama = Path.GetFileName(path);

        if (!AssetDatabase.IsValidFolder(induk)) PastikanFolder(induk);
        AssetDatabase.CreateFolder(induk, nama);
    }
}
