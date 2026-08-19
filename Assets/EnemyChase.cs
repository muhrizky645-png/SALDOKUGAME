using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 arah = ((Vector2)target.position - rb.position).normalized;
        rb.MovePosition(rb.position + arah * moveSpeed * Time.fixedDeltaTime);

        // hadap ke arah pemain (kiri/kanan)
        if (arah.x < 0) sr.flipX = true;
        else if (arah.x > 0) sr.flipX = false;
    }
}