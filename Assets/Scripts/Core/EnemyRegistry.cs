using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// EnemyRegistry
// ----------------------------------------------------------------------------
// Daftar pusat semua musuh yang hidup, plus spatial grid sederhana untuk
// menjawab pertanyaan "musuh mana yang dekat dengan titik ini?" dengan cepat.
//
// KENAPA ADA:
// Sebelumnya kode memakai GameObject.FindGameObjectsWithTag("Enemy").
// Fungsi itu menyisir SELURUH scene dan mengalokasikan array BARU setiap kali
// dipanggil. SenjataManager memanggilnya 3x (aura tiap 0.4 detik, tiap
// gelombang roket, dan MusuhTerdekat) dan ZombieSpawner memanggilnya tiap
// spawn. Dengan 300 musuh hidup itu jadi ratusan ribu operasi + sampah GC
// per detik, yang muncul sebagai patah-patah (stutter) di HP kelas menengah.
//
// Registry ini O(1) untuk daftar/hapus, dan query radius hanya memeriksa sel
// grid yang bersinggungan dengan radius, bukan semua musuh.
//
// CARA PAKAI:
//   EnemyRegistry.Jumlah                                  -> berapa musuh hidup
//   EnemyRegistry.Terdekat(pos, radius, kecuali)          -> 1 musuh terdekat
//   EnemyRegistry.DalamRadius(pos, radius, hasil)         -> semua dalam radius
//   EnemyRegistry.NTerdekat(pos, radius, n, hasil)        -> n terdekat, terurut
//
// Pakai EnemyRegistry.Buffer sebagai penampung supaya tidak alokasi List baru.
// ============================================================================
public static class EnemyRegistry
{
    // Ukuran satu sel grid dalam satuan dunia. 4 unit kira-kira seperempat
    // lebar layar pada zoom kamera 10, jadi query radius 2 biasanya hanya
    // menyentuh 1-4 sel.
    const float UkuranSel = 4f;
    const int Offset = 100000; // supaya koordinat sel negatif tetap unik

    static readonly List<EnemyChase> hidup = new List<EnemyChase>(512);
    static readonly Dictionary<long, List<EnemyChase>> grid = new Dictionary<long, List<EnemyChase>>(256);
    static readonly Stack<List<EnemyChase>> kolamSel = new Stack<List<EnemyChase>>();
    static int frameGridTerakhir = -1;

    // Penampung bersama supaya pemanggil tidak perlu alokasi List sendiri.
    // JANGAN simpan referensinya lintas frame.
    public static readonly List<EnemyChase> Buffer = new List<EnemyChase>(256);

    public static int Jumlah { get { return hidup.Count; } }

    // Akses langsung ke daftar mentah. Hanya untuk dibaca.
    public static List<EnemyChase> Semua { get { return hidup; } }

    // ---------------------------------------------------------------- daftar

    public static void Daftar(EnemyChase e)
    {
        if (e == null) return;
        if (e.IndexRegistry >= 0) return; // sudah terdaftar
        e.IndexRegistry = hidup.Count;
        hidup.Add(e);
    }

    public static void Hapus(EnemyChase e)
    {
        if (e == null) return;

        int i = e.IndexRegistry;
        e.IndexRegistry = -1;
        if (i < 0 || i >= hidup.Count) return;

        // Jaga-jaga kalau index sempat tidak sinkron.
        if (!ReferenceEquals(hidup[i], e))
        {
            i = hidup.IndexOf(e);
            if (i < 0) return;
        }

        BuangIndex(i);
    }

    // Hapus dengan cara tukar-dengan-terakhir: O(1), tidak menggeser array.
    static void BuangIndex(int i)
    {
        int akhir = hidup.Count - 1;
        hidup[i] = hidup[akhir];
        if (hidup[i] != null) hidup[i].IndexRegistry = i;
        hidup.RemoveAt(akhir);
    }

    // Panggil saat memulai / mengulang permainan supaya tidak ada sisa
    // referensi dari sesi sebelumnya.
    public static void Bersihkan()
    {
        for (int i = 0; i < hidup.Count; i++)
            if (hidup[i] != null) hidup[i].IndexRegistry = -1;
        hidup.Clear();

        foreach (var kv in grid) { kv.Value.Clear(); kolamSel.Push(kv.Value); }
        grid.Clear();
        frameGridTerakhir = -1;
    }

    // ------------------------------------------------------------------ grid

    static int KoordSel(float v) { return Mathf.FloorToInt(v / UkuranSel); }

    static long Kunci(int cx, int cy)
    {
        return ((long)(cx + Offset) << 32) | (uint)(cy + Offset);
    }

