using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public Vector3 direction = Vector3.right;
    public float putaran = 720f; // kecepatan putar shuriken (derajat per detik)

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
            // efek ledakan kecil di posisi musuh
            HitEffect.Munculkan(other.transform.position);

            Destroy(other.gameObject); // hancurkan zombie
            Destroy(gameObject);       // hancurkan peluru

            // tambah skor tiap zombie mati
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(10);
        }
    }
}