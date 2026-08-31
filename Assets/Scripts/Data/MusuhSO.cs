using UnityEngine;

// Arketipe musuh sesuai PRD (E01-E12).
public enum ArketipeMusuh
{
    Biasa,      // pengejar lurus
    Cepat,      // lincah, nyawa tipis
    Tank,       // lambat, nyawa tebal
    Peledak,    // meledak saat mati
    Penembak,   // menembak dari jarak jauh
    Perisai,    // kebal dari depan
    Penyembuh,  // memulihkan musuh sekitar
    Pemanggil,  // memanggil musuh kecil
    Terbang,    // menembus rintangan
    Penggali,   // muncul dari tanah dekat pemain
    Elite,      // versi kuat, menjatuhkan peti
    Bos
}

// ============================================================================
// MusuhSO
// ----------------------------------------------------------------------------
// Data satu jenis musuh. Menggantikan class ZombieSpawner.MusuhTier yang
// tertanam di dalam komponen, sehingga daftar musuh tidak bisa dipakai ulang
// antar stage dan hilang kalau prefab spawner diganti.
//
// CARA BIKIN ASETNYA:
// Klik kanan di Project window -> Create -> Zomburst -> Musuh
// ============================================================================
[CreateAssetMenu(fileName = "Musuh_Baru", menuName = "Zomburst/Musuh", order = 2)]
public class MusuhSO : ScriptableObject
{
    [Header("Identitas")]
    [Tooltip("Kode unik sesuai PRD, misal E01.")]
    public string id = "E01";
    public string namaTampil = "Musuh Baru";
    public GameObject prefab;
    public ArketipeMusuh arketipe = ArketipeMusuh.Biasa;

    [Header("Stat dasar (sebelum pengali stage)")]
    public int nyawa = 1;
    public float kecepatan = 2f;
    public int damageSentuh = 10;
    public int skor = 10;
    public int xp = 1;

    [Header("Tampilan")]
    [Tooltip("Pengali ukuran relatif terhadap 'ukuranMusuh' di spawner.")]
    public float skala = 1f;
    public Color tint = Color.white;

    [Header("Kemunculan")]
    [Tooltip("Musuh ini baru muncul mulai level pemain sekian.")]
    public int mulaiLevel = 1;
    [Tooltip("Bobot kemunculan relatif terhadap musuh lain yang sudah terbuka.")]
    public float bobot = 1f;

    [Header("Jatuhan")]
    [Range(0f, 1f)] public float peluangDropPermata = 0.5f;
    [Range(0f, 1f)] public float peluangDropBom = 0.02f;
    [Range(0f, 1f)] public float peluangDropMagnet = 0.03f;
    [Tooltip("Elite dan bos menjatuhkan peti yang membuka evolusi senjata.")]
    public bool jatuhkanPeti = false;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id)) id = name;
        if (nyawa < 1) nyawa = 1;
        if (mulaiLevel < 1) mulaiLevel = 1;
    }
#endif
}
