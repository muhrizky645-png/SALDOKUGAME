# HANDOFF — ZOMBURST: Auto Shooter (SALDOKUGAME)

_Update: 1 Sep 2026, 11:20 WIB. Ditulis untuk dibaca AI di sesi chat berikutnya._

---

## 0. CARA PAKAI HANDOFF INI

Kamu melanjutkan pekerjaan membangun game Unity 2D auto-shooter milik user. User berbahasa
Indonesia, zona waktu Asia/Jakarta, bekerja SENDIRI, dan sering hanya punya HP.

**BACA BAGIAN 2 SEBELUM MENULIS KODE APA PUN.** Isinya kesalahan yang sudah pernah
terjadi dan tidak perlu terulang.

### Yang berubah drastis dari handoff versi lama

Handoff sebelumnya menyatakan `mcpServer_github` DIBLOKIR dan alur kerjanya lewat sandbox
`/data` lalu user download-timpa-push manual. **Itu sudah tidak benar.** MCP GitHub bekerja
penuh: baca file, buat branch, push, buat PR, merge, hapus file — semua sudah terbukti
berhasil puluhan kali.

**Alur kerja sekarang:** tulis file langsung ke repo lewat `push_files` ke branch `main`.
User sudah minta JANGAN pakai branch terpisah lagi — dia bingung harus buka PR dan merge
sendiri. Push langsung ke `main`.

Yang TIDAK bisa dilakukan: menjalankan Unity, mengompilasi, menjalankan game, melihat
hasilnya. **Setiap baris C# yang kamu kirim belum pernah dikompilasi.** Selalu katakan ini
terus terang, jangan pernah bilang "sudah beres" untuk sesuatu yang belum pernah dijalankan.

---

## 1. IDENTITAS & STATUS

- Brand: **Zomburst**. Judul store: "Zomburst: Auto Shooter". Genre: auto-shooter 2D,
  kiblatnya **Survivor.io**.
- Repo: `muhrizky645-png/SALDOKUGAME`, publik, branch tunggal `main`.
- **Unity `6000.5.8f1`** (Unity 6). BUKAN Unity 2022 seperti yang tertulis di PRD.
  Artinya: Build Profiles, bukan Build Settings lama; `FindFirstObjectByType` bukan
  `FindObjectOfType`.
- Lisensi: **Unity Personal**. Ini lisensi penuh dan gratis, sah untuk rilis komersial
  sampai pendapatan 200 ribu USD. User sempat mengira "gratisan = tidak ada lisensi" —
  itu salah, jangan ikut salah.
- Semua UI masih `OnGUI`. Ini utang teknis terbesar, tapi JANGAN dimigrasi sebelum ada
  angka FPS nyata (lihat Bagian 6).
- 45 script lama di `Assets/*.cs` (flat). Kode baru diletakkan di `Assets/Scripts/Core/`
  dan `Assets/Scripts/Data/`.

### PRD

PRD lengkap 31 bab ada di **Notion**, judul "PRD — Project Nightfall: Survival Roguelite
Mobile Game (Survivor.io-like)", milik user pribadi.

Aku pernah mendorong PRD itu ke `docs/PRD.md` dan `docs/PRD-bagian-2.md`, tapi file
pertama **terpotong di tengah tabel Bab 23** karena satu string `push_files` ada batas
ukuran praktisnya. Kedua file itu SUDAH DIHAPUS atas permintaan user, dan user push
sendiri hasil export Notion dari komputer.

**Pelajaran: kalau harus mengirim dokumen panjang, pecah jadi beberapa file SEBELUM
menulis, jangan sesudah. Dan periksa ujung file setelah push.**

Realitas versus PRD: PRD menargetkan 20 senjata, 22 pasif, 10 chapter x 10 stage.
Repo punya 3 senjata, 6 pasif, 4 stage. Rekomendasi yang sudah disampaikan ke user dan
masih berlaku: **rescope "Zomburst Edition"** — 10 senjata, 5 chapter. PRD itu peta,
bukan daftar tugas.

---

## 2. ATURAN PENTING (PELAJARAN MAHAL)

