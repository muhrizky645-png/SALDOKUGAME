# 🧠 HANDOFF AI — SALDOKUGAME

> **Tujuan file ini:** catatan estafet supaya sesi AI berikutnya (di chat lain) langsung nyambung tanpa perlu menjelaskan ulang. Berisi konteks proyek, arsitektur, semua yang sudah dikerjakan, keputusan desain, bug yang sudah diperbaiki, dan tugas lanjutan.
>
> **Terakhir diperbarui:** 2026-08-29 (Asia/Jakarta)
>
> ⚠️ **Untuk AI sesi berikutnya:** setelah menyelesaikan pekerjaan penting, **perbarui file ini** (bagian "Log Perubahan", "SHA File Terbaru", "Tugas Lanjutan", dan "Checklist Ikon").

---

## 1. Ringkasan Proyek

- **Repo:** `muhrizky645-png/SALDOKUGAME` (privat, branch `main`).
- **Engine:** Unity (game 2D top-down **survivor / roguelite** ala **Survivor.io**).
- **Nama game:** ⚠️ SEDANG DIGANTI. Nama lama "SALDOKU LAST STAND" akan diganti; nama baru **belum ditentukan** user (lihat §7). Repo tetap `SALDOKUGAME`.
- **Bahasa kode:** C# (nama kelas/variabel campuran Indonesia–Inggris).
- **Filosofi visual:** **semua UI & ikon & tekstur dibuat lewat KODE saat runtime** (IMGUI + Texture2D prosedural), **tanpa file gambar**. Ikon skill boleh pakai file PNG kalau tersedia di `Assets/Resources/Icons/`, tapi selalu ada fallback ikon kode.
- **Tema warna:** cerah & playful ala Survivor.io — oranye-emas dominan, tombol default biru, tombol aksi hijau, teks putih dengan bayangan (kesan outline).

### Cara kerja MCP GitHub (untuk AI)
- Baca file: tool `get_file_contents` (`owner`, `repo`, `path`, `ref:"main"`).
- Tulis/replace file: tool `create_or_update_file` dengan **konten PENUH** (`owner`, `repo`, `branch:"main"`, `path`, `message`, `content`, dan `sha` bila meng-update file yang sudah ada). Server yang meng-encode base64.
- ⚠️ **Push SATU per satu (sekuensial) ke branch yang sama** — push paralel bikin error `409 conflict`.
- `search_code` tidak jalan di repo privat → pakai `get_file_contents`.

---

## 2. Arsitektur / File Penting

### Sistem visual bersama
- **`Tema.cs`** — pustaka gaya UI IMGUI Survivor.io. API utama:
  - Warna: `Overlay, Panel, PanelTerang, Plate, Garis, GarisRedup, Darah, Tulang, Army, Amber, Redup`.
  - Responsif: `Unit`, `Font(frac)`, `Pad`, `AmanKiri/Kanan/Atas/Bawah` (safe area/notch).
  - Gambar: `Kotak`, `KotakGradien(V)`, `KotakGradienH`, `Vignette()`, `BarIsi(rect,warna)` (gradien+kilau untuk XP/boss/nyawa), `Panel9(rect,isi,garis,tebal)` (panel rounded+bevel+shadow), `StripAtas`, `Teks(rect,teks,ukuran,warna,anchor,tebal)`.
  - Tombol: `GayaTombol(ukuran)` (biru default), `GayaTombolAksen(ukuran)` (hijau aksi).
  - Font: `FontUtama` = `Resources/ThaleahPixel.ttf` (pixel).
  - ⚠️ Semua tekstur runtime pakai `HideFlags.HideAndDontSave` biar tak terhapus saat scene reload.
- **`Ikon.cs`** — ikon prosedural (tanpa file). Skill: `petir, peluru, target, chevron, hati, berlian, pisau, aura, roket, bintang`. Item: `bom, magnet, peti` (berwarna). UI: **`Piala`** (trophy, dipakai panel rekor). `Ikon.UntukSkill(id)` mengutamakan FILE `Resources/Icons/<id>.png` via `Dari()`, fallback ikon kode. **PENTING:** `Ikon.UntukItem(id)` (bom/magnet/peti), mata uang (koin/permata di `MataUang.cs`), dan `Piala` **belum** lewat `Dari()` — masih ikon kode, jadi butuh hook kalau mau pakai PNG (lihat §9). Gambar: `Ikon.Gambar(rect, tex, warnaIsi[, warnaGaris])`.

