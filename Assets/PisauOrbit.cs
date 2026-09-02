using UnityEngine;
using System.Collections.Generic;

// Satu bilah yang berputar mengelilingi pemain (posisinya diatur SenjataManager).
// Spritenya SELALU shuriken untuk SEMUA karakter. Memutar senjata asli karakter
// (mis. senapan tentara) terasa tidak masuk akal, jadi dikembalikan ke shuriken.
//
// Ukuran (skala), warna, dan status evolusi (evo) DIISI oleh SenjataManager per
// level: makin tinggi level makin besar; saat evolusi bilah berubah ungu dan
// sesekali memercikkan petir.
public class PisauOrbit : MonoBehaviour
{
    public int dmg = 2;
    public float jarakKena = 0.85f;
    public float jedaPerMusuh = 0.4f;
    public float skala = 0.85f;        // ukuran bilah dalam satuan dunia (diisi per level)
    public Color warna = Color.white;  // putih biasa; ungu saat evolusi
    public bool evo = false;           // true kalau senjata sudah berevolusi

    private SpriteRenderer sr;
    private Dictionary<EnemyChase, float> kenaTerakhir = new Dictionary<EnemyChase, float>();

    // cache sprite shuriken (dipakai ulang semua bilah, tidak tergantung karakter)
    static Sprite _shuriken;

    static Sprite SpriteShuriken()
    {
        if (_shuriken == null)
        {
            // pakai ikon "pisau" (shuriken) dari asset pack; fallback ke ikon kode
            Texture2D t = Ikon.UntukSkill("pisau");
            if (t == null) t = Ikon.Pisau;
            _shuriken = Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100f);
        }
        return _shuriken;
    }

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 44;

        // SELALU shuriken, apa pun karakter yang dipilih
        Sprite spr = SpriteShuriken();
        sr.sprite = spr;
        sr.color = warna;
        float worldH = spr.rect.height / spr.pixelsPerUnit;
        // ukuran diatur per level lewat 'skala' (diisi SenjataManager)
        float sc = worldH > 0.001f ? skala / worldH : skala;
        transform.localScale = Vector3.one * sc;
    }

    void Update()
    {
        // putar bilah biar terlihat berputar
        transform.Rotate(0, 0, 360f * Time.deltaTime);

        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var m in musuh)
        {
            if (m == null) continue;
            if (Vector3.Distance(transform.position, m.transform.position) > jarakKena) continue;

            EnemyChase ec = m.GetComponentInParent<EnemyChase>();
            if (ec == null) continue;

            float last;
            if (kenaTerakhir.TryGetValue(ec, out last) && Time.time - last < jedaPerMusuh) continue;

            kenaTerakhir[ec] = Time.time;
            ec.KenaSerangan(dmg);

            // EVOLUSI: sesekali percikkan petir ungu dari bilah ke musuh yang
            // kena. Pakai peluang kecil supaya jadi kilatan sporadis, bukan
            // tembok cahaya yang menutupi layar.
            if (evo && Random.value < 0.20f)
                PetirEfek.Sambar(transform.position, m.transform.position, new Color(0.8f, 0.5f, 1f, 1f), 0.08f);
        }
    }
}
