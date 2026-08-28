using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;   // SortingGroup (untuk occlusion)
using UnityEngine.SceneManagement;

// ============================================================================
//  RINTANGAN ARENA - pohon / batu / semak sebagai OBJEK NYATA yang:
//   1) TIMBUL/MUNCUL  -> sprite berdiri (pivot di kaki) + bayangan lembut
//   2) MENABRAK       -> player TIDAK bisa menembus (didorong keluar)
//   3) OCCLUSION      -> kalau player ADA DI BELAKANG pohon/batu, player
//                        ketutupan (Y-sort: yang lebih ke bawah tampil depan)
//
//  Dunia TAK TERBATAS: objek di-spawn per \"sel\" di sekitar kamera secara
//  deterministik (seed per sel) lalu didaur ulang saat kamera menjauh.
//  Sepenuhnya OTOMATIS (RuntimeInitializeOnLoadMethod), tanpa setup Editor.
//  Sprite prosedural memakai HideAndDontSave.
// ============================================================================
public class RintanganArena : MonoBehaviour
{
    public static RintanganArena Instance;

    const float SEL = 8f;   // ukuran sel dunia
    const int   RADIUS = 3; // berapa sel di sekeliling kamera yang aktif

    // daftar penghalang solid (untuk dorong player keluar)
    public struct Halangan { public Vector2 pos; public float r; }
    public static readonly List<Halangan> Halangans = new List<Halangan>();

    static Sprite _sPohon, _sBatu, _sSemak;

