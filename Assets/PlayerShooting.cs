using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 0.5f;   // nembak tiap 0.5 detik
    public float range = 4f;        // jarak deteksi zombie (makin kecil = musuh harus lebih dekat dulu)

    void Start()
    {
        InvokeRepeating(nameof(Shoot), 0.5f, fireRate);
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

        // tembak ke arah zombie terdekat
        Vector3 arah = (terdekat.transform.position - transform.position).normalized;
        GameObject peluru = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        peluru.GetComponent<Bullet>().direction = arah;
    }
}