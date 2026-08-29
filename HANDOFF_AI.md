# HANDOFF — ZOMBURST: Auto Shooter (SALDOKUGAME)
_Update: 29 Aug 2026, 15:11 WIB. Untuk melanjutkan di sesi chat baru._

---

## 0. CARA PAKAI HANDOFF INI
Kamu (AI sesi berikutnya) melanjutkan pekerjaan membangun fitur game Unity 2D (C#) milik user (SK Music, Bahasa Indonesia, Asia/Jakarta). User TIDAK bisa aku push langsung ke repo (MCP GitHub diblokir) — alur kerjanya: **aku tulis/edit file di sandbox `/data` → verifikasi → download → user timpa file lokal & push → user tes di Unity.** Aku tidak bisa compile.

---

## 1. IDENTITAS GAME
- Brand: **Zomburst**. Judul store: **"Zomburst: Auto Shooter"**. Genre: auto-shooter / Survivor.io-clone 2D.
- Repo: `muhrizky645-png/SALDOKUGAME` (public, branch `main`). Script ada di `Assets/<Nama>.cs`.
- Raw read (HANYA via `web.loadPage`, sandbox tanpa internet): `https://raw.githubusercontent.com/muhrizky645-png/SALDOKUGAME/main/Assets/<file>.cs`
- Doc konsep (Notion, privat, 🎮): "Zomburst — Konsep & Alur Game", page id `cb56bd578ec341d39e469cf4e0d3ec14`.
- User: SK Music, id `397d872b-594c-81f1-838e-000248a9838f`, workspace "nastaresdim's Space", Owner.

---

## 2. ATURAN PENTING (JANGAN DILANGGAR)
1. **`web.loadPage` MERUSAK generic C#**: `GetComponent<T>()` → `GetComponent ()`, `System.Action<float>` → `System.Action`, dan indentasi jadi 1 spasi. → **Hasil baca file repo = HANYA untuk analisa, JANGAN pernah disimpan/di-download ulang.** File baru yang AKU tulis sendiri aman (aku ketik generic yang benar).
2. **Untuk mengedit file repo yang sudah ada** (mis. `Bullet.cs`), MINTA user upload file ASLI, lalu edit surgical (`editFile`, sisip seminimal mungkin). Ini yang bikin GameMenu.cs kemarin akhirnya benar.
3. **Mata uang upgrade = PERMATA** (`MataUang` API `TambahPermata`/`PakaiPermata(int)->bool`, PlayerPrefs `"permata"`). **KOIN (SALDOKU) READ-ONLY / server-only** — jangan dipakai buat spend in-game.
4. Selalu verifikasi brace/paren balance di terminal sebelum download.
5. MCP GitHub `mcpServer_github` DIBLOKIR (butuh re-approve admin). Jangan andalkan itu.
6. **Filosofi user (penting):** utamakan KUALITAS GAMEPLAY & ALUR dulu. Asset boleh belakangan — semua dibuat via KODE (nol file gambar), nanti tinggal swap art asli. Semua fitur so far 100% code-generated (nol PNG).

---

## 3. SUDAH SELESAI & TERKIRIM
### PAKET 1 — Stage select + Win condition + Result screen (SHIPPED)
- **`StageManager.cs`** (BARU → `Assets/`): static, 4 stage [HUTAN TERKONTAMINASI 180s / KOTA RUNTUH 240s ×1.15 / GURUN RERUNTUHAN 300s ×1.3 / KUTUB BEKU 360s ×1.5]. PlayerPrefs `"stage_dipilih"`,`"stage_terbuka"`. API: `Jumlah, Dipilih(get/set), TerbukaSampai, Terbuka(i), BukaSampai(i), Sekarang, TargetSekarang, PengaliMusuhSekarang, AdaBerikutnya`, nested `Stage{nama,tagline,targetDetik,pengaliMusuh}`.
- **`HasilMain.cs`** (BARU → `Assets/`): MonoBehaviour bootstrap. `static bool Menang`. Deteksi menang saat `GameTimer.Detik >= StageManager.TargetSekarang` → `Time.timeScale=0`, hadiah Permata `50+Dipilih*40+skor/20`, `BukaSampai(Dipilih+1)`, `SoundManager.LevelUp()`. OnGUI (GUI.depth=-1000 + bg opaque nutup HUD): layar MENANG (waktu bertahan, hadiah, tombol STAGE BERIKUTNYA / ULANGI / KE HOME). Kalah tetap ditangani PlayerHealth GAME OVER lama.
- **`GameMenu.cs`** (EDIT — DISUPERSEDE oleh Paket 2, lihat bawah).
- Target durasi 3/4/5/6 menit sudah DISETUJUI user.

### PAKET 2 — Upgrade Permanen pakai Permata (SHIPPED)
- **`UpgradePermanen.cs`** (BARU → `Assets/`): MonoBehaviour singleton (bootstrap + sceneLoaded), `[DefaultExecutionOrder ...]`, panel GUI.depth=-1000 + full-screen click swallower (pola dari Toko). Simpan 4 level di PlayerPrefs `"upg_perm"` = "l0,l1,l2,l3". Deteksi run-start via rising-edge `GameMenu.SedangMain`, apply bonus ADITIF ~3 frame kemudian (biar Start/karakter settle) dengan cari komponen di object tag `"Player"`. 4 upgrade:
  - MAX HP +20/lvl (maks 8, harga 60+40*lvl) → `PlayerHealth.maxHealth`(+`health`)
  - KECEPATAN GERAK +0.4/lvl (maks 6, 60+45*lvl) → `PlayerMovement.moveSpeed`
  - KECEPATAN TEMBAK fireRate*=(1-0.08*lvl) clamp min 0.3 (maks 6, 80+60*lvl) → `PlayerShooting.fireRate`
  - PELURU EKSTRA +1/lvl (maks 3, 150+120*lvl) → `PlayerShooting.jumlahPeluru`
  - (Damage per-peluru SENGAJA di-skip: butuh edit prefab/PlayerShooting, fragile.)
- **`GameMenu.cs`** (EDIT, dari `/data/user_GameMenu.cs`, 627 baris, brace 67/67, `System.Action<float>` utuh): semua fitur Paket 1 + tombol **UPGRADE** di baris bawah layar Peta (`UpgradePermanen.Instance.Buka()`) di samping KEMBALI. **INI menggantikan GameMenu.cs Paket 1 — pakai yang terbaru.**

### PAKET 3 (bag. 1) — Screen shake (SHIPPED)
- **`ScreenShake.cs`** (BARU → `Assets/`, 99 baris, brace 14/14): `[DefaultExecutionOrder(10000)]` → LateUpdate jalan SETELAH CameraFollow, tambah offset getar (tanpa edit CameraFollow). Auto-getar saat HP pemain turun (cooldown 0.22s biar tak gemetar terus). API publik `ScreenShake.Getar(kuat, durasi)`.

---

## 4. YANG BELUM (LANJUTKAN DI SINI)
### PAKET 3 (bag. 2) — Angka damage / floating damage number
**User SUDAH setuju & akan UPLOAD `Bullet.cs` + `EnemyChase.cs` asli.** Begitu file masuk:
1. Buat **`DamageNumber.cs`** (BARU): teks angka mengapung naik + memudar, digambar via kode (nol asset), API `DamageNumber.Munculkan(Vector3 pos, int jumlah)`. Warna beda hit biasa vs besar.
2. Edit SURGICAL (sisip 1 baris) di titik hit:
   - `Bullet.cs` → saat kena musuh / `musuh.KenaSerangan(damage)` → `DamageNumber.Munculkan(posisiMusuh, damage)`.
   - `EnemyChase.cs` → cek jalur damage lain (bom/area) biar konsisten.
3. Verifikasi brace/paren → download `DamageNumber.cs` + `Bullet.cs` + `EnemyChase.cs` (yang sudah diedit dari upload asli).

### Lain-lain / deferred
- Extra SFX (boss muncul, jingle menang) → butuh edit SoundManager (minta upload asli). Menang sudah bunyi LevelUp.
- Tema per-stage (background/tint musuh) pakai `StageManager.PengaliMusuhSekarang` — ZombieSpawner belum diwire ke pengali; stage sekarang beda hanya di durasi bertahan.
- Asset asli (nanti swap): diorama_stage1.png (hutan), ikon nav, logo wordmark "ZOMBURST" (army green + amber + red, glossy 3D). Style base: "mobile game icon, Survivor.io art style, cute chibi, vibrant, thick dark outline, glossy, transparent bg, no text". HUD icon = Texture2D Default di `Assets/Resources/Icons/`; world drop = Sprite; bg 1080x2400.

---

## 5. REFERENSI API (dari repo, sudah dikonfirmasi)
- **Player (semua public, di root tag `"Player"`):** `PlayerHealth.maxHealth/health/damagePerSecond`, `.Instance`, `.Kurangi(dmg)`, `.Pulih(amt)`, `.HidupLagi()`, `static bool GameOver`; `PlayerMovement.moveSpeed`(5f); `PlayerShooting.fireRate`(1.2f, kecil=cepat)/`range`/`jumlahPeluru`(1)/`sudutSebar`/`bulletPrefab`; `Bullet.damage`(int, di PREFAB)/`speed`/`direction`.
- **MataUang:** `Instance`, `Permata`(int), `Koin`(long RO), `Terhubung`, `TambahPermata(int)`, `PakaiPermata(int)->bool`, static `Ringkas(long)`, `GambarChip(...)`.
- **Toko:** `Instance`, `Buka()/Tutup()`, `Terbuka`; buff 0=BOM/1=PULIH HP/2=PERLAMBAT.
- **Runtime:** `GameMenu.SedangMain/SedangJeda`, `.UlangiDanMain()`, `.KeHome()`, `.langsungMainSetelahLoad`; `SkillManager.AktifMemilih`; `ModeDewa.Aktif`; `GameTimer.Detik`(static float, reset tiap load); `LevelSystem.Instance.Level`; `KarakterManager.Dipilih/Nama[]/Kepala(i)/Terbuka(i)`; `EnemyChase.KenaSerangan(dmg)/.Perlambat(t,f)/.JumlahBos/.BosSaatIni`; `ScoreManager.Instance.SkorSekarang/RekorTertinggi/AddScore(n)`; `SoundManager.Klik()/Tembak()/LevelUp()/GameOver()/PlayerKena()/AmbilXp()`; `Ledakan.Munculkan(...)`, `HitEffect.Munculkan(pos,ukuran=1f)`, `XpGem.Munculkan(...)`. Tag: `"Player"`, `"Enemy"`, kamera `"MainCamera"`.
- **CameraFollow:** LateUpdate, `transform.position = Lerp(current, target+offset, smooth*dt)`, `offset.z=-10`, orthographicSize=`zoom`(10).
- **Tema helper:** `Unit,Pad,AmanAtas/Kiri/Kanan/Bawah,Panel,Plate,Garis,Army,Amber,Darah,Tulang,Redup`, `LatarGelap(),Vignette(),Kotak(Rect,Color),Panel9(...),Teks(...),GayaTombol(f),GayaTombolAksen(f),Font(frac)`; `Ikon.Gambar(Rect,tex,Color)`, `Ikon.Berlian/Piala/Bintang`.
- **PlayerPrefs baru:** `"stage_dipilih"`,`"stage_terbuka"` (StageManager); `"upg_perm"` (UpgradePermanen).

---

## 6. FILE DI SANDBOX `/data` (siap download)
- `user_GameMenu.cs` ← GameMenu.cs TERBARU (Paket 2, timpa yg lama, buang prefix `user_`)
- `UpgradePermanen.cs`, `ScreenShake.cs`, `StageManager.cs`, `HasilMain.cs` ← file BARU ke `Assets/`
- `orig.cs` = versi rusak, JANGAN dipakai. `GameMenu.cs`/`GameMenu_src.cs` lama = abaikan.
- `HANDOFF_AI.md` (file ini), `ASSET_LIST.md`, `SkillManager.cs`.

## 7. TOOLS SANDBOX
`writeFile({path,content,overwrite?,append?})`, `editFile({file_path,edits:[{old_string,new_string,replace_all?}]})`, `downloadFile({path})`, `uploadFile(...)`, `terminal({command,taskDescription})`, `readFile`. Semua di bawah `/data`, tanpa internet.
