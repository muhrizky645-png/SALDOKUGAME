using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static bool GameOver = false;
    public static PlayerHealth Instance;

    public float maxHealth = 100f;
    public float health;
    public float damagePerSecond = 20f;
    private bool isDead = false;

    // jatah \"Hidup Lagi\" (tonton iklan) - hanya 1x per permainan
    private bool sudahHidupLagi = false;

    [Header("HP Bar")]
    public Transform hpFill;      // drag BarFill ke sini
    private float fillWidth = 1f;

    private SpriteRenderer[] srs;
    private Color[] warnaAsli;
    private float flashTimer = 0f;
    private float sfxKenaTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Instance = this;
        health = maxHealth;
        isDead = false;
        GameOver = false;
        sudahHidupLagi = false;
        if (hpFill != null) fillWidth = hpFill.localScale.x;
        UpdateBar();

        Transform ninja = transform.Find("Ninja_Character_5");
        srs = (ninja != null) ? ninja.GetComponentsInChildren<SpriteRenderer>() : new SpriteRenderer[0];
        warnaAsli = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++) warnaAsli[i] = srs[i].color;
    }

    public void Kurangi(float dmg)
    {
        if (isDead) return;
        if (ModeDewa.Aktif) return; // MODE DEWA: KEBAL -> tidak menerima damage sama sekali
        health -= dmg;
        flashTimer = 0.12f;
        if (sfxKenaTimer <= 0f)
        {
            SoundManager.PlayerKena();
            sfxKenaTimer = 0.4f;
        }
        if (health <= 0)
        {
            health = 0;
            isDead = true;
            GameOver = true;
            Time.timeScale = 0f;
            SoundManager.GameOver();
        }
        UpdateBar();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Enemy"))
            Kurangi(damagePerSecond * Time.deltaTime);
    }

    void UpdateBar()
    {
        if (hpFill == null) return;

        float ratio = Mathf.Clamp01(health / maxHealth);

        Vector3 s = hpFill.localScale;
        s.x = fillWidth * ratio;
        hpFill.localScale = s;

        Vector3 p = hpFill.localPosition;
        p.x = -(fillWidth - s.x) / 2f;
        hpFill.localPosition = p;
    }

    // ====== HIDUP LAGI (revive) ======
    // Dipanggil tombol \"HIDUP LAGI\" di Game Over. Untuk sekarang LANGSUNG revive.
    // Untuk iklan ASLI: tampilkan rewarded ad (Unity Ads / AdMob), lalu panggil
    // method ini pada callback \"reward diberikan\".
    public void HidupLagi()
    {
        sudahHidupLagi = true;
        isDead = false;
        GameOver = false;
        health = maxHealth;
        flashTimer = 0f;
        Time.timeScale = 1f;

        // beri ruang bernapas: bersihkan musuh yang sedang menempel
        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var m in musuh)
        {
            if (m == null) continue;
            EnemyChase ec = m.GetComponentInParent<EnemyChase>();
            if (ec != null) ec.KenaSerangan(9999);
        }

        UpdateBar();
    }

    void Update()
    {
        if (sfxKenaTimer > 0f) sfxKenaTimer -= Time.unscaledDeltaTime;

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

        if (isDead && Input.GetKeyDown(KeyCode.R)) GameMenu.UlangiDanMain();
    }

    void OnGUI()
    {
        if (!isDead) return;

        float h = Screen.height;
        float w = Screen.width;

        Tema.LatarGelap(new Color(0.35f, 0.03f, 0.03f, 0.35f));

        Tema.Teks(new Rect(0, h * 0.10f, w, h * 0.12f), "GAME OVER", Mathf.RoundToInt(h * 0.07f),
            Tema.Darah, TextAnchor.MiddleCenter, true);

        int skor = (ScoreManager.Instance != null) ? ScoreManager.Instance.SkorSekarang : 0;
        int rekor = (ScoreManager.Instance != null) ? ScoreManager.Instance.RekorTertinggi : 0;
        bool rekorBaru = (skor > 0 && skor >= rekor);

        float pw = w * 0.72f, ph = h * 0.15f, px = (w - pw) / 2f, py = h * 0.24f;
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Plate, Tema.GarisRedup, 2f);

        float colW = pw / 2f;
        Tema.Teks(new Rect(px, py + ph * 0.14f, colW, ph * 0.35f), "SKOR", Mathf.RoundToInt(h * 0.026f),
            Tema.Redup, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(px, py + ph * 0.45f, colW, ph * 0.5f), skor.ToString(), Mathf.RoundToInt(h * 0.048f),
            Tema.Tulang, TextAnchor.MiddleCenter, true);

        float ik = ph * 0.20f;
        Ikon.Gambar(new Rect(px + colW + (colW - ik) / 2f, py + ph * 0.04f, ik, ik), Ikon.Bintang, Tema.Amber);
        Tema.Teks(new Rect(px + colW, py + ph * 0.26f, colW, ph * 0.28f), "REKOR", Mathf.RoundToInt(h * 0.024f),
            Tema.Redup, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(px + colW, py + ph * 0.50f, colW, ph * 0.5f), rekor.ToString(), Mathf.RoundToInt(h * 0.048f),
            Tema.Amber, TextAnchor.MiddleCenter, true);

        if (rekorBaru)
        {
            Tema.Teks(new Rect(0, py + ph + h * 0.010f, w, h * 0.040f), "* REKOR BARU! *",
                Mathf.RoundToInt(h * 0.028f), Tema.Amber, TextAnchor.MiddleCenter, true);
        }

        // ====== TOMBOL ======
        float bw = w * 0.62f, bh = h * 0.082f, bx = (w - bw) / 2f;
        int fb = Mathf.RoundToInt(h * 0.032f);

        float yMain = h * 0.66f;
        float yHome = h * 0.78f;

        // ---- HIDUP LAGI (tonton iklan) - hanya sekali per permainan ----
        if (!sudahHidupLagi)
        {
            float rh = h * 0.105f;
            float ry = h * 0.53f;
            Rect rRevive = new Rect(bx, ry, bw, rh);
            if (GUI.Button(rRevive, "", Tema.GayaTombol(1)))
            {
                SoundManager.Klik();
                HidupLagi();
                return;
            }
            Tema.Teks(new Rect(rRevive.x, rRevive.y + rh * 0.12f, rRevive.width, rh * 0.44f),
                "HIDUP LAGI", Mathf.RoundToInt(h * 0.034f), Tema.Army, TextAnchor.MiddleCenter, true);
            Tema.Teks(new Rect(rRevive.x, rRevive.y + rh * 0.55f, rRevive.width, rh * 0.38f),
                "> TONTON IKLAN", Mathf.RoundToInt(h * 0.020f), Tema.Amber, TextAnchor.MiddleCenter, true);
        }
        else
        {
            // jatah hidup lagi habis -> naikkan tombol lain biar rapi
            yMain = h * 0.56f;
            yHome = h * 0.68f;
        }

        if (GUI.Button(new Rect(bx, yMain, bw, bh), "MAIN LAGI (R)", Tema.GayaTombol(fb)))
        {
            SoundManager.Klik();
            GameMenu.UlangiDanMain();
        }
        if (GUI.Button(new Rect(bx, yHome, bw, bh), "KE HOME", Tema.GayaTombol(fb)))
        {
            SoundManager.Klik();
            GameMenu.KeHome();
        }
    }
}
