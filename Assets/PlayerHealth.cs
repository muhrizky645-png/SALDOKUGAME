using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;
    public float damagePerSecond = 20f;
    private bool isDead = false;

    [Header("HP Bar")]
    public Transform hpFill;      // drag BarFill ke sini
    private float fillWidth = 1f;

    // efek kedip merah saat kena serang
    private SpriteRenderer[] srs;
    private Color[] warnaAsli;
    private float flashTimer = 0f;

    void Start()
    {
        health = maxHealth;
        Time.timeScale = 1f;
        if (hpFill != null) fillWidth = hpFill.localScale.x;
        UpdateBar();

        // ambil semua sprite badan ninja untuk efek kedip (tanpa ikut HP bar)
        Transform ninja = transform.Find("Ninja_Character_5");
        srs = (ninja != null) ? ninja.GetComponentsInChildren<SpriteRenderer>() : new SpriteRenderer[0];
        warnaAsli = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++) warnaAsli[i] = srs[i].color;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Enemy"))
        {
            health -= damagePerSecond * Time.deltaTime;
            flashTimer = 0.12f; // picu kedip merah
            if (health <= 0)
            {
                health = 0;
                isDead = true;
                Time.timeScale = 0f;
            }
            UpdateBar();
        }
    }

    void UpdateBar()
    {
        if (hpFill == null) return;

        float ratio = Mathf.Clamp01(health / maxHealth);

        // kecilkan lebar isi bar sesuai HP
        Vector3 s = hpFill.localScale;
        s.x = fillWidth * ratio;
        hpFill.localScale = s;

        // geser biar nyusutnya dari kanan (rata kiri)
        Vector3 p = hpFill.localPosition;
        p.x = -(fillWidth - s.x) / 2f;
        hpFill.localPosition = p;
    }

    void Update()
    {
        // efek kedip merah saat kena musuh
        if (srs != null && srs.Length > 0)
        {
            bool kena = (flashTimer > 0f) && !isDead;
            for (int i = 0; i < srs.Length; i++)
            {
                if (srs[i] == null) continue;
                srs[i].color = kena ? Color.red : warnaAsli[i];
            }
            if (flashTimer > 0f) flashTimer -= Time.unscaledDeltaTime;
        }

        if (isDead && Input.GetKeyDown(KeyCode.R)) RestartGame();
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnGUI()
    {
        if (isDead)
        {
            GUIStyle overStyle = new GUIStyle();
            overStyle.fontSize = 120;
            overStyle.fontStyle = FontStyle.Bold;
            overStyle.normal.textColor = Color.red;
            overStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, -Screen.height * 0.12f, Screen.width, Screen.height), "GAME OVER", overStyle);

            // skor akhir + rekor
            if (ScoreManager.Instance != null)
            {
                GUIStyle skorStyle = new GUIStyle();
                skorStyle.fontSize = 48;
                skorStyle.fontStyle = FontStyle.Bold;
                skorStyle.alignment = TextAnchor.MiddleCenter;
                skorStyle.normal.textColor = Color.white;
                string t = "Skor: " + ScoreManager.Instance.SkorSekarang + "    Rekor: " + ScoreManager.Instance.RekorTertinggi;
                GUI.Label(new Rect(0, Screen.height / 2 - 20, Screen.width, 80), t, skorStyle);
            }

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 40;
            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 90, 300, 90), "MAIN LAGI (R)", btnStyle))
            {
                RestartGame();
            }
        }
    }
}