1. **JANGAN menulis kode dari ingatan atau dari ringkasan.** Baca file aslinya dulu tiap
   kali. Tiga kali hampir terjadi error karena nama field ditebak. Yang menyelamatkan:
   membaca `*SO.cs` sebelum menulis generator, membaca `StageManager.cs` sebelum mengubah
   durasi, membaca `HasilMain.cs` sebelum menyentuh kondisi menang.

2. **`push_files` dan `create_or_update_file` itu PENGGANTI FILE UTUH.** Tidak ada operasi
   append. Kalau hanya mengubah satu fungsi, kamu tetap harus mengirim seluruh file — jadi
   baca file itu lebih dulu, jangan menyusun ulang dari ingatan.

3. **Mata uang upgrade = PERMATA**, bukan Koin. `MataUang.TambahPermata` /
   `PakaiPermata(int)->bool`, PlayerPrefs `"permata"`.
   **KOIN (Saldoku) READ-ONLY** — nilainya hanya masuk dari server lewat
   `SetKoinDariServer()`. Koin SUDAH server-authoritative; jangan pernah bilang ke user
   bahwa itu tidak aman (aku pernah salah bilang begitu dan harus meralat).

4. **Sebelum menilai balance apa pun, PlayerPrefs harus dibersihkan.** `UpgradePermanen`
   menyimpan level upgrade permanen dan tidak pernah direset. User pernah mengeluh
   "tembakan level awal terlalu cepat" padahal yang dia mainkan adalah save yang sudah
   ter-upgrade. Selalu ingatkan: **Edit > Clear All PlayerPrefs**.

5. **Satu tugas per sesi kerja.** User pernah menyatakan terus terang: "Aku tidak paham
   harus mulai dari mana." Jangan sodorkan daftar 20 item. Beri satu langkah, tunggu
   hasilnya.

6. **Asset boleh menyusul, dan user setuju.** Gameplay dan keseimbangan dulu. Gaya seninya
   **chibi kartun** bergaris tebal seperti Survivor.io, **bukan pixel art**. Untuk animasi
   tidak perlu aplikasi baru: `KarakterManager` sudah memisah Body/Head/Left_Foot/
   Right_Foot/Weapon, jadi itu rig cut-out — pakai package 2D Animation bawaan Unity.

7. **Jangan menimpa nilai yang sudah diisi user.** Mengisi field yang null itu aman dan
   boleh otomatis. Mengubah field yang sudah berisi harus lewat menu terpisah dengan
   konfirmasi. Pola ini dipakai di `BuatAsetBalance.cs`, ikuti.

---

## 3. ARSITEKTUR BARU (SUDAH DI `main`)

### `Assets/Scripts/Core/EnemyRegistry.cs`
Registry statis + spatial grid (`UkuranSel = 4f`). Menggantikan
`FindGameObjectsWithTag("Enemy")` yang dulu dipanggil di **empat** tempat panas, termasuk
`PlayerShooting.Shoot()` setiap tembakan. API: `Semua`, `Jumlah`, `Daftar`, `Hapus`,
`DalamRadius`, `Terdekat(pusat, radiusMaks[, kecuali])`, `NTerdekat`. `EnemyChase`
mendaftar di `OnEnable` dan keluar di `OnDisable` + awal `Mati()`.

### `Assets/Scripts/Core/Balance.cs` — SATU SUMBER KEBENARAN untuk "seberapa kuat"
- Stat dasar pemain: `JedaTembakAwal = 1.0f` (dulu 1.2 lalu terasa terlalu cepat karena
  upgrade permanen), `JumlahPeluruAwal = 1`, `JangkauanTembakAwal = 8f`.
  **`JangkauanTembakAwal` naik dari 1f ke 8f adalah tebakan paling berisiko yang pernah
  kubuat — wajib dinilai user.**
- `DurasiRunDetik = 900f` — **satu-satunya tuas untuk memendekkan run.** Ubah ke `600f`
  kalau paruh kedua terasa kosong.
- `GunakanKurvaXpPrd = true`, `XpUntukLevel(int)`. Kurva lama mencapai level 5 dalam ~30
  detik; itu penyebab gerbang variasi musuh tidak pernah terasa.
- `MusuhDasar = 40`, `MusuhPerMenit = 18f`, `MaxMusuhMutlak = 320`, `MaxMusuhHidup(...)`.
- Slot: 6 senjata + 6 pasif, level maks 5. Evolusi: senjata Lv5 + pasif Lv3 + menit >= 5.
- `JedaBosDetik = 45f` **SUDAH TIDAK DIPAKAI** — digantikan `JadwalRun.SiklusDetik`.
  Hapus atau sambungkan, jangan dibiarkan membingungkan.

