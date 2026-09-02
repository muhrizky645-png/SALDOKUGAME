using UnityEngine;

// Kilatan petir singkat berbentuk zig-zag dari satu titik ke titik lain, lalu
// menghapus dirinya sendiri. Dipakai senjata yang sudah BEREVOLUSI (aura &
// pisau) supaya evolusi terlihat sebagai medan listrik ungu yang menyambar,
// bukan sekadar lingkaran/pisau yang membesar.
//
// Dibuat sepenuhnya lewat kode (LineRenderer) - tidak perlu prefab atau aset
// apa pun di Inspector. Materialnya di-cache sekali supaya tidak membuat
// sampah memori tiap sambaran.
public class PetirEfek : MonoBehaviour
{
    static Material _mat;

    static Material Mat()
    {
        if (_mat == null)
        {
            Shader sh = Shader.Find("Sprites/Default");
            if (sh == null) sh = Shader.Find("Unlit/Color");
            _mat = new Material(sh);
        }
        return _mat;
    }

    // Panggil ini dari mana pun untuk memunculkan satu sambaran petir.
    public static void Sambar(Vector3 dari, Vector3 ke, Color warna, float durasi = 0.12f)
    {
        GameObject go = new GameObject("Petir");
        go.AddComponent<PetirEfek>().Mulai(dari, ke, warna, durasi);
    }

    LineRenderer lr;
    float sisa;
    float total;

    void Mulai(Vector3 dari, Vector3 ke, Color warna, float durasi)
    {
        total = Mathf.Max(0.02f, durasi);
        sisa = total;

        lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.material = Mat();
        lr.sortingOrder = 60;
        lr.numCapVertices = 2;
        lr.startWidth = 0.13f;
        lr.endWidth = 0.04f;
        lr.startColor = warna;
        lr.endColor = warna;

        // Titik-titik zig-zag: garis lurus dari 'dari' ke 'ke', tapi tiap titik
        // tengah digeser acak tegak lurus arah - itulah yang membuatnya terlihat
        // seperti petir, bukan garis lurus biasa.
        int seg = 6;
        lr.positionCount = seg + 1;
        Vector2 arah = (Vector2)(ke - dari);
        Vector2 tegak = Vector2.Perpendicular(arah.normalized);
        for (int i = 0; i <= seg; i++)
        {
            float t = i / (float)seg;
            Vector3 titik = Vector3.Lerp(dari, ke, t);
            if (i != 0 && i != seg)
                titik += (Vector3)(tegak * Random.Range(-0.28f, 0.28f));
            lr.SetPosition(i, titik);
        }
    }

    void Update()
    {
        sisa -= Time.deltaTime;
        if (sisa <= 0f) { Destroy(gameObject); return; }

        // Memudar cepat: alpha turun mengikuti sisa umur.
        if (lr != null)
        {
            float a = sisa / total;
            Color c = lr.startColor;
            c.a = a;
            lr.startColor = c;
            lr.endColor = c;
        }
    }
}
