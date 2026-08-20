using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1.2f;   // jeda antar tembakan (makin besar = makin pelan)
    public float range = 1f;        // jarak deteksi zombie
    public int jumlahPeluru = 1;    // berapa peluru sekali tembak (naik lewat skill)
    public float sudutSebar = 12f;  // sebaran sudut antar peluru (derajat)

    private float timer = 0.5f;      // sedikit jeda sebelum tembakan pertama

    void Update()
    {
        // pakai timer manual (bukan InvokeRepeating) supaya perubahan fireRate
        // dari skill langsung terasa saat itu juga
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Shoot();
            timer = fireRate;
        }
    }

    void Shoot()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
        if (zombies.Length == 0) return;

        // cari zombie terdekat
        GameObject terdekat = null;
        float jarakTerdekat = range;
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

        // tembak beberapa peluru sekaligus dengan sedikit sebaran (kalau punya skill)
        int n = Mathf.Max(1, jumlahPeluru);
        float total = (n - 1) * sudutSebar;
        float mulai = -total / 2f;
        for (int i = 0; i < n; i++)
        {
            float sudut = mulai + i * sudutSebar;
            Vector3 arahPeluru = Quaternion.Euler(0f, 0f, sudut) * arah;
            GameObject peluru = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            peluru.GetComponent<Bullet>().direction = arahPeluru;
        }
    }
}