### `Assets/Scripts/Core/JadwalRun.cs` — SATU SUMBER KEBENARAN untuk "kapan"
Irama satu siklus: `...tenang... -> GELOMBANG 45s -> HENING 20s -> BOS`.
- `SiklusDetik = 300f` (bos tiap 5 menit), `DetikWave = 65f`, `DetikHening = 20f`,
  `MajuBosAkhir = 60f`.
- `DurasiRun` dibaca dari `StageManager.TargetSekarang`, jadi jadwal menyesuaikan sendiri.
- `WaktuBos(n)`, `JumlahBosSeharusnya(detik)`, `DetikKeBosBerikut(detik)`, `Fase(detik)`.
- Variasi musuh bertahap: `JenisAwal = 2`, satu tier baru tiap 55 detik.
  Perilaku khusus dibuka bertahap: Cepat 1:00, Tank 2:30, Penembak 4:00, Peledak 6:00.
- `NyawaBos(nomor, detik, pengaliStage)` = `90 + 45/menit + 90/nomor`.
  **Angka ini TEBAKAN.** Target waktu bunuh 20-35 detik. Minta user melaporkan.

### `Assets/Scripts/Core/BosPola.cs`
Ditambahkan otomatis oleh spawner ke prefab bos, dengan `tingkat = nomor bos`. Tiga pola
bergilir tiap 4,5 detik: `TembakMelingkar` (celah bergeser tiap salvo),
`Terjangan` (memakai `moveSpeed` milik `EnemyChase` sendiri, bukan gerak terpisah),
`PanggilBawahan` (`spawner.SpawnDiSekitar`). Setiap pola punya **aba-aba berkedip** lebih
dulu. Aba-aba itulah yang membedakan "sulit" dari "tidak adil" — jangan dihapus.

### `Assets/Scripts/Core/PengumumanRun.cs`
Spanduk "GELOMBANG!", "SESUATU MENDEKAT...", "BOS MUNCUL", plus hitung mundur "BOS <n>s".
Sengaja memakai `GUI`/`GUIStyle` biasa, BUKAN helper `Tema`, supaya tidak bergantung pada
tanda tangan API yang belum diverifikasi.

### `Assets/Scripts/Data/` — `SenjataSO`, `PasifSO`, `MusuhSO`, `StageSO`
Lapisan data ScriptableObject. **Belum dikonsumsi runtime.** `SenjataManager` masih
memakai `int dmg = 3 + lvOrbit * 2 + (evo ? 5 : 0);` yang tertanam di kode.
Nama field memakai bahasa Indonesia (`namaTampil`, `jedaSerang`, `jumlahProyektil`,
`hasilEvolusi`) — jangan tebak, baca filenya.

### `Assets/Editor/BuatAsetBalance.cs`
Tiga menu:
- **Zomburst > Buat Semua Aset Balance** — membuat semua aset SO, idempoten, tidak pernah
  menimpa aset yang sudah ada. Di akhir menyambungkan prefab musuh otomatis.
- **Zomburst > Sambungkan Prefab Musuh** — mengisi `MusuhSO.prefab` yang masih null.
  Mencari 3 lapis: `Assets/Prefabs/ZOMBIE.prefab`, lalu prefab bernama ZOMBIE, lalu prefab
  apa pun yang punya komponen `EnemyChase`.
- **Zomburst > Selaraskan Stage dengan Balance** — satu-satunya menu yang menimpa nilai
  terisi, dan selalu konfirmasi dulu.

**PENTING:** di repo hanya ada SATU prefab musuh, `Assets/Prefabs/ZOMBIE.prefab`.
Ketujuh `MusuhSO` memakai prefab yang sama, dan itu memang cukup untuk sekarang karena
`EnemyChase.TerapkanVarian()` sudah mengubah warna dan ukuran tiap varian saat runtime.
Jangan pernah menyuruh user "isi 7 prefab" — prefabnya tidak ada. Aku pernah salah begitu.

---

## 4. JADWAL RUN SEKARANG (`DurasiRun = 900`)

