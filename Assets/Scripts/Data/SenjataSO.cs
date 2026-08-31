using UnityEngine;

public enum ElemenSenjata { Netral, Api, Listrik, Es, Racun, Kinetik }

// Cara senjata mencari sasaran / berperilaku.
public enum PerilakuSenjata
{
    Orbit,      // bilah berputar mengelilingi pemain (Pisau Berputar)
    Aura,       // kerusakan berkala di sekitar pemain (Aura Setrum)
    Pelacak,    // proyektil mengejar musuh terdekat (Roket)
    Lurus,      // proyektil lurus ke arah hadap
    AreaJatuh,  // serangan jatuh di posisi musuh
    Balok       // sinar menembus
}

// ============================================================================
// SenjataSO
// ----------------------------------------------------------------------------
// Data satu senjata, TERPISAH dari kode.
//
// KENAPA ADA:
// Sekarang angka balancing tertanam di dalam C#, contohnya di SenjataManager:
//     int dmg = 3 + lvOrbit * 2 + (evo ? 5 : 0);
// Artinya mengubah damage = mengedit kode = compile ulang = build ulang.
// Menambah senjata ke-4 berarti menyalin blok if baru. Tidak mungkin sampai
// 20 senjata seperti target PRD.
//
// Dengan ScriptableObject, tiap senjata jadi satu file aset yang bisa diatur
// dari Inspector, dibandingkan berdampingan, dan diubah tanpa menyentuh kode.
//
// CARA BIKIN ASETNYA:
// Klik kanan di Project window -> Create -> Zomburst -> Senjata
// ============================================================================
[CreateAssetMenu(fileName = "Senjata_Baru", menuName = "Zomburst/Senjata", order = 0)]
public class SenjataSO : ScriptableObject
{
    [Header("Identitas")]
    [Tooltip("Kode unik sesuai PRD, misal W01. Dipakai untuk save & analytics.")]
    public string id = "W01";
    public string namaTampil = "Senjata Baru";
    [TextArea(2, 4)] public string deskripsi = "";
    public Sprite ikon;
    public ElemenSenjata elemen = ElemenSenjata.Netral;
    public PerilakuSenjata perilaku = PerilakuSenjata.Orbit;

    [Header("Level")]
    [Range(1, 5)] public int levelMaks = 5;

    // --- Kurva per level. Index 0 = Lv.1, index 4 = Lv.5 ---
    // Kalau array lebih pendek dari level yang diminta, nilai terakhir dipakai.
    // Jadi kamu boleh isi 1 nilai saja kalau stat itu tidak naik per level.

    [Header("Kurva per level (index 0 = Lv.1)")]
    public int[] damage = new int[] { 5, 7, 9, 11, 13 };

    [Tooltip("Detik antar serangan. Makin kecil makin cepat.")]
    public float[] jedaSerang = new float[] { 2f, 1.8f, 1.6f, 1.4f, 1.2f };

    [Tooltip("Radius efek / jarak orbit, dalam satuan dunia.")]
    public float[] radius = new float[] { 1.6f, 1.7f, 1.8f, 1.9f, 2.0f };

    [Tooltip("Berapa bilah / proyektil per serangan.")]
    public int[] jumlahProyektil = new int[] { 2, 3, 4, 5, 6 };

    [Tooltip("Kecepatan proyektil atau kecepatan putar orbit (derajat/detik).")]
    public float[] kecepatan = new float[] { 140f };

    [Header("Evolusi")]
    [Tooltip("Senjata hasil evolusi. Kosongkan kalau senjata ini tidak bisa berevolusi.")]
    public SenjataSO hasilEvolusi;

    [Tooltip("Pasif yang wajib dimiliki agar evolusi terbuka.")]
    public PasifSO pasifSyarat;

    [Range(1, 5)] public int levelSenjataSyarat = 5;
    [Range(1, 5)] public int levelPasifSyarat = 3;

    [Tooltip("Evolusi baru boleh muncul setelah menit ke-berapa.")]
    public float menitMinimalEvolusi = 5f;

    [Tooltip("Kalau true, senjata ini adalah BENTUK EVOLUSI dan tidak akan " +
             "pernah ditawarkan sebagai kartu level-up biasa.")]
    public bool bentukEvolusi = false;

    [Header("Penawaran kartu")]
    [Tooltip("Bobot kemunculan di kartu level-up. Makin besar makin sering.")]
    public float bobotTawaran = 1f;

    // ----------------------------------------------------------- pembacaan

    public int Damage(int level) { return Ambil(damage, level); }
    public float JedaSerang(int level) { return Ambil(jedaSerang, level); }
    public float Radius(int level) { return Ambil(radius, level); }
    public int JumlahProyektil(int level) { return Ambil(jumlahProyektil, level); }
    public float Kecepatan(int level) { return Ambil(kecepatan, level); }

    // Apakah senjata ini siap berevolusi?
    public bool BisaEvolusi(int levelSenjata, int levelPasifDimiliki, float detikBerjalan)
    {
        if (hasilEvolusi == null) return false;
        if (levelSenjata < levelSenjataSyarat) return false;
        if (pasifSyarat != null && levelPasifDimiliki < levelPasifSyarat) return false;
        if (detikBerjalan < menitMinimalEvolusi * 60f) return false;
        return true;
    }

    static int Ambil(int[] a, int level)
    {
        if (a == null || a.Length == 0) return 0;
        return a[Mathf.Clamp(level - 1, 0, a.Length - 1)];
    }

    static float Ambil(float[] a, int level)
    {
        if (a == null || a.Length == 0) return 0f;
        return a[Mathf.Clamp(level - 1, 0, a.Length - 1)];
    }

#if UNITY_EDITOR
    // Peringatan dini di Inspector kalau data belum masuk akal.
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id)) id = name;
        if (levelMaks < 1) levelMaks = 1;
        if (hasilEvolusi == this) hasilEvolusi = null; // cegah evolusi ke diri sendiri
    }
#endif
}
