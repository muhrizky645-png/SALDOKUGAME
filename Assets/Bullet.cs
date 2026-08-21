using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public Vector3 direction = Vector3.right;
    public float putaran = 720f; // kecepatan putar shuriken (derajat per detik)
    public int damage = 1;       // seberapa banyak nyawa musuh yang dikurangi tiap kena

    void Start()
    {
        Destroy(gameObject, lifeTime); // peluru hancur sendiri setelah 3 detik
    }

    void Update()
    {
        // gerak lurus sesuai arah (putaran hanya visual, tidak mengubah arah)
        transform.position += direction * speed * Time.deltaTime;
        transform.Rotate(0f, 0f, putaran * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // cari EnemyChase (ada di objek induk musuh)
            EnemyChase musuh = other.GetComponentInParent<EnemyChase>();
            if (musuh != null)
            {
                // kurangi nyawa musuh; kalau habis, musuh mati (diatur di EnemyChase)
                musuh.KenaSerangan(damage);
            }
            else
            {
                // cadangan kalau musuh tidak punya EnemyChase
                HitEffect.Munculkan(other.transform.position);
                XpGem.Munculkan(other.transform.position, 1);
                if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(10);
                Destroy(other.gameObject);
            }

            Destroy(gameObject); // peluru hancur
        }
    }
}