| Peristiwa | Waktu |
|---|---|
| Bos 1 | 5:00 |
| Bos 2 | 10:00 |
| Bos 3 (terakhir) | 14:00 |
| Gelombang | 3:55-4:40, 8:55-9:40, 12:55-13:40 |
| Hening | 4:40-5:00, 9:40-10:00, 13:40-14:00 |

Keempat stage sekarang berdurasi SAMA (900 detik). Bedanya hanya pengali kekuatan musuh:
1.0 / 1.15 / 1.3 / 1.5.

**Kenapa bos terakhir di 14:00, bukan 15:00:** `HasilMain.Update()` memicu layar MENANG
begitu `GameTimer.Detik >= StageManager.TargetSekarang`. Bos di detik 900 akan terhapus di
frame yang sama. Selain itu kondisi menang sekarang punya penjaga
`&& EnemyChase.JumlahBos == 0` — tanpa itu pemain cukup berputar-putar sampai waktu habis
dan bos terakhir jadi tidak ada artinya.

**Peringatan jujur yang sudah disampaikan ke user:** dengan hanya 3 senjata + 6 pasif,
build sudah mentok sekitar menit 6-7, jadi paruh kedua run 15 menit akan terasa kosong
sampai kontennya bertambah. Kalau user mengeluh soal ini, ubah `Balance.DurasiRunDetik`
ke `600f` — satu angka, satu baris, jadwal menyesuaikan sendiri.

---

## 5. SUDAH BERES DAN TERVERIFIKASI ADA DI `main`

- `EnemyRegistry` + spatial grid; keempat `FindGameObjectsWithTag` dihapus.
- `Bullet` tidak lagi habis di mayat (`if (musuh.SudahMati) return;`); crit pindah ke
  field Inspector, tidak lagi hardcode.
- `Balance.cs`, `JadwalRun.cs`, `BosPola.cs`, `PengumumanRun.cs`.
- Lapisan data SO + generator editor.
- `LevelSystem` memakai kurva XP PRD.
- `ZombieSpawner`: jumlah musuh berbasis WAKTU, bukan level pemain (dulu ada umpan balik
  yang membuat kurvanya berbeda untuk tiap pemain). `SpawnDiSekitar` baru. `bossKe` naik
  SEBELUM early-return prefab null, supaya tidak retry tiap frame.
- `EnemyChase.RollTipe` mendelegasikan ke `JadwalRun.RollTipe(detik)`. Dulu memilih bebas
  di antara keempat tipe khusus **sejak detik ke-0 tanpa gerbang apa pun** — ini penyebab
  utama keluhan "musuh langsung muncul semua".
- `StageManager` 4 stage x 900 detik; `HasilMain` dengan penjaga bos.
- **`SkillManager` (SUDAH DIVERIFIKASI 1 Sep):** sistem slot 6+6, batas Lv5 dari `Balance`,
  `BolehDitawarkan()` menyaring skill yang sudah mentok, penjagaan kedua di `Pilih()`.
  Memperbaiki eksploit lama: semua pasif dulu `maks = 0` alias TAK TERBATAS, dan kartu
  bertuliskan "MAX!" tetap menjalankan efeknya (`fireRate 1.2 x 0.80^20 = 0.0138`).

---

## 6. BELUM BERES — URUT PRIORITAS

### Menunggu user membuka komputer (memblokir yang lain)
1. **Kompilasi.** Belum ada satu file pun yang terbukti bisa di-build.
2. **Angka 1% low FPS** dari stress test 300 musuh. Angka ini yang menentukan apakah
   pooling, batas 320 musuh, dan migrasi dari `OnGUI` layak dikerjakan. **Jangan
   mengerjakan optimasi itu sebelum angkanya ada** — mengoptimalkan tanpa mengukur itu
   menebak.
3. **Penilaian rasa main:** jeda tembak 1 detik terasa pas? `range = 8f` kejauhan?
   Bos mati dalam 20-35 detik? Irama gelombang-hening-bos terbaca?

