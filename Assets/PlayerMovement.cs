using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
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
    }
}