using System.Collections.Generic;
using UnityEngine;

// Angka damage mengapung (floating damage number) — 100% via KODE, nol asset.
// Panggil dari mana saja: DamageNumber.Munculkan(posisiDunia, jumlah).
// Angka kecil = putih; angka besar (>=5) = kuning emas + lebih gede.
// Digambar lewat OnGUI (World->Screen) biar tidak butuh prefab/font asset.
[DefaultExecutionOrder(9000)]
public class DamageNumber : MonoBehaviour
{
    // ===================== API STATIS =====================
    public static void Munculkan(Vector3 posisiDunia, int jumlah)
    {
        if (jumlah <= 0) jumlah = 1;
        Pastikan();
        if (instance != null) instance.Tambah(posisiDunia, jumlah);
    }

    // ===================== SINGLETON / BOOTSTRAP =====================
    static DamageNumber instance;

    static void Pastikan()
    {
        if (instance != null) return;
        GameObject go = new GameObject("DamageNumber");
        instance = go.AddComponent<DamageNumber>();
        DontDestroyOnLoad(go);
    }

    // ===================== DATA =====================
    class Angka
    {
        public Vector3 pos;    // posisi dunia dasar
        public string teks;
        public float lahir;    // Time.unscaledTime saat muncul
        public float durasi;
        public Color warna;
        public float ukuran;   // skala font relatif
        public float driftX;   // geser horizontal biar tidak numpuk
    }

    readonly List<Angka> daftar = new List<Angka>();
    GUIStyle gaya;

    void Tambah(Vector3 posisiDunia, int jumlah)
    {
        // Tiga tingkat: hit biasa (putih kecil), lumayan (kuning), CRIT/besar (emas gede).
        Angka a = new Angka();
        a.pos = posisiDunia + new Vector3(0f, 0.35f, 0f);
        a.teks = jumlah.ToString();
        a.lahir = Time.unscaledTime;
        a.driftX = Random.Range(-0.3f, 0.3f);
        if (jumlah >= 5)
        {
            a.durasi = 0.9f;
            a.warna = new Color(1f, 0.8f, 0.2f);    // emas
            a.ukuran = 1.05f;
        }
        else if (jumlah >= 2)
        {
            a.durasi = 0.75f;
            a.warna = new Color(1f, 0.95f, 0.55f);  // kuning muda
            a.ukuran = 0.9f;
        }
        else
        {
            a.durasi = 0.6f;
            a.warna = new Color(1f, 1f, 1f, 0.95f); // putih
            a.ukuran = 0.72f;
        }
        daftar.Add(a);
        if (daftar.Count > 80) daftar.RemoveAt(0); // jaga-jaga biar tak menumpuk
    }

    void OnGUI()
    {
        if (daftar.Count == 0) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        if (gaya == null)
        {
            gaya = new GUIStyle(GUI.skin.label);
            gaya.alignment = TextAnchor.MiddleCenter;
            gaya.fontStyle = FontStyle.Bold;
        }

        float t = Time.unscaledTime;
        int basisFont = Mathf.Max(10, Mathf.RoundToInt(Screen.height * 0.02f));

        for (int i = daftar.Count - 1; i >= 0; i--)
        {
            Angka a = daftar[i];
            float umur = t - a.lahir;
            if (umur >= a.durasi) { daftar.RemoveAt(i); continue; }

            float p = umur / a.durasi;              // 0..1
            float alpha = 1f - (p * p);             // fade cepat di ujung
            Vector3 dunia = a.pos + new Vector3(a.driftX, p * 0.9f, 0f); // naik

            Vector3 sp = cam.WorldToScreenPoint(dunia);
            if (sp.z < 0f) continue;                // di belakang kamera

            float x = sp.x;
            float y = Screen.height - sp.y;         // GUI: y dari atas

            gaya.fontSize = Mathf.RoundToInt(basisFont * a.ukuran);
            float w = 180f;
            float h = gaya.fontSize + 14f;
            Rect r = new Rect(x - w * 0.5f, y - h * 0.5f, w, h);

            // bayangan biar kebaca di background terang
            Color bayang = new Color(0f, 0f, 0f, alpha * 0.7f);
            gaya.normal.textColor = bayang;
            GUI.Label(new Rect(r.x + 2f, r.y + 2f, r.width, r.height), a.teks, gaya);

            Color c = a.warna; c.a = alpha;
            gaya.normal.textColor = c;
            GUI.Label(r, a.teks, gaya);
        }
    }
}