### Bug nyata yang belum ditambal
4. **`Assets/SaldokuKoin.cs` — cabang `#else` memberi hadiah TANPA iklan:**
   ```csharp
   public void TampilkanHadiah(System.Action onReward, System.Action<string> onFail)
   {
       if (onReward != null) onReward();
   }
   ```
   Kalau `SALDOKU_ADMOB` tidak terdefinisi, siapa pun dapat hadiah gratis. Usulan: bungkus
   `#if UNITY_EDITOR || DEVELOPMENT_BUILD`, selain itu panggil
   `onFail("Fitur iklan belum aktif di build ini.")`. **User belum menjawab soal ini,
   tawarkan lagi.**
   Juga di file yang sama: `AD_UNIT_PROD` masih placeholder nol semua,
   `USE_TEST_ADS = true`, token disimpan plaintext di PlayerPrefs.
5. **Folder artefak build ikut ter-commit**, perlu `git rm -r --cached`:
   `SALDOKUGAME_BurstDebugInformation_DoNotShip/`,
   `Saldokugame_BackUpThisFolder_ButDontShipItWithYourGame/`,
   `debug_BackUpThisFolder_ButDontShipItWithYourGame/`.

### Pembersihan kecil
6. Hapus atau sambungkan `Balance.JedaBosDetik` yang sudah mati.
7. Pindahkan damage `SenjataManager` ke `SenjataSO`; ganti evolusi otomatis dengan
   `Balance.BolehEvolusi`.

### Konten (penawar rasa kosong di paruh kedua)
8. Tambah senjata dan pasif menuju target rescope: 10 senjata, bukan 20.
9. Pooling **peluru** lebih dulu, bukan pooling musuh. `BosPola.TembakMelingkar` bisa
   memuntahkan sampai 64 `PeluruMusuh` sekali pola.

### Dokumen
10. `docs/BALANCE.md` menjelaskan tiap angka di `Balance.cs` dan `JadwalRun.cs`.
11. Gap analysis lengkap ada sebagai subhalaman PRD di Notion, judul
    "GAP Analysis & Rencana Eksekusi". Belum pernah masuk repo.

---

## 7. RISIKO YANG BELUM TERVERIFIKASI (JANGAN DIKLAIM BERES)

- Seluruh `BosPola` ditulis tanpa pernah dilihat hasilnya. Pola serangan, durasi aba-aba,
  dan kecepatan terjangan semuanya perkiraan.
- Nyawa bos 315 di menit ke-5 (bos 1, stage x1.0) adalah tebakan.
- `JangkauanTembakAwal = 8f` naik dari `1f`. Kalau terasa "pemain menembak dari luar layar",
  ini penyebabnya.
- `PengumumanRun` belum pernah dilihat di layar HP; ukuran font bisa salah.

---

## 8. REFERENSI API (TERKONFIRMASI DARI FILE ASLI)

- **Player (tag `"Player"`):** `PlayerHealth.maxHealth/health/damagePerSecond`, `.Instance`,
  `.Kurangi(dmg)`, `.Pulih(amt)`, `.HidupLagi()`, `static bool GameOver`;
  `PlayerMovement.moveSpeed` (5f);
  `PlayerShooting.fireRate/range/jumlahPeluru/sudutSebar/bulletPrefab` + `pakaiBalance`
  dan `Awake()` yang menerapkan `Balance` (sengaja `Awake`, bukan `Start`, supaya
  `UpgradePermanen` menumpuk di atasnya); `Bullet.damage` (int, di PREFAB).
- **MataUang:** `Instance`, `Permata`, `Koin` (long, read-only), `Terhubung`,
  `TambahPermata(int)`, `PakaiPermata(int)->bool`, `static Ringkas(long)`.
- **Runtime:** `GameMenu.SedangMain/SedangJeda/UlangiDanMain()/KeHome()`;
  `HasilMain.Menang`; `SkillManager.AktifMemilih`; `ModeDewa.Aktif`;
  `GameTimer.Detik` (static float); `LevelSystem.Instance.Level`;
  `EnemyChase.JumlahBos/BosSaatIni/KenaSerangan(int)/Perlambat(durasi,faktor)`;
  `ScoreManager.Instance.SkorSekarang/RekorTertinggi/AddScore(n)`;
  `SoundManager.Klik()/Tembak()/LevelUp()/GameOver()/PlayerKena()/AmbilXp()/MusuhKena()/`
  `MusuhMati()/BossMuncul()/Menang()`;
  `Ledakan.Munculkan(pos,radius,a,b,warna)`; `HitEffect.Munculkan(pos,ukuran=1f)`;
  `XpGem.Munculkan(...)` + `XpGem.MagnetMult`; `PermataGem.Munculkan(pos,n)`;
  `ItemLapangan.Jatuhkan(pos, Jenis.Peti|Bom|Magnet)`; `DamageNumber.Munculkan(Vector3,int)`;
  `ScreenShake.Getar(kuat,durasi)`; `HitStop.Beku(durasi,skalaWaktu)`; `ComboMeter.Tambah()`;
  `Roket.Tembak(pos,Transform,speed,dmg,radius)`;
  `SenjataManager.Instance.TambahOrbit()/TambahAura()/TambahRoket()`, `SenjataManager.MAX = 6`
  (**itu jumlah SLOT, bukan batas level** — kesalahan ini pernah membuat kartu berbohong);
  `UpgradePermanen.Instance.Buka()`.
