using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public float spawnInterval = 0.5f;
    public float spawnDistance = 10f;   // jarak spawn dari pemain
    private Transform player;

    void Start()
    {
        // cari pemain lewat tag "Player"
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        InvokeRepeating(nameof(SpawnZombie), 1f, spawnInterval);
    }

    void SpawnZombie()
    {
        if (player == null) return;

        // arah acak 360° di sekeliling pemain
        Vector2 arahAcak = Random.insideUnitCircle.normalized;
        Vector3 posisi = player.position + new Vector3(arahAcak.x, arahAcak.y, 0f) * spawnDistance;

        Instantiate(zombiePrefab, posisi, Quaternion.identity);
    }
}