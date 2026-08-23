using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1.2f;   // jeda antar tembakan (makin besar = makin pelan)
    public float range = 1f;        // jarak deteksi zombie
    public int jumlahPeluru = 1;    // berapa peluru sekali tembak (naik lewat skill)
    public float sudutSebar = 12f;  // sebaran sudut antar peluru (derajat)

    private float timer = 0.5f;      // sedikit jeda sebelum tembakan pertama

    // Sprite peluru = SENJATA karakter yang dipilih, jadi tiap karakter beda tembakannya
    // (ninja=shuriken, pemanah=panah, penyihir=tongkat, dst). Di-cache per index karakter.
    static readonly Dictionary<int, Sprite> _cachePeluru = new Dictionary<int, Sprite>();

    static Sprite SpritePeluru(int idx)
    {
        Sprite s;
        if (_cachePeluru.TryGetValue(idx, out s)) return s;
        Texture2D t = KarakterManager.Tekstur(idx, "Weapon");
        s = (t != null)
            ? Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100f)
            : null;
        _cachePeluru[idx] = s;
        return s;
    }

    void Update()
    {
        // pakai timer manual (bukan InvokeRepeating) supaya perubahan fireRate
        // dari skill langsung terasa saat itu juga
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Shoot();
            // MODE DEWA: BADAI PELURU -> tembak jauh lebih cepat
            timer = fireRate * (ModeDewa.Aktif ? 0.12f : 1f);
        }
    }

    void Shoot()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
        if (zombies.Length == 0) return;

        // MODE DEWA: jangkauan tembak jauh lebih luas
        float rangeEfektif = range * (ModeDewa.Aktif ? 4f : 1f);

        // cari zombie terdekat
        GameObject terdekat = null;
        float jarakTerdekat = rangeEfektif;
        foreach (GameObject z in zombies)
        {
            float jarak = Vector3.Distance(transform.position, z.transform.position);
            if (jarak < jarakTerdekat)
            {
                jarakTerdekat = jarak;
                terdekat = z;
            }
        }

        if (terdekat == null) return;

        Vector3 arah = (terdekat.transform.position - transform.position).normalized;

        // sprite peluru sesuai senjata karakter terpilih (null -> pakai sprite bawaan prefab)
        Sprite peluruSpr = SpritePeluru(KarakterManager.Dipilih);

        // tembak beberapa peluru sekaligus dengan sedikit sebaran (kalau punya skill)
        // MODE DEWA: BADAI PELURU -> peluru jauh lebih banyak
        int n = Mathf.Max(1, jumlahPeluru + (ModeDewa.Aktif ? 10 : 0));
        float total = (n - 1) * sudutSebar;
        float mulai = -total / 2f;
        for (int i = 0; i < n; i++)
        {
            float sudut = mulai + i * sudutSebar;
            Vector3 arahPeluru = Quaternion.Euler(0f, 0f, sudut) * arah;
            GameObject peluru = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            if (peluruSpr != null)
            {
                SpriteRenderer psr = peluru.GetComponent<SpriteRenderer>();
                if (psr != null) psr.sprite = peluruSpr;
            }
            peluru.GetComponent<Bullet>().direction = arahPeluru;
        }

        SoundManager.Tembak(); // suara tembak
    }
}
