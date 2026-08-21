using System.Collections;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;

    [Header("Nyawa musuh")]
    public int nyawa = 1;             // berapa kali kena tembak untuk mati (diatur oleh Spawner)

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
    private SpriteRenderer[] semuaSprite; // untuk efek kedip merah

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        nyawaSekarang = Mathf.Max(1, nyawa);

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }

        // cari gambar monster otomatis: anak yang punya Animator
        if (visual == null)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponentInChildren<Animator>() != null)
                {
                    visual = child;
                    break;
                }
            }
        }
        if (visual != null) visualBaseScale = visual.localScale;

        // animator untuk animasi jalan / serang / mati
        anim = GetComponentInChildren<Animator>();
        semuaSprite = GetComponentsInChildren<SpriteRenderer>();
        MulaiGerak(); // mulai animasi JALAN
    }

    void FixedUpdate()
    {
        if (sudahMati) return;
        if (target == null) return;

        Vector2 arah = ((Vector2)target.position - rb.position).normalized;
        float jarak = Vector2.Distance(target.position, rb.position);

        // hadapkan gambar ke arah pemain (baik saat jalan maupun serang)
        HadapkanKe(arah);

        if (jarak <= jarakSerang)
        {
            // cukup dekat -> berhenti & serang
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
            // masih jauh -> kejar pemain
            rb.MovePosition(rb.position + arah * moveSpeed * Time.fixedDeltaTime);
            if (!lagiGerak) MulaiGerak();
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
        // untuk sprite bawaan (kalau dipakai)
        if (sr != null)
        {
            if (arah.x < 0) sr.flipX = false;
            else if (arah.x > 0) sr.flipX = true;
        }

        // balik badan gambar monster (arah dibalik biar tidak menghadap mundur)
        if (visual != null)
        {
            if (arah.x < 0)
                visual.localScale = new Vector3(Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
            else if (arah.x > 0)
                visual.localScale = new Vector3(-Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
        }
    }

    // Dipanggil peluru saat musuh kena tembak. Kurangi nyawa; mati kalau habis.
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
            // masih hidup -> kedip merah sebagai tanda kena
            if (gameObject.activeInHierarchy) StartCoroutine(Kedip());
        }
    }

    IEnumerator Kedip()
    {
        if (semuaSprite != null)
            foreach (var s in semuaSprite) if (s != null) s.color = Color.red;
        yield return new WaitForSeconds(lamaKedip);
        if (!sudahMati && semuaSprite != null)
            foreach (var s in semuaSprite) if (s != null) s.color = Color.white;
    }

    // Musuh benar-benar mati (nyawa habis)
    public void Mati()
    {
        if (sudahMati) return;
        sudahMati = true;

        Vector3 pos = transform.position;
        HitEffect.Munculkan(pos);
        XpGem.Munculkan(pos, xp);
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(skor);

        // matikan tabrakan biar tidak menyerang pemain saat sekarat
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // mainkan animasi MATI
        Trig("DeathTrigger");

        // hapus objek setelah animasi mati selesai
        Destroy(gameObject, waktuHancur);
    }

    // Set trigger animator hanya kalau parameternya memang ada (hindari warning)
    void Trig(string nama)
    {
        if (anim == null) return;
        foreach (var p in anim.parameters)
            if (p.name == nama) { anim.SetTrigger(nama); return; }
    }
}
