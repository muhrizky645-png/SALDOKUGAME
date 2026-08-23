using UnityEngine;
using System.Collections.Generic;

// Satu bilah yang berputar mengelilingi pemain (posisinya diatur SenjataManager).
// Spritenya SELALU shuriken untuk SEMUA karakter. Memutar senjata asli karakter
// (mis. senapan tentara) terasa tidak masuk akal, jadi dikembalikan ke shuriken.
public class PisauOrbit : MonoBehaviour
{
    public int dmg = 2;
    public float jarakKena = 0.5f;
    public float jedaPerMusuh = 0.4f;

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
        sr.color = Color.white;
        float worldH = spr.rect.height / spr.pixelsPerUnit;
        float sc = worldH > 0.001f ? 0.85f / worldH : 0.85f;
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
        }
    }
}
