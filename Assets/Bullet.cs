using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public Vector3 direction = Vector3.right;

    void Start()
    {
        Destroy(gameObject, lifeTime); // peluru hancur sendiri setelah 3 detik
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // hancurkan zombie
            Destroy(gameObject);       // hancurkan peluru
        }
    }
}