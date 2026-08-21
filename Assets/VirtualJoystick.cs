using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VirtualJoystick : MonoBehaviour
{
    // Arah joystick (-1..1). Dibaca oleh PlayerMovement.
    public static Vector2 Direction = Vector2.zero;

    [Header("Pengaturan (opsional)")]
    public float skala = 0.75f;                          // besar-kecil joystick (1 = normal, 0.75 = 3/4)
    [Range(0f, 1f)] public float transparansi = 0.25f;   // transparansi lingkaran saat muncul (kecil = lebih transparan)
    [Range(1f, 3f)] public float sensitivitas = 1.6f;    // makin besar = makin responsif (arah cepat penuh)
    [Range(0.1f, 1f)] public float areaAktif = 1f;       // bagian layar dari kiri yang bisa dipakai (1 = seluruh layar)
    public bool selaluTersembunyi = false;               // true = gambar joystick tidak pernah muncul (kontrol geser murni)

    private RectTransform bg;
    private RectTransform handle;
    private CanvasGroup group;   // untuk menyembunyikan/menampilkan joystick
    private float ukuran;        // diameter background (piksel)
    private float maxHandle;     // jarak maksimal geser handle (piksel)
    private Vector2 pusat;       // titik awal sentuh (pusat joystick)
    private int pointerId = -99; // -99 = nonaktif, -1 = mouse, >=0 = jari

    // Otomatis dibuat saat game mulai DAN tiap scene di-reload (tanpa setup di Editor)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (scene, mode) => Buat();
    }

    static void Buat()
    {
        if (FindObjectOfType<VirtualJoystick>() == null)
        {
            GameObject go = new GameObject("VirtualJoystick");
            go.AddComponent<VirtualJoystick>();
        }
    }

    void Start()
    {
        float h = Screen.height;
        ukuran = h * 0.22f * skala;
        maxHandle = ukuran * 0.42f;
        BuildUI();
        Sembunyikan(); // joystick tidak terlihat sampai layar disentuh
    }

    void BuildUI()
    {
        // Canvas overlay (1 unit = 1 piksel layar)
        GameObject cgo = new GameObject("JoystickCanvas");
        Canvas canvas = cgo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = cgo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1f;
        group = cgo.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        Sprite lingkaran = MakeCircle(128);

        // background joystick
        GameObject bgo = new GameObject("JoyBG");
        bgo.transform.SetParent(cgo.transform, false);
        Image bgImg = bgo.AddComponent<Image>();
        bgImg.sprite = lingkaran;
        bgImg.color = new Color(1f, 1f, 1f, transparansi);
        bgImg.raycastTarget = false;
        bg = bgo.GetComponent<RectTransform>();
        bg.anchorMin = bg.anchorMax = Vector2.zero; // titik acuan = pojok kiri-bawah
        bg.pivot = new Vector2(0.5f, 0.5f);
        bg.sizeDelta = new Vector2(ukuran, ukuran);

        // handle (bulatan yang digeser)
        GameObject hgo = new GameObject("JoyHandle");
        hgo.transform.SetParent(bgo.transform, false);
        Image hImg = hgo.AddComponent<Image>();
        hImg.sprite = lingkaran;
        hImg.color = new Color(1f, 1f, 1f, Mathf.Clamp01(transparansi + 0.35f));
        hImg.raycastTarget = false;
        handle = hgo.GetComponent<RectTransform>();
        handle.anchorMin = handle.anchorMax = new Vector2(0.5f, 0.5f);
        handle.pivot = new Vector2(0.5f, 0.5f);
        handle.sizeDelta = new Vector2(ukuran * 0.5f, ukuran * 0.5f);
        handle.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        // nonaktif saat menu awal / menu jeda / game over
        if (!GameMenu.SedangMain || GameMenu.SedangJeda || PlayerHealth.GameOver)
        {
            if (pointerId != -99) Lepas();
            return;
        }

        if (pointerId == -99) CekMulaiTekan();

        if (pointerId != -99)
        {
            Vector2 pos;
            if (AmbilPosisi(out pos)) GeserHandle(pos);
            else Lepas();
        }
    }

    void CekMulaiTekan()
    {
        // sentuhan HP
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began && DiArea(t.position))
            {
                Aktifkan(t.fingerId, t.position);
                return;
            }
        }
        // mouse (tes di editor/PC)
        if (Input.GetMouseButtonDown(0) && DiArea(Input.mousePosition))
        {
            Aktifkan(-1, Input.mousePosition);
        }
    }

    // Joystick muncul tepat di titik sentuh
    void Aktifkan(int id, Vector2 titik)
    {
        pointerId = id;
        pusat = titik;
        bg.anchoredPosition = titik;
        handle.anchoredPosition = Vector2.zero;
        Tampilkan();
    }

    bool AmbilPosisi(out Vector2 pos)
    {
        pos = Vector2.zero;
        if (pointerId == -1)
        {
            if (Input.GetMouseButton(0)) { pos = Input.mousePosition; return true; }
            return false;
        }
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.fingerId == pointerId)
            {
                if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) return false;
                pos = t.position;
                return true;
            }
        }
        return false;
    }

    void GeserHandle(Vector2 screenPos)
    {
        Vector2 offset = screenPos - pusat;
        offset = Vector2.ClampMagnitude(offset, maxHandle);
        if (handle != null) handle.anchoredPosition = offset;
        // lebih responsif: arah mencapai nilai penuh sebelum handle sampai ke tepi
        Direction = Vector2.ClampMagnitude(offset / maxHandle * sensitivitas, 1f);
    }

    void Lepas()
    {
        pointerId = -99;
        Direction = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        Sembunyikan();
    }

    bool DiArea(Vector2 p)
    {
        return p.x <= Screen.width * areaAktif;
    }

    void Tampilkan() { if (group != null) group.alpha = selaluTersembunyi ? 0f : 1f; }
    void Sembunyikan() { if (group != null) group.alpha = 0f; }

    // Membuat sprite lingkaran lewat kode (tanpa perlu file gambar)
    Sprite MakeCircle(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f;
                float dy = y - r + 0.5f;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(r - d); // tepi halus (anti-alias)
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
