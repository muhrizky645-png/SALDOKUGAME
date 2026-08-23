using UnityEngine;

// Membuat pratinjau SELURUH BADAN karakter untuk menu Home. Caranya: meng-clone rig
// pemain (yang SUDAH dirakit lengkap: badan, kepala, kaki, senjata di scene) ke sebuah
// "panggung" jauh, lalu merendernya dengan kamera khusus ke RenderTexture. Dengan begitu
// proporsi & posisi tiap bagian pasti benar (persis seperti saat main), dan gameplay
// tidak terganggu karena clone berada jauh dari arena serta script-nya dimatikan.
public static class PratinjauKarakter
{
    static Camera _cam;
    static RenderTexture _rt;
    static GameObject _panggung;
    static GameObject _klon;
    static int _idxTerakhir = -999;

    static readonly Vector3 PosPanggung = new Vector3(10000f, 10000f, 0f);

    // Kembalikan tekstur pratinjau untuk karakter idx (atau null bila rig belum ada).
    public static Texture Ambil(int idx)
    {
        Siapkan();
        if (_cam == null || _rt == null) return null;

        if (idx != _idxTerakhir || _klon == null)
        {
            if (!BangunKlon()) return null;
            _idxTerakhir = idx;
        }

        _cam.Render();
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
            _cam.cullingMask = ~0;      // hanya clone yang ada di panggung ini
            _cam.nearClipPlane = 0.01f;
            _cam.farClipPlane = 100f;
            _cam.targetTexture = _rt;
            _cam.enabled = false;       // dirender manual lewat _cam.Render()
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
