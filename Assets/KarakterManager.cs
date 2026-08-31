using UnityEngine;

// ====== SISTEM PILIH KARAKTER ======
// Menyimpan karakter yang dipilih pemain (permanen lewat PlayerPrefs) dan
// memuat tekstur tiap bagian karakter dari Resources/Chars/<id>/<bagian>.
// PNG bagian karakter disalin otomatis dari asset pack ke Resources oleh
// Assets/Editor/PasangIkon.cs (jalan saat load editor & sebelum build).
//
// KUNCI KARAKTER: hanya NINJA (index 4) yang terbuka di awal. Karakter lain
// terkunci dan dibuka satu per satu dengan menonton iklan (rewarded ad).
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
    public const int NinjaIndex = 4; // Ninja = karakter terbuka pertama (default)

    public static int Jumlah { get { return Id.Length; } }

    public static int Dipilih
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt(Kunci, NinjaIndex), 0, Id.Length - 1); }
        set
        {
            PlayerPrefs.SetInt(Kunci, Mathf.Clamp(value, 0, Id.Length - 1));
            PlayerPrefs.Save();
        }
    }

    public static void Berikutnya() { Dipilih = (Dipilih + 1) % Id.Length; }
    public static void Sebelumnya() { Dipilih = (Dipilih - 1 + Id.Length) % Id.Length; }

    // ====== KUNCI / BUKA KARAKTER ======
    static string KunciBuka(int idx) { return "karakter_buka_" + idx; }

    // Ninja selalu terbuka; lainnya terbuka bila sudah dibuka (nonton iklan).
    public static bool Terbuka(int idx)
    {
        if (idx == NinjaIndex) return true;
        return PlayerPrefs.GetInt(KunciBuka(idx), 0) == 1;
    }

    public static void Buka(int idx)
    {
        if (idx < 0 || idx >= Id.Length) return;
        PlayerPrefs.SetInt(KunciBuka(idx), 1);
        PlayerPrefs.Save();
    }

    public static int JumlahTerbuka
    {
        get { int n = 0; for (int i = 0; i < Id.Length; i++) if (Terbuka(i)) n++; return n; }
    }

    // cache tekstur biar tidak load berulang
    static readonly System.Collections.Generic.Dictionary<string, Texture2D> _cache
        = new System.Collections.Generic.Dictionary<string, Texture2D>();

    // ===== FIX: reset cache tiap masuk Play Mode =====
    // Kalau "Reload Domain" dimatikan di Enter Play Mode Settings, variabel
    // static tetap hidup antar sesi Play, PADAHAL tekstur Resources-nya sudah
    // di-unload saat Stop. Tanpa reset ini, Play kedua memakai referensi
    // tekstur yang sudah mati -> sprite kosong -> karakter hilang.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetCache() { _cache.Clear(); }

    public static Texture2D Tekstur(int idx, string bagian)
    {
        if (idx < 0 || idx >= Id.Length) return null;
        string path = "Chars/" + Id[idx] + "/" + bagian;
        Texture2D t;
        // FIX: load ulang kalau belum ada di cache ATAU teksturnya sudah mati (null).
        if (!_cache.TryGetValue(path, out t) || t == null)
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
