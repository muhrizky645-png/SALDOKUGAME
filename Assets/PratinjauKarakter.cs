using UnityEngine;

// Membuat pratinjau SELURUH BADAN karakter untuk menu Home. Caranya: meng-clone rig
// pemain (yang SUDAH dirakit lengkap: badan, kepala, kaki, senjata di scene) ke sebuah
// "panggung" jauh, lalu merendernya dengan kamera khusus ke RenderTexture. Dengan begitu
// proporsi & posisi tiap bagian pasti benar (persis seperti saat main), dan gameplay
// tidak terganggu karena clone berada jauh dari arena serta script-nya dimatikan.
//
// PENTING (URP): kamera TIDAK dirender manual lewat _cam.Render(). Di Universal Render
// Pipeline, memanggil Camera.Render() dari dalam OnGUI melempar error
// "UniversalCameraData has already been created" dan membuat hasilnya KOSONG. Solusinya:
// biarkan kamera AKTIF (enabled = true) dengan targetTexture, sehingga URP merendernya
// otomatis tiap frame ke RenderTexture (tidak ikut tampil ke layar).
public static class PratinjauKarakter
{
    static Camera _cam;
    static RenderTexture _rt;
    static GameObject _panggung;
    static GameObject _klon;
    static int _idxTerakhir = -999;

    static readonly Vector3 PosPanggung = new Vector3(10000f, 10000f, 0f);

    // ===== FIX: bersihkan semua state static tiap masuk Play Mode =====
    // Objek panggung/kamera/klon dibuat dengan HideAndDontSave, jadi TIDAK ikut
    // hancur saat Stop Play. Kalau "Reload Domain" mati, sisa objek + _idxTerakhir
    // dari sesi sebelumnya nyangkut, sementara RenderTexture-nya sudah mati ->
    // kamera lama render ke texture kosong -> preview blank di Play kedua.
    // Reset ini maksa semuanya dibangun ulang fresh.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic()
    {
        if (_cam != null) _cam.targetTexture = null; // lepas dulu dari kamera -> hindari warning "Releasing render texture..."
        if (_rt != null) { _rt.Release(); Object.Destroy(_rt); }
        if (_panggung != null) Object.Destroy(_panggung); // ikut hapus kamera & klon (anak panggung)
        _rt = null;
        _cam = null;
        _panggung = null;
        _klon = null;
        _idxTerakhir = -999;
    }

    // Kembalikan tekstur pratinjau untuk karakter idx (atau null bila rig belum ada).
    // Klon hanya DIBANGUN ULANG saat karakter berganti (mahal: Instantiate rig). Proses
    // RENDER ditangani otomatis oleh kamera aktif (URP), jadi di sini cukup kembalikan RT.
    public static Texture Ambil(int idx)
    {
        Siapkan();
        if (_cam == null || _rt == null) return null;

        if (idx != _idxTerakhir || _klon == null)
        {
            if (!BangunKlon()) return null;
            _idxTerakhir = idx;
        }
        return _rt;
    }

    static void Siapkan()
    {
        if (_panggung == null)
        {
            _panggung = new GameObject("PratinjauKarakter_Panggung");
            _panggung.hideFlags = HideFlags.HideAndDontSave;
            _panggung.transform.position = PosPanggung;
        }
        if (_rt == null)
        {
            _rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            _rt.Create();
        }
        if (_cam == null)
        {
            GameObject cg = new GameObject("PratinjauKarakter_Kamera");
            cg.hideFlags = HideFlags.HideAndDontSave;
            cg.transform.SetParent(_panggung.transform, false);
            _cam = cg.AddComponent<Camera>();
            _cam.orthographic = true;
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.backgroundColor = new Color(0f, 0f, 0f, 0f); // transparan
            _cam.cullingMask = ~0;      // hanya clone yang ada di panggung jauh ini yang terlihat
            _cam.nearClipPlane = 0.01f;
            _cam.farClipPlane = 100f;
            _cam.targetTexture = _rt;   // punya targetTexture => TIDAK ikut tampil ke layar
            _cam.depth = -100;
            _cam.enabled = true;        // URP merender otomatis tiap frame (JANGAN panggil _cam.Render() manual di URP!)
        }
    }

    static bool BangunKlon()
    {
        if (_klon != null) { Object.DestroyImmediate(_klon); _klon = null; }

        Transform rig = CariRig();
        if (rig == null) return false;

        _klon = Object.Instantiate(rig.gameObject);
        _klon.hideFlags = HideFlags.HideAndDontSave;

        // matikan semua komponen aktif supaya clone diam & tak mempengaruhi game
        foreach (var mb in _klon.GetComponentsInChildren<MonoBehaviour>(true)) if (mb != null) mb.enabled = false;
        foreach (var an in _klon.GetComponentsInChildren<Animator>(true)) if (an != null) an.enabled = false;
        foreach (var co in _klon.GetComponentsInChildren<Collider2D>(true)) if (co != null) co.enabled = false;
        foreach (var rb in _klon.GetComponentsInChildren<Rigidbody2D>(true)) if (rb != null) rb.simulated = false;

        Transform t = _klon.transform;
        t.SetParent(_panggung.transform, false);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // hadapkan normal (x positif) supaya tidak terbalik
        Vector3 s = t.localScale;
        s.x = Mathf.Abs(s.x);
        t.localScale = s;

        FramingKamera();
        return true;
    }

    static Transform CariRig()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p == null) return null;
        foreach (Transform anak in p.transform)
            if (anak.name.Contains("_Character_")) return anak;
        return null;
    }

    static void FramingKamera()
    {
        Renderer[] rs = _klon.GetComponentsInChildren<Renderer>();
        if (rs == null || rs.Length == 0) return;

        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);

        Vector3 c = b.center;
        _cam.transform.position = new Vector3(c.x, c.y, c.z - 10f);
        _cam.transform.rotation = Quaternion.identity;

        float pad = 1.15f;
        float half = Mathf.Max(b.extents.x, b.extents.y) * pad;
        if (half < 0.01f) half = 1f;
        _cam.orthographicSize = half;
        _cam.aspect = 1f;
    }
}
