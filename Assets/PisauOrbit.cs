using UnityEngine;
using System.Collections.Generic;

// Satu bilah pisau yang berputar mengelilingi pemain (posisinya diatur SenjataManager).
// Melukai musuh yang tersentuh, dengan jeda per-musuh biar tidak menghabisi instan.
public class PisauOrbit : MonoBehaviour
{
    public int dmg = 2;
    public float jarakKena = 0.5f;
    public float jedaPerMusuh = 0.4f;

    private SpriteRenderer sr;
    private Dictionary<EnemyChase, float> kenaTerakhir = new Dictionary<EnemyChase, float>();

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(Ikon.Bintang, new Rect(0, 0, Ikon.Bintang.width, Ikon.Bintang.height), new Vector2(0.5f, 0.5f));
        sr.color = new Color(0.85f, 0.95f, 1f); // putih kebiruan (bilah logam)
        sr.sortingOrder = 44;
        transform.localScale = Vector3.one * 0.6f;
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
