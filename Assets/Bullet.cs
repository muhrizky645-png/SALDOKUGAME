using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public Vector3 direction = Vector3.right;
    public float putaran = 720f; // kecepatan putar shuriken (derajat per detik)
    public int damage = 1;       // seberapa banyak nyawa musuh yang dikurangi tiap kena

    // ---- KRITIS (sementara) ----
    // Dulu nilainya tertanam langsung di dalam OnTriggerEnter2D sehingga tidak bisa
    // diubah tanpa compile ulang dan tidak bisa di-upgrade lewat skill.
    // Sekarang minimal bisa diatur dari Inspector prefab peluru.
    // CATATAN: ini BELUM sesuai PRD. Kritis seharusnya jadi stat pemain
    // (PeluangKritis / DamageKritis di PasifSO), bukan properti peluru.
    [Header("Kritis (sementara; nanti pindah ke stat pemain)")]
    [Range(0f, 1f)] public float peluangKritis = 0.22f;
    public int pengaliKritMin = 2;
    public int pengaliKritMaks = 4; // eksklusif -> menghasilkan 2x atau 3x

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
                // Musuh yang sudah mati tetap ber-tag "Enemy" selama animasi hancur
                // (EnemyChase.waktuHancur). Tanpa penjagaan ini peluru habis di mayat
                // dan damage-nya terbuang percuma. Biarkan peluru menembus terus
                // supaya bisa mengenai musuh yang masih hidup di belakangnya.
                if (musuh.SudahMati) return;

                // kurangi nyawa musuh; kalau habis, musuh mati (diatur di EnemyChase)
                int dmg = damage;
                if (Random.value < peluangKritis)
                    dmg = damage * Random.Range(pengaliKritMin, pengaliKritMaks);
                musuh.KenaSerangan(dmg);
            }
            else
            {
                // cadangan kalau musuh tidak punya EnemyChase
                HitEffect.Munculkan(other.transform.position);
                DamageNumber.Munculkan(other.transform.position, damage);
                XpGem.Munculkan(other.transform.position, 1);
                if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(10);
                Destroy(other.gameObject);
            }

            Destroy(gameObject); // peluru hancur
        }
    }
}
