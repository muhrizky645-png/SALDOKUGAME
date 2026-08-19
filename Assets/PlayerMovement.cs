using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Efek terlihat jalan (mantul kecil)")]
    public float bobAmount = 0.08f;   // seberapa besar mantulannya (0 = mati)
    public float bobSpeed = 12f;      // seberapa cepat mantulannya

    private SpriteRenderer sr;
    private Vector3 baseScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale; // simpan ukuran asli
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 gerak = new Vector3(moveX, moveY, 0).normalized;
        transform.position += gerak * moveSpeed * Time.deltaTime;

        // hadap kiri/kanan sesuai arah gerak
        if (moveX < 0) sr.flipX = true;      // gerak kiri → hadap kiri
        else if (moveX > 0) sr.flipX = false; // gerak kanan → hadap kanan

        // efek "terlihat jalan": mantul kecil saat bergerak, diam saat berhenti
        bool sedangJalan = gerak.sqrMagnitude > 0.01f;
        if (sedangJalan)
        {
            float bob = 1f + Mathf.Abs(Mathf.Sin(Time.time * bobSpeed)) * bobAmount;
            transform.localScale = new Vector3(baseScale.x, baseScale.y * bob, baseScale.z);
        }
        else
        {
            transform.localScale = baseScale;
        }
    }
}