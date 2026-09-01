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
    private AudioClip cBossMuncul, cMenang, cLedak, cAuraZap;

    // throttle: batasi frekuensi bunyi 'player kena' biar tak menumpuk "brebet" saat dikepung musuh
    float tKenaTerakhir = -1f;
    const float JEDA_KENA = 0.12f;
    // throttle bunyi 'musuh kena' biar tak brebet saat spam tembak ke kerumunan
    float tMusuhKenaTerakhir = -1f;
    const float JEDA_MUSUH = 0.05f;
    // throttle bunyi 'aura setrum' (jaga-jaga kalau ada banyak sumber aura)
    float tAuraTerakhir = -1f;
    const float JEDA_AURA = 0.1f;

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
    public static void MusuhKena()
    {
        if (Instance == null || MuteEfek) return;
        // Spam tembak ke kerumunan: jangan tumpuk suara (biar tidak "brebet").
        if (Time.unscaledTime - Instance.tMusuhKenaTerakhir < JEDA_MUSUH) return;
        Instance.tMusuhKenaTerakhir = Time.unscaledTime;
        Play(Instance.cMusuhKena);
    }
    // Bunyi setrum aura: dipanggil SEKALI tiap denyut aura (bukan per musuh),
    // jadi terdengar sebagai "zzap" berirama, bukan dengungan brisik.
    public static void AuraZap()
    {
        if (Instance == null || MuteEfek) return;
        if (Time.unscaledTime - Instance.tAuraTerakhir < JEDA_AURA) return;
        Instance.tAuraTerakhir = Time.unscaledTime;
        Play(Instance.cAuraZap);
    }
    public static void MusuhMati()  { Play(Instance ? Instance.cMusuhMati : null); }
    public static void AmbilXp()    { Play(Instance ? Instance.cAmbilXp : null); }
    public static void LevelUp()    { Play(Instance ? Instance.cLevelUp : null); }
    public static void PlayerKena()
    {
        if (Instance == null || MuteEfek) return;
        // Saat dikepung / kena beruntun, jangan tumpuk suara (biar tidak "brebet").
        if (Time.unscaledTime - Instance.tKenaTerakhir < JEDA_KENA) return;
        Instance.tKenaTerakhir = Time.unscaledTime;
        Play(Instance.cKena);
    }
    public static void GameOver()   { Play(Instance ? Instance.cGameOver : null); }
    public static void Klik()       { Play(Instance ? Instance.cKlik : null); }
    public static void BossMuncul() { Play(Instance ? Instance.cBossMuncul : null); }
    public static void Menang()     { Play(Instance ? Instance.cMenang : null); }
    // Ledakan bom: dentuman berat, dipanggil ItemLapangan saat bom dipungut.
    public static void Bom()        { Play(Instance ? Instance.cLedak : null); }

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
        cLedak     = SuaraLedak();
        cAuraZap   = SuaraZap();

        // level up = arpeggio naik (C5 E5 G5 C6)
        float[] b = new float[(int)(0.40f * SR)];
        float[] fs = { 523.25f, 659.25f, 783.99f, 1046.5f };
        for (int i = 0; i < fs.Length; i++)
            TulisNada(b, (int)(i * 0.09f * SR), fs[i], 0.12f, 0.30f, 0);
        cLevelUp = BuatClip(b);

        // boss muncul = motif rendah menegangkan (turun) + drone bass panjang
        float[] bb = new float[(int)(0.9f * SR)];
        float[] bf = { 130.81f, 123.47f, 110.00f, 98.00f }; // C3 B2 A2 G2 (turun)
        for (int i = 0; i < bf.Length; i++)
            TulisNada(bb, (int)(i * 0.16f * SR), bf[i], 0.28f, 0.34f, 2);
        TulisNada(bb, 0, 55f, 0.9f, 0.20f, 2); // A1 drone biar terasa "besar"
        cBossMuncul = BuatClip(bb);

        // menang = fanfare naik + akor C mayor ditahan di akhir
        float[] mb = new float[(int)(1.1f * SR)];
        float[] mf = { 523.25f, 659.25f, 783.99f, 1046.5f }; // C5 E5 G5 C6
        for (int i = 0; i < mf.Length; i++)
            TulisNada(mb, (int)(i * 0.11f * SR), mf[i], 0.14f, 0.28f, 0);
        int akhir = (int)(0.5f * SR);
        TulisNada(mb, akhir, 523.25f, 0.55f, 0.20f, 1);
        TulisNada(mb, akhir, 659.25f, 0.55f, 0.18f, 1);
        TulisNada(mb, akhir, 783.99f, 0.55f, 0.18f, 1);
        TulisNada(mb, akhir, 1046.5f, 0.55f, 0.16f, 1);
        cMenang = BuatClip(mb);
    }

    // ledakan bom = dentuman rendah (frekuensi meluncur turun) + derau, meluruh
    // cepat dan berat. Sengaja beda jauh dari "tass" tembakan: panjang, penuh
    // bass, dengan hentakan tajam di awal biar terasa "BOOM".
    AudioClip SuaraLedak()
    {
        int n = (int)(0.55f * SR);
        float[] b = new float[n];
        float fase = 0f;
        for (int i = 0; i < n; i++)
        {
            float prog = (float)i / n;
            float f = Mathf.Lerp(115f, 35f, prog);       // rumble turun 115 -> 35 Hz
            fase += 2f * Mathf.PI * f / SR;
            float sine = Mathf.Sin(fase);
            float noise = Random.value * 2f - 1f;
            float attack = Mathf.Min(1f, i / (0.005f * SR)); // hentakan tajam di awal
            float decay = (1f - prog) * (1f - prog);         // meluruh kuadratik
            float env = attack * decay;
            b[i] = (sine * 0.8f + noise * 0.45f) * env;
        }
        return BuatClip(b);
    }

    // setrum listrik aura: luncuran nada turun cepat + sedikit desis, meluruh
    // tajam. Terasa "zzap!" yang renyah dan garang tanpa cempreng. Dibunyikan
    // sekali tiap denyut aura, jadi jadi identitas suara senjata setrum.
    AudioClip SuaraZap()
    {
        int n = (int)(0.12f * SR);
        float[] b = new float[n];
        float fase = 0f;
        for (int i = 0; i < n; i++)
        {
            float prog = (float)i / n;
            float f = Mathf.Lerp(1400f, 320f, prog);         // luncur turun = "zzeeuu"
            fase += 2f * Mathf.PI * f / SR;
            float sine = Mathf.Sin(fase);
            float noise = (Random.value * 2f - 1f) * 0.35f;  // desis listrik
            float env = Mathf.Min(1f, i / (0.003f * SR)) * (1f - prog) * (1f - prog);
            b[i] = (sine * 0.7f + noise) * env * 0.5f;
        }
        return BuatClip(b);
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
