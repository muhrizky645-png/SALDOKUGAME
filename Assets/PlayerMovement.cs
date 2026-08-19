using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Efek terlihat jalan (mantul kecil)")]
    public float bobAmount = 0.08f;   // seberapa besar mantulannya (0 = mati)
    public float bobSpeed = 12f;      // seberapa cepat mantulannya

    [Header("Efek goyang badan ninja saat jalan")]
    public Transform visual;          // isi dengan Ninja_Character_5 (kalau kosong, dicari otomatis)
    public float goyangSudut = 8f;    // seberapa miring saat jalan (derajat)
    public float goyangKecepatan = 12f;

    private SpriteRenderer sr;
    private Animator anim;
    private Vector3 baseScale;
    private bool wasMoving = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        baseScale = transform.localScale; // simpan ukuran asli

        // cari otomatis karakter ninja (anak dari player) kalau belum diisi
        if (visual == null)
        {
            Transform found = transform.Find("Ninja_Character_5");
            if (found != null) visual = found;
        }
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

        // efek goyang badan ninja (biar kelihatan melangkah)
        if (visual != null)
        {
            if (sedangJalan)
            {
                float sudut = Mathf.Sin(Time.time * goyangKecepatan) * goyangSudut;
                visual.localRotation = Quaternion.Euler(0f, 0f, sudut);
            }
            else
            {
                // kembali tegak pelan-pelan saat berhenti
                visual.localRotation = Quaternion.Lerp(visual.localRotation, Quaternion.identity, Time.deltaTime * 12f);
            }
        }

        // animasi kaki jalan: hanya main saat bergerak, berhenti di pose diam saat idle
        if (anim != null)
        {
            if (sedangJalan)
            {
                anim.speed = 1f;
            }
            else
            {
                if (wasMoving)
                {
                    // balik ke frame pertama (pose berdiri) lalu bekukan
                    anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
                }
                anim.speed = 0f;
            }
        }

        wasMoving = sedangJalan;
    }
}