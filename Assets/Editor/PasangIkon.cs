#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// =====================================================================
//  PasangIkon - otomatis menyalin ikon terpilih dari asset pack ke
//  Assets/Resources/Icons/<id>.png DAN font pixel TTF ke
//  Assets/Resources/ThaleahPixel.ttf supaya bisa dimuat runtime lewat
//  Resources.Load TANPA drag-drop manual.
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

    // id skill  ->  path sumber PNG di dalam folder Assets
    static readonly string[,] Peta = new string[,]
    {
        { "petir",   "Assets/Tiny Fantasy Icons/PowerUps/Bolt_A.png" },
        { "hati",    "Assets/Tiny Fantasy Icons/PowerUps/Heart_A.png" },
        { "berlian", "Assets/Tiny Fantasy Icons/Gems/Gems_Large_Diamond.png" },
        { "bintang", "Assets/Tiny Fantasy Icons/PowerUps/Star_A.png" },
        { "bom",     "Assets/Tiny Fantasy Icons/Explosives/Boom_A.png" },
        { "roket",   "Assets/Tiny Fantasy Icons/Explosives/Dinamite_A.png" },
        { "target",  "Assets/Tiny Fantasy Icons/Time/Compas_A.png" },
        { "peluru",  "Assets/Jovial Games/Simple 2D Cute Characters/Characters/Soldier_Character_7/Weapon.png" },
        { "pisau",   "Assets/Jovial Games/Simple 2D Cute Characters/Characters/Ninja_Character_5/Weapon.png" },
        { "aura",    "Assets/Jovial Games/Simple 2D Cute Characters/Characters/Wizard_Character_9/Weapon.png" },
    };

    const string FolderRes = "Assets/Resources";
    const string FolderIkon = "Assets/Resources/Icons";

    // ====== FONT PIXEL (TTF dinamis, bisa diskalakan ke semua ukuran) ======
    // Sumber TTF ada di dalam asset pack Thaleah. Disalin ke Resources supaya
    // Tema.FontUtama bisa memuatnya via Resources.Load<Font>("ThaleahPixel").
    const string SumberFontTTF = "Assets/Thaleah_PixelFont/Materials/ThaleahFat_TTF.ttf";
    const string FontRes = "Assets/Resources/ThaleahPixel.ttf";
    // font bitmap lama (.fontsettings) yang GAGAL di-load & tidak bisa diskalakan -> dibuang
    const string FontBitmapLama = "Assets/Resources/ThaleahFat.fontsettings";

    static PasangIkon()
    {
        // tunda sampai AssetDatabase siap
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
        Debug.Log("[PasangIkon] Selesai memasang ikon ke " + FolderIkon + " & font ke " + FontRes);
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

        if (berubah)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    // Salin font TTF ke Resources & pastikan bisa dimuat + tajam ala pixel.
    static bool PasangFont(bool paksaTimpa)
    {
        // buang font bitmap lama yang bikin error "Unable to load font face" (jika ada)
        if (File.Exists(FontBitmapLama))
            AssetDatabase.DeleteAsset(FontBitmapLama);

        bool adaDst = File.Exists(FontRes);
        if (adaDst && !paksaTimpa) return false; // sudah terpasang

        if (!File.Exists(SumberFontTTF))
        {
            Debug.LogWarning("[PasangIkon] Font TTF tidak ditemukan, dilewati: " + SumberFontTTF);
            return false;
        }

        if (adaDst) AssetDatabase.DeleteAsset(FontRes); // CopyAsset tidak menimpa -> hapus dulu
        if (AssetDatabase.CopyAsset(SumberFontTTF, FontRes))
        {
            AturFont(FontRes);
            return true;
        }

        Debug.LogWarning("[PasangIkon] Gagal menyalin font: " + SumberFontTTF + " -> " + FontRes);
        return false;
    }

    // Atur import TTF: dinamis (bisa skala) + sertakan data font + tajam (tanpa blur).
    static void AturFont(string path)
    {
        TrueTypeFontImporter fi = AssetImporter.GetAtPath(path) as TrueTypeFontImporter;
        if (fi == null) return;
        fi.fontTextureCase = FontTextureCase.Dynamic;    // dinamis -> ikut fontSize di semua ukuran
        fi.includeFontData = true;                       // WAJIB agar face ter-load saat runtime
        fi.fontRenderMode = FontRenderMode.HintedRaster; // tajam ala pixel-art (tanpa anti-alias blur)
        fi.SaveAndReimport();
    }

    // Atur import PNG supaya cocok dipakai sebagai Texture2D (Resources.Load) & tajam ala pixel-art
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
