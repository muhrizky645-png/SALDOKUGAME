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

    void Start()
    {
        health = maxHealth;
        Time.timeScale = 1f;
        if (hpFill != null) fillWidth = hpFill.localScale.x;
        UpdateBar();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Enemy"))
        {
            health -= damagePerSecond * Time.deltaTime;
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
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "GAME OVER", overStyle);

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 40;
            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 80, 300, 90), "MAIN LAGI (R)", btnStyle))
            {
                RestartGame();
            }
        }
    }
}