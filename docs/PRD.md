# PRD — Project Nightfall: Survival Roguelite Mobile Game (Survivor.io-like)

> **Product Requirements Document (PRD)**
>
> | Field | Value |
> | --- | --- |
> | Nama Produk (working title) | **Project Nightfall** |
> | Genre | Survival Roguelite / Bullet-Heaven / Horde Survival |
> | Platform | Android (primary), iOS (primary), WebGL (opsional, marketing demo) |
> | Engine | Unity 2022 LTS (URP 2D) |
> | Orientasi | Portrait (9:16), locked |
> | Mode | Single-player (online-assisted), asynchronous social |
> | Model Bisnis | Free-to-Play + IAP + Rewarded Ads + Battle Pass |
> | Versi Dokumen | 1.0 (Draft) |
> | Tanggal | 31 Agustus 2026 |
> | Owner | abigalhebeevie3 (Product) |
> | Status | In Review |

---

## Catatan implementasi

Dokumen ini adalah **target**, bukan keadaan repo saat ini. Repo `SALDOKUGAME`
(nama produk in-game: **ZOMBURST**) berada pada tahap jauh lebih awal.

Perbedaan penting yang perlu diingat saat membaca:

| Hal | PRD | Repo sekarang |
| --- | --- | --- |
| Engine | Unity 2022 LTS | Unity 6000.5.8f1 |
| Nama senjata/musuh | Inggris (Kunai, Walker) | Indonesia (Pisau Berputar, Perayap) |
| Jumlah senjata | 20 + 13 evolusi | 3 + 3 evolusi |
| Jumlah item pasif | 22 | 6 |
| Chapter | 10 chapter x 10 stage | 4 stage |
| UI | UI Toolkit + uGUI | seluruhnya OnGUI |

Untuk rencana eksekusi bertahap dari keadaan sekarang menuju dokumen ini,
lihat halaman **GAP Analysis** di Notion.

Rekomendasi scope realistis untuk satu orang ("Zomburst Edition"):
**10 senjata, bukan 20. 5 chapter, bukan 10.**

---

## Daftar Isi

