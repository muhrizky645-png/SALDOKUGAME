// ============================================================================
// StressTest
// ----------------------------------------------------------------------------
// Alat ukur performa. INI YANG HARUS KAMU JALANKAN PERTAMA.
//
// KENAPA PENTING:
// PRD menargetkan sampai 320 musuh hidup bersamaan pada 60 FPS. Angka itu
// cuma harapan sampai ada yang mengukurnya di HP sungguhan. Kalau ternyata
// 300 musuh hanya dapat 20 FPS, maka tidak ada gunanya membuat 20 senjata
// atau 100 stage: game-nya akan terasa murah apa pun isinya. Ukur dulu,
// baru putuskan.
//
// CARA PAKAI:
// 1. Build ke HP dengan opsi "Development Build" DICENTANG
//    (File > Build Settings > centang Development Build).
// 2. Mulai bermain seperti biasa.
// 3. Ketuk tombol FPS di pojok kiri atas untuk membuka panel.
// 4. Tekan +100 tiga kali, lalu perhatikan angka "1% rendah".
//
// ANGKA MANA YANG PENTING:
// Bukan FPS rata-rata, tapi "1% RENDAH". Itu mewakili frame terburuk, dan
// justru itulah yang dirasakan pemain sebagai patah-patah. Rata-rata 60 FPS
// dengan 1% rendah 12 FPS tetap terasa rusak.
//
// TARGET: 1% rendah minimal 30 FPS pada 300 musuh.
//
// Seluruh file ini hanya ikut ter-compile di Editor dan Development Build,
// jadi mustahil ikut terbawa ke rilis Play Store.
// ============================================================================
#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System.Collections.Generic;
using UnityEngine;

public class StressTest : MonoBehaviour
{
    const int MaksSampel = 3000;

    static StressTest instance;

    bool panelTerbuka = false;
    float fpsHalus = 0f;
    readonly List<float> sampel = new List<float>(MaksSampel);
    readonly List<float> urut = new List<float>(MaksSampel);
    readonly List<EnemyChase> salinan = new List<EnemyChase>(512);

    float rata = 0f;
    float satuPersen = 0f;
    float terburuk = 0f;
    float timerHitung = 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (instance != null) return;
        GameObject go = new GameObject("StressTest");
        instance = go.AddComponent<StressTest>();
        DontDestroyOnLoad(go);
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        if (dt <= 0.00001f) return;

        float fps = 1f / dt;
        fpsHalus = (fpsHalus <= 0f) ? fps : Mathf.Lerp(fpsHalus, fps, 0.08f);

        if (sampel.Count >= MaksSampel) sampel.RemoveAt(0);
        sampel.Add(fps);

