using UnityEngine;
using System.Collections.Generic;

// SENJATA: Kilat Rantai.
// Menyambar musuh terdekat, lalu petir MELONCAT ke musuh terdekat berikutnya
// yang belum kena, sampai sejumlah 'lompatan'. Tiap sambaran memberi damage +
// kilatan petir (PetirEfek). Damage sedikit meluruh tiap loncatan supaya ujung
// rantai tidak sekuat awalnya (khas chain-lightning).
//
// Dipanggil dari SenjataManager tiap interval. Semua target dicari lewat
// EnemyRegistry (grid), bukan menyisir seluruh scene.
public static class KilatRantai
{
    // Daftar musuh yang sudah kena pada satu sambaran, supaya petir tidak
    // meloncat bolak-balik ke musuh yang sama. Dipakai ulang biar tak alokasi.
    static readonly List<EnemyChase> sudah = new List<EnemyChase>(24);

    public static void Sambar(Vector3 asal, int dmg, int lompatan, float radiusLompat, Color warna)
    {
        sudah.Clear();
        Vector3 dari = asal;

        for (int hop = 0; hop < lompatan; hop++)
        {
            EnemyChase target = TerdekatBelumKena(dari, radiusLompat);
            if (target == null) break;

            sudah.Add(target);
            Vector3 ke = target.transform.position;
            PetirEfek.Sambar(dari, ke, warna, 0.12f);

            // damage meluruh 15% tiap loncatan (minimal 1)
            int d = Mathf.Max(1, Mathf.RoundToInt(dmg * Mathf.Pow(0.85f, hop)));
            // bunyi=false: satu bunyi "zzap" untuk seluruh rantai (di bawah),
            // bukan per loncatan, biar tidak jadi dengungan brisik.
            target.KenaSerangan(d, false);

            dari = ke;
        }

        if (sudah.Count > 0) SoundManager.AuraZap();
    }

    // Musuh terdekat dalam radius yang BELUM ada di daftar 'sudah'.
    static EnemyChase TerdekatBelumKena(Vector3 pos, float radius)
    {
        int n = EnemyRegistry.DalamRadius(pos, radius, EnemyRegistry.Buffer);
        EnemyChase best = null;
        float min = Mathf.Infinity;
        for (int i = 0; i < n; i++)
        {
            EnemyChase ec = EnemyRegistry.Buffer[i];
            if (ec == null || ec.SudahMati) continue;
            if (sudah.Contains(ec)) continue;
            float d = ((Vector3)ec.transform.position - pos).sqrMagnitude;
            if (d < min) { min = d; best = ec; }
        }
        return best;
    }
}
