using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================================
//  SALDOKU LAST STAND - SISTEM DUA MATA UANG (Permata & Koin)
//  Mengikuti pola manager lain (bootstrap + sceneLoaded singleton).
//
//   * PERMATA : mata uang IN-GAME lokal (PlayerPrefs). Didapat dari drop
//               musuh (mirip XP, tapi lebih jarang). Dipakai beli buff di TOKO.
//   * KOIN    : cermin poin SALDOKU (1 Koin = 1 poin). READ-ONLY di game;
//               hanya server yang menambah (via SSV iklan). Terkunci sampai
//               akun SALDOKU dihubungkan.
// =====================================================================
public class MataUang : MonoBehaviour
{
    public static MataUang Instance;

    const string PP_PERMATA    = "permata";
    const string PP_KOIN_CACHE = "koin_cache";
    const string PP_LINKED     = "saldoku_linked";

    int  permata;
    long koin;
    bool linked;
    bool online;

    public int  Permata   { get { return permata; } }
    public long Koin      { get { return koin; } }
    public bool Terhubung { get { return linked; } }
    public bool Online    { get { return online; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        Buat();
        SceneManager.sceneLoaded += (s, m) => Buat();
    }

    static void Buat()
    {
        if (Instance == null) new GameObject("MataUang", typeof(MataUang));
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(