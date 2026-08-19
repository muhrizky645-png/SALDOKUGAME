using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;

    [Header("Gambar musuh (biar bisa balik badan)")]
    public Transform visual;   // gambar monster (objek anak). Kalau kosong, dicari otomatis.

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector3 visualBaseScale = Vector3.one;

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
    }

    void FixedUpdate()
    {
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
}