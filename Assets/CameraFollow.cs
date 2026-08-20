using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                              // pemain yang diikuti
    public float smoothSpeed = 5f;                        // kehalusan gerak kamera
    public Vector3 offset = new Vector3(0f, 0f, -10f);    // jarak kamera (Z HARUS -10!)
    public float zoom = 10f;                              // makin BESAR = area terlihat makin luas & objek makin kecil

    void Start()
    {
        // cari pemain otomatis lewat tag "Player"
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }

        // atur seberapa luas area yang terlihat kamera
        Camera cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic) cam.orthographicSize = zoom;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 posisiTujuan = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, posisiTujuan, smoothSpeed * Time.deltaTime);
    }
}