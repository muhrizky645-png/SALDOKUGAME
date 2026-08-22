using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    // dibaca script lain (GameMenu/joystick) untuk tahu game sedang Game Over
    public static bool GameOver = false;
    // referensi ke pemain (dipakai Ledakan & PeluruMusuh untuk memberi damage)
    public static PlayerHealth Instance;

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
    private float sfxKenaTimer = 0f; // jeda antar suara \"kena\" biar tidak spam

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Instance = this;
        health = maxHealth;
        isDead = false;
        GameOver = false; // reset tiap scene mulai/di-reload
        // Catatan: timeScale diatur oleh GameMenu (menu awal / jeda / restart),
        // jadi TIDAK dipaksa 1 di sini supaya menu awal tetap beku.
        if (hpFill != null) fillWidth = hpFill.localScale.x;
        UpdateBar();

        // ambil semua sprite badan ninja untuk efek kedip (tanpa ikut HP bar)
        Transform ninja = transform.Find("Ninja_Character_5");
        srs = (ninja != null) ? ninja.GetComponentsInChildren<SpriteRenderer>() : new SpriteRenderer[0];
        warnaAsli = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++) warnaAsli[i] = srs[i].color;
    }

    // Kurangi nyawa pemain sebesar dmg. Dipakai oleh musuh sentuh, ledakan, dan peluru musuh.
    public void Kurangi(float dmg)
    {
        if (isDead) return;
        health -= dmg;
        flashTimer = 0.12f; // picu kedip merah
        if (sfxKenaTimer <= 0f)
        {
            SoundManager.PlayerKena(); // suara pemain kena (dibatasi biar tidak spam)
            sfxKenaTimer = 0.4f;
        }
        if (health <= 0)
        {
            health = 0;
            isDead = true;
            GameOver = true;
            Time.timeScale = 0f;
            SoundManager.GameOver(); // suara game over
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
        if (sfxKenaTimer > 0f) sfxKenaTimer -= Time.unscaledDeltaTime;

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

        // tekan R saat game over = main lagi (restart lalu langsung main)
        if (isDead && Input.GetKeyDown(KeyCode.R)) GameMenu.UlangiDanMain();
    }

    void OnGUI()
    {
        if (!isDead) return;

        float h = Screen.height;
        float w = Screen.width;

        // latar gelap + semburat merah darah (menutupi seluruh HUD di belakangnya)
        Tema.LatarGelap(new Color(0.35f, 0.03f, 0.03f, 0.35f));

        // judul GAME OVER (ukuran diatur supaya muat 1 baris, tidak menimpa panel)
        Tema.Teks(new Rect(0, h * 0.12f, w, h * 0.12f), "GAME OVER", Mathf.RoundToInt(h * 0.07f),
            Tema.Darah, TextAnchor.MiddleCenter, true);

        // panel skor akhir + rekor
        int skor = (ScoreManager.Instance != null) ? ScoreManager.Instance.SkorSekarang : 0;
        int rekor = (ScoreManager.Instance != null) ? ScoreManager.Instance.RekorTertinggi : 0;
        bool rekorBaru = (skor > 0 && skor >= rekor);

        float pw = w * 0.72f, ph = h * 0.16f, px = (w - pw) / 2f, py = h * 0.28f;
        Tema.Panel9(new Rect(px, py, pw, ph), Tema.Plate, Tema.GarisRedup, 2f);

        // dua kolom: SKOR (kiri) & REKOR (kanan)
        float colW = pw / 2f;
        Tema.Teks(new Rect(px, py + ph * 0.14f, colW, ph * 0.35f), "SKOR", Mathf.RoundToInt(h * 0.026f),
            Tema.Redup, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(px, py + ph * 0.45f, colW, ph * 0.5f), skor.ToString(), Mathf.RoundToInt(h * 0.048f),
            Tema.Tulang, TextAnchor.MiddleCenter, true);

        // ikon bintang kecil di atas label REKOR
        float ik = ph * 0.20f;
        Ikon.Gambar(new Rect(px + colW + (colW - ik) / 2f, py + ph * 0.04f, ik, ik), Ikon.Bintang, Tema.Amber);
        Tema.Teks(new Rect(px + colW, py + ph * 0.26f, colW, ph * 0.28f), "REKOR", Mathf.RoundToInt(h * 0.024f),
            Tema.Redup, TextAnchor.MiddleCenter, true);
        Tema.Teks(new Rect(px + colW, py + ph * 0.50f, colW, ph * 0.5f), rekor.ToString(), Mathf.RoundToInt(h * 0.048f),
            Tema.Amber, TextAnchor.MiddleCenter, true);

        // badge rekor baru
        if (rekorBaru)
        {
            Tema.Teks(new Rect(0, py + ph + h * 0.012f, w, h * 0.045f), "* REKOR BARU! *",
                Mathf.RoundToInt(h * 0.03f), Tema.Amber, TextAnchor.MiddleCenter, true);
        }

        // tombol (jarak konsisten, tidak saling menempel)
        float bw = w * 0.62f, bh = h * 0.082f, bx = (w - bw) / 2f;
        int fb = Mathf.RoundToInt(h * 0.032f);
        if (GUI.Button(new Rect(bx, h * 0.56f, bw, bh), "MAIN LAGI (R)", Tema.GayaTombol(fb)))
        {
            SoundManager.Klik();
            GameMenu.UlangiDanMain();
        }
        if (GUI.Button(new Rect(bx, h * 0.68f, bw, bh), "KE HOME", Tema.GayaTombol(fb)))
        {
            SoundManager.Klik();
            GameMenu.KeHome();
        }
    }
}
