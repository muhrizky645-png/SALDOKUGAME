using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public float spawnDistance = 10f;    // jarak spawn dari pemain

    [Header("Level & kesulitan (naik tiap beberapa detik)")]
    public float detikPerLevel = 20f;         // tiap sekian detik naik 1 level
    public float spawnAwal = 1.5f;            // jeda spawn di Level 1 (detik)
    public float penguranganTiapLevel = 0.12f;// jeda spawn berkurang tiap naik level
    public float spawnTercepat = 0.35f;       // batas jeda spawn tercepat
    public int maxAwal = 10;                  // batas musuh di layar Level 1
    public int tambahMaxTiapLevel = 3;        // batas musuh nambah tiap level
    public int maxMutlak = 50;                // batas musuh paling banyak

    private Transform player;
    private float timer = 0f;
    private float waktuMain = 0f;
    private int level = 1;
    private float levelUpFlash = 0f;          // sisa waktu tampil "LEVEL UP!"

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // hitung level dari waktu bermain
        waktuMain += Time.deltaTime;
        int levelBaru = 1 + Mathf.FloorToInt(waktuMain / detikPerLevel);
        if (levelBaru > level)
        {
            level = levelBaru;
            levelUpFlash = 1.5f; // tampilkan notif "LEVEL UP!" 1.5 detik
        }

        // kesulitan sesuai level
        float jedaSpawn = Mathf.Max(spawnTercepat, spawnAwal - penguranganTiapLevel * (level - 1));
        int maxSekarang = Mathf.Min(maxMutlak, maxAwal + tambahMaxTiapLevel * (level - 1));

        // spawn sesuai jeda
        timer += Time.deltaTime;
        if (timer >= jedaSpawn)
        {
            timer = 0f;
            SpawnZombie(maxSekarang);
        }

        if (levelUpFlash > 0f) levelUpFlash -= Time.deltaTime;
    }

    void SpawnZombie(int maxSekarang)
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxSekarang) return;

        Vector2 arahAcak = Random.insideUnitCircle.normalized;
        Vector3 posisi = player.position + new Vector3(arahAcak.x, arahAcak.y, 0f) * spawnDistance;
        Instantiate(zombiePrefab, posisi, Quaternion.identity);
    }

    void OnGUI()
    {
        // tulisan "Level X" di kiri atas
        int fontSize = Mathf.RoundToInt(Screen.height * 0.032f);
        float pad = Screen.height * 0.02f;
        string teks = "Level " + level;

        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.UpperLeft;

        style.normal.textColor = new Color(0f, 0f, 0f, 0.6f); // bayangan
        GUI.Label(new Rect(pad + 2, pad + 2, 400, fontSize * 2), teks, style);
        style.normal.textColor = new Color(0.5f, 1f, 0.5f, 1f); // hijau terang
        GUI.Label(new Rect(pad, pad, 400, fontSize * 2), teks, style);

        // notif besar "LEVEL UP!" saat baru naik level
        if (levelUpFlash > 0f)
        {
            GUIStyle big = new GUIStyle();
            big.fontSize = Mathf.RoundToInt(Screen.height * 0.06f);
            big.fontStyle = FontStyle.Bold;
            big.alignment = TextAnchor.MiddleCenter;
            float a = Mathf.Clamp01(levelUpFlash);
            big.normal.textColor = new Color(1f, 0.9f, 0.2f, a); // kuning memudar
            GUI.Label(new Rect(0, Screen.height * 0.28f, Screen.width, big.fontSize * 1.6f), "LEVEL UP!", big);
        }
    }
}