1. [Ringkasan Eksekutif](#1-ringkasan-eksekutif)
2. [Visi, Tujuan & Non-Goals](#2-visi-tujuan--non-goals)
3. [Target Audiens & Persona](#3-target-audiens--persona)
4. [Analisis Kompetitor & Diferensiasi](#4-analisis-kompetitor--diferensiasi)
5. [Core Gameplay Loop](#5-core-gameplay-loop)
6. [Mekanik Gameplay Inti](#6-mekanik-gameplay-inti-detail-teknis)
7. [Sistem Senjata](#7-sistem-senjata)
8. [Item Pasif](#8-item-pasif-passive-items)
9. [Sistem Evolusi](#9-sistem-evolusi-evolution)
10. [Karakter Playable](#10-karakter-playable)
11. [Musuh, Elite & Boss](#11-musuh-elite--boss)
12. [Desain Stage & Chapter](#12-desain-stage--chapter)
13. [Meta Progression](#13-meta-progression-di-luar-run)
14. [Ekonomi & Mata Uang](#14-ekonomi--mata-uang)
15. [Monetisasi](#15-monetisasi)
16. [Live Ops & Event](#16-live-ops--event)
17. [Sosial & Kompetitif](#17-sosial--kompetitif)
18. [Onboarding & FTUE](#18-onboarding--ftue-first-time-user-experience)
19. [UI/UX & Struktur Layar](#19-uiux--struktur-layar)
20. [Art Direction & Audio](#20-art-direction--audio)
21. [Kebutuhan Teknis](#21-kebutuhan-teknis-technical-requirements)
22. [Backend, Save Data & Anti-Cheat](#22-backend-save-data--anti-cheat)
23. [Analytics & KPI](#23-analytics--kpi)
24. [Kerangka Balancing](#24-kerangka-balancing)
25. [Retensi & Notifikasi](#25-retensi--notifikasi)
26. [Lokalisasi & Aksesibilitas](#26-lokalisasi--aksesibilitas)
27. [QA & Testing Plan](#27-qa--testing-plan)
28. [Roadmap & Milestone](#28-roadmap--milestone)
29. [Tim, Resource & Estimasi Anggaran](#29-tim-resource--estimasi-anggaran)
30. [Risiko & Mitigasi](#30-risiko--mitigasi)
31. [Lampiran](#31-lampiran-appendix)

---

# 1. Ringkasan Eksekutif

**Project Nightfall** adalah game mobile survival-roguelite bergaya top-down di mana pemain mengendalikan satu karakter yang **menyerang secara otomatis**, sementara pemain hanya fokus pada **pergerakan (movement)** untuk menghindari gerombolan musuh yang jumlahnya terus bertambah. Setiap sesi berlangsung **15 menit**, di mana pemain naik level, memilih senjata & item pasif secara acak (roguelite draft), meng-evolve senjata, dan bertahan sampai boss akhir muncul.

Di luar sesi, terdapat **meta-progression** yang dalam: equipment/gear dengan rarity, tech parts, skill tree, collectibles, dan upgrade karakter — yang membuat pemain semakin kuat lintas sesi dan mendorong retensi jangka panjang.

**Pilar desain:**

1. **Zero-friction control** — satu jempol, satu virtual joystick. Tidak ada tombol serang.
2. **Power fantasy eskalatif** — dari lemah di menit 0 sampai layar penuh ledakan di menit 15.
3. **Setiap run terasa berbeda** — RNG draft senjata + item + kombinasi evolusi.
4. **Progresi yang selalu terasa** — selalu ada sesuatu yang naik levelnya setiap sesi.
5. **Sesi pendek, bisa dimainkan sambil menunggu** — 3–15 menit per run.

---

# 2. Visi, Tujuan & Non-Goals

## 2.1 Visi Produk

> Menjadi game survival-roguelite mobile nomor satu di pasar Asia Tenggara dengan gameplay yang sangat mudah dipelajari, sistem build yang dalam, dan monetisasi yang adil (fair F2P).

## 2.2 Tujuan Bisnis

| # | Tujuan | Metrik | Target (6 bulan pasca-launch) |
| --- | --- | --- | --- |
| G1 | Akuisisi pemain | Total Install | 2.000.000 |
| G2 | Retensi jangka pendek | D1 Retention | >= 42% |
| G3 | Retensi jangka menengah | D7 Retention | >= 18% |
| G4 | Retensi jangka panjang | D30 Retention | >= 7% |
| G5 | Monetisasi | ARPDAU | >= $0,085 |
| G6 | Konversi pembayar | Payer Conversion Rate | >= 3,5% |
| G7 | Engagement | Avg. Session Length | >= 12 menit |
| G8 | Engagement | Sessions/DAU/day | >= 4,5 |
| G9 | Kualitas | Crash-free session rate | >= 99,5% |
| G10 | Rating | Store Rating | >= 4,4 |

## 2.3 Tujuan Pemain (Player Goals)

- Merasakan sensasi "membabat ribuan musuh" dengan effort input minimal.
- Menemukan kombinasi build baru yang overpowered.
- Menyelesaikan chapter demi chapter dan melihat karakter makin kuat.
- Bersaing di leaderboard mingguan.

## 2.4 Non-Goals (Yang TIDAK dikerjakan di v1.0)

> **Dikecualikan secara sengaja** dari scope rilis 1.0 untuk menjaga fokus:
>
> - Real-time PvP / co-op multiplayer sinkron
> - Mode landscape
> - Console / PC port
> - Character voice acting penuh
> - Player-generated content / level editor
> - Cross-platform account merging (di luar Google/Apple/Guest)
> - Trading antar pemain

---

# 3. Target Audiens & Persona

## 3.1 Demografi Target

| Atribut | Detail |
| --- | --- |
| Usia | 16–34 tahun (core: 18–28) |
| Gender | 65% pria / 35% wanita |
| Wilayah Prioritas | Tier 1: Indonesia, Filipina, Vietnam, Thailand<br>Tier 2: Brasil, Meksiko, India<br>Tier 3: US, Jepang, Korea |
| Device | Android mid-range (RAM 3–4GB, Snapdragon 6xx setara), iPhone 8+ |
| Koneksi | 4G tidak stabil, **wajib bisa dimainkan offline untuk gameplay inti** |
| Waktu bermain | Commute, istirahat kerja, sebelum tidur |

## 3.2 Persona

### Persona 1 — "Rian, si Casual Commuter" (55% populasi)

| Aspek | Detail |
| --- | --- |
| Usia / Pekerjaan | 24 / karyawan swasta |
| Motivasi | Hiburan cepat tanpa berpikir keras, relaksasi |
| Perilaku | Main 3–5 sesi/hari, @10 menit. Tidak baca guide. |
| Spending | Rp0 – Rp50.000/bulan (beli Battle Pass kalau murah) |
| Kebutuhan Desain | Onboarding super jelas, auto-progress, reward harian |
| Pain Point | Benci jika kalah karena tidak paham sistem |

### Persona 2 — "Dita, si Optimizer" (30% populasi)

| Aspek | Detail |
| --- | --- |
| Usia / Pekerjaan | 21 / mahasiswa |
| Motivasi | Mencari build terkuat, menaklukkan stage tersulit |
| Perilaku | Main 6–10 sesi/hari, nonton guide, ikut komunitas Discord |
| Spending | Rp100.000 – Rp400.000/bulan |
| Kebutuhan Desain | Damage number detail, stat sheet lengkap, endgame, leaderboard |
| Pain Point | Benci RNG yang tidak bisa dimitigasi & konten habis |

### Persona 3 — "Bagus, si Whale Kompetitif" (5% populasi, 60% revenue)

| Aspek | Detail |
| --- | --- |
| Usia / Pekerjaan | 31 / wiraswasta |
| Motivasi | Jadi yang terkuat, ranking #1, koleksi lengkap |
| Perilaku | Login 8x/hari, selalu beli semua paket event |
| Spending | Rp1.000.000+/bulan |
| Kebutuhan Desain | Leaderboard global & guild, konten eksklusif, skin prestise |
| Pain Point | Benci jika uangnya tidak menghasilkan keunggulan yang terlihat |

### Persona 4 — "Sari, si Collector Santai" (10% populasi)

| Aspek | Detail |
| --- | --- |
| Motivasi | Mengumpulkan semua karakter, skin lucu, pet |
| Kebutuhan Desain | Kosmetik, galeri koleksi, event bertema, pet system |

---

# 4. Analisis Kompetitor & Diferensiasi

## 4.1 Lanskap Kompetitor

| Game | Developer | Kekuatan | Kelemahan | Pelajaran |
| --- | --- | --- | --- | --- |
| **Survivor.io** | Habby | Polish tinggi, evolusi memuaskan, monetisasi halus | Late-game grind berat, paywall gear S | Ambil: evolusi & feel. Perbaiki: kurva grind |
| **Vampire Survivors** | poncle | Depth luar biasa, harga sekali bayar, secret berlimpah | UI mobile kurang optimal, tidak F2P-native | Ambil: hidden unlock & secret |
| **Archero** | Habby | Skill draft, room-based progression | Energy system membatasi, RNG kejam | Hindari: hard energy gate |
| **Soul Knight** | ChillyRoom | Co-op, variasi senjata besar | Grafik kurang modern | Ambil: variasi senjata |
| **Magic Survival** | LEME | Sistem sinergi elemen kuat | Produksi rendah | Ambil: sinergi elemen |
| **Dead Cells Mobile** | Motion Twin | Kualitas premium | Bukan F2P, sulit | Ambil: game feel |

## 4.2 Diferensiasi Utama (USP)

> **5 Pembeda Project Nightfall:**
>
> 1. **Sistem Elemen & Sinergi** — 5 elemen (Fire/Ice/Lightning/Poison/Void) dengan status effect yang saling bereaksi (mis. Ice + Lightning = *Superconduct*, radius damage besar).
> 2. **Dual-Evolution** — satu senjata bisa punya **2 jalur evolusi berbeda** tergantung item pasif yang dipilih, sehingga variasi build 2x lipat.
> 3. **Loadout Pre-Run** — pemain memilih 2 senjata "starter" sebelum run, mengurangi frustrasi RNG.
> 4. **Zero Energy System** — tidak ada stamina/energy gate. Main sepuasnya. Monetisasi lewat progression speed & kosmetik.
> 5. **Offline-First** — semua gameplay inti berjalan offline; sinkronisasi saat online.

---

# 5. Core Gameplay Loop

## 5.1 Loop Mikro (dalam 1 detik)

```
Gerakkan joystick -> Karakter bergerak -> Senjata auto-fire ke musuh terdekat
-> Musuh mati -> Drop XP gem / koin / item -> Pemain menyerap dengan mendekat
-> XP bar naik
```

## 5.2 Loop Menengah (dalam 1 run, 15 menit)

```
Start run (Lv.1, 1 senjata)
  |
Bunuh musuh -> kumpulkan XP
  |
LEVEL UP -> Muncul 3 kartu pilihan (senjata baru / upgrade / item pasif)
  |
Pilih 1 -> Build makin kuat
  |
Buka Chest dari Elite -> upgrade gratis / evolusi
  |
[Menit 5 / 10] Mini-Boss muncul
  |
Lanjut farming, evolve senjata
  |
[Menit 15] BOSS AKHIR
  |
Menang -> Reward Screen -> Kembali ke Lobby
Kalah  -> Opsi Revive (ads/gem) atau Lobby (reward parsial)
```

## 5.3 Loop Makro (harian / mingguan)

```
Lobby
  |
Cek Daily Mission & Login Reward
  |
Upgrade Gear / Tech Parts / Skill Tree dengan resource hasil run
  |
Pilih Chapter / Event Stage
  |
RUN (loop menengah)
  |
Dapat: Koin, EXP Karakter, Gear Drop, Tech Parts, Event Token
  |
Gear naik level -> Power Rating naik -> Buka stage lebih sulit
  |
Selesaikan Battle Pass tier & Event
  |
(ulangi)
```

## 5.4 Diagram Waktu Sesi (Session Pacing)

| Menit | Level Pemain | Musuh/detik | Event | Emosi Target |
| --- | --- | --- | --- | --- |
| 0:00–1:00 | 1–5 | 2–4 | Tutorial spawn ringan | Tenang, belajar |
| 1:00–3:00 | 5–14 | 5–10 | Spawn grup pertama | Mulai sibuk |
| 3:00–5:00 | 14–24 | 10–18 | Elite #1 + Chest | Tegang |
| 5:00–7:00 | 24–32 | 18–28 | **Mini-Boss #1** | Puncak kecil |
| 7:00–9:00 | 32–40 | 25–35 | Swarm wave, evolusi pertama | Lega + kuat |
| 9:00–11:00 | 40–48 | 35–50 | **Mini-Boss #2** + Elite x3 | Tegang tinggi |
| 11:00–13:00 | 48–55 | 50–70 | Horde massal, screen penuh | Power fantasy |
| 13:00–14:30 | 55–60 | 70–90 | Pre-boss buildup, spawn berhenti | Antisipasi |
| 14:30–15:00 | 60+ | Boss saja | **BOSS AKHIR** | Klimaks |

---

# 6. Mekanik Gameplay Inti (Detail Teknis)

## 6.1 Kontrol

| Elemen | Spesifikasi |
| --- | --- |
| Input utama | **Floating virtual joystick** — muncul di titik sentuh pertama di layar bawah |
| Dead zone | 8% dari radius joystick |
| Radius joystick | 120 px @ referensi 1080x1920 |
| Respons | Analog penuh (0–1), kecepatan proporsional terhadap jarak dari center |
| Smoothing | Lerp 0,12 detik untuk menghindari jitter |
| Auto-attack | **Selalu aktif**, tidak bisa dimatikan |
| Tombol aktif | Maks. 2 tombol skill aktif, pojok kanan bawah |
| Pause | Tombol pojok kanan atas |
| Haptic | Getaran ringan saat level up, dapat chest, dan kena damage (bisa dimatikan) |

## 6.2 Kamera

| Properti | Nilai |
| --- | --- |
| Tipe | Orthographic, top-down |
| Follow | Smooth damp, `smoothTime = 0,15s` |
| Look-ahead | 1,2 unit ke arah gerakan pemain |
| Ortho size default | 6,5 unit (~13 unit tinggi dunia) |
| Zoom-out otomatis | Saat boss aktif -> ortho size 8,0 (lerp 1,5 detik) |
| Screen shake | Trauma-based (Perlin noise), max 0,35 unit, decay 1,8/detik |
| Batas | Tidak ada (map infinite scrolling) |

## 6.3 Karakter Pemain — Stat Dasar

| Stat | Simbol | Nilai Dasar | Cap | Keterangan |
| --- | --- | --- | --- | --- |
| Max HP | `HP` | 100 | — | Naik dari gear & level karakter |
| HP Regen | `REG` | 0,0/detik | 50/s | Dari item pasif |
| Move Speed | `SPD` | 4,0 unit/detik | 12,0 | +% dari item |
| Attack | `ATK` | 10 | — | Multiplier utama damage |
| Crit Rate | `CR` | 5% | 100% | |
| Crit Damage | `CD` | 150% | 1000% | |
| Armor | `ARM` | 0 | 90% DR | Damage reduction |
| Dodge | `DGE` | 0% | 60% | Peluang hindar total |
| Pickup Radius | `PR` | 1,5 unit | 12,0 | Radius serap XP/koin |
| Cooldown Reduction | `CDR` | 0% | 70% | Mempercepat semua senjata |
| Area Size | `AoE` | 100% | 400% | Ukuran hitbox skill |
| Projectile Speed | `PS` | 100% | 300% | |
| Duration | `DUR` | 100% | 300% | Lama efek bertahan |
| Amount (+Proj) | `AMT` | +0 | +8 | Jumlah proyektil tambahan |
| Luck | `LCK` | 0% | 200% | Pengaruh drop rate & kualitas kartu |
| Growth (EXP Gain) | `GRW` | 100% | 300% | |
| Greed (Coin Gain) | `GRD` | 100% | 500% | |
| Revive | `RVV` | 0 | 5 | Auto-revive dalam run |
| Magnet | `MAG` | — | — | Konsumabel, tarik semua XP di layar |

## 6.4 Formula Damage

```js
// 1. Base damage per hit
BaseDamage = WeaponBaseDamage * (1 + WeaponLevelBonus) * (1 + ATK_Percent) + ATK_Flat

// 2. Critical
isCrit = random() < CritRate
CritMultiplier = isCrit ? (1 + CritDamage) : 1

// 3. Elemental bonus & resistance musuh
ElementMultiplier = 1 + ElementBonus - EnemyElementResist

// 4. Damage tipe musuh (Normal / Elite / Boss)
TypeMultiplier = 1 + DamageVsType

// 5. Armor musuh
ArmorReduction = EnemyArmor / (EnemyArmor + 100 + 10 * PlayerLevel)

// 6. Final
FinalDamage = BaseDamage
            * CritMultiplier
            * ElementMultiplier
            * TypeMultiplier
            * (1 - ArmorReduction)
            * (1 + AllDamageBonus)
            * RandomVariance(0.95 .. 1.05)
```

**Damage yang diterima pemain:**

```js
IncomingDamage = EnemyATK * (1 - min(0.90, PlayerArmor / (PlayerArmor + 200)))
               * (1 - DamageReductionPercent)
if (random() < DodgeRate) IncomingDamage = 0  // tampilkan teks "MISS"
```

**Invincibility Frame (i-frame):** 0,5 detik setelah menerima damage kontak. Damage area (DoT) mengabaikan i-frame tetapi memiliki tick rate sendiri (0,5 detik).

## 6.5 Sistem XP & Level Up

| Aspek | Spesifikasi |
| --- | --- |
| Sumber XP | Gem drop musuh (hijau = 1, biru = 5, ungu = 25, emas = 100) |
| Drop rate gem | Normal: 100% hijau; Elite: 1x biru + 3x hijau; Mini-Boss: 1x ungu; Boss: 1x emas |
| Radius serap | `PickupRadius`, gem terbang ke pemain dengan easing |
| Formula XP butuh | `XPRequired(n) = floor(5 + 8*n + 0.55*n^2)` untuk level n -> n+1 |
| Contoh | Lv1->2: 13 XP - Lv10->11: 140 XP - Lv30->31: 740 XP - Lv60->61: 2.463 XP |
| Level maksimum | Tidak ada hard cap (praktis ~70 dalam 15 menit) |
| Saat level up | Game **pause total (timescale 0)**, tampil 3 kartu pilihan |
| Antrean level up | Jika naik beberapa level sekaligus, kartu ditampilkan berurutan |

## 6.6 Sistem Kartu Level-Up (Draft)

1. Slot senjata maksimal **6**, slot item pasif maksimal **6**.
2. Jika slot senjata penuh, kartu senjata **baru** tidak lagi muncul, hanya upgrade.
3. Kartu upgrade senjata muncul hanya jika senjata belum Lv.MAX (Lv.5).
4. Sistem **weighted random** dengan bobot:

| Tipe Kartu | Bobot Dasar | Modifier |
| --- | --- | --- |
| Senjata baru | 30 | x0 jika slot penuh |
| Upgrade senjata | 40 | x1,5 jika senjata mendekati evolusi |
| Item pasif baru | 20 | x0 jika slot penuh |
| Upgrade item pasif | 25 | — |
| Kartu HP instan (+30% HP) | 5 | x3 jika HP < 40% |
| Kartu Koin (+150 coin) | 5 | — |

5. **Pity system:** jika 3 kali level up berturut-turut tidak muncul kartu untuk senjata yang bisa di-evolve, paksa munculkan pada level up ke-4.
6. **Reroll:** 1 reroll gratis per run (bisa +2 dari gear). Reroll tambahan: tonton iklan (maks. 3x/run).
7. **Banish:** menghapus 1 kartu dari pool selamanya dalam run tersebut. 2 charge dari gear.
8. **Skip:** melewati level up dan mendapat +20 koin.
9. **Luck:** setiap 10% Luck menambah 1% peluang kartu berkualitas lebih tinggi (Common -> Rare -> Epic).

## 6.7 Sistem Spawn Musuh

Spawn di luar layar, pada elips dengan radius 1,3x dimensi kamera, sudut acak.

```js
SpawnRate(t) = BaseRate * (1 + 0.09 * t_minutes)^1.35 * StageMultiplier
MaxAliveEnemies = min(320, 40 + 18 * t_minutes)   // hard cap untuk performa
```

| Aturan | Detail |
| --- | --- |
| Culling | Musuh berjarak > 2,2x layar dari pemain di-despawn & di-respawn di depan |
| Object pooling | Semua musuh, proyektil, damage number, dan VFX memakai pool |
| Formasi | 8 pola spawn: Random Ring, Line Charge, Pincer, Circle Trap, Snake, Grid Wall, Corner Rush, Spiral |
| Wave scripted | Setiap 30 detik ada 1 wave khusus (didefinisikan di ScriptableObject per stage) |
| Elite spawn | Menit 3, 6, 9, 12 (+random 1–2 tambahan), ditandai aura merah & healthbar |
| Chest drop | Elite selalu drop chest; chest memberi 1–5 upgrade acak gratis |

## 6.8 Sistem Drop

| Item | Peluang Drop | Efek |
| --- | --- | --- |
| XP Gem | 100% dari semua musuh | Menambah XP |
| Koin | 25% (normal), 100% (elite) | Mata uang, dikonversi di akhir run |
| Heal Pack | 1,2% | Memulihkan 30% Max HP |
| Magnet | 0,8% | Menarik semua XP gem di layar |
| Bomb | 0,6% | Membunuh semua musuh normal di layar |
| Freeze | 0,4% | Membekukan semua musuh 3 detik |
| Chest | 100% dari Elite | 1–5 upgrade acak (dipengaruhi Luck) |
| Tech Part | 3% dari Elite/Boss | Material meta-progression |
| Gear | Lihat tabel drop stage | Equipment |

## 6.9 Kondisi Menang / Kalah

| Kondisi | Trigger | Hasil |
| --- | --- | --- |
| **Menang** | Membunuh Boss akhir sebelum timer 15:00 habis | Reward penuh (100%) + first-clear bonus |
| **Survive** | Bertahan sampai 15:00 tapi boss belum mati | Reward 70% |
| **Kalah** | HP = 0 dan tidak revive | `reward% = waktu_detik / 900 * 60%` |
| **Quit** | Pemain keluar manual | Reward 50%, run tidak dihitung untuk mission |
| **Revive** | HP = 0 | Iklan (1x/run gratis) atau 60 gem (harga naik 2x tiap pakai). Memulihkan 50% HP + membunuh semua musuh di layar + i-frame 3 detik |

---

# 7. Sistem Senjata

## 7.1 Aturan Umum Senjata

- Maksimal **6 senjata** aktif per run.
- Setiap senjata punya **Lv.1 – Lv.5**. Lv.5 = MAX (syarat evolusi).
- Senjata menyerang otomatis dengan cooldown masing-masing.
- Targeting mode: `Nearest`, `Random`, `Forward`, `Aura`, `Orbit`, `Manual-none`.
- Semua senjata memiliki **elemen** (atau Neutral).

## 7.2 Daftar Senjata Dasar (20 Senjata)

| # | Nama Senjata | Elemen | Tipe Serangan | Base DMG | Cooldown | Targeting | Deskripsi Lv.1 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| W01 | **Kunai** | Neutral | Proyektil menembus | 12 | 1,0s | Nearest | Lempar 1 kunai menembus 1 musuh |
| W02 | **Baseball Bat** | Neutral | Melee arc | 20 | 1,4s | Forward | Ayunan busur 120 derajat di depan |
| W03 | **Force Field** | Void | Aura persisten | 6/tick | 0,5s tick | Aura | Aura radius 2 unit di sekitar pemain |
| W04 | **Drone** | Lightning | Orbit + tembak | 9 | 0,8s | Orbit | 1 drone mengorbit & menembak musuh terdekat |
| W05 | **RPG Launcher** | Fire | Proyektil ledak | 45 | 2,6s | Nearest | Roket meledak radius 2,5 unit |
| W06 | **Lightning Emitter** | Lightning | Chain instan | 18 | 1,8s | Random | Petir menyambar 3 musuh berantai |
| W07 | **Molotov** | Fire | Ground DoT | 8/tick | 2,2s | Random | Lempar botol, area api 4 detik |
| W08 | **Frost Blade** | Ice | Melee slash | 16 | 1,1s | Nearest | Tebasan yang memperlambat 30% |
| W09 | **Toxic Grenade** | Poison | AoE DoT | 10/tick | 2,4s | Nearest | Awan racun radius 3, durasi 5 detik |
| W10 | **Shuriken Storm** | Neutral | Proyektil menyebar | 8 | 1,6s | Forward | 3 shuriken menyebar kipas 45 derajat |
| W11 | **Laser Beam** | Void | Sinar kontinu | 14/tick | 3,0s (channel 1,5s) | Nearest | Sinar menembus semua musuh dalam garis |
| W12 | **Guardian Drone** | Neutral | Orbit shield | 25 | Pasif | Orbit | 2 bola mengorbit, damage kontak |
| W13 | **Boomerang** | Neutral | Proyektil balik | 22 | 1,9s | Forward | Terbang & kembali, kena 2x |
| W14 | **Ice Nova** | Ice | AoE burst | 30 | 3,2s | Aura | Ledakan es radius 3,5, freeze 1 detik |
| W15 | **Flame Thrower** | Fire | Cone kontinu | 7/tick | 0,2s tick | Forward | Kerucut api 90 derajat, jarak 4 unit |
| W16 | **Landmine** | Fire | Trap | 60 | 3,5s | Drop | Jatuhkan ranjau, meledak saat diinjak |
| W17 | **Chain Whip** | Neutral | Melee sweep | 18 | 1,3s | Nearest | Cambuk memutar 360 derajat radius 2,5 |
| W18 | **Void Orb** | Void | Black hole | 12/tick | 5,0s | Random | Lubang hitam menarik musuh 3 detik |
| W19 | **Sentry Turret** | Lightning | Deployable | 11 | 6,0s | Drop | Turret otomatis menembak 8 detik |
| W20 | **Poison Dart** | Poison | Proyektil stack | 9 | 0,7s | Nearest | Dart menumpuk racun (maks 5 stack) |

## 7.3 Tabel Progresi Level Senjata (Contoh: Kunai)

| Level | Perubahan | DMG | Cooldown | Jumlah Proyektil | Pierce |
| --- | --- | --- | --- | --- | --- |
| Lv.1 | Dasar | 12 | 1,00s | 1 | 1 |
| Lv.2 | +1 proyektil | 14 | 1,00s | 2 | 1 |
| Lv.3 | +Damage, -CD | 18 | 0,85s | 2 | 2 |
| Lv.4 | +1 proyektil | 21 | 0,85s | 3 | 2 |
| Lv.5 (MAX) | +Pierce, -CD | 26 | 0,70s | 3 | 4 |

> **Aturan skala umum antar level senjata:**
>
> - Damage: **+18% s.d. +25%** per level (kumulatif x2,1 dari Lv1 ke Lv5)
> - Cooldown: **-8% s.d. -12%** pada level 3 dan 5 saja
> - Jumlah proyektil / hit: +1 pada level 2 dan 4 (untuk senjata proyektil)
> - Area: +15% pada level 3 dan 5 (untuk senjata AoE)

---

# 8. Item Pasif (Passive Items)

Maksimal **6 item pasif** per run, masing-masing Lv.1–Lv.5.

| # | Item | Efek per Level | Total di Lv.5 | Fungsi Evolusi |
| --- | --- | --- | --- | --- |
| P01 | **Sharp Blade** | +10% ATK | +50% ATK | Kunai -> **Shadow Blade** |
| P02 | **Whistle** | +8% Area | +40% Area | Baseball Bat -> **Quantum Bat** |
| P03 | **Energy Cube** | +8% CDR | +40% CDR | Force Field -> **Void Barrier** |
| P04 | **Circuit Board** | +1 Drone / 2 lvl | +2 Drone | Drone -> **Swarm Legion** |
| P05 | **Ammo Pack** | +12% Proj. Damage | +60% | RPG -> **Nuke Launcher** |
| P06 | **Capacitor** | +10% Lightning DMG | +50% | Lightning Emitter -> **Thunder God** |
| P07 | **Oil Can** | +15% Burn duration | +75% | Molotov -> **Inferno Field** |
| P08 | **Cryo Core** | +12% Slow effect | +60% | Frost Blade -> **Absolute Zero** |
| P09 | **Bio Filter** | +15% Poison DMG | +75% | Toxic Grenade -> **Plague Bloom** |
| P10 | **Running Shoes** | +6% Move Speed | +30% | Shuriken -> **Sonic Storm** |
| P11 | **Magnet Core** | +25% Pickup Radius | +125% | — (utility) |
| P12 | **Lucky Coin** | +12% Luck | +60% | — (utility) |
| P13 | **Piggy Bank** | +20% Coin Gain | +100% | — (utility) |
| P14 | **Vitamin** | +12% Max HP | +60% | — (survival) |
| P15 | **Bandage** | +0,6 HP Regen/s | +3,0 HP/s | — (survival) |
| P16 | **Kevlar Vest** | +6 Armor | +30 Armor | — (survival) |
| P17 | **Reflex Chip** | +3% Dodge | +15% Dodge | — (survival) |
| P18 | **Hourglass** | +10% Duration | +50% | Void Orb -> **Singularity** |
| P19 | **Scope** | +8% Crit Rate | +40% Crit Rate | Laser -> **Death Ray** |
| P20 | **Adrenaline** | +10% Crit DMG | +50% Crit DMG | Boomerang -> **Reaper's Return** |
| P21 | **Textbook** | +10% EXP Gain | +50% | — (utility) |
| P22 | **Spare Battery** | +1 Revive (maks 2) | +2 Revive | — (survival) |

---

# 9. Sistem Evolusi (Evolution)

## 9.1 Syarat Evolusi

> Sebuah senjata dapat berevolusi ketika **SEMUA** kondisi terpenuhi:
>
> 1. Senjata mencapai **Lv.5 (MAX)**
> 2. Item pasif pasangannya mencapai **Lv.3 atau lebih**
> 3. Pemain membuka **Chest dari Elite/Boss** setelah kedua syarat terpenuhi
> 4. Waktu run sudah melewati **menit ke-5** (mencegah evolusi terlalu dini)

Saat evolusi terjadi: layar flash putih, slow-motion 0,8 detik, VFX khusus, SFX "EVOLVED!", dan senjata lama diganti oleh versi evolusi (slot tetap 1).

## 9.2 Tabel Evolusi Lengkap

| Senjata Dasar | + Item Pasif | = Senjata Evolusi | Efek Evolusi |
| --- | --- | --- | --- |
| Kunai | Sharp Blade | **Shadow Blade** | 6 bilah bayangan menembus tanpa batas, homing, DMG 65 |
| Baseball Bat | Whistle | **Quantum Bat** | Ayunan 360 derajat, knockback besar, shockwave DMG 90 |
| Force Field | Energy Cube | **Void Barrier** | Aura radius 5, DMG 28/tick, memantulkan proyektil musuh |
| Drone | Circuit Board | **Swarm Legion** | 6 drone mengorbit, tembak rentetan, DMG 30 x3 burst |
| RPG Launcher | Ammo Pack | **Nuke Launcher** | Ledakan radius 7, DMG 320, screen shake besar |
| Lightning Emitter | Capacitor | **Thunder God** | Petir menyambar 12 musuh, stun 0,5s, DMG 85 |
| Molotov | Oil Can | **Inferno Field** | Lautan api permanen mengikuti pemain, DMG 40/tick |
| Frost Blade | Cryo Core | **Absolute Zero** | Membekukan total 2 detik, DMG x3 ke musuh beku |
| Toxic Grenade | Bio Filter | **Plague Bloom** | Racun menular antar musuh, DMG 45/tick |
| Shuriken Storm | Running Shoes | **Sonic Storm** | 12 shuriken memantul, kecepatan x3, DMG 55 |
| Laser Beam | Scope | **Death Ray** | Sinar permanen berputar, DMG 70/tick, crit +100% |
| Boomerang | Adrenaline | **Reaper's Return** | 4 sabit terbang, lifesteal 3%, DMG 110 |
| Void Orb | Hourglass | **Singularity** | Black hole 8 detik, DMG 60/tick, instakill musuh <10% HP |

## 9.3 Dual-Evolution (USP)

Beberapa senjata memiliki **jalur evolusi kedua**, ditentukan oleh item pasif alternatif:

| Senjata | Jalur A | Jalur B (alternatif) |
| --- | --- | --- |
| Kunai | + Sharp Blade -> **Shadow Blade** (single-target tinggi) | + Lucky Coin -> **Fortune Kunai** (drop koin x3, DMG sedang) |
| RPG Launcher | + Ammo Pack -> **Nuke Launcher** (burst besar) | + Energy Cube -> **Barrage Cannon** (CD 0,6s, DMG 90 spam) |
| Force Field | + Energy Cube -> **Void Barrier** (defensif) | + Sharp Blade -> **Razor Aura** (radius kecil, DMG 65/tick) |
| Frost Blade | + Cryo Core -> **Absolute Zero** (kontrol) | + Adrenaline -> **Frostbite Edge** (crit x4 ke musuh melambat) |

Jika kedua item pasangan aktif, pemain **diberi pilihan** lewat popup saat membuka chest.

## 9.4 Sistem Elemen & Reaksi (USP)

| Elemen | Status Effect | Durasi | Efek |
| --- | --- | --- | --- |
| Fire | **Burn** | 3s | 5% ATK/detik DoT, stack maks 5 |
| Ice | **Chill / Freeze** | 2s / 1s | -40% speed / diam total |
| Lightning | **Shock** | 2s | +15% damage diterima, chain ke 2 musuh |
| Poison | **Toxin** | 5s | 3% Max HP musuh/detik, menembus armor |
| Void | **Fragile** | 4s | -30% armor musuh, tarik ke pusat |

**Reaksi Elemen (Combo):**

| Kombinasi | Nama Reaksi | Efek |
| --- | --- | --- |
| Fire + Ice | **Thermal Shock** | Ledakan DMG = 250% ATK, radius 3 |
| Ice + Lightning | **Superconduct** | Radius 4, -50% armor semua musuh, 6 detik |
| Fire + Lightning | **Overload** | Ledakan berantai, DMG 180% ATK ke 5 musuh |
| Poison + Fire | **Toxic Combustion** | Racun menyebar radius 5, DMG x2 |
| Void + apa saja | **Collapse** | Menarik musuh & menggandakan durasi status lain |
| Ice + Poison | **Cryo Toxin** | Racun tetap aktif meski musuh beku, DMG +80% |

---

# 10. Karakter Playable

## 10.1 Daftar Karakter (Launch: 8 Karakter)

| # | Karakter | Rarity | Senjata Awal | Passive Unik | Cara Unlock |
| --- | --- | --- | --- | --- | --- |
| C01 | **Rex** (Rookie) | Common | Kunai | +10% ATK | Default |
| C02 | **Mia** (Scout) | Common | Shuriken Storm | +15% Move Speed, +20% Pickup | Selesaikan Chapter 2 |
| C03 | **Bruno** (Tank) | Rare | Baseball Bat | +40% Max HP, +10 Armor, -10% Speed | Selesaikan Chapter 5 |
| C04 | **Ayu** (Pyromancer) | Rare | Molotov | +30% Fire DMG, Burn stack maks 8 | 3.000 Gem / Event |
| C05 | **Volt** (Engineer) | Epic | Drone | Mulai dengan 2 drone, +25% Lightning DMG | Chapter 10 + 5.000 Gem |
| C06 | **Frost** (Cryomancer) | Epic | Frost Blade | Musuh melambat 15% permanen di radius 4 | Event / 8.000 Gem |
| C07 | **Nyx** (Void Walker) | Legendary | Void Orb | Dash tak terbatas (i-frame 0,3s), +50% Void DMG | Battle Pass Premium S1 |
| C08 | **Dr. Vex** (Alchemist) | Legendary | Toxic Grenade | Racun menular, +1 Revive | Achievement: 100 boss kill |

## 10.2 Progresi Karakter

| Level Karakter | Syarat | Bonus Kumulatif |
| --- | --- | --- |
| Lv.1–10 | Character Shard x10 per level | +2% ATK & +2% HP per level |
| Lv.11–20 | Shard x25 + Coin | +3% per level, buka slot gear ke-5 |
| Lv.21–30 | Shard x50 + Tech Part | +4% per level, buka Star Rating |
| Star 1–6 | Duplikat karakter | Setiap bintang: +8% semua stat + 1 perk unik |

---

# 11. Musuh, Elite & Boss

## 11.1 Arketipe Musuh Normal

| # | Musuh | HP (base) | ATK | Speed | Perilaku |
| --- | --- | --- | --- | --- | --- |
| E01 | **Walker** | 20 | 6 | 1,6 | Berjalan lurus ke pemain |
| E02 | **Runner** | 12 | 5 | 3,4 | Lari cepat, HP rendah |
| E03 | **Brute** | 90 | 14 | 1,1 | Lambat, knockback resist |
| E04 | **Spitter** | 25 | 9 | 1,3 | Berhenti jarak 5, tembak proyektil |
| E05 | **Bomber** | 30 | 25 | 2,2 | Meledak saat dekat (radius 2) |
| E06 | **Shielder** | 60 | 8 | 1,4 | Perisai depan (-70% DMG dari depan) |
| E07 | **Swarmling** | 6 | 3 | 2,8 | Spawn bergerombol 15–25 ekor |
| E08 | **Leech** | 35 | 7 | 1,8 | Menempel, DoT + memperlambat pemain |
| E09 | **Splitter** | 45 | 8 | 1,5 | Pecah jadi 3 Swarmling saat mati |
| E10 | **Sniper** | 22 | 20 | 0,9 | Tembak dari luar layar, ada laser sight |
| E11 | **Healer** | 40 | 4 | 1,2 | Menyembuhkan musuh sekitar 5%/detik |
| E12 | **Shaman** | 50 | 6 | 1,3 | Memberi buff +30% speed ke musuh sekitar |

## 11.2 Scaling Musuh

```js
EnemyHP(t, chapter)  = BaseHP  * (1 + 0.16 * t_minutes)^1.42 * ChapterMultiplier
EnemyATK(t, chapter) = BaseATK * (1 + 0.11 * t_minutes)^1.25 * ChapterMultiplier
ChapterMultiplier    = 1.0 * (1.35)^(chapter - 1)
```

## 11.3 Elite Enemy

| Properti | Nilai |
| --- | --- |
| HP | 12x musuh normal setara |
| Ukuran | 1,6x |
| Visual | Aura merah menyala + outline, healthbar di atas kepala |
| Reward | Chest (1–5 upgrade) + 1 gem biru + 3% Tech Part |
| Modifier acak | 1 dari: *Enraged* (+50% speed), *Armored* (+30 armor), *Regenerating* (+2%HP/s), *Explosive* (meledak saat mati), *Summoner* (spawn 5 swarmling tiap 3 detik) |

## 11.4 Boss — Struktur Umum

Setiap boss memiliki **3 fase** yang dipicu oleh threshold HP (100–66%, 66–33%, 33–0%). Setiap fase menambah 1 pola serangan baru.

### Daftar Boss (Launch: 10 Boss)

| # | Boss | Chapter | Tema | Pola Serangan Utama |
| --- | --- | --- | --- | --- |
| B01 | **Colossus** | 1 | Zombie raksasa | Ground slam (AoE telegraf 1,2s), charge lurus, summon 20 walker |
| B02 | **Hive Queen** | 2 | Serangga | Spawn swarm terus-menerus, semburan asam kerucut, terbang & jatuh |
| B03 | **Iron Warden** | 3 | Mech | Laser sweep 180 derajat, roket homing x6, shield phase (hancurkan 4 node) |
| B04 | **Pyroclast** | 4 | Api | Lantai lava menyebar, meteor telegraf, ring api mengecil |
| B05 | **Glacier Titan** | 5 | Es | Ice spike dari tanah, blizzard (-60% speed), membekukan area |
| B06 | **The Butcher** | 6 | Slasher | Dash 3x beruntun, bear trap, enrage di 33% (speed x2) |
| B07 | **Storm Caller** | 7 | Petir | Chain lightning menandai posisi, tornado bergerak, EMP |
| B08 | **Plague Lord** | 8 | Racun | Kabut racun menyebar, spawn Splitter, zona aman mengecil |
| B09 | **Void Devourer** | 9 | Void | Black hole menarik pemain, membalik kontrol 2 detik, clone bayangan |
| B10 | **Omega Prime** | 10 | Final | Gabungan semua pola sebelumnya, 4 fase, HP 5x |

### Aturan Desain Boss

> - Semua serangan boss **wajib memiliki telegraph visual** minimal **0,8 detik** sebelum eksekusi (indikator merah di tanah).
> - Boss tidak boleh membunuh pemain dari full HP dengan 1 serangan (max 45% Max HP per hit).
> - Selama fase boss, spawn musuh normal berhenti (kecuali boss yang memang summon).
> - Boss punya **enrage timer 90 detik** — setelah itu ATK x1,5 setiap 15 detik untuk mencegah stall.
> - Boss immune terhadap Freeze & Stun total; hanya menerima **50% efek slow**.

---

# 12. Desain Stage & Chapter

## 12.1 Struktur Chapter

Setiap **Chapter** terdiri dari **10 Stage**. Stage 1–9 adalah stage normal (5 menit), Stage 10 adalah **Boss Stage** (15 menit).

| Chapter | Nama | Tema Visual | Musuh Utama | Boss | Power Rating | Unlock |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | **Downtown Ruins** | Kota runtuh, malam | Walker, Runner | Colossus | 500 | Default |
| 2 | **Sewer Depths** | Gorong-gorong hijau | Swarmling, Leech | Hive Queen | 1.200 | Clear Ch.1 |
| 3 | **Abandoned Factory** | Pabrik industri | Shielder, Brute | Iron Warden | 2.500 | Clear Ch.2 |
| 4 | **Volcanic Rift** | Lava, batu vulkanik | Bomber, Spitter | Pyroclast | 4.800 | Clear Ch.3 |
| 5 | **Frozen Wasteland** | Salju badai | Brute, Sniper | Glacier Titan | 8.500 | Clear Ch.4 |
| 6 | **Blood Slaughterhouse** | Merah gelap | Runner, Splitter | The Butcher | 14.000 | Clear Ch.5 |
| 7 | **Sky Fortress** | Platform melayang | Sniper, Shaman | Storm Caller | 22.000 | Clear Ch.6 |
| 8 | **Toxic Marsh** | Rawa beracun | Healer, Splitter | Plague Lord | 34.000 | Clear Ch.7 |
| 9 | **The Rift** | Dimensi ungu | Semua tipe | Void Devourer | 50.000 | Clear Ch.8 |
| 10 | **Omega Facility** | Lab futuristik | Semua + varian elite | Omega Prime | 75.000 | Clear Ch.9 |

## 12.2 Hazard Lingkungan per Chapter

| Chapter | Hazard | Efek |
| --- | --- | --- |
| 1 | Mobil terbakar | DMG 5/detik jika terlalu dekat |
| 2 | Genangan asam | -25% speed + 3 DMG/detik |
| 3 | Conveyor belt | Mendorong pemain ke satu arah |
| 4 | Semburan lava periodik | Telegraf 1s, DMG 20% Max HP |
| 5 | Lantai es | Pergerakan licin (inersia) |
| 6 | Bear trap | Immobilize 1,5 detik |
| 7 | Angin kencang | Dorongan konstan 1,2 unit/s |
| 8 | Kabut racun | Visibility turun + Toxin stack |
| 9 | Zona anti-gravitasi | Speed x1,5 tapi kontrol sulit |
| 10 | Laser grid bergerak | DMG 30% Max HP |

## 12.3 Tipe Stage Tambahan

| Tipe Stage | Durasi | Tujuan | Reward Khusus |
| --- | --- | --- | --- |
| **Normal Stage** | 5 menit | Bertahan sampai timer habis | Coin, EXP, Gear drop |
| **Boss Stage** | 15 menit | Kalahkan boss | Gear rarity tinggi, Tech Part |
| **Elite Rush** | 3 menit | Bunuh 15 elite | Chest Key, Tech Part x5 |
| **Endless Mode** | tak terbatas | Bertahan selama mungkin | Leaderboard, Coin scaling |
| **Daily Dungeon** | 4 menit | Rotasi harian per material | Material upgrade spesifik |
| **Weekly Raid Boss** | 6 menit | Damage race (shared HP guild) | Guild Coin, Gear Legendary |
| **Time Attack** | Sampai boss mati | Bunuh boss secepat mungkin | Leaderboard mingguan, Gem |
| **Nightmare Mode** | 15 menit | Versi x3 sulit dari boss stage | Gear S-tier, Title eksklusif |
| **Event Stage** | Variabel | Mekanik unik per event | Event Token |

## 12.4 Sistem Difficulty Tier

Setelah menyelesaikan Chapter 10, terbuka **Difficulty Tier**:

| Tier | Nama | Enemy HP | Enemy ATK | Reward Mult. | Modifier Khusus |
| --- | --- | --- | --- | --- | --- |
| I | Normal | x1,0 | x1,0 | x1,0 | — |
| II | Hard | x2,5 | x1,6 | x1,8 | Elite +50% |
| III | Nightmare | x6,0 | x2,4 | x3,2 | Musuh punya 1 modifier acak |
| IV | Hell | x15,0 | x3,5 | x5,5 | Tidak ada Heal Pack drop |
| V | Apocalypse | x40,0 | x5,0 | x9,0 | Revive dinonaktifkan |
| VI | Void | x110,0 | x7,0 | x15,0 | Timer 12 menit saja |

---

# 13. Meta Progression (Di Luar Run)

## 13.1 Sistem Gear / Equipment

Pemain memiliki **6 slot equipment**:

| Slot | Nama Slot | Stat Utama | Stat Sekunder Umum |
| --- | --- | --- | --- |
| 1 | Senjata (Weapon) | ATK | Crit Rate, Crit DMG, Proj. DMG |
| 2 | Baju Zirah (Armor) | Max HP | Armor, Damage Reduction |
| 3 | Sepatu (Boots) | Move Speed | Dodge, Pickup Radius |
| 4 | Sarung Tangan (Gloves) | Attack Speed / CDR | Crit Rate, Amount |
| 5 | Kalung (Necklace) | Crit DMG | Elemental DMG, Luck |
| 6 | Cincin (Ring) | Elemental DMG | HP Regen, EXP Gain, Coin Gain |

### Rarity Gear

| Rarity | Warna | Sub-Stat | Level Max | Mult. Stat Utama | Drop Rate (Ch.5) |
| --- | --- | --- | --- | --- | --- |
| Common (C) | Abu-abu | 0 | 20 | x1,0 | 55% |
| Uncommon (B) | Hijau | 1 | 30 | x1,4 | 25% |
| Rare (A) | Biru | 2 | 40 | x2,0 | 13% |
| Epic (S) | Ungu | 3 | 60 | x3,2 | 5,5% |
| Legendary (SS) | Emas | 4 | 80 | x5,5 | 1,3% |
| Mythic (SSS) | Merah | 4 + Set Effect | 100 | x9,0 | 0,2% |

### Mekanik Gear

> **Operasi yang bisa dilakukan pada gear:**
>
> 1. **Enhance (Upgrade Level)** — biaya Coin + Enhance Stone. Setiap 5 level = milestone bonus.
> 2. **Ascend (Naik Rarity)** — gabungkan 3 gear rarity sama untuk naik 1 tingkat.
> 3. **Reforge (Re-roll Sub-Stat)** — mengacak ulang sub-stat, biaya Reforge Dust. Bisa mengunci 1 sub-stat (biaya 2x).
> 4. **Awaken** — pada Lv.Max, buka efek pasif unik.
> 5. **Dismantle** — hancurkan gear jadi material (60% nilai kembali).
> 6. **Lock** — kunci gear agar tidak terkena auto-dismantle.
> 7. **Set Effect** — memakai 2/4/6 gear dari set yang sama memberi bonus bertingkat.

### Set Effect (6 Set di Launch)

| Set | Bonus 2 Piece | Bonus 4 Piece | Bonus 6 Piece |
| --- | --- | --- | --- |
| **Berserker** | ATK +12% | ATK +25%, Crit Rate +8% | Di bawah 50% HP, ATK +60% |
| **Guardian** | Max HP +15% | Armor +40, DR +10% | Sekali per run: bertahan dari damage fatal dengan 1 HP |
| **Swiftness** | Move Speed +10% | CDR +15% | Setiap 5 detik bergerak, serangan berikutnya x2 |
| **Elementalist** | Elemental DMG +15% | Status effect +50% durasi | Reaksi elemen memicu 2x |
| **Fortune** | Luck +20% | Coin Gain +50% | Chest memberi 2 upgrade tambahan |
| **Void Walker** | Void DMG +20% | Dodge +12% | Setelah dodge, i-frame +1 detik dan ATK +30% (3 detik) |

## 13.2 Tech Parts (Sistem Modul)

Sistem sekunder yang memodifikasi **senjata dalam run**.

| Tech Part | Efek | Material Upgrade |
| --- | --- | --- |
| **Overclock Chip** | Semua senjata mulai run di Lv.2 | Circuit Fragment x30 |
| **Ammo Expander** | Amount +1 untuk semua senjata proyektil | Ammo Core x25 |
| **Evolution Catalyst** | Syarat evolusi turun: item pasif cukup Lv.2 | Catalyst Shard x50 |
| **Reroll Module** | Reroll gratis +2 per run | Data Chip x20 |
| **Banish Module** | Banish charge +2 per run | Data Chip x20 |
| **Magnet Core** | Pickup Radius +60% | Magnet Ore x15 |
| **Starter Kit** | Mulai run dengan 1 item pasif acak Lv.2 | Kit Fragment x35 |
| **Revive Cell** | +1 auto-revive per run | Bio Cell x60 |
| **Greed Engine** | Coin Gain +80% | Gold Fragment x40 |
| **Boss Slayer** | Damage ke boss +25% | Slayer Mark x45 |

Setiap Tech Part punya **Lv.1–Lv.10**; efeknya meningkat linear per level.

## 13.3 Skill Tree (Talent Board)

Skill tree global (berlaku untuk semua karakter), terbuka setelah Chapter 3. Mata uang: **Talent Point**.

| Cabang | Fokus | Contoh Node |
| --- | --- | --- |
| **Offense** (18 node) | Damage | ATK +2%/node, Crit Rate +1%/node, Boss DMG +3%/node |
| **Defense** (15 node) | Bertahan | Max HP +2%/node, Armor +3/node, Dodge +0,5%/node |
| **Utility** (14 node) | Kualitas hidup | Pickup Radius +5%, Luck +2%, EXP Gain +2%, Reroll +1 |
| **Economy** (12 node) | Resource | Coin Gain +3%/node, Gear drop rate +1%/node |
| **Mastery** (10 node, endgame) | Spesialisasi | Pilih 1 elemen untuk +25% damage, Weapon slot ke-7 (butuh 80 TP) |

Aturan: node harus dibuka berurutan; ada **keystone node** di setiap 5 node. Respec biaya 500 Gem (gratis 1x per season).

## 13.4 Collectibles (Koleksi)

| Kategori | Jumlah Item | Bonus Set Lengkap |
| --- | --- | --- |
| Kartu Senjata | 33 (20 dasar + 13 evolusi) | ATK +10% |
| Kartu Musuh | 12 arketipe + 10 boss | HP +10% |
| Kartu Chapter | 10 | Coin Gain +15% |
| Kartu Karakter | 8 | Semua stat +5% |
| Kartu Artefak (event) | 20 | Luck +25% |

## 13.5 Pet System (Companion)

| Pet | Rarity | Efek Pasif | Skill Aktif (auto) |
| --- | --- | --- | --- |
| **Buddy** | Common | Pickup Radius +20% | Menyalak: knockback musuh sekitar (CD 15s) |
| **Falcon** | Rare | Crit Rate +5% | Menukik: DMG 200% ATK ke 5 musuh (CD 12s) |
| **Fenrir** | Epic | ATK +12% | Auman: musuh sekitar ketakutan 2 detik (CD 20s) |
| **Draco** | Legendary | Fire DMG +25%, HP +10% | Napas api kerucut, DMG 400% ATK (CD 18s) |
| **Wisp** | Legendary | HP Regen +1,5/s, Luck +15% | Menyembuhkan 25% Max HP (CD 45s) |

Pet punya level (1–50) dan makanan (Pet Food). Pet aktif hanya 1 per run.

## 13.6 Account Level & Power Rating

```js
PowerRating = (TotalGearScore * 1.0)
            + (CharacterLevel * 45)
            + (TalentPointsSpent * 30)
            + (TechPartLevelSum * 25)
            + (CollectibleBonus * 15)
            + (PetLevel * 20)

GearScore(item) = (MainStatValue * RarityMultiplier)
                + sum(SubStatValue * 0.6)
                + (EnhanceLevel * 12)
```

---

# 14. Ekonomi & Mata Uang

## 14.1 Daftar Mata Uang

| Mata Uang | Tipe | Sumber Utama | Kegunaan | Cap |
| --- | --- | --- | --- | --- |
| **Coin** | Soft | Semua run, mission | Enhance gear, reforge | Tidak ada |
| **Gem** | Hard | IAP, achievement, event, BP | Gacha, revive, karakter | Tidak ada |
| **Summon Ticket** | Premium | Event, BP, shop | Gacha gear/karakter | Tidak ada |
| **Chest Key** | Sekunder | Elite Rush, daily | Membuka chest di lobby | 99 |
| **Tech Part** | Material | Elite/Boss drop | Upgrade Tech Parts | Tidak ada |
| **Enhance Stone** | Material | Daily Dungeon | Naikkan level gear | Tidak ada |
| **Reforge Dust** | Material | Dismantle gear | Re-roll sub-stat | Tidak ada |
| **Character Shard** | Material | Gacha, event, shop | Level/bintang karakter | Tidak ada |
| **Guild Coin** | Sosial | Guild raid, donasi | Guild shop | Tidak ada |
| **Event Token** | Temporer | Event stage | Event shop (hangus) | Tidak ada |
| **Talent Point** | Progresi | Account Level up | Skill tree | Tidak ada |

## 14.2 Faucet & Sink

| Mata Uang | Faucet (per hari) | Sink | Target Rasio |
| --- | --- | --- | --- |
| Coin | ~180.000 | Enhance (~150.000), Reforge (~40.000) | Sink/Faucet ~1,05 |
| Gem | ~180 (F2P) | Gacha (300/pull), revive (60) | Defisit terkontrol |
| Enhance Stone | ~400 | Enhance (~380) | ~0,95 |
| Tech Part | ~25 | Upgrade Tech (~30) | Defisit -> mendorong farming |

> **Prinsip ekonomi:** pemain F2P harus bisa menyelesaikan **Chapter 10 dalam ~35 hari** tanpa membayar. Pemain berbayar bisa mempercepat menjadi ~10 hari, tapi **tidak boleh melewati konten yang belum di-unlock** (no pay-to-skip content, hanya pay-to-accelerate).

## 14.3 Gacha / Summon

| Banner | Biaya | Isi | Pity |
| --- | --- | --- | --- |
| **Standard Gear** | 300 Gem / 1 Tiket | Gear C–SS | SS terjamin di pull ke-60 |
| **Character Banner** | 300 Gem | Shard karakter, gear | Legendary terjamin di pull ke-80 |
| **Event Banner** | 300 Gem / Event Token | Gear eksklusif event | 50/50 rate-up, terjamin di pull ke-90 |
| **Pet Banner** | 250 Gem | Pet + Pet Food | Epic pet terjamin di pull ke-50 |

### Rate Gacha (Standard Gear)

| Rarity | Rate |
| --- | --- |
| Common (C) | 40,0% |
| Uncommon (B) | 33,0% |
| Rare (A) | 19,0% |
| Epic (S) | 6,5% |
| Legendary (SS) | 1,4% |
| Mythic (SSS) | 0,1% |

**Wajib:** rate ditampilkan transparan di UI (kepatuhan regulasi Google Play, App Store, dan aturan lootbox di beberapa negara).

---

# 15. Monetisasi

## 15.1 Ringkasan Sumber Pendapatan

| Sumber | Estimasi Kontribusi Revenue |
| --- | --- |
| IAP — Paket Gem | 30% |
| IAP — Battle Pass | 22% |
| IAP — Bundle Event / Starter Pack | 20% |
| IAP — Langganan (Monthly Card) | 15% |
| Rewarded Ads | 10% |
| Interstitial / Banner | 3% |

## 15.2 Katalog IAP

| SKU | Nama | Harga (IDR) | Isi | Batas |
| --- | --- | --- | --- | --- |
| P001 | Starter Pack | 15.000 | 500 Gem + Gear A + 10.000 Coin | 1x seumur akun |
| P002 | Gem Kecil | 15.000 | 300 Gem (+60 bonus pertama) | — |
| P003 | Gem Sedang | 79.000 | 1.700 Gem | — |
| P004 | Gem Besar | 159.000 | 3.600 Gem | — |
| P005 | Gem Jumbo | 399.000 | 9.800 Gem | — |
| P006 | Gem Mega | 799.000 | 21.000 Gem | — |
| P007 | Monthly Card | 45.000 | 300 Gem instan + 100 Gem/hari (30 hari) + 2 reroll | 1 aktif |
| P008 | Growth Fund | 129.000 | Reward bertahap di Chapter 3/5/7/10 (total 4.500 Gem) | 1x |
| P009 | Battle Pass Premium | 89.000 | 60 tier reward + karakter eksklusif | Per season |
| P010 | Battle Pass Elite | 199.000 | Premium + skip 15 tier + skin | Per season |
| P011 | Weekly Deal | 29.000 | Rotasi mingguan (material paket) | 1x/minggu |
| P012 | Event Bundle | 99.000–499.000 | Gear event + token | Per event |
| P013 | Remove Ads | 69.000 | Hilangkan iklan paksa (rewarded tetap ada) | 1x |

## 15.3 Iklan (Ads)

| Penempatan | Tipe | Frekuensi | Reward |
| --- | --- | --- | --- |
| Revive dalam run | Rewarded | 1x per run | Revive gratis |
| Reroll kartu | Rewarded | Maks 3x per run | Reroll ekstra |
| Double reward akhir run | Rewarded | 1x per run | Reward x2 |
| Free chest lobby | Rewarded | 3x per hari | Chest gratis |
| Coin doubler daily | Rewarded | 1x per hari | Coin x2 selama 30 menit |
| Gem gratis | Rewarded | 5x per hari | 10 Gem |
| Interstitial | Full-screen | Maks 1 per 3 run, tidak pernah di 3 hari pertama | — |

> **Aturan iklan:**
>
> - Tidak ada iklan sama sekali dalam **3 hari pertama** pemain baru.
> - Interstitial tidak pernah muncul saat pemain **kalah**.
> - Semua rewarded ads bersifat **opsional** dan selalu memberi nilai jelas.
> - Ad mediation: AppLovin MAX dengan waterfall (AdMob, Unity Ads, ironSource, Meta).

## 15.4 Battle Pass

| Aspek | Spesifikasi |
| --- | --- |
| Durasi season | 35 hari |
| Jumlah tier | 60 |
| Sumber XP BP | Daily mission (100 XP), weekly (500 XP), stage clear (10 XP), event |
| XP per tier | 1.000 (tetap) |
| Estimasi selesai F2P | ~28 hari main normal (45 menit/hari) |
| Jalur gratis | Coin, Enhance Stone, 1 Gear A, 300 Gem total |
| Jalur premium | 2.500 Gem, Gear SS, 1 karakter eksklusif, skin, Pet Food, 5.000 Enhance Stone |
| Tier setelah 60 | Infinite tier: setiap 2.000 XP = 50 Gem |

---

# 16. Live Ops & Event

## 16.1 Kalender Event (Siklus 4 Minggu)

| Minggu | Event Utama | Event Sampingan | Durasi |
| --- | --- | --- | --- |
| 1 | **Boss Rush Festival** | Login 7 hari | 7 hari |
| 2 | **Gear Fever** (drop rate gear x2) | Guild Raid | 5 hari |
| 3 | **Character Banner Baru** | Time Attack Tournament | 10 hari |
| 4 | **Themed Event Stage** (Halloween/Natal/Ramadan/Imlek) | Collection event | 14 hari |

## 16.2 Tipe Event

| Tipe Event | Mekanik | Reward |
| --- | --- | --- |
| **Login Event** | Login harian berturut-turut | Gem, Tiket, Shard |
| **Grind Event** | Kumpulkan X token dari stage | Event shop |
| **Tower Event** | 50 lantai naik, tiap lantai makin sulit | Gear SS di lantai 50 |
| **Boss Rush** | Kalahkan 10 boss beruntun tanpa mati | Title + Gem 3.000 |
| **Damage Race** | Boss dengan HP besar, ranking damage | Bracket reward |
| **Collection Event** | Kumpulkan kartu bertema | Set kosmetik |
| **Puzzle Event** | Stage dengan modifier aneh | Gem, Tech Part |
| **Guild War** | Guild vs Guild, total skor anggota | Guild Coin, ranking |

## 16.3 Daily & Weekly Mission

| Misi Harian | Reward |
| --- | --- |
| Selesaikan 3 stage | 50 Gem + 100 BP XP |
| Bunuh 1.000 musuh | 10.000 Coin |
| Kalahkan 1 elite | 5 Enhance Stone |
| Tonton 1 iklan | 20 Gem |
| Enhance gear 1x | 100 BP XP |
| Login | 20 Gem |

| Misi Mingguan | Reward |
| --- | --- |
| Selesaikan 25 stage | 300 Gem |
| Kalahkan 3 boss | 1 Summon Ticket |
| Capai Power Rating tertentu | Gear A |
| Selesaikan semua daily 5 hari | 500 BP XP |

---

# 17. Sosial & Kompetitif

## 17.1 Guild (Klan)

| Fitur | Detail |
| --- | --- |
| Kapasitas | 30 anggota |
| Peran | Leader, Co-Leader (3), Elder (5), Member |
| Syarat gabung | Account Level 10 |
| Guild Level | 1–20, naik dari kontribusi anggota. Tiap level: buff pasif (+1% ATK/level) |
| Guild Raid | Boss mingguan dengan HP bersama. Reward berdasarkan kontribusi + total |
| Guild Shop | Beli material dengan Guild Coin |
| Guild Chat | Text chat + emoji + auto-translate |
| Guild Check-in | Harian, memberi Guild Coin ke semua anggota |
| Guild War | Mingguan, 1 vs 1 guild, total skor anggota |

## 17.2 Leaderboard

| Leaderboard | Reset | Metrik | Reward |
| --- | --- | --- | --- |
| Endless Mode | Mingguan | Waktu bertahan | Top 1/10/100/1000 bracket |
| Time Attack | Mingguan | Waktu tercepat bunuh boss | Gem, Title |
| Power Rating | Musiman | Total Power | Frame profil eksklusif |
| Guild Ranking | Mingguan | Total skor guild | Guild Coin |
| Damage Race Event | Per event | Total damage | Gear eksklusif |
| Regional (per negara) | Mingguan | Sama seperti global | Bendera + Title |

## 17.3 Fitur Sosial Lain

- **Friend List** — maks 50 teman, kirim/terima 5 gift/hari (berupa Coin).
- **Profile Card** — avatar, frame, title, Power Rating, karakter favorit, statistik.
- **Replay Sharing** — rekam 30 detik terakhir run terbaik, bagikan sebagai video.
- **Build Sharing** — bagikan kode build (loadout + gear + talent) lewat kode 8 karakter.

---

# 18. Onboarding & FTUE (First-Time User Experience)

## 18.1 Menit-per-Menit 10 Menit Pertama

| Waktu | Yang Terjadi | Tujuan Desain |
| --- | --- | --- |
| 0:00–0:15 | Splash + logo, langsung masuk gameplay (**tanpa login wall**) | Time-to-first-action < 15 detik |
| 0:15–0:45 | Cinematic 8 detik (bisa di-skip) + tutorial gerak | Ajarkan 1 kontrol saja |
| 0:45–1:30 | Musuh mudah datang, pemain melihat auto-attack bekerja | "Saya tidak perlu menekan tombol serang" |
| 1:30–2:00 | Level up pertama, highlight kartu dengan glow + pointer | Ajarkan sistem draft |
| 2:00–3:00 | Musuh lebih banyak, level up 3–4x | Rasakan power growth |
| 3:00–3:30 | Elite pertama muncul + chest | Ajarkan chest |
| 3:30–4:30 | Mini-boss scripted (mudah, pasti menang) | Beri kemenangan awal |
| 4:30–5:00 | Reward screen dengan animasi mewah, gear pertama | Dopamine hit |
| 5:00–6:00 | Tutorial lobby: equip gear (dipandu) | Ajarkan meta-progression |
| 6:00–6:30 | Tutorial enhance gear (gratis 1x) | Ajarkan upgrade |
| 6:30–7:00 | Klaim daily reward + starter pack (**tanpa hard sell**) | Perkenalkan ekonomi |
| 7:00–10:00 | Stage 2 & 3, evolusi pertama (scripted) | **Momen "WOW"** |

> **Aturan FTUE:**
>
> - **Tidak ada** pop-up IAP dalam 10 menit pertama.
> - **Tidak ada** iklan dalam 3 hari pertama.
> - **Tidak ada** login/registrasi wajib — guest account otomatis, prompt link akun di hari ke-3.
> - Pemain **tidak boleh kalah** di 3 stage pertama (HP diberi "safety floor" tersembunyi).
> - Setiap fitur baru diperkenalkan **satu per satu**, tidak sekaligus.

## 18.2 Progressive Feature Unlock

| Fitur | Terbuka Pada | Alasan |
| --- | --- | --- |
| Equip Gear | Setelah Stage 1-3 | Fitur inti pertama |
| Enhance Gear | Setelah Stage 1-5 | Sink Coin pertama |
| Gacha / Summon | Setelah Chapter 1 clear | Setelah paham nilai gear |
| Tech Parts | Chapter 2 | Layer kedua |
| Skill Tree | Chapter 3 | Butuh Talent Point |
| Event | Chapter 3 | Setelah paham loop dasar |
| Guild | Account Lv.10 | Sosial setelah retensi terbentuk |
| Pet | Chapter 5 | Layer ketiga |
| Endless Mode | Chapter 6 | Endgame awal |
| Nightmare Mode | Chapter 10 clear | Endgame |
| Difficulty Tier | Chapter 10 clear | Endgame |

---

# 19. UI/UX & Struktur Layar

## 19.1 Peta Navigasi (Screen Map)

```
[Splash] -> [Loading] -> [LOBBY / MAIN MENU]
                              |
   +------------+-------------+-----------+------------+----------+
   |            |             |           |            |          |
[Character] [Equipment]    [PLAY]      [Shop]      [Guild]    [Event]
   |            |             |           |            |          |
   |       +----+----+   [Chapter     +---+---+    [Raid]   [Event Stage]
   |       |         |    Select]     |       |    [Chat]   [Event Shop]
[Level] [Gear    [Tech       |     [Gacha] [IAP]  [Ranking]
[Star]   List]   Parts]      |     [Daily] [Pass]
[Skin]  [Enhance][Skill      |
         Reforge] Tree]      |
                             v
                     [Loadout Select]
                             v
                       [IN-GAME HUD]
                             v
                     [Result Screen]
                             v
                         [LOBBY]
```

## 19.2 In-Game HUD (Layout Portrait)

| Posisi | Elemen | Catatan |
| --- | --- | --- |
| Atas-tengah | Timer (MM:SS) besar | Font tebal, selalu terbaca |
| Bawah timer | XP Bar + Level | Bar penuh lebar layar, tipis |
| Atas-kiri | Kill count + Coin count | Ikon kecil |
| Atas-kanan | Tombol Pause | Hit area minimal 48x48 dp |
| Di bawah HUD atas | Ikon senjata & item aktif (2 baris x 6) | Dengan indikator level |
| Tengah | Area gameplay | 70% layar |
| Bawah-kiri | Area joystick (invisible sampai disentuh) | Zona 40% kiri bawah |
| Bawah-kanan | Tombol skill aktif (maks 2) + radial cooldown | Zona 30% kanan bawah |
| Atas boss | Boss health bar + nama + indikator fase | Hanya saat boss aktif |
| Tepi layar | Indikator panah untuk musuh/item di luar layar | Warna sesuai tipe |

## 19.3 Prinsip UX yang Wajib

> 1. **Thumb-zone friendly** — semua tombol sering-tekan di 1/3 bawah layar.
> 2. **Safe area** — hormati notch, punch-hole, gesture bar (padding minimal 24 dp).
> 3. **Hit area minimal 48x48 dp** untuk semua tombol interaktif.
> 4. **Maksimal 3 tap** dari lobby ke mulai bermain.
> 5. **Feedback instan** — setiap tap harus punya respons visual < 100 ms.
> 6. **Red dot notification** pada semua menu yang punya reward belum diklaim.
> 7. **One-tap upgrade** — tombol "Auto Equip Best" dan "Enhance Max".
> 8. **Damage number** bisa dimatikan di settings.
> 9. **Loading screen** maksimal 4 detik; tampilkan tips gameplay.
> 10. **Tidak ada teks kecil** — minimum font size 12 sp.

## 19.4 Daftar Layar Lengkap (Screen Inventory)

| # | Layar | Elemen Utama |
| --- | --- | --- |
| S01 | Splash / Logo | Logo, versi, progress bar |
| S02 | Loading | Tips, art, progress |
| S03 | Lobby | Karakter, Power Rating, tombol Play, nav bar, red dots |
| S04 | Chapter Select | Peta chapter, stage node, bintang, rekomendasi power |
| S05 | Loadout Select | Pilih karakter, 2 senjata starter, pet, tampilan stat |
| S06 | In-Game HUD | (lihat 19.2) |
| S07 | Level-Up Card | 3 kartu, tombol reroll/banish/skip |
| S08 | Pause Menu | Stat detail run, settings, resume, quit |
| S09 | Death / Revive | Countdown 10 detik, tombol iklan/gem, quit |
| S10 | Result Screen | Reward animasi, statistik run, double reward, share |
| S11 | Character List | Grid karakter, filter, indikator shard |
| S12 | Character Detail | Stat, skill, level up, star up, skin |
| S13 | Equipment | 6 slot, inventory list, filter, sort, auto-equip |
| S14 | Gear Detail | Stat utama & sub, enhance, ascend, reforge, awaken, lock |
| S15 | Tech Parts | Grid modul, level, material |
| S16 | Skill Tree | Board node interaktif, zoom/pan, talent point counter |
| S17 | Collection | Galeri kartu, progress set |
| S18 | Pet | List pet, level, feed, pilih aktif |
| S19 | Shop — Gacha | Banner, rate display, pull 1x/10x, pity counter |
| S20 | Shop — IAP | Paket, harga lokal, badge "best value" |
| S21 | Shop — Daily | Item rotasi harian, refresh timer |
| S22 | Battle Pass | Track 60 tier, progress, klaim, beli |
| S23 | Mission | Daily, weekly, achievement tab |
| S24 | Event Hub | List event aktif, timer, banner |
| S25 | Guild | Info, member list, chat, raid, shop, war |
| S26 | Leaderboard | Tab kategori, ranking, reward preview |
| S27 | Profile | Avatar, frame, title, statistik |
| S28 | Friends | List, add, gift |
| S29 | Settings | Audio, grafis, bahasa, akun, privasi, support |
| S30 | Mailbox | Pesan sistem, reward, klaim semua |
| S31 | Notice / News | Pengumuman, patch note |
| S32 | Tutorial Overlay | Highlight + pointer + teks |

---

# 20. Art Direction & Audio

## 20.1 Arahan Visual

| Aspek | Spesifikasi |
| --- | --- |
| Gaya | 2D stylized semi-realistis, outline tipis, warna saturasi tinggi |
| Perspektif | Top-down 3/4 view |
| Palet | Latar gelap agar musuh & efek menonjol |
| Kontras | Pemain selalu paling terang di layar; musuh outline merah tipis |
| Resolusi sprite | Karakter 128x128 px, musuh normal 96x96 px, boss 512x512 px |
| Animasi | Skeletal (Spine 2D / Unity 2D Animation), 12–24 fps |
| Frame animasi minimal | Idle (4), Walk (8), Attack (6), Hurt (2), Death (8) |
| Tileset | Modular 256x256 px, seamless tiling untuk map infinite |
| VFX | Particle system + shader; wajib punya versi "Low" |
| UI Style | Flat modern + neon accent, rounded corner 12 dp |

## 20.2 Aturan Readability (Sangat Kritis)

> Karena layar bisa dipenuhi 300+ musuh dan ratusan efek, aturan berikut **wajib**:
>
> 1. VFX pemain berwarna **terang & cerah** (cyan, putih, kuning); VFX musuh **merah/ungu gelap**.
> 2. Semua telegraph serangan musuh berwarna **merah dengan alpha 40%**, tidak pernah warna lain.
> 3. Batas maksimal partikel di layar: **1.500** (Low: 400, Medium: 800, High: 1.500).
> 4. Damage number di-**pool** dan digabung jika > 30 angka/detik.
> 5. Karakter pemain memiliki **outline putih permanen** agar tidak hilang di kerumunan.
> 6. Ada opsi settings: "Kurangi Efek Visual" dan "Sembunyikan Damage Number".
> 7. Musuh berbahaya (Bomber, Sniper) punya **indikator ikon di atas kepala**.

## 20.3 Audio

| Kategori | Spesifikasi |
| --- | --- |
| Musik lobby | 1 track loop, tenang, 2–3 menit |
| Musik stage | 1 track per chapter (10 track), layered: bass -> drum -> lead |
| Musik boss | 1 track per boss tier (3 track), intens |
| SFX senjata | Unik per senjata (20+ SFX), variasi pitch +/-10% |
| SFX musuh | Spawn, hurt, death per arketipe |
| SFX UI | Tap, confirm, cancel, error, level up, evolve, chest, reward |
| SFX prioritas | Voice limiting: maks 24 SFX bersamaan (UI > Boss > Player > Enemy) |
| Format | OGG Vorbis (Android), AAC (iOS), 44,1 kHz |
| Ukuran total audio | Target < 45 MB |
| Settings | Slider terpisah BGM, SFX, Haptic. Auto-mute saat background |

---

# 21. Kebutuhan Teknis (Technical Requirements)

## 21.1 Stack Teknologi

| Layer | Teknologi | Alasan |
| --- | --- | --- |
| Engine | **Unity 2022.3 LTS**, URP 2D | Ekosistem mobile terbaik |
| Bahasa | C# | Standar Unity |
| Crowd simulation | **Unity DOTS / ECS + Burst + Jobs** | Wajib untuk 300+ musuh @60 fps |
| Rendering | GPU Instancing + Sprite Atlas + SRP Batcher | Kurangi draw call |
| UI | UI Toolkit (menu) + uGUI (HUD in-game) | Performa & fleksibilitas |
| Animasi | Spine 2D atau Unity 2D Animation | Skeletal ringan |
| Backend | **PlayFab** atau **Firebase + Cloud Functions** | Save cloud, leaderboard, remote config |
| Database | Firestore / PlayFab Entities | Data pemain |
| Analytics | Firebase Analytics + GameAnalytics + Adjust | Funnel & UA |
| Crash | Firebase Crashlytics | Stabilitas |
| Remote Config | Firebase Remote Config / PlayFab Title Data | Live balancing tanpa update |
| A/B Testing | Firebase A/B Testing | Optimasi monetisasi & retensi |
| Ads | AppLovin MAX (mediation) | eCPM optimal |
| IAP | Unity IAP + server-side receipt validation | Anti-fraud |
| Push | Firebase Cloud Messaging | Retensi |
| CI/CD | GitHub Actions + Unity Build Automation + Fastlane | Otomasi build |
| Version Control | Git + Git LFS | Kolaborasi |
| Asset Delivery | Unity Addressables + CDN | Ukuran install kecil |

## 21.2 Target Performa

| Metrik | Low-end | Mid-range | High-end |
| --- | --- | --- | --- |
| Device referensi | Redmi 9A (Helio G25, 2GB) | Redmi Note 11 (SD 680, 4GB) | iPhone 14 / SD 8 Gen 2 |
| Target FPS | 30 fps stabil | 60 fps | 60–120 fps |
| Musuh maks di layar | 120 | 250 | 320 |
| Partikel maks | 400 | 800 | 1.500 |
| RAM usage | < 700 MB | < 1,1 GB | < 1,6 GB |
| Draw call | < 80 | < 150 | < 250 |
| Waktu boot (cold start) | < 8 detik | < 5 detik | < 3 detik |
| Waktu loading stage | < 4 detik | < 2,5 detik | < 1,5 detik |
| Ukuran APK/IPA awal | < 150 MB | — | — |
| Total setelah download asset | < 600 MB | — | — |
| Konsumsi baterai | < 12%/jam | < 10%/jam | < 8%/jam |
| Suhu device | Tidak melebihi 42 C setelah 30 menit | — | — |

## 21.3 Optimasi Wajib

> 1. **Object pooling** untuk SEMUA: musuh, proyektil, VFX, damage number, XP gem, koin, audio source.
> 2. **Spatial hashing / grid partitioning** untuk collision detection.
> 3. **Sprite atlas** per kategori, maks 2048x2048.
> 4. **Texture compression:** ASTC 6x6 (Android & iOS), fallback ETC2.
> 5. **Zero GC allocation** di Update loop — wajib diverifikasi dengan Unity Profiler.
> 6. **LOD musuh:** musuh jauh memakai animasi frame-rate rendah (6 fps).
> 7. **Batching damage number** — gabungkan damage dalam window 0,2 detik.
> 8. **Fixed timestep 0,02s** untuk simulasi gameplay agar deterministik.
> 9. **Addressables** — chapter asset di-download on-demand.
> 10. **Async scene loading** dengan progress bar nyata.
> 11. **Audio streaming** untuk BGM, decompress on load untuk SFX pendek.
> 12. **Adaptive quality** — deteksi FPS drop > 3 detik, otomatis turunkan kualitas partikel.

## 21.4 Arsitektur Kode (Struktur Modul)

```
Assets/
  _Project/
    Scripts/
      Core/            // GameManager, SceneLoader, ServiceLocator, EventBus
      Gameplay/
        Player/        // PlayerController, PlayerStats, InputHandler
        Weapons/       // WeaponBase, WeaponSystem, EvolutionSystem
        Enemies/       // EnemySpawner, EnemyAI (ECS), EnemyData
        Bosses/        // BossController, BossPhase, AttackPattern
        Combat/        // DamageCalculator, StatusEffectSystem, ElementReaction
        Pickups/       // XPGem, Coin, Chest, Consumable
        Stage/         // StageController, WaveScheduler, HazardSystem
      Meta/
        Inventory/     // GearManager, GearData, EnhanceService
        Progression/   // SkillTree, TechParts, CharacterProgress
        Economy/       // CurrencyManager, ShopService, GachaService
        Missions/      // MissionService, BattlePassService
      UI/
        Screens/       // Satu class per layar (S01-S32)
        Components/    // Reusable widgets
        HUD/
      Services/
        Save/          // LocalSave (encrypted), CloudSync, ConflictResolver
        Network/       // ApiClient, RetryPolicy, OfflineQueue
        Analytics/     // AnalyticsService, EventDefinitions
        Ads/           // AdService
        IAP/           // PurchaseService, ReceiptValidator
        RemoteConfig/
      Data/            // ScriptableObjects: WeaponSO, EnemySO, StageSO, GearSO
      Utils/           // Pooling, Math, Extensions, Timers
    Art/
    Audio/
    Prefabs/
    Scenes/            // Boot, Lobby, Gameplay
```

### Pola Arsitektur

| Pola | Penggunaan |
| --- | --- |
| **ScriptableObject-driven data** | Semua data senjata, musuh, gear, stage -> balancing tanpa coding |
| **Event Bus** | Komunikasi antar sistem tanpa coupling (`OnEnemyKilled`, `OnLevelUp`) |
| **Service Locator / DI (VContainer)** | Akses service global |
| **State Machine** | Boss phase, game state, UI screen state |
| **Command Pattern** | Aksi meta (enhance, purchase) agar bisa di-queue offline |
| **ECS/DOTS** | Hanya untuk sistem musuh & proyektil |

---

# 22. Backend, Save Data & Anti-Cheat

## 22.1 Save Data

| Aspek | Spesifikasi |
| --- | --- |
| Local save | JSON ter-enkripsi (AES-256) di `Application.persistentDataPath` |
| Auto-save | Setiap akhir run, setiap transaksi, dan setiap 60 detik di lobby |
| Cloud save | Sinkron saat login, saat app pause, dan setiap 5 menit online |
| Konflik | Bandingkan `lastModified` + `totalPowerRating`; tampilkan dialog pilihan |
| Offline mode | Semua gameplay bisa offline; transaksi di-queue |
| Backup | Server menyimpan 3 snapshot terakhir (rollback support untuk CS) |
| Account link | Guest -> Google Play Games / Apple Sign-In / Email |

### Struktur Data Pemain (Player Profile Schema)

```json
{
  "playerId": "uuid",
  "createdAt": "ISO8601",
  "lastLoginAt": "ISO8601",
  "accountLevel": 24,
  "accountExp": 15230,
  "powerRating": 18450,
  "currencies": {
    "coin": 1250000, "gem": 3400, "summonTicket": 12,
    "chestKey": 5, "techPart": 230, "enhanceStone": 890,
    "reforgeDust": 340, "guildCoin": 1200, "talentPoint": 46
  },
  "characters": [
    { "id": "C01", "level": 22, "stars": 3, "shards": 45, "skinId": "C01_S02" }
  ],
  "activeCharacterId": "C05",
  "gear": [
    { "uid": "g_8f2a", "defId": "GR_WEAPON_012", "rarity": "SS",
      "level": 62, "mainStat": { "type": "ATK", "value": 1840 },
      "subStats": [ { "type": "CRIT_RATE", "value": 12.4 } ],
      "setId": "BERSERKER", "awakened": true, "locked": true, "equippedSlot": 1 }
  ],
  "techParts": { "OVERCLOCK": 7, "AMMO_EXPANDER": 5 },
  "talentNodes": ["OFF_01", "OFF_02", "DEF_01"],
  "collectibles": { "weapons": ["W01","W02"], "bosses": ["B01"] },
  "pets": [ { "id": "PET_03", "level": 18, "active": true } ],
  "progress": {
    "chaptersCleared": 7, "stageStars": { "1-1": 3, "1-2": 3 },
    "difficultyTier": 2, "endlessBest": 1842
  },
  "battlePass": { "seasonId": "S01", "tier": 34, "exp": 420, "premium": true },
  "missions": { "dailyResetAt": "ISO8601" },
  "guildId": "gld_1234",
  "settings": { "bgm": 0.7, "sfx": 1.0, "haptic": true, "lang": "id", "quality": "medium" },
  "flags": { "tutorialCompleted": true, "starterPackBought": false },
  "checksum": "hmac-sha256"
}
```

## 22.2 Anti-Cheat

| Ancaman | Mitigasi |
| --- | --- |
| Memory editing (GameGuardian) | Enkripsi nilai penting di memori, checksum berkala |
| Speed hack | Deteksi anomali `Time.deltaTime` vs `Stopwatch`; validasi durasi run di server |
| Save file editing | HMAC-SHA256 checksum pada save; server-side validation saat sync |
| IAP fraud | Server-side receipt validation (Google Play & App Store Server API) |
| Reward injection | Semua reward penting divalidasi server; telemetry untuk sanity check |
| Leaderboard cheating | Server validasi: skor vs Power Rating vs durasi vs kill count |
| APK modding | Play Integrity API (Android), DeviceCheck (iOS); obfuscation IL2CPP |
| Emulator farming | Deteksi emulator, batasi reward event untuk akun mencurigakan |

### Aturan Validasi Run Result

```js
// Server menolak hasil run jika:
killCount > (durationSeconds * MaxKillRatePerSecond * 1.3)
|| coinsEarned > (killCount * MaxCoinPerKill * 1.2)
|| durationSeconds < MinPossibleDuration(stageId)
|| playerLevel > MaxTheoreticalLevel(durationSeconds)
|| damageDealt > (powerRating * DamageCoefficient * durationSeconds)
```

## 22.3 Endpoint API Utama

| Endpoint | Method | Fungsi |
| --- | --- | --- |
| `/auth/login` | POST | Login guest / Google / Apple |
| `/player/profile` | GET | Ambil profil |
| `/player/sync` | POST | Sinkronisasi save |
| `/run/start` | POST | Daftarkan sesi run (dapat `runToken`) |
| `/run/complete` | POST | Kirim hasil run + telemetry, terima reward tervalidasi |
| `/shop/purchase` | POST | Beli item dengan currency |
| `/iap/validate` | POST | Validasi receipt |
| `/gacha/pull` | POST | Server-authoritative gacha (rate & pity di server) |
| `/leaderboard/{type}` | GET | Ambil ranking |
| `/leaderboard/submit` | POST | Kirim skor |
| `/guild/*` | — | CRUD guild, raid, chat |
| `/mail` | GET/POST | Mailbox & klaim |
| `/config` | GET | Remote config & balancing |
| `/events/active` | GET | Event aktif |

> **Aturan keamanan:** Gacha, reward, dan semua transaksi currency **harus server-authoritative**. Client tidak pernah menentukan hasil gacha atau jumlah reward.

---

# 23. Analytics & KPI

## 23.1 Event yang Wajib Di-track

| Kategori | Event | Parameter |
| --- | --- | --- |
| Lifecycle | `app_open`, `session_start`, `session_end` | duration, source |
| FTUE | `tutorial_step` | step_id, time_since_install, completed |
| Gameplay | `run_start` | stage_id, character_id, power_rating, loadout |
| Gameplay | `run_end` | result, duration, level_reached, kills, damage, coins |
| Gameplay | `level_up_choice` | level, options_shown, option_chosen, rerolled, banished |
| Gameplay | `weapon_evolved` | weapon_id, evolution_id, time_in_run |
| Gameplay | `player_death` | stage_id, time, cause, hp_at_last_10s |
| Gameplay | `revive_used` | method (ad/gem), count_in_run |
| Progression | `chapter_cleared` | chapter_id, attempts, days_since_install |
| Progression | `gear_enhanced` | gear_id, from_level, to_level, cost |
| Progression | `talent_unlocked` | node_id |
| Economy | `currency_earned` | type, amount, source |
| Economy | `currency_spent` | type, amount, sink |
| Monetization | `iap_initiated` / `iap_completed` / `iap_failed` | sku, price, currency, placement |
| Monetization | `shop_viewed` | shop_tab, entry_point |
| Monetization | `gacha_pull` | banner_id, count, results, pity_counter |
| Ads | `ad_requested`, `ad_shown`, `ad_completed`, `ad_failed` | placement, network, revenue |
| Social | `guild_joined`, `guild_raid_participated` | guild_id, contribution |
| Retention | `push_received`, `push_opened` | campaign_id |
| Technical | `fps_report` | avg_fps, min_fps, device_model, quality_setting |
| Technical | `load_time` | scene, duration |

## 23.2 Dashboard KPI

| Kategori | Metrik |
| --- | --- |
| Akuisisi | Install, CPI, Organic %, Source breakdown |
| Retensi | D1, D3, D7, D14, D30, D60, Rolling retention |
| Engagement | DAU, MAU, DAU/MAU, Session/DAU, Session length, Playtime/DAU |
| Progresi | Avg. chapter at D1/D7/D30, Funnel drop-off per stage |
| Monetisasi | ARPDAU, ARPPU, ARPU, LTV (D7/D30/D180), Conversion rate |
| Iklan | Ad impressions/DAU, eCPM, Ad ARPDAU, Fill rate |
| Ekonomi | Currency inflow/outflow, Balance median per segment |
| Kual