        // Hitung ulang statistik 4x per detik saja; menyortir 3000 angka tiap
        // frame justru akan ikut memperlambat yang sedang kita ukur.
        timerHitung += dt;
        if (timerHitung >= 0.25f)
        {
            timerHitung = 0f;
            HitungStatistik();
        }
    }

    void HitungStatistik()
    {
        if (sampel.Count == 0) return;

        float total = 0f;
        for (int i = 0; i < sampel.Count; i++) total += sampel[i];
        rata = total / sampel.Count;

        urut.Clear();
        urut.AddRange(sampel);
        urut.Sort();

        terburuk = urut[0];
        int idx = Mathf.Clamp(Mathf.FloorToInt(urut.Count * 0.01f), 0, urut.Count - 1);
        satuPersen = urut[idx];
    }

    void Reset()
    {
        sampel.Clear();
        urut.Clear();
        rata = satuPersen = terburuk = 0f;
    }

    void Spawn(int jumlah)
    {
        ZombieSpawner sp = Object.FindObjectOfType<ZombieSpawner>();
        if (sp != null) sp.SpawnPaksa(jumlah);
    }

    void BunuhSemua()
    {
        salinan.Clear();
        salinan.AddRange(EnemyRegistry.Semua);
        for (int i = 0; i < salinan.Count; i++)
            if (salinan[i] != null) salinan[i].Mati();
    }

    void OnGUI()
    {
        float u = Mathf.Min(Screen.width, Screen.height) / 100f;
        int fontKecil = Mathf.RoundToInt(u * 3.2f);
        int fontBesar = Mathf.RoundToInt(u * 4.2f);

        GUIStyle gaya = new GUIStyle(GUI.skin.label);
        gaya.fontSize = fontKecil;
        gaya.normal.textColor = Color.white;

        GUIStyle gayaTombol = new GUIStyle(GUI.skin.button);
        gayaTombol.fontSize = fontKecil;

        float pad = u * 2f;

        if (!panelTerbuka)
        {
            // Tombol kecil saja, supaya tidak menghalangi saat main normal.
            GUIStyle kecil = new GUIStyle(GUI.skin.button);
            kecil.fontSize = fontKecil;
            Color c = WarnaFps(satuPersen > 0f ? satuPersen : fpsHalus);
            GUI.backgroundColor = c;
            if (GUI.Button(new Rect(pad, pad, u * 22f, u * 8f),
                Mathf.RoundToInt(fpsHalus) + " FPS", kecil))
                panelTerbuka = true;
            GUI.backgroundColor = Color.white;
            return;
        }

        float lebar = u * 52f;
        float tinggi = u * 62f;
        Rect kotak = new Rect(pad, pad, lebar, tinggi);

        GUI.backgroundColor = new Color(0f, 0f, 0f, 0.88f);
        GUI.Box(kotak, GUIContent.none);
        GUI.backgroundColor = Color.white;

        float x = kotak.x + u * 2f;
        float y = kotak.y + u * 2f;
        float w = lebar - u * 4f;
        float h = u * 5.2f;

        GUIStyle judul = new GUIStyle(gaya);
        judul.fontSize = fontBesar;
        judul.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(x, y, w, h), "UJI BEBAN", judul); y += h;

        GUI.Label(new Rect(x, y, w, h), "Musuh hidup : " + EnemyRegistry.Jumlah, gaya); y += h;
        GUI.Label(new Rect(x, y, w, h), "FPS sekarang: " + Mathf.RoundToInt(fpsHalus), gaya); y += h;
        GUI.Label(new Rect(x, y, w, h), "Rata-rata   : " + Mathf.RoundToInt(rata), gaya); y += h;

        // Inilah angka yang menentukan. Diberi warna supaya langsung terbaca.
        GUIStyle gayaPenting = new GUIStyle(gaya);
        gayaPenting.fontStyle = FontStyle.Bold;
        gayaPenting.normal.textColor = WarnaFps(satuPersen);
        GUI.Label(new Rect(x, y, w, h), "1% rendah   : " + Mathf.RoundToInt(satuPersen) + "  <-- INI", gayaPenting);
        y += h;

        GUI.Label(new Rect(x, y, w, h), "Terburuk    : " + Mathf.RoundToInt(terburuk), gaya); y += h * 1.2f;

        float wt = (w - u * 2f) / 3f;
        if (GUI.Button(new Rect(x, y, wt, h * 1.4f), "+50", gayaTombol)) Spawn(50);
        if (GUI.Button(new Rect(x + wt + u, y, wt, h * 1.4f), "+100", gayaTombol)) Spawn(100);
        if (GUI.Button(new Rect(x + (wt + u) * 2f, y, wt, h * 1.4f), "+300", gayaTombol)) Spawn(300);
        y += h * 1.4f + u;

        float w2 = (w - u) / 2f;
        if (GUI.Button(new Rect(x, y, w2, h * 1.4f), "Bunuh semua", gayaTombol)) BunuhSemua();
        if (GUI.Button(new Rect(x + w2 + u, y, w2, h * 1.4f), "Reset ukur", gayaTombol)) Reset();
        y += h * 1.4f + u;

        if (GUI.Button(new Rect(x, y, w, h * 1.4f), "Tutup", gayaTombol)) panelTerbuka = false;
    }

    static Color WarnaFps(float fps)
    {
        if (fps <= 0f) return Color.gray;
        if (fps >= 50f) return new Color(0.4f, 1f, 0.4f);   // hijau: mulus
        if (fps >= 30f) return new Color(1f, 0.85f, 0.3f);  // kuning: masih layak
        return new Color(1f, 0.4f, 0.4f);                   // merah: bermasalah
    }
}

#endif