    Camera cam;
    Transform pemain;
    Transform wadah;
    readonly Dictionary<long, List<GameObject>> _aktif = new Dictionary<long, List<GameObject>>();
    float _scanEnemy = 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("RintanganArena", typeof(RintanganArena));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Halangans.Clear();
        _aktif.Clear();
        wadah = new GameObject("Rintangan").transform;
        if (_sPohon == null) { _sPohon = BuatPohon(); _sBatu = BuatBatu(); _sSemak = BuatSemak(); }
    }

    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        // beri tahu UrutanY posisi kamera (untuk Y-sort relatif kamera)
        UrutanY.CamY = cam.transform.position.y;

        // pastikan PLAYER punya sorting (occlusion) + komponen tabrakan
        if (pemain == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) pemain = p.transform;
        }
        if (pemain != null)
        {
            if (pemain.GetComponent<UrutanY>() == null) { var u = pemain.gameObject.AddComponent<UrutanY>(); u.offsetY = -0.35f; }
            if (pemain.GetComponent<PemainTabrak>() == null) { var t = pemain.gameObject.AddComponent<PemainTabrak>(); t.radius = 0.28f; }
        }

        // beri Y-sort juga ke musuh (scan berkala) supaya ikut ketutup pohon
        _scanEnemy -= Time.unscaledDeltaTime;
        if (_scanEnemy <= 0f)
        {
            _scanEnemy = 0.25f;
            var musuh = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var m in musuh)
                if (m.GetComponent<UrutanY>() == null) { var u = m.AddComponent<UrutanY>(); u.offsetY = -0.25f; }
        }

        // ===== kelola sel di sekitar kamera =====
        int ccx = Mathf.FloorToInt(cam.transform.position.x / SEL);
        int ccy = Mathf.FloorToInt(cam.transform.position.y / SEL);

        HashSet<long> perlu = new HashSet<long>();
        for (int dy = -RADIUS; dy <= RADIUS; dy++)
            for (int dx = -RADIUS; dx <= RADIUS; dx++)
            {
                int cx = ccx + dx, cy = ccy + dy;
                long key = Key(cx, cy);
                perlu.Add(key);
                if (!_aktif.ContainsKey(key)) _aktif[key] = SpawnSel(cx, cy);
            }

        // buang sel yang sudah jauh
        List<long> buang = null;
        foreach (var kv in _aktif)
            if (!perlu.Contains(kv.Key)) { (buang ?? (buang = new List<long>())).Add(kv.Key); }
        if (buang != null)
            foreach (var key in buang)
            {
                foreach (var go in _aktif[key]) if (go != null) Destroy(go);
                _aktif.Remove(key);
            }

        // ===== bangun ulang daftar Halangan (solid) untuk tabrakan =====
        Halangans.Clear();
        foreach (var kv in _aktif)
            foreach (var go in kv.Value)
            {
                if (go == null) continue;
                var ri = go.GetComponent<Rintang>();
                if (ri != null && ri.solid)
                    Halangans.Add(new Halangan { pos = go.transform.position, r = ri.radius });
            }
    }

    static long Key(int x, int y) { return ((long)(x + 100000)) * 200000L + (y + 100000); }

    List<GameObject> SpawnSel(int cx, int cy)
    {
        var list = new List<GameObject>();
        System.Random r = new System.Random((cx * 73856093) ^ (cy * 19349663) ^ 2026);
        int pohon = r.NextDouble() < 0.30 ? 1 : 0;
        int batu = r.Next(0, 2);
        int semak = r.Next(0, 2);
        for (int i = 0; i < pohon; i++) list.Add(Buat1(_sPohon, Pos(r, cx, cy), (float)(0.85 + r.NextDouble() * 0.55), 0.34f));
        for (int i = 0; i < batu; i++)  list.Add(Buat1(_sBatu,  Pos(r, cx, cy), (float)(0.70 + r.NextDouble() * 0.60), 0.36f));
        for (int i = 0; i < semak; i++) list.Add(Buat1(_sSemak, Pos(r, cx, cy), (float)(0.75 + r.NextDouble() * 0.55), 0.28f));
        return list;
    }

    Vector3 Pos(System.Random r, int cx, int cy)
    {
        float x = (cx + (float)r.NextDouble()) * SEL;
        float y = (cy + (float)r.NextDouble()) * SEL;
        return new Vector3(x, y, 0f);
    }

    GameObject Buat1(Sprite spr, Vector3 pos, float skala, float radBasis)
    {
        GameObject go = new GameObject("Rintang");
        go.transform.SetParent(wadah);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * skala;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        go.AddComponent<UrutanY>();               // occlusion via Y-sort
        var ri = go.AddComponent<Rintang>();      // data tabrakan
        ri.solid = true;
        ri.radius = radBasis * skala;
        return go;
    }

    // ===================== SPRITE PROSEDURAL (dengan alpha) =====================
    static Color[] _b; static int _w, _h;
    static void Clear(int w, int h) { _w = w; _h = h; _b = new Color[w * h]; }

    // src-over compositing (buffer transparan)
    static void Px(int x, int y, Color c, float a)
    {
        if (a <= 0f) return; if (a > 1f) a = 1f;
        if (x < 0 || y < 0 || x >= _w || y >= _h) return;
        int i = y * _w + x; Color d = _b[i];
        float o = a + d.a * (1f - a);
        if (o <= 0f) { _b[i] = new Color(0, 0, 0, 0); return; }
        _b[i] = new Color(
            (c.r * a + d.r * d.a * (1f - a)) / o,
            (c.g * a + d.g * d.a * (1f - a)) / o,
            (c.b * a + d.b * d.a * (1f - a)) / o, o);
    }

    static void DiscE(float cx, float cy, float rx, float ry, Color isi, Color rim)
    {
        int x0 = Mathf.FloorToInt(cx - rx) - 1, x1 = Mathf.CeilToInt(cx + rx) + 1;
        int y0 = Mathf.FloorToInt(cy - ry) - 1, y1 = Mathf.CeilToInt(cy + ry) + 1;
        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                float dx = (x + 0.5f - cx) / rx, dy = (y + 0.5f - cy) / ry;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d > 1f) continue;
                float a = Mathf.Clamp01((1f - d) * 7f);
                Px(x, y, d > 0.78f ? rim : isi, a);
            }
    }

    static void SoftE(float cx, float cy, float rx, float ry, Color col, float kuat)
    {
        int x0 = Mathf.FloorToInt(cx - rx) - 1, x1 = Mathf.CeilToInt(cx + rx) + 1;
        int y0 = Mathf.FloorToInt(cy - ry) - 1, y1 = Mathf.CeilToInt(cy + ry) + 1;
        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                float dx = (x + 0.5f - cx) / rx, dy = (y + 0.5f - cy) / ry;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d >= 1f) continue;
                float a = (1f - d); a *= a; a *= kuat;
                Px(x, y, col, a);
            }
    }

    static Sprite Jadikan(float pivotYfrac)
    {
        var t = new Texture2D(_w, _h, TextureFormat.RGBA32, false);
        t.hideFlags = HideFlags.HideAndDontSave;
        t.wrapMode = TextureWrapMode.Clamp;
        t.filterMode = FilterMode.Point;
        t.SetPixels(_b); t.Apply();
        var s = Sprite.Create(t, new Rect(0, 0, _w, _h), new Vector2(0.5f, pivotYfrac), 64f, 0, SpriteMeshType.FullRect);
        _b = null;
        return s;
    }

    static Sprite BuatPohon()
    {
        Clear(96, 140);
        float cx = 48f;
        SoftE(cx, 16, 30, 10, Color.black, 0.35f);                 // bayangan kaki
        Color bat = new Color(0.44f, 0.29f, 0.17f), batD = new Color(0.30f, 0.19f, 0.10f);
        DiscE(cx, 44, 9, 30, bat, batD);                            // batang
        Color kan = new Color(0.28f, 0.55f, 0.24f), rim = new Color(0.18f, 0.38f, 0.15f), hi = new Color(0.46f, 0.72f, 0.32f);
        DiscE(cx, 96, 34, 34, kan, rim);                            // kanopi
        DiscE(cx - 24, 84, 22, 22, kan, rim);
        DiscE(cx + 24, 84, 22, 22, kan, rim);
        DiscE(cx, 118, 24, 24, kan, rim);
        SoftE(cx - 12, 108, 20, 20, hi, 0.55f);                     // highlight
        return Jadikan(16f / 140f);
    }

    static Sprite BuatBatu()
    {
        Clear(76, 64);
        float cx = 38f;
        SoftE(cx, 12, 26, 8, Color.black, 0.32f);
        Color isi = new Color(0.64f, 0.64f, 0.68f), rim = new Color(0.44f, 0.44f, 0.50f), hi = new Color(0.82f, 0.82f, 0.86f);
        DiscE(cx, 30, 26, 22, isi, rim);
        DiscE(cx + 12, 26, 14, 12, isi, rim);
        SoftE(cx - 8, 22, 12, 10, hi, 0.60f);
        return Jadikan(12f / 64f);
    }

    static Sprite BuatSemak()
    {
        Clear(88, 64);
        float cx = 44f;
        SoftE(cx, 12, 30, 9, Color.black, 0.30f);
        Color isi = new Color(0.32f, 0.60f, 0.26f), rim = new Color(0.20f, 0.42f, 0.16f), hi = new Color(0.50f, 0.76f, 0.34f);
        DiscE(cx, 28, 28, 22, isi, rim);
        DiscE(cx - 16, 24, 16, 14, isi, rim);
        DiscE(cx + 16, 24, 16, 14, isi, rim);
        SoftE(cx - 10, 20, 14, 12, hi, 0.55f);
        return Jadikan(12f / 64f);
    }
}

