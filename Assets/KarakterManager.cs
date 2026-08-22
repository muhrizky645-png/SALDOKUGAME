using UnityEngine;

// ====== SISTEM PILIH KARAKTER ======
// Menyimpan karakter yang dipilih pemain (permanen lewat PlayerPrefs) dan
// memuat tekstur tiap bagian karakter dari Resources/Chars/<id>/<bagian>.
// PNG bagian karakter disalin otomatis dari asset pack ke Resources oleh
// Assets/Editor/PasangIkon.cs (jalan saat load editor & sebelum build).
public static class KarakterManager
{
    // id = nama folder di asset pack & di Resources/Chars
    public static readonly string[] Id = {
        "Archer_Character_1",
        "Cave_Man_Character_2",
        "Clown_Character_3",
        "Monk_Character_4",
        "Ninja_Character_5",
        "Pirate_Character_6",
        "Soldier_Character_7",
        "Warrior_Character_8",
        "Wizard_Character_9",
    };

    // nama tampil (bahasa Indonesia) - urutannya sama dgn Id
    public static readonly string[] Nama = {
        "PEMANAH",
        "MANUSIA GUA",
        "BADUT",
        "BIKSU",
        "NINJA",
        "BAJAK LAUT",
        "TENTARA",
        "KESATRIA",
        "PENYIHIR",
    };

    // bagian rig yang ditukar spritenya
    public static readonly string[] Bagian = { "Body", "Head", "Left_Foot", "Right_Foot", "Weapon" };

    const string Kunci = "karakter_dipilih";
    const int Bawaan = 4; // Ninja (sesuai karakter default di scene)

    public static int Jumlah { get { return Id.Length; } }

    public static int Dipilih
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt(Kunci, Bawaan), 0, Id.Length - 1); }
        set
        {
            PlayerPrefs.SetInt(Kunci, Mathf.Clamp(value, 0, Id.Length - 1));
            PlayerPrefs.Save();
        }
    }

    public static void Berikutnya() { Dipilih = (Dipilih + 1) % Id.Length; }
    public static void Sebelumnya() { Dipilih = (Dipilih - 1 + Id.Length) % Id.Length; }

    // cache tekstur biar tidak load berulang
    static readonly System.Collections.Generic.Dictionary<string, Texture2D> _cache
        = new System.Collections.Generic.Dictionary<string, Texture2D>();

    public static Texture2D Tekstur(int idx, string bagian)
    {
        if (idx < 0 || idx >= Id.Length) return null;
        string path = "Chars/" + Id[idx] + "/" + bagian;
        Texture2D t;
        if (!_cache.TryGetValue(path, out t))
        {
            t = Resources.Load<Texture2D>(path);
            _cache[path] = t;
        }
        return t;
    }

    // preview di Home: pakai kepala (paling khas membedakan karakter)
    public static Texture2D Kepala(int idx) { return Tekstur(idx, "Head"); }
    public static Texture2D Badan(int idx) { return Tekstur(idx, "Body"); }
}
