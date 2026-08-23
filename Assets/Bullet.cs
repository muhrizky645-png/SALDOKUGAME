using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public Vector3 direction = Vector3.right;
    public float putaran = 720f; // kecepatan putar shuriken (derajat per detik)
    public int damage = 1;       // seberapa banyak nyawa musuh yang dikurangi tiap kena

    // Kalau true: proyektil MENGHADAP arah terbang & TIDAK berputar (peluru/anak panah).
    // Kalau false: proyektil berputar (shuriken / pedang yang dilempar).
    public bool orientKeArah = false;
    public float sudutOffset = -90f; // sprite digambar menghadap ATAS (+Y)

    void Start()
    {
        Destroy(gameObject, lifeTime); // peluru hancur sendiri setelah 3 detik

        if (orientKeArah)
        {
            float a = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + sudutOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, a);
        }
    }

    void Update()
    {
        // gerak lurus sesuai arah
        transform.position += direction * speed * Time.deltaTime;
        // putaran hanya visual; matikan untuk proyektil yang menghadap arah
        if (!orientKeArah) transform.Rotate(0f, 0f, putaran * Time.deltaTime);
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
