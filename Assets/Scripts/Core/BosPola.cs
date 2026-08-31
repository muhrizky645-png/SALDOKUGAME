using System.Collections;
using UnityEngine;

// ============================================================================
//  POLA SERANGAN BOS
// ----------------------------------------------------------------------------
//  Sebelum ini, bos hanyalah musuh biasa yang besar dan tebal. Ia berjalan
//  lurus ke pemain dan itu saja. Tidak ada alasan untuk takut, tidak ada yang
//  perlu dipelajari, dan satu-satunya strategi adalah lari mundur sambil
//  menembak - sama persis seperti melawan zombi biasa.
//
//  Komponen ini memberi bos tiga pola yang bergantian:
//
//    1. TEMBAKAN MELINGKAR - proyektil ke segala arah, memaksa pemain
//       mencari celah alih-alih hanya menjauh.
//    2. TERJANGAN          - bos diam sejenak, lalu melesat. Menghukum
//       pemain yang berdiri terlalu nyaman.
//    3. PANGGIL BAWAHAN    - musuh baru muncul mengelilingi bos, memutus
//       jalur mundur pemain.
//
//  SETIAP POLA DIDAHULUI KEDIPAN PERINGATAN.
//  Ini bukan hiasan. Serangan tanpa aba-aba terasa TIDAK ADIL, bukan sulit.
//  Pemain harus selalu bisa berkata "itu salahku", bukan "itu tiba-tiba".
//  Aba-aba adalah yang membedakan tantangan dari frustrasi.
//
//  Dipasang OTOMATIS oleh ZombieSpawner lewat AddComponent, jadi kamu tidak
//  perlu mengedit satu pun prefab bos.
// ============================================================================
public class BosPola : MonoBehaviour
{
    [Tooltip("Bos ke berapa dalam run ini. Diisi otomatis oleh ZombieSpawner.")]
    public int tingkat = 1;

    [Tooltip("Jeda tenang antar pola serangan, dalam detik.")]
    public float jedaAntarPola = 4.5f;

    [Tooltip("Jeda sebelum pola pertama, memberi pemain waktu bereaksi.")]
    public float jedaPembuka = 2.5f;

    EnemyChase ec;
    ZombieSpawner spawner;
    SpriteRenderer[] sprite;
    Color[] warnaDasar;

    void Start()
    {
        ec = GetComponent<EnemyChase>();
        if (ec == null) { enabled = false; return; }

        // Dicari sekali di Start, bukan tiap kali memanggil bawahan.
        spawner = Object.FindFirstObjectByType<ZombieSpawner>();

        sprite = GetComponentsInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            warnaDasar = new Color[sprite.Length];
            for (int i = 0; i < sprite.Length; i++)
                if (sprite[i] != null) warnaDasar[i] = sprite[i].color;
        }