- **`PeluruMusuh.Tembak(Vector3 pos, Vector3 arah, float speed, float dmg)`** — `dmg` itu
  **float**, terverifikasi dari situs pemanggilnya.
- **Tema:** `Unit, Pad, AmanAtas/Kiri/Kanan/Bawah, Panel, PanelTerang, Plate, Garis,`
  `GarisRedup, Army, Amber, Darah, Tulang, Redup`, `LatarGelap()`, `Vignette()`,
  `Kotak(Rect,Color)`, `Panel9(Rect,Color,Color,float)`,
  `Teks(Rect,string,int,Color,TextAnchor,bool)`, `BarIsi(Rect,Color)`, `GayaTombol(f)`,
  `GayaTombolAksen(f)`, `Font(frac)`; `Ikon.Gambar/Dari/UntukSkill/Berlian/Piala/Bintang/Hati`.
- **PlayerPrefs:** `"permata"`, `"stage_dipilih"`, `"stage_terbuka"`, `"upg_perm"`,
  `"karakter_dipilih"`, `"karakter_buka_<idx>"`, `"saldoku_game_token"`.
- **Pola bootstrap:** `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  + `SceneManager.sceneLoaded`. Ikuti pola ini untuk komponen baru yang harus selalu ada.

---

## 9. LANGKAH PERTAMA SAAT USER MEMBUKA UNITY

Urutannya penting, jangan diacak:

1. `git pull` di `main`.
2. Tunggu import selesai, **lihat Console**. Ada error merah? Berhenti, minta teks
   error-nya, perbaiki. Jangan lanjut.
3. **Zomburst > Buat Semua Aset Balance**.
4. Kalau dialognya menyebut stage belum selaras: **Zomburst > Selaraskan Stage dengan
   Balance**.
5. **Zomburst > Periksa Aset Balance**. Keluhan soal ikon boleh diabaikan —
   `Ikon.UntukSkill` masih menggambar ikon prosedural.
6. **Edit > Clear All PlayerPrefs.** JANGAN dilewati.
7. Main satu run penuh 15 menit dan laporkan poin-poin di Bagian 6 nomor 3.
8. Untuk build Android tanpa PC: Unity Build Automation, hubungkan repo di
   `cloud.unity.com`. Wajib **Development Build** kalau mau `StressTest` jalan
   (butuh `DEVELOPMENT_BUILD`). Free tier kira-kira 8-15 build Android per bulan.
   **Jangan pasang kartu kredit.** GameCI tidak bisa dipakai: aktivasi manual lisensi
   Personal sudah dihentikan Unity, jadi GameCI justru MEMBUTUHKAN PC.

---

## 10. GAYA KERJA USER

- Bahasa Indonesia, santai. Balas dengan bahasa Indonesia.
- Bukan programmer ahli. Jelaskan **kenapa**, bukan hanya **apa**.
- Sering hanya punya HP. Solusi yang butuh PC harus disebut jelas butuh PC.
- **Menghargai kejujuran lebih dari kesan cepat beres.** Setiap kali aku mengaku salah,
   percakapan justru maju. Kalau sesuatu belum terbukti, katakan belum terbukti.
- Kalau dia menyerahkan keputusan ("bagusnya bagaimana, sesuai saranmu saja"), dia benar
  benar minta PENDAPAT, bukan sekadar persetujuan. Beri rekomendasi tegas plus alasannya.
- Jangan pakai branch. Push langsung ke `main`.
