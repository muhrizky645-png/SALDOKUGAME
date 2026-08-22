using UnityEngine;
using UnityEngine.SceneManagement;

// Menempel otomatis ke Player. Mengganti sprite rig karakter (Body/Head/kaki/senjata)
// sesuai pilihan pemain di Home. Sprite dibuat ulang saat runtime dari tekstur di
// Resources, dengan MEMPERTAHANKAN pivot & pixels-per-unit rig asli supaya posisi
// tiap bagian tetap pas (semua karakter di pack ini memakai rig yang sama).
public class KarakterPemain : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Pasang();
        SceneManager.sceneLoaded += (s, m) => Pasang();
    }

    static void Pasang()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p == null) return;
        if (p.GetComponent<KarakterPemain>() == null)
            p.AddComponent<KarakterPemain>();
    }

    // dipanggil GameMenu saat mulai main (tanpa reload scene) supaya pilihan
    // terbaru langsung dipakai.
    public static void TerapkanPilihan()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p == null) return;
        KarakterPemain kp = p.GetComponent<KarakterPemain>();
        if (kp == null) kp = p.AddComponent<KarakterPemain>();
        kp.Terapkan(KarakterManager.Dipilih);
    }

    void Start()
    {
        Terapkan(KarakterManager.Dipilih);
    }

    public void Terapkan(int idx)
    {
        // cari rig visual: anak yang namanya mengandung "_Character_"
        Transform rig = null;
        foreach (Transform anak in transform)
        {
            if (anak.name.Contains("_Character_")) { rig = anak; break; }
        }
        if (rig == null) return;

        SpriteRenderer[] semua = rig.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in semua)
        {
            string bagian = CocokBagian(sr.gameObject.name);
            if (bagian == null) continue;
            Texture2D tex = KarakterManager.Tekstur(idx, bagian);
            if (tex == null) continue;
            GantiSprite(sr, tex);
        }
    }

    static string CocokBagian(string nama)
    {
        string l = nama.ToLower();
        if (l.Contains("body")) return "Body";
        if (l.Contains("head")) return "Head";
        if (l.Contains("left") && l.Contains("foot")) return "Left_Foot";
        if (l.Contains("right") && l.Contains("foot")) return "Right_Foot";
        if (l.Contains("weapon")) return "Weapon";
        return null;
    }

    // Buat sprite baru dari tekstur, pertahankan pivot & PPU sprite lama biar posisinya pas.
    static void GantiSprite(SpriteRenderer sr, Texture2D tex)
    {
        Sprite lama = sr.sprite;
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        float ppu = 100f;
        if (lama != null)
        {
            if (lama.rect.width > 0f && lama.rect.height > 0f)
                pivot = new Vector2(lama.pivot.x / lama.rect.width, lama.pivot.y / lama.rect.height);
            ppu = lama.pixelsPerUnit;
        }
        Sprite baru = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), pivot, ppu);
        sr.sprite = baru;
    }
}
