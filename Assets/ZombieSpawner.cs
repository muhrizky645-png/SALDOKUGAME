using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public float spawnDistance = 10f;    // jarak spawn dari pemain

    [Header("Kesulitan mengikuti Level pemain")]
    public float spawnAwal = 0.9f;            // jeda spawn di Level 1 (detik) - lebih kecil = lebih ramai
    public float penguranganTiapLevel = 0.1f; // jeda spawn berkurang tiap naik level
    public float spawnTercepat = 0.2f;        // batas jeda spawn tercepat
    public int maxAwal = 20;                  // batas musuh di layar Level 1
    public int tambahMaxTiapLevel = 5;        // batas musuh nambah tiap level
    public int maxMutlak = 90;                // batas musuh paling banyak
    public int spawnSekaligus = 2;            // berapa musuh muncul tiap spawn

    private Transform player;
    private float timer = 0f;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // level diambil dari LevelSystem (naik dari XP permata)
        int level = (LevelSystem.Instance != null) ? LevelSystem.Instance.Level : 1;

        float jedaSpawn = Mathf.Max(spawnTercepat, spawnAwal - penguranganTiapLevel * (level - 1));
        int maxSekarang = Mathf.Min(maxMutlak, maxAwal + tambahMaxTiapLevel * (level - 1));

        timer += Time.deltaTime;
        if (timer >= jedaSpawn)
        {
            timer = 0f;
            for (int i = 0; i < spawnSekaligus; i++)
                SpawnZombie(maxSekarang);
        }
    }

    void SpawnZombie(int maxSekarang)
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxSekarang) return;

        Vector2 arahAcak = Random.insideUnitCircle.normalized;
        Vector3 posisi = player.position + new Vector3(arahAcak.x, arahAcak.y, 0f) * spawnDistance;
        Instantiate(zombiePrefab, posisi, Quaternion.identity);
    }
}