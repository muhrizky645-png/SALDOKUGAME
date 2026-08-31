# Fase 0 — Fondasi

Dokumen ini menjelaskan apa yang berubah, apa yang harus kamu lakukan manual
di Unity, dan apa langkah berikutnya.

---

## Kenapa Fase 0 ada

PRD menargetkan 20 senjata, 22 pasif, 12 arketipe musuh, 10 bos, 100 stage,
dan 320 musuh hidup bersamaan pada 60 FPS.

Kode saat ini tidak bisa sampai ke sana bukan karena kurang fitur, tapi karena
tiga hambatan struktural:

1. **Pencarian musuh memakai `FindGameObjectsWithTag`.** Fungsi ini menyisir
   seluruh scene dan mengalokasikan array baru tiap pemanggilan. Dipanggil
   dari empat tempat, salah satunya 2,5 kali per detik.
2. **Angka balancing tertanam di dalam C#.** Contoh nyata di `SenjataManager`:
   `int dmg = 3 + lvOrbit * 2 + (evo ? 5 : 0);`
   Mengubah damage berarti compile ulang. Menambah senjata berarti menyalin
   blok `if` baru.
3. **Tidak ada alat ukur.** Tidak ada yang tahu FPS sebenarnya di HP saat
   layar penuh musuh, jadi semua target PRD masih berupa tebakan.

Fase 0 membereskan ketiganya. **Tidak ada fitur baru yang ditambahkan** —
ini murni pembersihan jalan.

---

## Yang berubah

### File baru

| File | Fungsi |
|---|---|
| `Assets/Scripts/Core/EnemyRegistry.cs` | Daftar pusat musuh hidup + spatial grid 4×4 untuk query jarak |
| `Assets/Scripts/Data/SenjataSO.cs` | Data senjata sebagai aset, termasuk syarat evolusi |
| `Assets/Scripts/Data/PasifSO.cs` | Data skill pasif sebagai aset |
| `Assets/Scripts/Data/MusuhSO.cs` | Data musuh sebagai aset, 12 arketipe PRD |
| `Assets/Scripts/Data/StageSO.cs` | Data stage sebagai aset, chapter + tema + komposisi musuh |
| `Assets/Scripts/Debug/StressTest.cs` | Overlay FPS + tombol spawn massal |

### File yang diubah

| File | Perubahan |
|---|---|
| `Assets/EnemyChase.cs` | Lapor ke registry di `OnEnable`, keluar di `OnDisable` dan saat `Mati()`. `SudahMati` dibuka sebagai properti publik. |
| `Assets/SenjataManager.cs` | Tiga pemanggilan `FindGameObjectsWithTag` dihapus. Aura, roket, dan `MusuhTerdekat` sekarang lewat registry. |
| `Assets/ZombieSpawner.cs` | Hitung musuh lewat registry. **Pengali stage akhirnya tersambung.** Tambah `SpawnPaksa()`. |
| `.gitignore` | Abaikan folder artefak build |

---

## Bug yang ikut terperbaiki

Dua hal yang bukan sekadar optimasi:

**1. Pengali kesulitan stage tidak pernah dipakai.**
`StageManager.PengaliMusuhSekarang` sudah ada sejak lama dengan nilai
1.0 / 1.15 / 1.3 / 1.5 — tetapi tidak ada satu pun kode yang membacanya.
Akibatnya keempat stage praktis identik dan hanya berbeda durasi. Sekarang
pengali benar-benar mempengaruhi nyawa musuh, nyawa bos, dan batas jumlah
musuh hidup.

**2. Senjata menembaki mayat.**
`EnemyChase.Mati()` menunda `Destroy` selama `waktuHancur` (1 detik) supaya
animasi mati sempat jalan. Selama satu detik itu objeknya masih ber-tag
`"Enemy"`, jadi aura dan roket tetap menganggapnya sasaran sah dan membuang
damage ke sana. Sekarang musuh keluar dari registry tepat saat mati.

---

## Yang harus kamu lakukan manual

Ada beberapa hal yang tidak bisa dikerjakan lewat commit:

### 1. Buka project di Unity sekali

Unity akan otomatis membuat file `.meta` untuk keenam script baru. Tanpa ini
file-nya ada di disk tapi tidak dikenali Unity.

### 2. Hapus artefak build dari pelacakan git

Menambahkan baris di `.gitignore` tidak menghapus file yang terlanjur
terlacak. Jalankan sekali:

```bash
git rm -r --cached "SALDOKUGAME_BurstDebugInformation_DoNotShip"
git rm -r --cached "Saldokugame_BackUpThisFolder_ButDontShipItWithYourGame"
git rm -r --cached "debug_BackUpThisFolder_ButDontShipItWithYourGame"
git commit -m "Hapus artefak build dari pelacakan git"
```

### 3. JALANKAN UJI BEBAN — ini yang terpenting

1. `File > Build Settings` → centang **Development Build**
2. Build APK, pasang di HP
3. Main seperti biasa, ketuk tombol FPS di pojok kiri atas
4. Tekan **+100** tiga kali
5. Catat angka **1% rendah**

**Perhatikan angka "1% rendah", bukan rata-rata.** Angka itu mewakili frame
terburuk, dan justru itulah yang dirasakan pemain sebagai patah-patah.
Rata-rata 60 FPS dengan 1% rendah 12 FPS tetap terasa rusak.

**Target: 1% rendah minimal 30 FPS pada 300 musuh.**

Hasilnya menentukan langkah berikutnya:

| Hasil | Artinya |
|---|---|
| 1% rendah ≥ 30 | Aman. Lanjut ke Fase 1, tambah konten. |
| 1% rendah 15–30 | Perlu object pooling dulu sebelum tambah konten. |
| 1% rendah < 15 | Turunkan target PRD. 320 musuh tidak realistis; revisi ke 120–150. |

---

## Yang SENGAJA belum dikerjakan

Agar jujur soal ruang lingkup:

- **Object pooling belum dibuat.** Pooling musuh menyentuh `Instantiate`,
  `SiapkanMusuh`, dan reset state di `EnemyChase`. Membuatnya tanpa hasil
  pengukuran dulu berisiko memperkenalkan bug state yang sulit dilacak.
  Kerjakan setelah uji beban.
- **SenjataManager belum membaca `SenjataSO`.** File SO-nya sudah ada, tapi
  memindahkan tiga senjata yang berjalan ke sistem baru adalah Fase 1.
  Struktur dulu, migrasi kemudian.
- **UI masih `OnGUI` seluruhnya.** Migrasi ke UI Toolkit adalah proyek
  tersendiri dan tidak boleh dicampur dengan perubahan performa.
- **Kritis masih hardcoded di `Bullet.cs`** (`Random.value < 0.22f`,
  `Random.Range(2,4)`). Tidak ada di formula damage PRD dan tidak bisa
  di-upgrade. Perlu jadi stat sungguhan atau dijadikan murni visual.
- **Save masih PlayerPrefs.** Ini risiko tertinggi di seluruh proyek karena
  `SaldokuKoin.cs` menyentuh saldo bernilai nyata. Wajib server-authoritative
  sebelum Saldoku diaktifkan. Bukan isu cheat — isu keuangan.

---

## Urutan berikutnya

1. Uji beban → dapatkan angka
2. Object pooling (kalau perlu berdasarkan angka)
3. Migrasi 3 senjata ke `SenjataSO`
4. Tambah senjata ke-4 sampai ke-10 (setelah langkah 3, ini hitungan menit)
5. Pisahkan slot senjata dan slot pasif di `SkillManager`
6. Gerbang evolusi: butuh peti Elite, bukan otomatis di Lv.5
