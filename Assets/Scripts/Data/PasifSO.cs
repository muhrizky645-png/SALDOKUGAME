using UnityEngine;

// Stat apa yang diubah oleh pasif ini.
public enum EfekPasif
{
    KecepatanTembak,   // fireRate turun (lebih cepat)
    JumlahPeluru,      // +N peluru per tembakan
    Jangkauan,         // range naik
    KecepatanGerak,    // moveSpeed naik
    MaxHP,             // maxHealth naik
    Magnet,            // radius tarik XP/permata naik
    Damage,            // damage global naik
    PeluangKritis,     // peluang kritis naik
    DamageKritis,      // pengali kritis naik
    Armor,             // kerusakan diterima turun
    RegenHP,           // HP pulih per detik
    Cooldown           // semua jeda senjata turun
}

// ============================================================================
// PasifSO
// ----------------------------------------------------------------------------
// Data satu skill pasif. Sama alasannya dengan SenjataSO: sekarang keenam
// pasif tertanam sebagai case di dalam switch besar di SkillManager, jadi
// menambah pasif ke-7 berarti mengedit kode. PRD menargetkan 22 pasif.
//
// CARA BIKIN ASETNYA:
// Klik kanan di Project window -> Create -> Zomburst -> Pasif
// ============================================================================
[CreateAssetMenu(fileName = "Pasif_Baru", menuName = "Zomburst/Pasif", order = 1)]
public class PasifSO : ScriptableObject
{
    [Header("Identitas")]
    [Tooltip("Kode unik sesuai PRD, misal P01.")]
    public string id = "P01";
    public string namaTampil = "Pasif Baru";
    [TextArea(2, 4)] public string deskripsi = "";
    public Sprite ikon;

    [Header("Efek")]
    public EfekPasif efek = EfekPasif.Damage;

    [Range(1, 5)] public int levelMaks = 5;

    [Tooltip("Nilai KUMULATIF di tiap level. Index 0 = Lv.1.\n" +
             "Kalau 'persentase' dicentang, 0.15 berarti +15%.")]
    public float[] nilaiPerLevel = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

    [Tooltip("Centang kalau nilai di atas adalah persentase, bukan angka mentah.")]
    public bool persentase = true;

    [Header("Penawaran kartu")]
    public float bobotTawaran = 1f;

    public float Nilai(int level)
    {
        if (nilaiPerLevel == null || nilaiPerLevel.Length == 0) return 0f;
        return nilaiPerLevel[Mathf.Clamp(level - 1, 0, nilaiPerLevel.Length - 1)];
    }

    // Teks siap tampil untuk kartu level-up, misal "+15%" atau "+30".
    public string TeksNilai(int level)
    {
        float v = Nilai(level);
        if (persentase) return (v >= 0f ? "+" : "") + Mathf.RoundToInt(v * 100f) + "%";
        return (v >= 0f ? "+" : "") + v.ToString("0.##");
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id)) id = name;
        if (levelMaks < 1) levelMaks = 1;
    }
#endif
}