        StartCoroutine(Rutin());
    }

    IEnumerator Rutin()
    {
        yield return new WaitForSeconds(jedaPembuka);

        int pola = 0;
        while (ec != null && !ec.SudahMati)
        {
            // Jangan menyerang saat permainan sedang jeda atau saat pemain
            // sedang memilih kartu skill.
            if (!GameMenu.SedangMain) { yield return null; continue; }

            switch (pola % 3)
            {
                case 0: yield return StartCoroutine(TembakMelingkar()); break;
                case 1: yield return StartCoroutine(Terjangan()); break;
                default: yield return StartCoroutine(PanggilBawahan()); break;
            }

            pola++;
            yield return new WaitForSeconds(jedaAntarPola);
        }
    }

    // ---------------------------------------------------------- pola 1
    IEnumerator TembakMelingkar()
    {
        yield return StartCoroutine(Aba(0.65f, new Color(1f, 0.55f, 0.15f)));
        if (ec == null || ec.SudahMati) yield break;

        // Sengaja DIBATASI. Tiap PeluruMusuh membuat GameObject + SpriteRenderer
        // baru, jadi salvo besar bisa menjatuhkan FPS di HP kentang justru pada
        // momen paling penting. Angka ini menunggu hasil stress test.
        int perSalvo = Mathf.Min(16, 9 + tingkat);
        int salvo = Mathf.Min(4, 2 + tingkat / 2);
        float dmg = 7f + tingkat * 1.5f;

        for (int s = 0; s < salvo; s++)
        {
            if (ec == null || ec.SudahMati) yield break;

            // Tiap salvo digeser setengah langkah supaya celahnya bergerak.
            // Kalau semua salvo sejajar, pemain cukup berdiri di satu celah
            // dan seluruh pola jadi tidak berbahaya.
            float geser = (s % 2 == 0) ? 0f : (180f / perSalvo);

            for (int i = 0; i < perSalvo; i++)
            {
                float sudut = (geser + i * (360f / perSalvo)) * Mathf.Deg2Rad;
                Vector3 arah = new Vector3(Mathf.Cos(sudut), Mathf.Sin(sudut), 0f);
                PeluruMusuh.Tembak(transform.position + arah * 0.9f, arah, 5f, dmg);
            }

            ScreenShake.Getar(0.12f, 0.12f);
            yield return new WaitForSeconds(0.55f);
        }
    }

    // ---------------------------------------------------------- pola 2
    IEnumerator Terjangan()
    {
        if (ec == null) yield break;

        float kecAsli = ec.moveSpeed;

        // Berhenti total dulu. Diam adalah aba-aba paling jelas untuk terjangan;
        // pemain langsung tahu sesuatu akan terjadi tanpa perlu ikon apa pun.
        ec.moveSpeed = 0f;
        yield return StartCoroutine(Aba(0.85f, new Color(1f, 0.2f, 0.2f)));

        if (ec == null || ec.SudahMati) yield break;

        ScreenShake.Getar(0.3f, 0.25f);

        // EnemyChase sudah mengejar pemain tiap FixedUpdate, jadi terjangan
        // cukup dibuat dengan menaikkan kecepatannya sesaat. Tidak perlu
        // menggerakkan transform sendiri - kalau dua skrip sama-sama menulis
        // posisi, keduanya akan saling menimpa dan gerakannya patah-patah.
        ec.moveSpeed = kecAsli * (3.2f + tingkat * 0.25f);
        yield return new WaitForSeconds(1.15f);

        if (ec != null) ec.moveSpeed = kecAsli;
    }

    // ---------------------------------------------------------- pola 3
    IEnumerator PanggilBawahan()
    {
        yield return StartCoroutine(Aba(0.7f, new Color(0.65f, 0.35f, 1f)));
        if (ec == null || ec.SudahMati) yield break;

        if (spawner != null)
            spawner.SpawnDiSekitar(transform.position, Mathf.Min(8, 3 + tingkat));

        ScreenShake.Getar(0.15f, 0.15f);
        yield return new WaitForSeconds(0.3f);
    }

    // ------------------------------------------------------- aba-aba
    // Kedipan yang makin cepat menjelang serangan. Percepatannya penting:
    // kedipan berirama tetap mudah diabaikan, kedipan yang memburu terbaca
    // sebagai hitung mundur.
    IEnumerator Aba(float durasi, Color warna)
    {
        float t = 0f;
        bool nyala = false;

        while (t < durasi)
        {
            if (ec == null || ec.SudahMati) { Kembalikan(); yield break; }

            nyala = !nyala;
            Warnai(nyala ? warna : Color.white);

            float langkah = Mathf.Lerp(0.15f, 0.05f, t / durasi);
            yield return new WaitForSeconds(langkah);
            t += langkah;
        }

        Kembalikan();
    }

    void Warnai(Color c)
    {
        if (sprite == null) return;
        for (int i = 0; i < sprite.Length; i++)
            if (sprite[i] != null) sprite[i].color = c;
    }

    void Kembalikan()
    {
        if (sprite == null || warnaDasar == null) return;
        for (int i = 0; i < sprite.Length && i < warnaDasar.Length; i++)
            if (sprite[i] != null) sprite[i].color = warnaDasar[i];
    }
}
