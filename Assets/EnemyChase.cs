using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;

    [Header("Gambar musuh (biar bisa balik badan)")]
    public Transform visual;   // gambar monster (objek anak). Kalau kosong, dicari otomatis.

    [Header("Animasi & mati")]
    public float waktuHancur = 1f;   // durasi animasi mati sebelum objek dihapus
    public int skor = 10;            // skor saat musuh mati
    public int xp = 1;               // XP yang dijatuhkan saat mati

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private Vector3 visualBaseScale = Vector3.one;
    private bool sudahMati = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

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

        // animator untuk animasi jalan / mati
        anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("MoveTrigger"); // mulai animasi JALAN
    }

    void FixedUpdate()
    {
        if (sudahMati) return;
        if (target == null) return;

        Vector2 arah = ((Vector2)target.position - rb.position).normalized;
        rb.MovePosition(rb.position + arah * moveSpeed * Time.fixedDeltaTime);

        // untuk sprite bawaan (kalau dipakai)
        if (arah.x < 0) sr.flipX = false;
        else if (arah.x > 0) sr.flipX = true;

        // balik badan gambar monster (arah dibalik biar tidak menghadap mundur)
        if (visual != null)
        {
            if (arah.x < 0)
                visual.localScale = new Vector3(Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
            else if (arah.x > 0)
                visual.localScale = new Vector3(-Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
        }
    }

    // Dipanggil peluru saat musuh kena tembak
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
        if (anim != null) anim.SetTrigger("DeathTrigger");

        // hapus objek setelah animasi mati selesai
        Destroy(gameObject, waktuHancur);
    }
}