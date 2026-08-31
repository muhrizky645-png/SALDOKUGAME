using UnityEngine;

// ============================================================================
// StageSO
// ----------------------------------------------------------------------------
// Data satu stage. StageManager saat ini menyimpan 4 stage sebagai array
// hardcoded di dalam kode, dan stage hanya berbeda pada DURASI dan satu angka
// pengali. PRD menargetkan 10 chapter x 10 stage dengan komposisi musuh,
// bos, dan tema yang berbeda-beda.
//
// CARA BIKIN ASETNYA:
// Klik kanan di Project window -> Create -> Zomburst -> Stage
// ============================================================================
[CreateAssetMenu(fileName = "Stage_Baru", menuName = "Zomburst/Stage", order = 3)]
public class StageSO : ScriptableObject
{
    [Header("Identitas")]
    public int chapter = 1;
    public int nomorStage = 1;
    public string namaTampil = "HUTAN TERKONTAMINASI";
    [TextArea(1, 2)] public string tagline = "Bertahanlah sampai fajar.";

    [Header("Durasi")]
    [Tooltip("Berapa detik pemain harus bertahan untuk menang.")]
    public float targetDetik = 300f;

    [Header("Kesulitan")]
    [Tooltip("Pengali nyawa semua musuh di stage ini.")]
    public float pengaliNyawaMusuh = 1f;
    [Tooltip("Pengali batas jumlah musuh hidup bersamaan.")]
    public float pengaliJumlahMusuh = 1f;
    [Tooltip("Pengali kecepatan gerak musuh.")]
    public float pengaliKecepatanMusuh = 1f;

    [Header("Isi stage")]
    public MusuhSO[] musuhTersedia;
    public MusuhSO[] bosTersedia;
    [Tooltip("Jeda kemunculan bos dalam detik.")]
    public float jedaBosDetik = 45f;

    [Header("Tema visual")]
    public Color warnaLatar = new Color(0.10f, 0.13f, 0.11f);
    public Color warnaKabut = new Color(0.15f, 0.25f, 0.18f, 0.35f);
    public Sprite tileLatar;

    [Header("Hadiah")]
    public int permataMenangPertamaKali = 50;
    public int permataMenangUlang = 10;

    public string KodeStage { get { return chapter + "-" + nomorStage; } }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (targetDetik < 30f) targetDetik = 30f;
        if (pengaliNyawaMusuh < 0.1f) pengaliNyawaMusuh = 0.1f;
    }
#endif
}