    // Grid dibangun ulang maksimal SEKALI per frame, dan hanya kalau ada yang
    // benar-benar bertanya. Kalau tidak ada senjata yang query di frame ini,
    // biayanya nol.
    static void PastikanGrid()
    {
        if (frameGridTerakhir == Time.frameCount) return;
        frameGridTerakhir = Time.frameCount;

        foreach (var kv in grid) { kv.Value.Clear(); kolamSel.Push(kv.Value); }
        grid.Clear();

        // Mundur, supaya aman saat membuang entri yang sudah hancur.
        for (int i = hidup.Count - 1; i >= 0; i--)
        {
            EnemyChase e = hidup[i];

            // Musuh yang di-Destroy tanpa sempat lapor akan jadi "null palsu"
            // milik Unity. Bersihkan di sini.
            if (e == null) { BuangIndex(i); continue; }

            Vector3 p = e.transform.position;
            long k = Kunci(KoordSel(p.x), KoordSel(p.y));

            List<EnemyChase> sel;
            if (!grid.TryGetValue(k, out sel))
            {
                sel = kolamSel.Count > 0 ? kolamSel.Pop() : new List<EnemyChase>(16);
                grid[k] = sel;
            }
            sel.Add(e);
        }
    }

    // ----------------------------------------------------------------- query

    // Semua musuh hidup di dalam radius. Hasil ditulis ke 'hasil'.
    // Mengembalikan jumlah yang ketemu.
    public static int DalamRadius(Vector3 pusat, float radius, List<EnemyChase> hasil)
    {
        hasil.Clear();
        if (hidup.Count == 0) return 0;
        PastikanGrid();

        float r2 = radius * radius;
        int x0 = KoordSel(pusat.x - radius), x1 = KoordSel(pusat.x + radius);
        int y0 = KoordSel(pusat.y - radius), y1 = KoordSel(pusat.y + radius);

        for (int cx = x0; cx <= x1; cx++)
        {
            for (int cy = y0; cy <= y1; cy++)
            {
                List<EnemyChase> sel;
                if (!grid.TryGetValue(Kunci(cx, cy), out sel)) continue;

                for (int i = 0; i < sel.Count; i++)
                {
                    EnemyChase e = sel[i];
                    if (e == null || e.SudahMati) continue;
                    if ((e.transform.position - pusat).sqrMagnitude <= r2) hasil.Add(e);
                }
            }
        }
        return hasil.Count;
    }

    // Satu musuh terdekat. Mencari per cincin sel dari dalam ke luar, jadi
    // biasanya berhenti setelah memeriksa segelintir musuh saja.
    public static EnemyChase Terdekat(Vector3 pusat, float radiusMaks, EnemyChase kecuali)
    {
        if (hidup.Count == 0) return null;
        PastikanGrid();

        int px = KoordSel(pusat.x), py = KoordSel(pusat.y);
        int cincinMaks = Mathf.Max(1, Mathf.CeilToInt(radiusMaks / UkuranSel));

        EnemyChase terbaik = null;
        float jarakTerbaik = radiusMaks * radiusMaks;
        int cincinKetemu = -1;

        for (int cincin = 0; cincin <= cincinMaks; cincin++)
        {
            for (int dx = -cincin; dx <= cincin; dx++)
            {
                for (int dy = -cincin; dy <= cincin; dy++)
                {
                    // Hanya tepi cincin; bagian dalam sudah diperiksa.
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != cincin) continue;

                    List<EnemyChase> sel;
                    if (!grid.TryGetValue(Kunci(px + dx, py + dy), out sel)) continue;

                    for (int i = 0; i < sel.Count; i++)
                    {
                        EnemyChase e = sel[i];
                        if (e == null || e.SudahMati) continue;
                        if (!ReferenceEquals(kecuali, null) && ReferenceEquals(e, kecuali)) continue;

                        float d = (e.transform.position - pusat).sqrMagnitude;
                        if (d < jarakTerbaik) { jarakTerbaik = d; terbaik = e; }
                    }
                }
            }

            if (terbaik != null && cincinKetemu < 0) cincinKetemu = cincin;

            // Sudah ketemu? Periksa satu cincin lagi (bisa saja ada yang lebih
            // dekat di sel tetangga), lalu berhenti.
            if (cincinKetemu >= 0 && cincin >= cincinKetemu + 1) break;
        }

        return terbaik;
    }

    public static EnemyChase Terdekat(Vector3 pusat, float radiusMaks)
    {
        return Terdekat(pusat, radiusMaks, null);
    }

    // N musuh terdekat, sudah terurut dari yang paling dekat.
    // Dipakai roket supaya tiap roket mengejar target berbeda.
    public static int NTerdekat(Vector3 pusat, float radiusMaks, int n, List<EnemyChase> hasil)
    {
        DalamRadius(pusat, radiusMaks, hasil);
        if (hasil.Count == 0) return 0;

        pusatUrut = pusat;
        hasil.Sort(pembanding);

        if (n > 0 && hasil.Count > n) hasil.RemoveRange(n, hasil.Count - n);
        return hasil.Count;
    }

    // Comparison disimpan statis supaya Sort tidak mengalokasikan delegate baru
    // setiap pemanggilan.
    static Vector3 pusatUrut;
    static readonly System.Comparison<EnemyChase> pembanding = delegate (EnemyChase a, EnemyChase b)
    {
        float da = (a.transform.position - pusatUrut).sqrMagnitude;
        float db = (b.transform.position - pusatUrut).sqrMagnitude;
        return da.CompareTo(db);
    };
}