### HUD & menu (IMGUI, digambar di `OnGUI`)
- **`GameMenu.cs`** — state game: `SedangMain`, `SedangJeda` (static). Home/pause/settings set `Time.timeScale=0`. Latar home = demo battle blur via `LatarDemo.Gambar(w,h)`. **Judul Home** saat ini teks "SALDOKU / LAST STAND" (font pixel) — ⚠️ AKAN DIGANTI nama (lihat §7). Jaraknya sudah DIRAPATKAN. **Panel rekor** kini menampilkan `[ikon Piala] angka` (label "REKOR TERTINGGI" & 2 bintang SUDAH DIHAPUS). Helper `GameMenu.Tombol(...)`, `UlangiDanMain()`, `KeHome()`.
- **`LatarDemo.cs`** — latar menu home: simulasi battle blur (dasar rumput hijau + hint blur + partikel). Kelas: `Musuh/Tracer/Kilat/Bara`.
- **`LevelSystem.cs`** — panel HUD kiri-atas. **BARIS ATAS = `LEVEL x` (TERPISAH di kiri, tulisan DIPERBESAR `fLv*1.28`, ruang `lvW` disesuaikan lebar teks "LEVEL 88" = pas untuk 2 digit) + ikon HATI merah + BAR NYAWA** yang mengisi sisa lebar ke kanan (isi warna dinamis hijau→kuning→merah dari `PlayerHealth.Instance`, angka `HP / MAX` di TENGAH bar). Karena `lvW` mengikuti lebar teks, hati & bar MERAPAT (tidak ada celah kosong lebar). Tulisan LEVEL TIDAK menimpa bar. **BARIS BAWAH = bar XP biru.** `LevelSystem.TinggiPanel(w)` dipakai HUD lain untuk sejajar.
- **`GameTimer.cs`** — timer bertahan (kanan, sejajar skor) + **bar nyawa BOSS** (tengah, muncul saat `EnemyChase.JumlahBos>0`). Contoh bagus pemakaian `BarIsi` + `Panel9`.
- **`ScoreManager.cs`** — skor & rekor (`RekorTertinggi`).
- **`PlayerHealth.cs`** — nyawa player + layar Game Over (revive/tonton iklan sekali, main lagi, home). **HP bar sekarang TIDAK digambar di sini** — sudah dipindah menyatu ke panel LEVEL di `LevelSystem.cs`. Menyediakan `public static Instance`, `health`, `maxHealth`, `Kurangi/Pulih/HidupLagi`.

