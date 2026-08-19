using UnityEngine;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour
{
    // Arah joystick (-1..1). Dibaca oleh PlayerMovement.
    public static Vector2 Direction = Vector2.zero;

    [Header("Pengaturan (opsional)")]
    public float skala = 1f;                              // besar-kecil joystick (1 = normal)
    [Range(0f, 1f)] public float transparansi = 0.25f;   // transparansi lingkaran

    private RectTransform bg;
    private RectTransform handle;
    private float ukuran;     // diameter background (piksel)
    private float margin;     // jarak pusat dari pojok kiri-bawah (piksel)
    private float maxHandle;  // jarak maksimal geser handle (piksel)
    private Vector2 pusat;    // posisi pusat joystick di layar (piksel)
    private int pointerId = -99; // -99 = nonaktif, -1 = mouse, >=0 = jari

    // Otomatis dibuat saat game mulai (tanpa setup di Editor)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
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
        ukuran = h * 0.20f * skala;
        margin = h * 0.14f * skala;
        maxHandle = ukuran * 0.42f;
        pusat = new Vector2(margin, margin);
        BuildUI();
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

        Sprite lingkaran = MakeCircle(128);

        // background joystick
        GameObject bgo = new GameObject("JoyBG");
        bgo.transform.SetParent(cgo.transform, false);
        Image bgImg = bgo.AddComponent<Image>();
        bgImg.sprite = lingkaran;
        bgImg.color = new Color(1f, 1f, 1f, transparansi);
        bgImg.raycastTarget = false;
        bg = bgo.GetComponent<RectTransform>();
        bg.anchorMin = bg.anchorMax = Vector2.zero;
        bg.pivot = new Vector2(0.5f, 0.5f);
        bg.sizeDelta = new Vector2(ukuran, ukuran);
        bg.anchoredPosition = new Vector2(margin, margin);

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
            if (t.phase == TouchPhase.Began && Dekat(t.position))
            {
                pointerId = t.fingerId;
                GeserHandle(t.position);
                return;
            }
        }
        // mouse (tes di editor/PC)
        if (Input.GetMouseButtonDown(0) && Dekat(Input.mousePosition))
        {
            pointerId = -1;
            GeserHandle(Input.mousePosition);
        }
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
        Direction = offset / maxHandle;
    }

    void Lepas()
    {
        pointerId = -99;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
    }

    bool Dekat(Vector2 p)
    {
        return Vector2.Distance(p, pusat) <= ukuran * 0.75f;
    }

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