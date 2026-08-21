using UnityEngine;

// Proyektil yang ditembakkan musuh tipe \"Penembak\" ke arah pemain.
public class PeluruMusuh : MonoBehaviour
{
    public static void Tembak(Vector3 pos, Vector3 arah, float speed, float dmg)
    {
        GameObject go = new GameObject("PeluruMusuh");
        go.transform.position = pos;
        PeluruMusuh p = go.AddComponent<PeluruMusuh>();
        p.arah = arah.normalized;
        p.speed = speed;
        p.dmg = dmg;
    }

    public Vector3 arah = Vector3.right;
    public float speed = 4.5f;
    public float dmg = 7f;
    public float lifeTime = 4f;

    private SpriteRenderer sr;
    private Transform player;

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BuatLingkaran(20);
        sr.color = new Color(1f, 0.35f, 0.2f, 1f);
        sr.sortingOrder = 45;
        transform.localScale = Vector3.one * 0.4f;

        GameObject pl = GameObject.FindWithTag("Player");
        if (pl != null) player = pl.transform;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += arah * speed * Time.deltaTime;
        if (player != null && Vector3.Distance(transform.position, player.position) <= 0.45f)
        {
            if (PlayerHealth.Instance != null) PlayerHealth.Instance.Kurangi(dmg);
            Destroy(gameObject);
        }
    }

    Sprite BuatLingkaran(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f, dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(r - d);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
