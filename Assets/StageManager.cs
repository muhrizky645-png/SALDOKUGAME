using UnityEngine;

// =====================================================================
// ZOMBURST - DATA STAGE + PROGRES BUKA STAGE
// Static class murni (tanpa MonoBehaviour): cuma data + penyimpanan
// PlayerPrefs. Dipakai GameMenu (layar Peta) & HasilMain (win condition).
//
// PERUBAHAN PENTING - durasi run kini 15 MENIT untuk semua stage.
//
// Dulu tiap stage dibedakan oleh DURASI (3/4/5/6 menit). Itu bermasalah
// begitu jadwal bos masuk: bos tiap 5 menit berarti stage 1 dan 2 tidak
// akan pernah kedatangan bos sama sekali, dan musuh Peledak yang terbuka
// di menit ke-6 tidak akan pernah muncul di stage mana pun.
//
// Sekarang semua stage berdurasi sama (Balance.DurasiRunDetik) dan yang
// membedakannya adalah PENGALI KESULITAN - persis seperti ChapterMultiplier
// di PRD. Ini juga lebih jujur: stage yang lebih sulit seharusnya terasa
// lebih berat, bukan sekadar lebih lama.
// =====================================================================
public static class StageManager
{
    public class Stage
    {
        public string nama;
        public string tagline;
        public float targetDetik;   // bertahan sampai sekian detik = MENANG
        public float pengaliMusuh;  // 1.0 = normal

        public Stage(string nama, string tagline, float targetDetik, float pengaliMusuh)
        {
            this.nama = nama;
            this.tagline = tagline;
            this.targetDetik = targetDetik;
            this.pengaliMusuh = pengaliMusuh;
        }
    }

    // Daftar stage (urut: hutan dulu, sesuai konsep doc).
    // Durasi diambil dari Balance supaya cukup diubah di SATU tempat.
    public static readonly Stage[] Daftar = new Stage[]
    {
        new Stage("HUTAN TERKONTAMINASI", "Tingkat I - 3 bos",   Balance.DurasiRunDetik, 1.0f),
        new Stage("KOTA RUNTUH",          "Tingkat II - 3 bos",  Balance.DurasiRunDetik, 1.15f),
        new Stage("GURUN RERUNTUHAN",     "Tingkat III - 3 bos", Balance.DurasiRunDetik, 1.3f),
        new Stage("KUTUB BEKU",           "Tingkat IV - 3 bos",  Balance.DurasiRunDetik, 1.5f),
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