### Dunia / gameplay
- **`ArenaTakTerbatas.cs`** — LANTAI tak terbatas yang mengikuti kamera & berulang (tiled). Meng-\"bake\" satu tekstur petak besar (UKURAN_TILE=24, RES=384, ppu=16) berisi: rumput dasar cerah + noise, 28 bercak nuansa rumput, bercak TANAH coklat, rumput kecil, BUNGA warna-warni, kerikil. Stamp WRAP-AROUND (modulo) agar menyambung mulus. Helper: `Clamp01/Acak/Jit/Titik/BlobLembut/BlobPadat/RumputKecil/Bunga`. Seed `2026`. **Pohon/batu/semak TIDAK di sini** (dulu iya, sudah dipindah — lihat `RintanganArena.cs`).
- **`RintanganArena.cs`** — **objek rintangan NYATA** (pohon/batu/semak) yang timbul, menabrak, & menutupi player. MonoBehaviour auto-create (`RuntimeInitializeOnLoadMethod` + `sceneLoaded`). Chunk-spawn per sel di sekitar kamera (`SEL=8`, `RADIUS=3`, seed deterministik), di-recycle saat jauh. Sprite prosedural (pivot di kaki + bayangan lembut, ppu=64). Kelas dalam file: `RintanganArena`, `UrutanY` (Y-sort via `SortingGroup`), `Rintang` (solid+radius), `PemainTabrak` (dorong player keluar, dijaga `GameMenu.SedangMain`). `public static readonly List<Halangan> Halangans` di-rebuild tiap frame.
- **`PlayerMovement.cs`** — player gerak via `transform.position +=` (BUKAN fisika). Tag `Player`.
- **`EnemyChase.cs`** — musuh via `Rigidbody2D.MovePosition` di `FixedUpdate`. Tag `Enemy`. Punya `JumlahBos`, `BosSaatIni`, `NyawaSisa/NyawaMaks`, `KenaSerangan(dmg)`.
- **`CameraFollow.cs`** — kamera orthographic (MainCamera, Z=-10, size 10), mengikuti Player.
- **`KarakterPemain.cs`** — rig karakter (anak bernama `*_Character_*`, mis. `Ninja_Character_5`).
- **`ZombieSpawner.cs`**, **`SkillManager.cs`** (`AktifMemilih`), **`ModeDewa.cs`** (`Aktif`, `MenuTerbuka`), **`MataUang.cs`**, **`Toko.cs`**, **`SoundManager.cs`** (`Klik/PlayerKena/GameOver`).
- Skrip lain di `Assets/`: `Roket.cs, PisauOrbit.cs, Ledakan.cs, HitEffect.cs, Bullet.cs, SenjataManager.cs, PlayerShooting.cs, KarakterManager.cs, PratinjauKarakter.cs, SaldokuKoin.cs, PeluruMusuh.cs, XpGem.cs, PermataGem.cs, ItemLapangan.cs, VirtualJoystick.cs, Saldoku.cs, IklanKoin.cs, ScoreManager.cs`.

---

## 3. Keputusan Desain (penting, jangan diubah tanpa alasan)

1. **Semua visual dibuat lewat kode** (tanpa aset gambar). Kalau nambah ikon, tetap sediakan fallback kode + dukungan file `Resources/Icons/`.
2. **Player gerak via transform (bukan physics)**, jadi tabrakan rintangan pakai **push-out manual** (`PemainTabrak`), bukan collider fisika.
3. **Musuh menembus rintangan** (hanya player yang menabrak) — sengaja, biar swarm musuh tidak macet. **User belum keberatan.**
4. **Y-sort relatif kamera** untuk occlusion (lihat §4 bug fix). Lantai `sortingOrder = -9` (paling bawah), jangan diubah.
5. **Pohon/batu/semak = objek nyata** (RintanganArena), **bukan** bagian tekstur lantai (ArenaTakTerbatas). Lantai hanya rumput/tanah/bunga/kerikil.
6. **HP bar player = baris atas panel LEVEL/XP** dengan urutan: **`LEVEL x` (teks terpisah, diperbesar, ruang pas 2 digit) + ikon HATI + bar nyawa** (baris bawah = XP). Tulisan LEVEL TIDAK menimpa bar. Jangan bikin HP bar terpisah lagi.
7. **Panel rekor Home** = `[ikon Piala] angka` saja (tanpa label teks & tanpa bintang).
8. **RENCANA ASET:** user akan mengganti SELURUH ikon/logo dengan render AI sendiri (lihat CHECKLIST §9). **Gaya seni target = ikon Survivor.io asli: glossy, semi-3D render, chunky, cartoon mengilap — BUKAN pixel-art, BUKAN realistis** (referensi: screenshot "Weapons & evolutions" dari user). Filosofi ikon-kode tetap jadi FALLBACK bila file belum ada.
9. **NAMA GAME akan diganti** (bukan "SALDOKU LAST STAND"). Nama baru belum ditentukan — pengaruhi judul Home (`GameMenu`), subtitle, logo, dan (opsional) app name. Lihat §7.

---

## 4. Yang Ditemukan & Diperbaiki

