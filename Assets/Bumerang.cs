using UnityEngine;
using System.Collections.Generic;

// SENJATA: Bumerang.
// Dilempar ke arah musuh terdekat, MELESAT keluar sampai jarak 'jangkauan',
// lalu BERBALIK dan terbang kembali ke pemain. Sepanjang jalur ia MENEMBUS &
// melukai setiap musuh yang tersentuh (tiap musuh bisa kena lagi tiap 0.35 dtk
// supaya tidak menghajar 60x/detik). L1 kecil & pendek; naik tiap level;
// evolusi (lvl 5+): ungu, lebih besar/jauh + menyambar petir.
public class Bumerang : MonoBehaviour
{
    Transform pemain;
    Vector3 arah = Vector3.right;
    float speed = 9f;
    float jangkauan = 3.5f;
    int dmg = 6;
    float skala = 0.5f;
    bool evo = false;
    float jarakTempuh = 0f;
    bool balik = false;
    float radiusKena = 0.55f;
    readonly Dictionary<EnemyChase, float> kenaTerakhir = new Dictionary<EnemyChase, float>();
    SpriteRenderer sr;

    public static void Lempar(Vector3 pos, Transform pemain, Vector3 arah, float speed, float jangkauan, int dmg, float skala, bool evo)
    {
        GameObject go = new GameObject("Bumerang");
        go.transform.position = pos;
        Bumerang b = go.AddComponent<Bumerang>();
        b.pemain = pemain;
        b.arah = arah.normalized;
        b.speed = speed;
        b.jangkauan = jangkauan;
        b.dmg = dmg;
        b.skala = skala;
        b.evo = evo;
    }

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatBilah(48, evo ? new Color(0.8f, 0.5f, 1f) : new Color(0.85f, 0.95f, 1f));
        sr.sortingOrder = 47;
        transform.localScale = Vector3.one * skala;
    }

    void Update()
    {
        transform.Rotate(0, 0, 720f * Time.deltaTime); // berputar cepat

        if (!balik)
        {
            float step = speed * Time.deltaTime;
            transform.position += arah * step;
            jarakTempuh += step;
            if (jarakTempuh >= jangkauan) balik = true;
        }
        else
        {
            if (pemain == null) { Destroy(gameObject); return; }
            Vector3 ke = pemain.position - transform.position;
            if (ke.magnitude < 0.6f) { Destroy(gameObject); return; }
            transform.position += ke.normalized * (speed * 1.15f) * Time.deltaTime;
        }

        // damage menembus: tiap musuh bisa kena lagi tiap 0.35 dtk
        int n = EnemyRegistry.DalamRadius(transform.position, radiusKena, EnemyRegistry.Buffer);
        for (int i = 0; i < n; i++)
        {
            EnemyChase ec = EnemyRegistry.Buffer[i];
            if (ec == null || ec.SudahMati) continue;
            float last;
            if (kenaTerakhir.TryGetValue(ec, out last) && Time.time - last < 0.35f) continue;
            kenaTerakhir[ec] = Time.time;
            ec.KenaSerangan(dmg, false);
            if (evo) PetirEfek.Sambar(transform.position, ec.transform.position, new Color(0.8f, 0.5f, 1f, 1f), 0.08f);
        }
    }

    static Sprite BuatBilah(int S, Color c)
    {
        Texture2D t = new Texture2D(S, S, TextureFormat.RGBA32, false);
        t.wrapMode = TextureWrapMode.Clamp;
        float r = S / 2f;
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float nx = (x - r + 0.5f) / r, ny = (y - r + 0.5f) / r;
                float d = Mathf.Sqrt(nx * nx + ny * ny);
                float a = (d >= 0.55f && d <= 1f) ? 1f : 0f; // cincin gergaji
                t.SetPixel(x, y, new Color(c.r, c.g, c.b, a));
            }
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }
}
