using UnityEngine;
using System.Collections.Generic;

// Satu bilah yang berputar mengelilingi pemain (posisinya diatur SenjataManager).
// Spritenya = SENJATA milik karakter yang dipilih (ninja=shuriken, kesatria=pedang, dst).
// Melukai musuh yang tersentuh, dengan jeda per-musuh biar tidak menghabisi instan.
public class PisauOrbit : MonoBehaviour
{
    public int dmg = 2;
    public float jarakKena = 0.5f;
    public float jedaPerMusuh = 0.4f;

    private SpriteRenderer sr;
    private Dictionary<EnemyChase, float> kenaTerakhir = new Dictionary<EnemyChase, float>();

    // cache sprite senjata per index karakter biar tidak dibuat ulang tiap bilah
    static readonly Dictionary<int, Sprite> _cacheSenjata = new Dictionary<int, Sprite>();

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 44;

        int idx = KarakterManager.Dipilih;
        Sprite spr = SpriteSenjata(idx);

        if (spr != null)
        {
            // pakai senjata karakter; ukuran dinormalkan supaya semua senjata seukuran
            sr.sprite = spr;
            sr.color = Color.white;
            float worldH = spr.rect.height / spr.pixelsPerUnit;
            float sc = worldH > 0.001f ? 0.85f / worldH : 0.85f;
            transform.localScale = Vector3.one * sc;
        }
        else
        {
            // fallback: bilah bintang logam (kalau senjata karakter tidak ada)
            sr.sprite = Sprite.Create(Ikon.Bintang, new Rect(0, 0, Ikon.Bintang.width, Ikon.Bintang.height), new Vector2(0.5f, 0.5f));
            sr.color = new Color(0.85f, 0.95f, 1f);
            transform.localScale = Vector3.one * 0.6f;
        }
    }

    static Sprite SpriteSenjata(int idx)
    {
        Sprite s;
        if (_cacheSenjata.TryGetValue(idx, out s)) return s;
        Texture2D t = KarakterManager.Tekstur(idx, "Weapon");
        s = (t != null)
            ? Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), 100f)
            : null;
        _cacheSenjata[idx] = s;
        return s;
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
