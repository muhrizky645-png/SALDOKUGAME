using UnityEngine;

// ============================================================================
//  PENGUMUMAN FASE
// ----------------------------------------------------------------------------
//  Menampilkan spanduk saat fase berganti: GELOMBANG, hitung mundur bos,
//  dan peringatan bos muncul.
//
//  KENAPA INI BUKAN SEKADAR HIASAN:
//  Sistem jadwal boleh sempurna, tapi kalau pemain tidak MENYADARI
//  gelombang sedang datang, yang ia rasakan hanya "kok tiba-tiba ramai".
//  Perubahan yang tidak terbaca sama saja dengan tidak ada. Spanduk inilah
//  yang mengubah perubahan angka menjadi peristiwa.
//
//  Sengaja memakai GUI polos, bukan helper Tema, supaya tidak bergantung pada
//  API yang bisa berubah. Nanti saat migrasi ke UI Toolkit, file ini cukup
//  dibuang dan diganti - tidak ada yang lain yang bergantung padanya.
//
//  Dipasang otomatis, tidak perlu diletakkan di scene.
// ============================================================================
public class PengumumanRun : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        GameObject go = new GameObject("PengumumanRun");
        go.AddComponent<PengumumanRun>();
        DontDestroyOnLoad(go);
    }

    FaseRun faseTerakhir = FaseRun.Normal;
    string teks = "";
    float tampilSampai = 0f;
    Color warna = Color.white;

    static GUIStyle _gaya;
    static GUIStyle _gayaKecil;

    void Update()
    {
        if (!GameMenu.SedangMain) return;

        FaseRun fase = JadwalRun.Fase(GameTimer.Detik);
        if (fase == faseTerakhir) return;

        faseTerakhir = fase;

        switch (fase)
        {
            case FaseRun.Wave:
                Umumkan("GELOMBANG!", new Color(1f, 0.65f, 0.15f), 2.2f);
                break;
            case FaseRun.Hening:
                Umumkan("SESUATU MENDEKAT...", new Color(0.95f, 0.3f, 0.3f), 2.8f);
                break;
            case FaseRun.Bos:
                Umumkan("BOS MUNCUL", new Color(1f, 0.2f, 0.2f), 2.2f);
                ScreenShake.Getar(0.4f, 0.4f);
                break;
        }
    }

    void Umumkan(string t, Color c, float durasi)
    {
        teks = t;
        warna = c;
        tampilSampai = Time.unscaledTime + durasi;
    }

    void OnGUI()
    {
        if (!GameMenu.SedangMain) return;

        SiapkanGaya();

        float w = Screen.width;
        float h = Screen.height;

        // ---- hitung mundur bos, selalu tampil saat mendekat ----
        float sisa = JadwalRun.DetikKeBosBerikut(GameTimer.Detik);
        if (EnemyChase.JumlahBos == 0 && sisa <= 30f)
        {
            _gayaKecil.normal.textColor = new Color(1f, 0.5f, 0.35f,
                0.55f + 0.45f * Mathf.PingPong(Time.unscaledTime * 2f, 1f));
            GUI.Label(new Rect(0f, h * 0.13f, w, h * 0.05f),
                "BOS " + Mathf.CeilToInt(sisa) + "s", _gayaKecil);
        }

        // ---- spanduk fase ----
        if (Time.unscaledTime >= tampilSampai || string.IsNullOrEmpty(teks)) return;

        float sisaTampil = tampilSampai - Time.unscaledTime;
        float alpha = Mathf.Clamp01(sisaTampil / 0.6f);   // memudar di akhir

        Rect pita = new Rect(0f, h * 0.30f, w, h * 0.11f);
        Color lama = GUI.color;

        GUI.color = new Color(0f, 0f, 0f, 0.55f * alpha);
        GUI.DrawTexture(pita, Texture2D.whiteTexture);

        _gaya.normal.textColor = new Color(warna.r, warna.g, warna.b, alpha);
        GUI.color = Color.white;
        GUI.Label(pita, teks, _gaya);

        GUI.color = lama;
    }

    static void SiapkanGaya()
    {
        if (_gaya != null) return;

        _gaya = new GUIStyle(GUI.skin.label);
        _gaya.alignment = TextAnchor.MiddleCenter;
        _gaya.fontStyle = FontStyle.Bold;
        _gaya.fontSize = Mathf.RoundToInt(Screen.height * 0.048f);

        _gayaKecil = new GUIStyle(GUI.skin.label);
        _gayaKecil.alignment = TextAnchor.MiddleCenter;
        _gayaKecil.fontStyle = FontStyle.Bold;
        _gayaKecil.fontSize = Mathf.RoundToInt(Screen.height * 0.028f);
    }
}
