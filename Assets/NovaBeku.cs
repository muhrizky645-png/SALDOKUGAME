using UnityEngine;

// SENJATA: Nova Beku.
// Meledakkan cincin es di sekitar pemain: memberi damage sekali ke musuh dalam
// radius DAN MEMPERLAMBAT semua musuh sebentar (EnemyChase.Perlambat). L1 kecil
// & perlambatannya ringan; naik tiap level; evolusi (lvl 5+): ungu, damage &
// perlambatan melonjak + menyambar petir ke beberapa musuh.
public class NovaBeku : MonoBehaviour
{
    float radius = 1.6f;
    float t = 0f;
    float durasi = 0.4f;
    SpriteRenderer sr;
    Color warna;

    public static void Ledak(Vector3 pos, float radius, int dmg, float slowDurasi, float slowFaktor, bool evo)
    {
        // ---- damage musuh dalam radius ----
        int n = EnemyRegistry.DalamRadius(pos, radius, EnemyRegistry.Buffer);
        Color ungu = new Color(0.8f, 0.5f, 1f, 1f);
        int petir = 0;
        for (int i = 0; i < n; i++)
        {
            EnemyChase ec = EnemyRegistry.Buffer[i];
            if (ec == null || ec.SudahMati) continue;
            ec.KenaSerangan(dmg, false);
            if (evo && petir < 3) { PetirEfek.Sambar(pos, ec.transform.position, ungu, 0.10f); petir++; }
        }

        // ---- perlambat SEMUA musuh sementara (efek beku) ----
        EnemyChase.Perlambat(slowDurasi, slowFaktor);

        // ---- suara + visual cincin ----
        SoundManager.AuraZap();

        GameObject go = new GameObject("NovaBeku");
        go.transform.position = pos;
        NovaBeku nb = go.AddComponent<NovaBeku>();
        nb.radius = radius;
        nb.warna = evo ? new Color(0.72f, 0.45f, 1f, 0.9f) : new Color(0.55f, 0.85f, 1f, 0.9f);
    }

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatCincin(64);
        sr.color = warna;
        sr.sortingOrder = 54;
        transform.localScale = Vector3.one * 0.2f;
    }

    void Update()
    {
        t += Time.deltaTime;
        float p = Mathf.Clamp01(t / durasi);
        float sMul = (2f * radius) / 0.64f;
        transform.localScale = Vector3.one * Mathf.Lerp(sMul * 0.15f, sMul, p);
        Color c = sr.color; c.a = Mathf.Lerp(warna.a, 0f, p); sr.color = c;
        if (p >= 1f) Destroy(gameObject);
    }

    static Sprite BuatCincin(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                float a = (d >= 0.72f && d <= 1f) ? 1f : 0f; // cincin tepi
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
