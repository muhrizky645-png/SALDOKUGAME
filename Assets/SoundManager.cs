using UnityEngine;

// Backsound + sound effect yang DI-GENERATE lewat kode (tanpa file audio sama sekali).
// Otomatis dibuat saat game mulai dan tetap hidup antar scene (DontDestroyOnLoad),
// jadi musik tidak putus saat restart.
// Mendukung: volume musik & efek (besar/kecil) + mute terpisah, tersimpan otomatis.
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    const int SR = 22050;            // sample rate
    const float BASE_MUSIK = 0.5f;   // batas atas volume musik (biar tidak terlalu keras)

    // ---- pengaturan suara (0..1) + mute, dibaca/ditulis UI pengaturan ----
    public static float VolMusik = 0.8f;
    public static float VolEfek = 0.9f;
    public static bool MuteMusik = false;
    public static bool MuteEfek = false;

    private AudioSource musik;
    private AudioSource efek;

    private AudioClip cTembak, cMusuhKena, cMusuhMati, cAmbilXp, cLevelUp, cKena, cGameOver, cKlik;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null) new GameObject("SoundManager", typeof(SoundManager));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // musik tetap jalan walau scene di-reload

        // muat pengaturan tersimpan
        VolMusik = PlayerPrefs.GetFloat("volMusik", 0.8f);
        VolEfek = PlayerPrefs.GetFloat("volEfek", 0.9f);
        MuteMusik = PlayerPrefs.GetInt("muteMusik", 0) == 1;
        MuteEfek = PlayerPrefs.GetInt("muteEfek", 0) == 1;

        musik = gameObject.AddComponent<AudioSource>();
        musik.loop = true;
        musik.playOnAwake = false;

        efek = gameObject.AddComponent<AudioSource>();
        efek.playOnAwake = false;
        efek.volume = 1f;

        BuatSemuaSuara();
        musik.clip = BuatMusik();
        TerapkanMusik();
        musik.Play();
    }

    void TerapkanMusik()
    {
        if (musik != null) musik.volume = MuteMusik ? 0f : VolMusik * BASE_MUSIK;
    }

    // ---------------- API pengaturan (dipanggil menu) ----------------
    public static void SetVolMusik(float v)
    {
        VolMusik = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat("volMusik", VolMusik);
        PlayerPrefs.Save();
        if (Instance != null) Instance.TerapkanMusik();
    }

    public static void SetVolEfek(float v)
    {
        VolEfek = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat("volEfek", VolEfek);
        PlayerPrefs.Save();
    }

    public static void ToggleMuteMusik()
    {
        MuteMusik = !MuteMusik;
        PlayerPrefs.SetInt("muteMusik", MuteMusik ? 1 : 0);
        PlayerPrefs.Save();
        if (Instance != null) Instance.TerapkanMusik();
    }

    public static void ToggleMuteEfek()
    {
        MuteEfek = !MuteEfek;
        PlayerPrefs.SetInt("muteEfek", MuteEfek ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ---------------- API statis dipanggil script lain ----------------
    public static void Tembak()     { Play(Instance ? Instance.cTembak : null); }
    public static void MusuhKena()  { Play(Instance ? Instance.cMusuhKena : null); }
    public static void MusuhMati()  { Play(Instance ? Instance.cMusuhMati : null); }
    public static void AmbilXp()    { Play(Instance ? Instance.cAmbilXp : null); }
    public static void LevelUp()    { Play(Instance ? Instance.cLevelUp : null); }
    public static void PlayerKena() { Play(Instance ? Instance.cKena : null); }
    public static void GameOver()   { Play(Instance ? Instance.cGameOver : null); }
    public static void Klik()       { Play(Instance ? Instance.cKlik : null); }

    static void Play(AudioClip c)
    {
        if (Instance != null && c != null && !MuteEfek)
            Instance.efek.PlayOneShot(c, VolEfek);
    }

    // ---------------- generator suara ----------------
    void BuatSemuaSuara()
    {
        cTembak    = Sweep(900f, 1500f, 0.07f, 0, 0.30f);
        cMusuhKena = Sweep(260f, 150f, 0.06f, 0, 0.28f);
        cMusuhMati = Derau(0.20f, 0.40f);
        cAmbilXp   = Sweep(700f, 1300f, 0.09f, 1, 0.30f);
        cKena      = Sweep(220f, 80f, 0.18f, 0, 0.40f);
        cGameOver  = Sweep(420f, 110f, 0.60f, 2, 0.40f);
        cKlik      = Sweep(680f, 680f, 0.035f, 0, 0.22f);

        // level up = arpeggio naik (C5 E5 G5 C6)
        float[] b = new float[(int)(0.40f * SR)];
        float[] fs = { 523.25f, 659.25f, 783.99f, 1046.5f };
        for (int i = 0; i < fs.Length; i++)
            TulisNada(b, (int)(i * 0.09f * SR), fs[i], 0.12f, 0.30f, 0);
        cLevelUp = BuatClip(b);
    }

    // tulis satu nada ke buffer (additif). tipe: 0=kotak, 1=sinus, 2=segitiga
    void TulisNada(float[] buf, int mulai, float freq, float dur, float vol, int tipe)
    {
        int n = (int)(dur * SR);
        for (int i = 0; i < n && mulai + i < buf.Length; i++)
        {
            float t = (float)i / SR;
            float env = Mathf.Min(1f, i / (0.004f * SR)) * Mathf.Min(1f, (n - i) / (0.03f * SR));
            buf[mulai + i] += Gelombang(tipe, freq, t) * vol * env;
        }
    }

    float Gelombang(int tipe, float freq, float t)
    {
        if (tipe == 1) return Mathf.Sin(2f * Mathf.PI * freq * t);
        if (tipe == 2) { float p = (freq * t) % 1f; return 4f * Mathf.Abs(p - 0.5f) - 1f; }
        return Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t)); // kotak
    }

    // nada dengan frekuensi meluncur (untuk efek pendek)
    AudioClip Sweep(float f0, float f1, float dur, int tipe, float vol)
    {
        int n = (int)(dur * SR);
        float[] b = new float[n];
        float fase = 0f;
        for (int i = 0; i < n; i++)
        {
            float f = Mathf.Lerp(f0, f1, (float)i / n);
            fase += 2f * Mathf.PI * f / SR;
            float w;
            if (tipe == 1) w = Mathf.Sin(fase);
            else if (tipe == 2) { float p = (fase / (2f * Mathf.PI)) % 1f; w = 4f * Mathf.Abs(p - 0.5f) - 1f; }
            else w = Mathf.Sign(Mathf.Sin(fase));
            float env = Mathf.Min(1f, i / (0.003f * SR)) * Mathf.Min(1f, (n - i) / (0.02f * SR));
            b[i] = w * vol * env;
        }
        return BuatClip(b);
    }

    // suara derau (noise) untuk ledakan/musuh mati
    AudioClip Derau(float dur, float vol)
    {
        int n = (int)(dur * SR);
        float[] b = new float[n];
        for (int i = 0; i < n; i++)
        {
            float env = 1f - (float)i / n; // meluruh
            b[i] = (Random.value * 2f - 1f) * vol * env * env;
        }
        return BuatClip(b);
    }

    AudioClip BuatMusik()
    {
        float[] buf = new float[(int)(8f * SR)];
        int R = -99; // tanda diam
        // melodi (offset semiton dari A4 = 440Hz), tiap langkah 0.25 detik
        int[] mel = {
            0,3,7,12, 7,3,7,3,        // Am
            -4,0,5,8, 5,0,-4,0,       // F
            3,7,10,15, 10,7,3,7,      // C
            -2,2,5,10, 5,2,-2,2       // G
        };
        for (int i = 0; i < mel.Length; i++)
        {
            if (mel[i] == R) continue;
            float f = 440f * Mathf.Pow(2f, mel[i] / 12f);
            TulisNada(buf, (int)(i * 0.25f * SR), f, 0.24f, 0.14f, 0);
        }
        // bass (segitiga), 4 ketukan tiap bar
        int[] bassRoot = { -24, -28, -21, -26 };
        for (int bar = 0; bar < 4; bar++)
            for (int k = 0; k < 4; k++)
            {
                float f = 440f * Mathf.Pow(2f, bassRoot[bar] / 12f);
                TulisNada(buf, (int)((bar * 2f + k * 0.5f) * SR), f, 0.45f, 0.16f, 2);
            }
        return BuatClip(buf);
    }

    AudioClip BuatClip(float[] b)
    {
        for (int i = 0; i < b.Length; i++) b[i] = Mathf.Clamp(b[i], -1f, 1f);
        AudioClip c = AudioClip.Create("gen", b.Length, 1, SR, false);
        c.SetData(b, 0);
        return c;
    }
}