### 🏠 Rapikan Home + ❤️ layout bar nyawa — sesi 2026-08-28 (lanjutan)
- **Judul Home dirapatkan** (`GameMenu.cs`): jarak `SALDOKU` → `LAST STAND` → subtitle dipadatkan (SALDOKU `y=0.075h`, LAST STAND `y=0.138h`, subtitle `y=0.205h`) supaya tidak melebar ke atas.
- **Panel rekor** (`GameMenu.cs`): hapus label "REKOR TERTINGGI" & 2 ikon bintang. Sekarang render **`[ikon Piala] angka`** (ikon + `ScoreManager.Instance.RekorTertinggi`) yang di-center sebagai grup.
- **Ikon `Piala`** baru (`Ikon.cs`): trophy prosedural (mangkuk setengah-elips + rim + pegangan C kiri/kanan + batang + alas), dirender via `Buat(FPiala,72)` (monokrom + gradasi + outline, diwarnai `Tema.Amber`).
- **Layout bar nyawa** (`LevelSystem.cs`): dari "LEVEL menimpa bar" menjadi **`LEVEL x` TERPISAH di kiri → ikon HATI → BAR NYAWA mengisi sisa ke kanan**. Angka `HP / MAX` di TENGAH bar. Warna bar tetap dinamis hijau→kuning→merah.
- **Tuning bar nyawa (revisi):** celah terasa terlalu jauh karena tulisan LEVEL kecil di ruang tetap `rowW*0.26`. Diperbaiki: tulisan LEVEL **diperbesar** (`fLvTeks = min(fLv*1.28, hpH*0.92)`) dan `lvW` disesuaikan lebar teks (`"LEVEL 88".Length * fLvTeks*0.62`) = **ruang pas untuk 2 digit** (level 10, 88, dst). Hati mulai `rowX + lvW + hpH*0.06`, sehingga hati & bar MERAPAT. Kalau perlu tetap tuning: `charW` (0.62) atau string acuan `"LEVEL 88"`.

### ❤️➡️📊 HP bar DIPINDAH menyatu ke panel LEVEL/XP — sesi 2026-08-28
- HP bar player (dulu terpisah di bawah baris skor) dipindah menyatu ke panel LEVEL. `PlayerHealth.cs` tidak lagi menggambar HP bar sendiri (method `GambarBarNyawa()` dihapus).

### 🐛 BUG: player hilang di tempat tertentu — DIPERBAIKI
- **Gejala:** di titik tertentu, sprite player menghilang.
- **Akar masalah:** `UrutanY` (di `RintanganArena.cs`) dulu memakai **sortingOrder ABSOLUT** `= -(y+offsetY)*100`. Saat player berjalan ke Y positif besar, order jatuh **di bawah** `sortingOrder` lantai (`-9`) → player ter-render di belakang lantai → tampak hilang.
- **Perbaikan:** `UrutanY` kini menghitung order **relatif terhadap Y kamera** dengan **basis besar** (`BASE=20000`, `order = BASE - round((y+offsetY-camY)*100)`) supaya actor/rintangan selalu di atas lantai, occlusion antar-objek tetap benar. Lantai `-9` tidak diubah.

### 🌸 Kepadatan bunga & tanah dikurangi (`ArenaTakTerbatas.cs`)
- `jmlTanah`: `6+Next(3)` → `3+Next(2)`; `jmlBunga`: `30+Next(14)` → `9+Next(5)`; grup bunga/kluster: `1+Next(4)` → `1+Next(2)`; `jmlKerikil`: `26+Next(14)` → `16+Next(8)`.

---

## 5. Log Perubahan (commit utama)

| Urutan | File | Ringkas |
|---|---|---|
| 1 | `RintanganArena.cs` | Sistem rintangan nyata (pohon/batu/semak) + tabrakan + occlusion; lalu fix Y-sort relatif kamera |
| 2 | `ArenaTakTerbatas.cs` | Lantai floor-only; lalu kurangi kepadatan bunga/tanah/kerikil |
| 3 | `LatarDemo.cs` | Latar home = demo battle blur, dasar rumput hijau |
| 4 | `Tema.cs` | Tema cerah Survivor.io (oranye-emas, tombol biru/hijau) |
| 5 | `PlayerHealth.cs` | HP bar bertema + sembunyikan bar sprite lama; lalu HP bar terpisah DIHAPUS (pindah ke LevelSystem) |
| 6 | `LevelSystem.cs` | HP bar menyatu di panel LEVEL; layout `LEVEL x` terpisah + hati + bar; lalu tulisan LEVEL diperbesar + ruang pas 2 digit |
| 7 | `Ikon.cs` | Tambah ikon **`Piala`** (trophy) prosedural untuk panel rekor |
| 8 | `GameMenu.cs` | Rapatkan judul SALDOKU/LAST STAND + panel rekor `[Piala] angka` (hapus label & bintang) |

