using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public float spawnDistance = 10f;    // jarak spawn dari pemain

    [Header("Tingkat kesulitan (naik seiring waktu)")]
    public float spawnAwal = 1.5f;       // jeda spawn di awal (detik) - makin besar makin jarang
    public float spawnTercepat = 0.35f;  // jeda spawn tercepat saat sudah susah penuh
    public int maxAwal = 10;             // batas musuh di layar saat awal
    public int maxAkhir = 40;            // batas musuh di layar saat susah penuh
    public float waktuMenujuMaksimal = 120f; // detik untuk mencapai kesulitan penuh (2 menit)

    private Transform player;
    private float timer = 0f;
    private float waktuMain = 0f;

    void Start()
    {
        // cari pemain lewat tag "Player"
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // seberapa "susah" sekarang (0 = awal, 1 = maksimal)
        waktuMain += Time.deltaTime;
        float t = Mathf.Clamp01(waktuMain / waktuMenujuMaksimal);

        float jedaSpawn = Mathf.Lerp(spawnAwal, spawnTercepat, t);
        int maxSekarang = Mathf.RoundToInt(Mathf.Lerp(maxAwal, maxAkhir, t));

        // spawn sesuai jeda yang makin lama makin cepat
        timer += Time.deltaTime;
        if (timer >= jedaSpawn)
        {
            timer = 0f;
            SpawnZombie(maxSekarang);
        }
    }

    void SpawnZombie(int maxSekarang)
    {
        // jangan spawn kalau musuh sudah mencapai batas saat ini
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxSekarang) return;

        // arah acak 360° di sekeliling pemain
        Vector2 arahAcak = Random.insideUnitCircle.normalized;
        Vector3 posisi = player.position + new Vector3(arahAcak.x, arahAcak.y, 0f) * spawnDistance;

        Instantiate(zombiePrefab, posisi, Quaternion.identity);
    }
}