// ---- Y-sort RELATIF KAMERA: objek lebih ke BAWAH (y kecil) tampil di DEPAN ----
// Basis BESAR + relatif kamera -> nilai selalu positif jauh di atas lantai (-9),
// jadi player/objek TIDAK pernah tenggelam di balik tekstur tanah.
// Memakai SortingGroup supaya rig multi-sprite (player/musuh) jadi satu urutan.
public class UrutanY : MonoBehaviour
{
    public static float CamY = 0f;
    public float offsetY = 0f;
    const int BASIS = 20000;
    SortingGroup sg;
    void Awake()
    {
        sg = GetComponent<SortingGroup>();
        if (sg == null) sg = gameObject.AddComponent<SortingGroup>();
    }
    void LateUpdate()
    {
        if (sg != null)
            sg.sortingOrder = BASIS - Mathf.RoundToInt((transform.position.y + offsetY - CamY) * 100f);
    }
}

// ---- data penghalang solid ----
public class Rintang : MonoBehaviour { public bool solid = true; public float radius = 0.3f; }

// ---- dorong PLAYER keluar dari penghalang solid (player bergerak via transform) ----
public class PemainTabrak : MonoBehaviour
{
    public float radius = 0.28f;
    void LateUpdate()
    {
        if (!GameMenu.SedangMain) return; // jangan ganggu posisi saat di menu
        Vector2 p = transform.position;
        var list = RintanganArena.Halangans;
        for (int i = 0; i < list.Count; i++)
        {
            Vector2 c = list[i].pos;
            float min = list[i].r + radius;
            Vector2 d = p - c;
            float dist = d.magnitude;
            if (dist < min && dist > 0.0001f) p += d / dist * (min - dist);
        }
        transform.position = new Vector3(p.x, p.y, transform.position.z);
    }
}
