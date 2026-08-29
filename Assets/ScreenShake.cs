using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================================
// ZOMBURST - SCREEN SHAKE (getar kamera)
// Tanpa mengedit CameraFollow. Manager ini pakai execution order TINGGI
// (10000) supaya LateUpdate-nya jalan SETELAH CameraFollow selesai
// memosisikan kamera, lalu menambah offset getar kecil di atasnya.
//
// Auto-getar saat pemain kena damage. Bisa juga dipanggil manual:
//   ScreenShake.Getar(kekuatan, durasi);
// =====================================================================
[DefaultExecutionOrder(10000)]
public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    static float sisa = 0f;      // sisa durasi getar
    static float durasi = 0f;
    static float kekuatan = 0f;

    Camera cam;
    float healthLast = -1f;
    float cooldownGetar = 0f;

    // Panggil untuk memicu getar. Getar yang lebih kuat menimpa yang lemah.
    public static void Getar(float kuat, float lama)
    {
        if (kuat >= kekuatan || sisa <= 0f)
        {
            kekuatan = kuat;
            durasi = Mathf.Max(0.01f, lama);
            sisa = durasi;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }
    static void Buat() { if (Instance == null) new GameObject("ScreenShake", typeof(ScreenShake)); }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        sisa = 0f; kekuatan = 0f;
        healthLast = -1f;
        cooldownGetar = 0f;
    }

    void Update()
    {
        if (cooldownGetar > 0f) cooldownGetar -= Time.unscaledDeltaTime;

        if (!Main()) { healthLast = -1f; return; }

        // Auto-getar saat HP pemain turun (dibatasi cooldown supaya tidak
        // gemetar terus saat dikepung banyak musuh).
        if (PlayerHealth.Instance != null)
        {
            float hp = PlayerHealth.Instance.health;
            if (healthLast < 0f) healthLast = hp;
            float turun = healthLast - hp;
            if (turun > 0.5f && cooldownGetar <= 0f)
            {
                float k = Mathf.Clamp(0.06f + turun * 0.01f, 0.06f, 0.22f);
                Getar(k, 0.18f);
                cooldownGetar = 0.22f;
            }
            healthLast = hp;
        }
    }

    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;
        if (sisa <= 0f) return;
        if (!Main()) { sisa = 0f; return; }

        sisa -= Time.unscaledDeltaTime;
        float p = Mathf.Clamp01(sisa / durasi);
        float amp = kekuatan * p;                 // amplitudo mengecil ke 0
        Vector2 o = Random.insideUnitCircle * amp;

        // tambah offset getar DI ATAS posisi hasil CameraFollow (frame ini)
        Vector3 pos = cam.transform.position;
        cam.transform.position = new Vector3(pos.x + o.x, pos.y + o.y, pos.z);
    }

    static bool Main()
    {
        return GameMenu.SedangMain && !GameMenu.SedangJeda &&
               !PlayerHealth.GameOver && !HasilMain.Menang;
    }
}
