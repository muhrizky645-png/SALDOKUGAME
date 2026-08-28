# 🧠 HANDOFF AI — SALDOKUGAME

> **Tujuan file ini:** catatan estafet supaya sesi AI berikutnya (di chat lain) langsung nyambung tanpa perlu menjelaskan ulang. Berisi konteks proyek, arsitektur, semua yang sudah dikerjakan, keputusan desain, bug yang sudah diperbaiki, dan tugas lanjutan.
>
> **Terakhir diperbarui:** 2026-08-28 (Asia/Jakarta)
>
> ⚠️ **Untuk AI sesi berikutnya:** setelah menyelesaikan pekerjaan penting, **perbarui file ini** (bagian "Log Perubahan", "SHA File Terbaru", dan "Tugas Lanjutan").

---

## 1. Ringkasan Proyek

- **Repo:** `muhrizky645-png/SALDOKUGAME` (privat, branch `main`).
- **Engine:** Unity (game 2D top-down **survivor / roguelite** ala **Survivor.io**).
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
- **`Ikon.cs`** — ikon prosedural (tanpa file). Skill: `petir, peluru, target, chevron, hati, berlian, pisau, aura, roket, bintang`. Item: `bom, magnet, peti` (berwarna). `Ikon.UntukSkill(id)` & `Ikon.UntukItem(id)` mengutamakan FILE `Resources/Icons/<id>.png`, fallback ikon kode. Gambar: `Ikon.Gambar(rect, tex, warnaIsi[, warnaGaris])`.

### HUD & menu (IMGUI, digambar di `OnGUI`)
- **`GameMenu.cs`** — state game: `SedangMain`, `SedangJeda` (static). Home/pause/settings set `Time.timeScale=0`. Latar home = demo battle blur via `LatarDemo.Gambar(w,h)`. Helper `GameMenu.Tombol(...)`, `UlangiDanMain()`, `KeHome()`.
- **`LatarDemo.cs`** — latar menu home: simulasi battle blur (dasar rumput hijau + hint blur + partikel). Kelas: `Musuh/Tracer/Kilat/Bara`.
- **`LevelSystem.cs`** — bar level + XP di atas. `LevelSystem.TinggiPanel(w)` dipakai HUD lain untuk sejajar.
- **`GameTimer.cs`** — timer bertahan (kanan, sejajar skor) + **bar nyawa BOSS** (tengah, muncul saat `EnemyChase.JumlahBos>0`). Contoh bagus pemakaian `BarIsi` + `Panel9`.
- **`ScoreManager.cs`** — skor & rekor.
- **`PlayerHealth.cs`** — nyawa player + **HP bar bertema** (lihat §4) + layar Game Over (revive/tonton iklan sekali, main lagi, home).

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

---

## 4. Yang Ditemukan & Diperbaiki (sesi ini)

### 🐛 BUG: player hilang di tempat tertentu — DIPERBAIKI
- **Gejala:** di titik tertentu, sprite player menghilang.
- **Akar masalah:** `UrutanY` (di `RintanganArena.cs`) dulu memakai **sortingOrder ABSOLUT** `= -(y+offsetY)*100`. Saat player berjalan ke Y positif besar, nilai order jatuh **di bawah** `sortingOrder` lantai (`-9`), sehingga player ter-render **di belakang lantai/rumput** → tampak hilang.
- **Perbaikan:** `UrutanY` kini menghitung order **relatif terhadap Y kamera** dengan **basis besar** supaya actor/rintangan **selalu di atas lantai**, sementara occlusion antar-objek tetap benar:
  ```csharp
  const int BASE = 20000; static Camera _cam;
  void LateUpdate(){
    if(_cam==null)_cam=Camera.main;
    float camY=_cam!=null?_cam.transform.position.y:0f;
    if(sg!=null) sg.sortingOrder = BASE - Mathf.RoundToInt((transform.position.y+offsetY-camY)*100f);
  }
  ```
  Y lebih tinggi → order lebih kecil → di belakang (occlusion benar). Lantai `-9` tidak diubah.

### 🌸 Kepadatan bunga & tanah dikurangi (`ArenaTakTerbatas.cs`)
- User: bunga & lumpur/tanah terlalu penuh. Batu/pohon/rumput sudah bagus & sudah menabrak.
- Perubahan hitungan (elemen lain tidak diubah):
  - `jmlTanah`: `6 + Next(3)` → `3 + Next(2)`
  - `jmlBunga`: `30 + Next(14)` → `9 + Next(5)` *(dikurangi banyak)*
  - grup bunga per kluster: `1 + Next(4)` → `1 + Next(2)`
  - `jmlKerikil`: `26 + Next(14)` → `16 + Next(8)`

