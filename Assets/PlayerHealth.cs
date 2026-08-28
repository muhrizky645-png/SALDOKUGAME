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

    // jatah "Hidup Lagi" (tonton iklan) - hanya 1x per permainan
    private bool sudahHidupLagi = false;

    [Header("HP Bar (lama - sprite scene, kini disembunyikan; HUD bertema menggantikan)")]
    public Transform hpFill;      // drag BarFill ke sini (opsional, kini di-hide)
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
        SembunyikanBarLama();   // matikan bar sprite lama; diganti HUD bertema di OnGUI

        Transform ninja = transform.Find("Ninja_Character_5");
        srs = (ninja != null) ? ninja.GetComponentsInChildren<SpriteRenderer>() : new SpriteRenderer[0];
        warnaAsli = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++) warnaAsli[i] = srs[i].color;
    }

    // Sembunyikan bar HP lama berbasis sprite (fill + background + border) supaya
    // tidak dobel dengan HUD bertema yang baru.
    void SembunyikanBarLama()
    {
        if (hpFill == null) return;
        SpriteRenderer f = hpFill.GetComponent<SpriteRenderer>();
        if (f != null) f.enabled = false;
        Transform bar = hpFill.parent;
        if (bar != null)
        {
            SpriteRenderer bg = bar.GetComponent<SpriteRenderer>();
            if (bg != null) bg.enabled = false;
            foreach (Transform c in bar)
            {
                SpriteRenderer sc = c.GetComponent<SpriteRenderer>();
                if (sc != null) sc.enabled = false;
            }
        }
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

    // Pulihkan HP (dipakai buff "Pulih HP" dari Toko).
    public void Pulih(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;
        health = Mathf.Min(health + amount, maxHealth);
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
    // Dipanggil tombol "HIDUP LAGI" di Game Over. Untuk sekarang LANGSUNG revive.
    // Untuk iklan ASLI: tampilkan rewarded ad (Unity Ads / AdMob), lalu panggil
    // method ini pada callback "reward diberikan".
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

    // ====== HP BAR BERTEMA (menyatu dgn HUD) ======
    // Digambar saat MAIN: backing rounded gelap + isi gradien warna DINAMIS
    // (hijau penuh -> kuning -> merah saat kritis) + ikon HATI + angka HP.
    void GambarBarNyawa()
    {
        if (isDead) return;
        if (!GameMenu.SedangMain || GameMenu.SedangJeda ||
            SkillManager.AktifMemilih || PlayerHealth.GameOver) return;
        if (ModeDewa.MenuTerbuka) return; // jangan timpa overlay Peti Dewa

        float w = Screen.width, h = Screen.height;
        float atas = Tema.AmanAtas, pad = Tema.Pad;

        // sejajarkan tepat DI BAWAH baris skor/timer (samakan hitungan dgn GameTimer)
        float levelBawah = atas + pad + LevelSystem.TinggiPanel(w);
        int fSkor = Mathf.Min(Tema.Font(0.05f), Mathf.RoundToInt(w * 0.10f));
        float skorH = fSkor * 1.7f;
        float skorY = levelBawah + pad * 0.6f;
        float y = skorY + skorH + pad * 0.55f;

        float bw = w * 0.66f;
        float bh = Mathf.Max(Tema.Unit * 0.030f, h * 0.024f);
        float bx = (w - bw) / 2f;

        // backing rounded gelap (senada bar boss)
        Tema.Panel9(new Rect(bx, y, bw, bh), new Color(0f, 0f, 0f, 0.55f), Tema.GarisRedup, 1f);

        // isi bar dgn warna DINAMIS sesuai sisa nyawa
        float ratio = Mathf.Clamp01(health / maxHealth);
        Color warna = (ratio > 0.5f)
            ? Color.Lerp(Tema.Amber, Tema.Army, (ratio - 0.5f) * 2f)   // sehat: kuning -> hijau
            : Color.Lerp(Tema.Darah, Tema.Amber, ratio * 2f);          // kritis: merah -> kuning
        if (ratio > 0f)
            Tema.BarIsi(new Rect(bx + 2f, y + 2f, (bw - 4f) * ratio, bh - 4f), warna);

        // ikon HATI menonjol di ujung kiri (sedikit keluar dari bar biar "melekat")
        float ik = bh * 2.0f;
        Ikon.Gambar(new Rect(bx - ik * 0.32f, y + bh / 2f - ik / 2f, ik, ik), Ikon.Hati, Tema.Darah);

        // angka HP di tengah bar
        int hpNow = Mathf.CeilToInt(health);
        int hpMax = Mathf.RoundToInt(maxHealth);
        int fHp = Mathf.Min(Tema.Font(0.026f), Mathf.RoundToInt(bh * 0.95f));
        Tema.Teks(new Rect(bx, y - bh * 0.02f, bw, bh), hpNow + " / " + hpMax,
            fHp, Tema.Tulang, TextAnchor.MiddleCenter, true);
    }

    void OnGUI()
    {
        GambarBarNyawa(); // HP bar bertema saat sedang main

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