(Perubahan sebelumnya: UI semi-3D di `Tema/LevelSystem/GameTimer`, `GameMenu` demo-blur, `Ikon.cs` chevron image-loadable.)

---

## 6. SHA File Terbaru (update tiap kali push!)

> Ambil SHA terbaru dengan `get_file_contents` sebelum meng-update, karena bisa berubah tiap commit.

- `RintanganArena.cs` = `fdab1dd5ca477a10125b22183e7606e24b4cbe98`
- `ArenaTakTerbatas.cs` = `ba6cfe370ae2122770036f6dc604564104fd391d`
- `PlayerHealth.cs` = `7cbbc7151868b1b0f74c8f9938a99f6600a3b37e`
- `Tema.cs` = `7fbbeb537933829e721a4fdabd4222006f3517fb`
- `LatarDemo.cs` = `11c18ee27245586574eb474f4ebe93a54fa9d47d`
- `GameMenu.cs` = `5777a7a0cc280fcdb951566bb7b4d6ff18d10770`
- `GameTimer.cs` = `9a6c26cbbfe438ced4479d4bf068cf2888913ffe`
- `LevelSystem.cs` = `432ca79478bfd67281e1d772640336a929126c23`
- `Ikon.cs` = `27ff259e4c25fdad923467713534166f11b2b0a4`
- `MataUang.cs` = `571f605a5def5224fd1dbe175cbe85fa82cf4acc`
- `SkillManager.cs` = `86df8e0d7a87147ae1f80cee2d7e6357abf88a79`
- `ZombieSpawner.cs` = `a7b4c347f378b14a13091795c650597b5c47bb9b`
- `CameraFollow.cs` = `7e8e1def31aac471069a6eabb1dc6a8bdb07e815`
- `PlayerMovement.cs` = `1bb55711f93f010c9b102663a51bb2ef2570ac49`
- `PlayerShooting.cs` = `e1cfb76ef0d13da71d22b9fc8dae9fbff1632a48`
- `KarakterPemain.cs` = `fc4b8a15457a97dff152084d000ee416ac832616`
- `EnemyChase.cs` = `c0fe04f79debfd964a7b7072ca940133e6fc6f79`

---

## 7. Tugas Lanjutan (belum dikerjakan)

1. **🆕 GANTI NAMA GAME** — user mau ganti nama (BUKAN "SALDOKU LAST STAND"). **Nama baru belum ditentukan.** Setelah nama final:
   - Update judul Home + subtitle di `GameMenu.cs`.
   - Baru bikin **logo** (§9 grup E) dengan nama baru.
   - (Opsional) update nama aplikasi di Player Settings + appicon.
2. **Ganti seluruh ikon/logo dengan render AI** — lihat **CHECKLIST + prompt lengkap di §9**. User render sendiri via ChatGPT/Flow, taruh PNG di `Assets/Resources/Icons/<id>.png`. Item bertanda 🔧 perlu di-hook dulu oleh AI. **PROGRES:** lihat tanda ceklis & status di §9. Semua prompt ikon (grup A–D) SUDAH dikirim; user lagi render satu-satu.
3. **(Opsional) Musuh ikut menabrak rintangan** — sekarang hanya player.
4. **(Opsional) Tuning sorting peluru/gem** kalau occlusion terlihat aneh.
5. **(Opsional) Tuning kepadatan rintangan** di `RintanganArena.cs` (`SEL/RADIUS`/peluang spawn).
6. **(Opsional) Verifikasi visual di device asli:** bar nyawa (tulisan LEVEL & ruang 2 digit), panel rekor (Piala + angka center), jarak judul Home.

---

## 8. Info Kontekstual