### ❤️ HP bar player dibuat menyatu dengan HUD (`PlayerHealth.cs`)
- **Sebelum:** HP bar = objek **sprite scene** (`hpFill`) yang flat & kotak, beda gaya dari HUD bertema.
- **Sesudah:**
  - `SembunyikanBarLama()` mematikan `SpriteRenderer` bar lama (fill + background + border) agar tidak dobel.
  - `GambarBarNyawa()` (dipanggil di `OnGUI` saat main) menggambar HP bar **bertema**: backing `Panel9` rounded gelap + `BarIsi` gradien, **warna dinamis** (hijau→kuning→merah sesuai sisa nyawa) + **ikon Hati** di ujung kiri + **angka `HP / MAX`** di tengah.
  - Diposisikan tepat di bawah baris skor/timer (pakai `LevelSystem.TinggiPanel` + safe area). Sembunyi saat pause/pilih skill/game over/overlay Peti Dewa.

---

## 5. Log Perubahan (commit utama sesi ini)

| Urutan | File | Ringkas |
|---|---|---|
| 1 | `RintanganArena.cs` | Buat sistem rintangan nyata (pohon/batu/semak) + tabrakan + occlusion; lalu **fix Y-sort relatif kamera** (bug player hilang) |
| 2 | `ArenaTakTerbatas.cs` | Lantai floor-only (pohon dll dipindah keluar); lalu **kurangi kepadatan bunga/tanah/kerikil** |
| 3 | `LatarDemo.cs` | Latar home = demo battle blur, dasar rumput hijau (kurangi dominasi kuning) |
| 4 | `Tema.cs` | Tema cerah Survivor.io (oranye-emas, tombol biru/hijau) |
| 5 | `PlayerHealth.cs` | **HP bar bertema** + sembunyikan bar sprite lama |

(Perubahan sebelumnya: UI semi-3D di `Tema/LevelSystem/GameTimer`, `GameMenu` demo-blur, `Ikon.cs` chevron image-loadable.)

---

## 6. SHA File Terbaru (update tiap kali push!)

> Ambil SHA terbaru dengan `get_file_contents` sebelum meng-update, karena bisa berubah tiap commit.

- `RintanganArena.cs` = `fdab1dd5ca477a10125b22183e7606e24b4cbe98`
- `ArenaTakTerbatas.cs` = `ba6cfe370ae2122770036f6dc604564104fd391d`
- `PlayerHealth.cs` = `becfb5daaa679d7befade083f1c77f3cfee61270`
- `Tema.cs` = `7fbbeb537933829e721a4fdabd4222006f3517fb`
- `LatarDemo.cs` = `11c18ee27245586574eb474f4ebe93a54fa9d47d`
- `GameMenu.cs` = `906d54b5121c182c942dd6350a1ba6dd672bbeeb`
- `GameTimer.cs` = `9a6c26cbbfe438ced4479d4bf068cf2888913ffe`
- `LevelSystem.cs` = `e74913302efd555c26d5ce3e717f684f545591c9`
- `Ikon.cs` = `f167b2cd1683a8b7abd84ac80125f73b901c3e38`
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

1. **Ikon skill via PNG** (ditunda oleh user): user mungkin render ikon lewat ChatGPT/Flow → zip → upload → AI push ke `Assets/Resources/Icons/<id>.png`. Skill id: `petir, peluru, target, chevron, hati, berlian, pisau, aura, roket`. Semua sudah image-loadable via `Ikon.Dari`.
   - **Prompt tema ikon (ChatGPT):** *2D pixel-art game icon, survivor.io / survival roguelite style, single centered emblem, thick dark outline, flat cel shading with slight top light, palette army green #A9D961 / blood red #D12B21 / amber gold #FFCC38 / bone white #F2F0DE, transparent background, square 1024×1024, readable small, no text, no drop shadow.*
2. **(Opsional) Musuh ikut menabrak rintangan** — sekarang hanya player. Perlu hati-hati agar swarm tidak macet.
3. **(Opsional) Tuning sorting peluru/gem** kalau occlusion terlihat aneh.
4. **(Opsional) Tuning kepadatan rintangan** (hutan lebat vs jarang) di `RintanganArena.cs` (`SEL/RADIUS`/peluang spawn).
5. **(Opsional) Verifikasi visual HP bar** — cek posisi/ukuran di device asli; sesuaikan bila perlu.

---

## 8. Info Kontekstual

- **GitHub user:** `muhrizky645-png` (muhrizky645@gmail.com).
- **Bahasa komunikasi dengan user:** Indonesia.
- **Timezone:** Asia/Jakarta.
- **Konvensi balasan AI:** bahasa Indonesia, ringkas, sebutkan yang dikerjakan, tawarkan langkah lanjut, dan (bila mengubah file) sertakan referensi commit.
