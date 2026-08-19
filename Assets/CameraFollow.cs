using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                              // pemain yang diikuti
    public float smoothSpeed = 5f;                        // kehalusan gerak kamera
    public Vector3 offset = new Vector3(0f, 0f, -10f);    // jarak kamera (Z HARUS -10!)

    void Start()
    {
        // cari pemain otomatis lewat tag "Player"
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 posisiTujuan = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, posisiTujuan, smoothSpeed * Time.deltaTime);
    }
}