- **GitHub user:** `muhrizky645-png` (muhrizky645@gmail.com).
- **Bahasa komunikasi dengan user:** Indonesia.
- **Timezone:** Asia/Jakarta.
- **Konvensi balasan AI:** bahasa Indonesia, ringkas, sebutkan yang dikerjakan, tawarkan langkah lanjut, dan (bila mengubah file) sertakan referensi commit.

---

## 9. ✅ CHECKLIST IKON / LOGO KUSTOM (render via AI)

> **Rencana user:** render SENDIRI semua ikon lewat ChatGPT/Flow (atau AI gambar lain), lalu
> taruh file PNG di `Assets/Resources/Icons/<id>.png`. **Pakai id PERSIS** seperti di bawah
> (huruf kecil, tanpa spasi). Setelah satu ikon selesai, ganti `- [ ]` jadi `- [x]`.
>
> **🔄 ALUR KERJA SAAT INI (per 2026-08-29):** user render ikon SATU per satu berurutan.
> Tiap kali user minta "prompt berikutnya", AI: (a) beri prompt final (BASE STYLE + Subjek),
> (b) update progres di checklist ini. Tanda status per item: **[x] = sudah dirender & OK**,
> **⏳ = prompt sudah dikirim, nunggu hasil render dari user**, **[ ] = belum**.
> **Style reference:** gunakan `petir.png` (ikon pertama, hasil bagus & disetujui user) sebagai
> acuan gaya untuk ikon-ikon berikutnya biar seragam.
> ⚠️ **LOGO DITUNDA:** user mau GANTI NAMA game dulu (bukan "SALDOKU LAST STAND"). Prompt logo
> baru dibuat setelah nama final ditentukan (lihat §7).
>
> **🎨 GAYA SENI TARGET (WAJIB):** meniru ikon **Survivor.io asli** — render **glossy semi-3D**,
> bentuk **chunky/tebal**, cartoon **mengilap**, outline gelap tebal, highlight kilau + bayangan
> lembut, warna cerah & jenuh. **BUKAN pixel-art. BUKAN realistis/foto.** (Referensi: screenshot
> "Weapons & evolutions" yang dikirim user — gaya persis seperti itu.)
>
> **Status wiring (tanda di tiap item):**
> - ✅ = otomatis kebaca dari file (lewat `Ikon.Dari`). Tinggal taruh PNG → langsung dipakai, fallback ikon kode kalau file belum ada.
> - 🔧 = **butuh hook kode dulu**. Setelah PNG siap, minta AI: *"wire-kan ikon <id> supaya kebaca dari Resources/Icons"* (AI arahkan `UntukItem`/`MataUang`/`Piala` ke `Ikon.Dari`).
>
> **Spesifikasi file:** PNG, background TRANSPARAN, persegi (mis. 1024×1024), objek di tengah, ada ruang kosong tipis di pinggir.

### 🎨 BASE STYLE (tempel di DEPAN tiap prompt, lalu tambahkan "Subjek")

````text
Mobile game item icon in the style of Survivor.io / Archero, glossy semi-3D rendered look,
stylized chunky cartoon, thick dark outline, smooth rounded shapes, rich gradient shading
with glossy specular highlights and soft ambient occlusion, vibrant saturated colors, subtle
top-left light source, single object centered, clean isolated render on a transparent
background, square 1024x1024, crisp and readable at small size. NOT pixel-art, NOT flat, NOT
photorealistic, no text, no watermark, no ground shadow.
````

**Prompt final = BASE STYLE + spasi + Subjek item.**

### A. Skill / Buff  — ✅ sudah file-ready (`Ikon.UntukSkill` → `Dari`)

