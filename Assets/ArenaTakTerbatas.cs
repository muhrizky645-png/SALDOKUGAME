using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================================
//  ARENA TAK TERBATAS
//  Membuat hamparan rumput yang MENGIKUTI kamera dan BERULANG (tiled) tanpa
//  batas. Dengan begini pemain tidak pernah bisa "keluar arena" -> tidak ada
//  lagi latar biru kosong di tepi.
//
//  Caranya: satu SpriteRenderer mode Tiled dibuat sedikit lebih besar dari
//  layar, lalu tiap frame diposisikan mengikuti kamera TAPI di-"snap" ke
//  kelipatan ukuran petak. Karena petak selalu sejajar di kelipatan yang sama,
//  polanya menyambung mulus dan terasa tak terhingga.
//
//  Otomatis dibuat saat game mulai & tiap scene di-reload (pola sama seperti
//  manager lain), jadi TIDAK perlu setup apa pun di Editor.
// ============================================================================
public class ArenaTakTerbatas : MonoBehaviour
{
    public static ArenaTakTerbatas Instance;

    const float UKURAN_TILE = 8f;   // besar satu petak rumput (unit dunia)
    const int   RES = 128;          // resolusi tekstur rumput

    Camera cam;
    Transform tanah;
    SpriteRenderer sr;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("ArenaTakTerbatas", typeof(ArenaTakTerbatas));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        BuatTanah();
        SembunyikanTanahLama();
    }

    void BuatTanah()
    {
        Texture2D tex = BuatTeksturRumput(RES);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        // ppu diatur supaya satu ulangan sprite = UKURAN_TILE unit dunia
        float ppu = RES / UKURAN_TILE;
        Sprite spr = Sprite.Create(tex, new Rect(0, 0, RES, RES),
            new Vector2(0.5f, 0.5f), ppu, 0, SpriteMeshType.FullRect);

        GameObject go = new GameObject("TanahTakTerbatas");
        tanah = go.transform;
        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.tileMode = SpriteTileMode.Continuous;
        // di ATAS latar lama (-10) tapi di BAWAH semua objek gameplay (>=0)
        sr.sortingOrder = -9;
        sr.color = Color.white;
    }

    // Sembunyikan hamparan rumput lama yang berukuran tetap (objek "Paper_4").
    void SembunyikanTanahLama()
    {
        GameObject lama = GameObject.Find("Paper_4");
        if (lama != null)
        {
            SpriteRenderer s = lama.GetComponent<SpriteRenderer>();
            if (s != null) s.enabled = false;
        }
    }

    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null || tanah == null || sr == null) return;

        // ukuran area yang terlihat kamera
        float tinggi = cam.orthographicSize * 2f;
        float lebar = tinggi * cam.aspect;

        // sprite selalu dibuat lebih besar dari layar + margin 2 petak
        sr.size = new Vector2(lebar + UKURAN_TILE * 2f, tinggi + UKURAN_TILE * 2f);

        // ikuti kamera tapi SNAP ke kelipatan petak -> pola rumput menyambung mulus
        Vector3 c = cam.transform.position;
        float x = Mathf.Floor(c.x / UKURAN_TILE) * UKURAN_TILE;
        float y = Mathf.Floor(c.y / UKURAN_TILE) * UKURAN_TILE;
        tanah.position = new Vector3(x, y, 1f);
    }

    // Tekstur rumput prosedural: hijau dasar + bintik gelap/terang halus
    // (meniru tampilan tanah yang sekarang). Frekuensi tinggi biar sambungan
    // antar petak tidak kelihatan.
    static Texture2D BuatTeksturRumput(int n)
    {
        Texture2D t = new Texture2D(n, n, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave;
        Color dasar = new Color(0.31f, 0.62f, 0.30f, 1f);
        System.Random r = new System.Random(12345);
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float v = (float)(r.NextDouble() - 0.5) * 0.16f; // variasi kecil
                if (r.NextDouble() < 0.06) v -= 0.12f;            // bintik lebih gelap
                Color c = new Color(
                    Mathf.Clamp01(dasar.r + v * 0.7f),
                    Mathf.Clamp01(dasar.g + v),
                    Mathf.Clamp01(dasar.b + v * 0.7f),
                    1f);
                t.SetPixel(x, y, c);
            }
        }
        t.Apply();
        return t;
    }
}
