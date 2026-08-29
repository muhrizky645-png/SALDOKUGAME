using System.Collections;
using UnityEngine;

// Hit-stop: bekukan game sepersekian detik biar momen membunuh musuh terasa "nendang".
// Auto-bootstrap, nol asset. Rate-limited biar saat spam-kill tidak patah-patah.
public class HitStop : MonoBehaviour
{
    static HitStop Instance;
    static float tTerakhir = -999f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("HitStop");
        Instance = go.AddComponent<HitStop>();
        DontDestroyOnLoad(go);
    }

    // durasi: lama beku (detik nyata). minJeda: jarak minimal antar hit-stop.
    public static void Beku(float durasi, float minJeda)
    {
        if (Instance == null || !GameMenu.SedangMain) return;
        if (Time.timeScale <= 0f) return;                  // jangan ganggu saat pause
        if (Time.unscaledTime - tTerakhir < minJeda) return;
        tTerakhir = Time.unscaledTime;
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.Proses(durasi));
    }

    IEnumerator Proses(float durasi)
    {
        Time.timeScale = 0f;
        float sampai = Time.realtimeSinceStartup + durasi;
        while (Time.realtimeSinceStartup < sampai) yield return null;
        if (GameMenu.SedangMain) Time.timeScale = 1f;      // kembalikan hanya bila masih main
    }
}
