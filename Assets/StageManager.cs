using UnityEngine;

// =====================================================================
// ZOMBURST - DATA STAGE + PROGRES BUKA STAGE
// Static class murni (tanpa MonoBehaviour): cuma data + penyimpanan
// PlayerPrefs. Dipakai GameMenu (layar Peta) & HasilMain (win condition).
//
// Untuk sekarang tiap stage BEDA di TARGET BERTAHAN (durasi menang) dan
// pengali kesulitan ringan. Tema latar / musuh khusus per-stage bisa
// ditambah belakangan (aset alakadarnya dulu -> lantai prosedural).
// =====================================================================
public static class StageManager
{
    public class Stage
    {
        public string nama;
        public string tagline;
        public float targetDetik;   // bertahan sampai sekian detik = MENANG
        public float pengaliMusuh;  // 1.0 = normal (dipakai nanti untuk balance per-stage)

        public Stage(string nama, string tagline, float targetDetik, float pengaliMusuh)
        {
            this.nama = nama;
            this.tagline = tagline;
            this.targetDetik = targetDetik;
            this.pengaliMusuh = pengaliMusuh;
        }
    }

    // Daftar stage (urut: hutan dulu, sesuai konsep doc).
    public static readonly Stage[] Daftar = new Stage[]
    {
        new Stage("HUTAN TERKONTAMINASI", "Bertahan 3 menit", 180f, 1.0f),
        new Stage("KOTA RUNTUH",          "Bertahan 4 menit", 240f, 1.15f),
        new Stage("GURUN RERUNTUHAN",     "Bertahan 5 menit", 300f, 1.3f),
        new Stage("KUTUB BEKU",           "Bertahan 6 menit", 360f, 1.5f),
    };

    const string PP_DIPILIH = "stage_dipilih";
    const string PP_TERBUKA = "stage_terbuka"; // index tertinggi yang sudah dibuka

    public static int Jumlah { get { return Daftar.Length; } }

    static int Klem(int i) { return Mathf.Clamp(i, 0, Daftar.Length - 1); }

    // Stage yang sedang dipilih (persist supaya bertahan setelah reload scene).
    public static int Dipilih
    {
        get { return Klem(PlayerPrefs.GetInt(PP_DIPILIH, 0)); }
        set { PlayerPrefs.SetInt(PP_DIPILIH, Klem(value)); PlayerPrefs.Save(); }
    }

    // Index stage tertinggi yang terbuka (stage 0 selalu terbuka).
    public static int TerbukaSampai
    {
        get { return Klem(PlayerPrefs.GetInt(PP_TERBUKA, 0)); }
    }

    public static bool Terbuka(int i) { return i >= 0 && i <= TerbukaSampai; }

    // Buka stage ke-i (dipanggil saat menang). Simpan hanya bila lebih tinggi.
    public static void BukaSampai(int i)
    {
        i = Klem(i);
        if (i > TerbukaSampai)
        {
            PlayerPrefs.SetInt(PP_TERBUKA, i);
            PlayerPrefs.Save();
        }
    }

    public static Stage Sekarang { get { return Daftar[Dipilih]; } }
    public static float TargetSekarang { get { return Sekarang.targetDetik; } }
    public static float PengaliMusuhSekarang { get { return Sekarang.pengaliMusuh; } }

    public static bool AdaBerikutnya { get { return Dipilih < Daftar.Length - 1; } }
}
