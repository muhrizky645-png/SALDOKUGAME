#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// =====================================================================
//  PasangIkon - otomatis menyalin ikon terpilih dari asset pack ke
//  Assets/Resources/Icons/<id>.png, font pixel TTF ke
//  Assets/Resources/ThaleahPixel.ttf, DAN part tiap karakter ke
//  Assets/Resources/Chars/<karakter>/<bagian>.png supaya semua bisa
//  dimuat runtime lewat Resources.Load TANPA drag-drop manual.
//
//  Jalan OTOMATIS:
//   1) saat Editor selesai load (InitializeOnLoad) -> hanya menyalin yang belum ada
//   2) sebelum build, TERMASUK Unity Cloud Build (IPreprocessBuildWithReport)
//   3) manual lewat menu: Tools > Pasang Ikon Fantasy
//
//  Karena PNG & TTF sudah ada di dalam repo, penyalinan cukup lewat
//  AssetDatabase (tanpa transfer file biner apa pun).
// =====================================================================
[InitializeOnLoad]
public class PasangIkon : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    // id skill/item  ->  path sumber PNG di dalam folder Assets
    static readonly string[,] Peta = new string[,]
    {
        { "petir",    "Assets/Tiny Fantasy Icons/PowerUps/Bolt_A.png" },
        { "hati",     "Assets/Tiny Fantasy Icons/PowerUps/Heart_A.png" },
        { "berlian",  "Assets/Tiny Fantasy Icons/Gems/Gems_Large_Diamond.png" },
        { "bintang",  "Assets/Tiny Fantasy Icons/PowerUps/Star_A.png" },
        { "bom",      "Assets/Tiny Fantasy Icons/Explosives/Boom_A.png" },
        { "roket",    "Assets/Tiny Fantasy Icons/Explosives/Dinamite_A.png" },
        { "target",   "Assets/Tiny Fantasy Icons/Time/Compas_A.png" },
        { "peti",     "Assets/Tiny Fantasy Icons/Presents/Present_A.png" },
        { "petidewa", "Assets/Tiny Fantasy Icons/Chests/ChestA.png" },
        { "peluru",   "Assets/Jovial Games/Simple 2D Cute Characters/Characters/Soldier_Character_7/Weapon.png" },
        { "pisau",    "Assets/Jovial Games/Simple 2D Cute Characters/Characters/Ninja_Character_5/Weapon.png" },
        { "aura",     "Assets/Jovial Games/Simple 2D Cute Characters/Characters/Wizard_Character_9/Weapon.png" },
    };

    const string FolderRes = "Assets/Resources";
    const string FolderIkon = "Assets/Resources/Icons";

    // ====== KARAKTER (untuk fitur pilih karakter di Home) ======
    const string FolderChars = "Assets/Resources/Chars";
    const string SumberKarakter = "Assets/Jovial Games/Simple 2D Cute Characters/Characters";
    static readonly string[] KarakterFolder = {
        "Archer_Character_1", "Cave_Man_Character_2", "Clown_Character_3", "Monk_Character_4",
        "Ninja_Character_5", "Pirate_Character_6", "Soldier_Character_7", "Warrior_Character_8", "Wizard_Character_9",
    };
    static readonly string[] KarakterBagian = { "Body", "Head", "Left_Foot", "Right_Foot", "Weapon" };

    // ====== FONT PIXEL (TTF dinamis, bisa diskalakan ke semua ukuran) ======
    const string SumberFontTTF = "Assets/Thaleah_PixelFont/Materials/ThaleahFat_TTF.ttf";
    const string FontRes = "Assets/Resources/ThaleahPixel.ttf";
    const string FontBitmapLama = "Assets/Resources/ThaleahFat.fontsettings";

    static PasangIkon()
    {
        EditorApplication.delayCall += () => Jalan(false);
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Jalan(true); // paksa segarkan sebelum build (termasuk Cloud Build)
    }

    [MenuItem("Tools/Pasang Ikon Fantasy")]
    static void JalanManual()
    {
        Jalan(true);
        Debug.Log("[PasangIkon] Selesai memasang ikon, font & karakter ke " + FolderRes);
    }

    static void Jalan(bool paksaTimpa)
    {
        if (!AssetDatabase.IsValidFolder(FolderRes))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(FolderIkon))
            AssetDatabase.CreateFolder(FolderRes, "Icons");

        bool berubah = false;
        int n = Peta.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            string id = Peta[i, 0];
            string src = Peta[i, 1];
            string dst = FolderIkon + "/" + id + ".png";

            bool adaDst = File.Exists(dst);
            if (adaDst && !paksaTimpa) continue;

            if (!File.Exists(src))
            {
                Debug.LogWarning("[PasangIkon] Sumber tidak ditemukan, dilewati: " + src);
                continue;
            }

            if (adaDst) AssetDatabase.DeleteAsset(dst); // CopyAsset tidak menimpa -> hapus dulu
            if (AssetDatabase.CopyAsset(src, dst))
            {
                AturImport(dst);
                berubah = true;
            }
            else
            {
                Debug.LogWarning("[PasangIkon] Gagal menyalin: " + src + " -> " + dst);
            }
        }

        if (PasangFont(paksaTimpa)) berubah = true;
        if (PasangKarakter(paksaTimpa)) berubah = true;

        if (berubah)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    // Salin part tiap karakter -> Resources/Chars/<karakter>/<bagian>.png
    static bool PasangKarakter(bool paksaTimpa)
    {
        if (!AssetDatabase.IsValidFolder(FolderChars))
            AssetDatabase.CreateFolder(FolderRes, "Chars");

        bool berubah = false;
        foreach (string ch in KarakterFolder)
        {
            string folderTujuan = FolderChars + "/" + ch;
            if (!AssetDatabase.IsValidFolder(folderTujuan))
                AssetDatabase.CreateFolder(FolderChars, ch);

            foreach (string bg in KarakterBagian)
            {
                string src = SumberKarakter + "/" + ch + "/" + bg + ".png";
                string dst = folderTujuan + "/" + bg + ".png";

                bool adaDst = File.Exists(dst);
                if (adaDst && !paksaTimpa) continue;

                if (!File.Exists(src))
                {
                    Debug.LogWarning("[PasangIkon] Sumber karakter tidak ada, dilewati: " + src);
                    continue;
                }

                if (adaDst) AssetDatabase.DeleteAsset(dst);
                if (AssetDatabase.CopyAsset(src, dst))
                {
                    AturImport(dst);
                    berubah = true;
                }
                else
                {
                    Debug.LogWarning("[PasangIkon] Gagal menyalin karakter: " + src + " -> " + dst);
                }
            }
        }
        return berubah;
    }

    static bool PasangFont(bool paksaTimpa)
    {
        if (File.Exists(FontBitmapLama))
            AssetDatabase.DeleteAsset(FontBitmapLama);

        bool adaDst = File.Exists(FontRes);
        if (adaDst && !paksaTimpa) return false;

        if (!File.Exists(SumberFontTTF))
        {
            Debug.LogWarning("[PasangIkon] Font TTF tidak ditemukan, dilewati: " + SumberFontTTF);
            return false;
        }

        if (adaDst) AssetDatabase.DeleteAsset(FontRes);
        if (AssetDatabase.CopyAsset(SumberFontTTF, FontRes))
        {
            AturFont(FontRes);
            return true;
        }

        Debug.LogWarning("[PasangIkon] Gagal menyalin font: " + SumberFontTTF + " -> " + FontRes);
        return false;
    }

    static void AturFont(string path)
    {
        TrueTypeFontImporter fi = AssetImporter.GetAtPath(path) as TrueTypeFontImporter;
        if (fi == null) return;
        fi.fontTextureCase = FontTextureCase.Dynamic;
        fi.includeFontData = true;
        fi.SaveAndReimport();
    }

    static void AturImport(string path)
    {
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) return;
        ti.textureType = TextureImporterType.Default; // WAJIB Default agar Resources.Load<Texture2D> mengembalikan tekstur
        ti.filterMode = FilterMode.Point;             // tajam (pixel-art)
        ti.alphaIsTransparency = true;
        ti.mipmapEnabled = false;
        ti.wrapMode = TextureWrapMode.Clamp;
        ti.SaveAndReimport();
    }
}
#endif