- [x] ✅ **petir** → `Icons/petir.png` — (✅ SUDAH dirender, hasil bagus & disetujui user; jadi style reference) Subjek: `a bold glossy yellow lightning bolt, chain lightning power`
- [ ] ⏳ ✅ **peluru** → `Icons/peluru.png` — (prompt terkirim, user render) Subjek: `three shiny golden bullets stacked pointing up, extra projectile buff`
- [ ] ⏳ ✅ **target** → `Icons/target.png` — (prompt terkirim, user render) Subjek: `a red-and-white bullseye target with crosshair, critical/aim buff`
- [ ] ⏳ ✅ **chevron** → `Icons/chevron.png` — (prompt terkirim, user render) Subjek: `double upward chevron arrows, glossy green, attack/move speed buff`
- [ ] ⏳ ✅ **hati** → `Icons/hati.png` — (prompt terkirim, user render) Subjek: `a plump glossy red heart, max health / heal buff`
- [ ] ⏳ ✅ **berlian** → `Icons/berlian.png` — (prompt terkirim, user render) Subjek: `a brilliant faceted blue diamond gem, luck/bonus buff`
- [ ] ⏳ ✅ **pisau** → `Icons/pisau.png` — (prompt terkirim, user render) Subjek: `a shiny steel four-point throwing blade, orbiting knife weapon`
- [ ] ⏳ ✅ **aura** → `Icons/aura.png` — (prompt terkirim, user render) Subjek: `concentric glowing energy rings radiating outward, purple aura damage field`
- [ ] ⏳ ✅ **roket** → `Icons/roket.png` — (prompt terkirim, user render) Subjek: `a small stubby rocket/missile pointing up with fins and flame, rocket weapon`
- [ ] ⏳ ✅ **bintang** → `Icons/bintang.png` — (prompt terkirim, user render) Subjek: `a bold glossy golden five-pointed star, generic upgrade / default icon`

### B. Item Lapangan  — 🔧 butuh hook (`Ikon.UntukItem`)

- [ ] ⏳ 🔧 **bom** → `Icons/bom.png` — (prompt terkirim, user render) Subjek: `a round black cartoon bomb with a lit sparking fuse, screen-clear item`
- [ ] ⏳ 🔧 **magnet** → `Icons/magnet.png` — (prompt terkirim, user render) Subjek: `a glossy red horseshoe magnet with silver poles, attract-pickups item`
- [ ] ⏳ 🔧 **peti** → `Icons/peti.png` — (prompt terkirim, user render) Subjek: `a wooden treasure chest with gold trim and a lock, reward crate`

### C. Mata Uang & UI  — 🔧 butuh hook (`MataUang.cs` / `Ikon.Piala`)

- [ ] ⏳ 🔧 **koin** → `Icons/koin.png` — (prompt terkirim, user render) Subjek: `a shiny round gold coin with a simple embossed emblem, game currency`
- [ ] ⏳ 🔧 **permata** → `Icons/permata.png` — (prompt terkirim, user render) Subjek: `a violet/purple faceted crystal gem, premium currency`
- [ ] ⏳ 🔧 **piala** → `Icons/piala.png` — (prompt terkirim, user render) Subjek: `a golden victory trophy cup with two side handles on a base, high score`

### D. XP / Permata Lapangan  — 🔧 butuh hook (`XpGem.cs` / `PermataGem.cs`, sprite dunia)

- [ ] ⏳ 🔧 **xpgem** → `Icons/xpgem.png` — (prompt terkirim, user render) Subjek: `a small glowing cyan/blue XP crystal shard, floating pickup`
- [ ] ⏳ 🔧 **permatagem** → `Icons/permatagem.png` — (prompt terkirim, user render) Subjek: `a small purple gem pickup with sparkle`

### E. Opsional (logo & app)

- [ ] ⏸️ 🔧 **logo** (wordmark) → `Icons/logo.png` — **DITUNDA: nunggu NAMA GAME baru** (user mau ganti nama, bukan "SALDOKU LAST STAND"). Setelah nama final, buat prompt wordmark: `game logo wordmark reading "<NAMA BARU>", bold glossy 3D letters, army green and amber with red accents` lalu wire-kan di `GameMenu`.
- [ ] **appicon** (ikon aplikasi Android) → di-set di Player Settings (bukan Resources). Nunggu nama/tema final. Subjek: `mobile game app launcher icon, glossy 3D, a heart + trophy motif, bold, centered, filled background`.

> **Catatan untuk AI sesi berikutnya:** kalau user bilang "ikon X sudah aku render", (1) pastikan file ada di `Assets/Resources/Icons/X.png`, (2) untuk item 🔧 tambahkan hook `Dari("X", <ikonKodeBawaan>)`, (3) ceklis item ini di §9, (4) update §6 SHA.
