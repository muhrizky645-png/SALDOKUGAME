using System.Collections;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public enum Tipe { Biasa, Cepat, Tank, Peledak, Penembak }

    public Transform target;
    public float moveSpeed = 2f;

    [Header("Nyawa musuh")]
    public int nyawa = 1;             // berapa kali kena tembak untuk mati (diatur oleh Spawner)

    [Header("Variasi / Boss")]
    public Tipe tipe = Tipe.Biasa;   // diacak otomatis saat Start (kecuali boss)
    public bool bos = false;         // diset oleh Spawner untuk musuh boss

    public static int JumlahBos = 0;         // berapa boss hidup sekarang
    public static EnemyChase BosSaatIni = null; // boss terakhir (untuk bar nyawa)

    [Header("Gambar musuh (biar bisa balik badan)")]
    public Transform visual;   // gambar monster (objek anak). Kalau kosong, dicari otomatis.

    [Header("Animasi & mati")]
    public float waktuHancur = 1f;   // durasi animasi mati sebelum objek dihapus
    public int skor = 10;            // skor saat musuh mati
    public int xp = 1;               // XP yang dijatuhkan saat mati

    [Header("Serang pemain")]
    public float jarakSerang = 0.9f; // sedekat apa musuh mulai menyerang (berhenti jalan)
    public float jedaSerang = 0.8f;  // jeda antar animasi serang (detik)

    [Header("Efek kena tembak")]
    public float lamaKedip = 0.08f;  // durasi kedip merah saat kena (kalau nyawa > 1)

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Vector3 visualBaseScale = Vector3.one;
    private bool sudahMati = false;
    private bool lagiGerak = false;   // sedang animasi jalan?
    private float timerSerang = 0f;   // hitung mundur jeda serang
    private int nyawaSekarang;        // sisa nyawa saat ini
    private int nyawaMaks;            // nyawa penuh (untuk bar boss)
    private SpriteRenderer[] semuaSprite; // untuk efek kedip merah
    private Color[] warnaDasar;       // warna asli tiap sprite (untuk balik setelah kedip)
    private float tTembak = 1.2f;     // timer untuk musuh Penembak
    private float jedaTembak = 2f;

    public int NyawaSisa { get { return nyawaSekarang; } }
    public int NyawaMaks { get { return nyawaMaks; } }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }

        // Cari gambar monster otomatis untuk dibalik badannya.
        if (visual == null)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponentInChildren<Animator>() != null ||
                    child.GetComponentInChildren<Renderer>() != null)
                {
                    visual = child;
                    break;
                }
            }
            if (visual == null && transform.childCount > 0)
                visual = transform.GetChild(0);
        }
        if (visual != null) visualBaseScale = visual.localScale;

        anim = GetComponentInChildren<Animator>();
        semuaSprite = GetComponentsInChildren<SpriteRenderer>();

        // === VARIASI MUSUH ===
        if (!bos) RollTipe();
        TerapkanVarian();

        nyawaSekarang = Mathf.Max(1, nyawa);
        nyawaMaks = nyawaSekarang;

        // simpan warna dasar SETELAH tint variasi dipasang
        if (semuaSprite != null)
        {
            warnaDasar = new Color[semuaSprite.Length];
            for (int i = 0; i < semuaSprite.Length; i++)
                if (semuaSprite[i] != null) warnaDasar[i] = semuaSprite[i].color;
        }

        if (bos)
        {
            JumlahBos++;
            BosSaatIni = this;
        }

        MulaiGerak(); // mulai animasi JALAN
    }

    // Acak tipe musuh; peluang tipe khusus naik seiring waktu bertahan
    void RollTipe()
    {
        float menit = GameTimer.Detik / 60f;
        float peluang = Mathf.Clamp01(0.12f + menit * 0.05f);
        if (Random.value < peluang)
            tipe = (Tipe)Random.Range(1, 5); // Cepat/Tank/Peledak/Penembak
        else
            tipe = Tipe.Biasa;
    }

    // Terapkan pengali stat + warna sesuai tipe / boss
    void TerapkanVarian()
    {
        Color tint = Color.white;
        float skala = 1f;

        switch (tipe)
        {
            case Tipe.Cepat:
                moveSpeed *= 1.6f; skala = 0.8f; tint = new Color(0.6f, 1f, 1f); break;
            case Tipe.Tank:
                moveSpeed *= 0.6f; nyawa = nyawa * 3 + 2; skor += 15; xp += 1;
                skala = 1.4f; tint = new Color(0.8f, 0.6f, 1f); break;
            case Tipe.Peledak:
                skala = 1.1f; skor += 5; tint = new Color(1f, 0.6f, 0.3f); break;
            case Tipe.Penembak:
                moveSpeed *= 0.85f; skor += 10; tint = new Color(1f, 0.9f, 0.4f); break;
        }

        if (bos)
        {
            tint = new Color(1f, 0.4f, 0.4f);
            skala = 1f; // ukuran boss sudah diatur Spawner
        }

        transform.localScale *= skala;
        if (tint != Color.white && semuaSprite != null)
            foreach (var s in semuaSprite) if (s != null) s.color = tint;
    }

    void FixedUpdate()
    {
        if (sudahMati) return;
        if (target == null) return;

        Vector2 arah = ((Vector2)target.position - rb.position).normalized;
        float jarak = Vector2.Distance(target.position, rb.position);

        HadapkanKe(arah);

        if (jarak <= jarakSerang)
        {
            lagiGerak = false;
            timerSerang -= Time.fixedDeltaTime;
            if (timerSerang <= 0f)
            {
                Trig("AttackTrigger");
                timerSerang = jedaSerang;
            }
        }
        else
        {
            rb.MovePosition(rb.position + arah * moveSpeed * Time.fixedDeltaTime);
            if (!lagiGerak) MulaiGerak();
        }
    }

    // Musuh Penembak melempar proyektil ke pemain dari jauh
    void Update()
    {
        if (sudahMati || tipe != Tipe.Penembak || target == null) return;
        if (!GameMenu.SedangMain) return;

        tTembak -= Time.deltaTime;
        if (tTembak <= 0f)
        {
            float jarak = Vector3.Distance(target.position, transform.position);
            if (jarak <= 8f)
            {
                Vector3 a = (target.position - transform.position).normalized;
                PeluruMusuh.Tembak(transform.position, a, 4.5f, 7f);
                tTembak = jedaTembak;
            }
            else tTembak = 0.3f;
        }
    }

    void MulaiGerak()
    {
        lagiGerak = true;
        timerSerang = 0f;
        Trig("MoveTrigger");
    }

    void HadapkanKe(Vector2 arah)
    {
        if (Mathf.Abs(arah.x) < 0.01f) return;

        if (sr != null)
        {
            if (arah.x < 0) sr.flipX = false;
            else if (arah.x > 0) sr.flipX = true;
        }

        if (visual != null)
        {
            if (arah.x < 0)
                visual.localScale = new Vector3(Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
            else if (arah.x > 0)
                visual.localScale = new Vector3(-Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
        }
    }

    public void KenaSerangan(int damage)
    {
        if (sudahMati) return;
        nyawaSekarang -= Mathf.Max(1, damage);
        if (nyawaSekarang <= 0)
        {
            Mati();
        }
        else
        {
            SoundManager.MusuhKena();
            if (gameObject.activeInHierarchy) StartCoroutine(Kedip());
        }
    }

    IEnumerator Kedip()
    {
        if (semuaSprite != null)
            foreach (var s in semuaSprite) if (s != null) s.color = Color.red;
        yield return new WaitForSeconds(lamaKedip);
        if (!sudahMati && semuaSprite != null)
            for (int i = 0; i < semuaSprite.Length; i++)
                if (semuaSprite[i] != null)
                    semuaSprite[i].color = (warnaDasar != null && i < warnaDasar.Length) ? warnaDasar[i] : Color.white;
    }

    public void Mati()
    {
        if (sudahMati) return;
        sudahMati = true;

        SoundManager.MusuhMati();

        Vector3 pos = transform.position;
        HitEffect.Munculkan(pos);
        XpGem.Munculkan(pos, xp);
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(skor);

        // musuh Peledak: meledak melukai pemain di sekitarnya
        if (tipe == Tipe.Peledak)
            Ledakan.Munculkan(pos, 1.9f, 0, 16f, new Color(1f, 0.6f, 0.2f, 0.85f));

        // === HADIAH / DROP ITEM ===
        if (bos)
        {
            JumlahBos = Mathf.Max(0, JumlahBos - 1);
            if (BosSaatIni == this) BosSaatIni = null;
            // boss jatuhkan banyak XP + PETI
            for (int i = 0; i < 6; i++)
                XpGem.Munculkan(pos + (Vector3)(Random.insideUnitCircle * 1.2f), 5);
            ItemLapangan.Jatuhkan(pos, ItemLapangan.Jenis.Peti);
        }
        else
        {
            float roll = Random.value;
            if (roll < 0.02f) ItemLapangan.Jatuhkan(pos, ItemLapangan.Jenis.Bom);
            else if (roll < 0.05f) ItemLapangan.Jatuhkan(pos, ItemLapangan.Jenis.Magnet);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Trig("DeathTrigger");
        Destroy(gameObject, waktuHancur);
    }

    void Trig(string nama)
    {
        if (anim == null) return;
        foreach (var p in anim.parameters)
            if (p.name == nama) { anim.SetTrigger(nama); return; }
    }
}
