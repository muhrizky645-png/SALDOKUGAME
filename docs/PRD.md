# PRD — Project Nightfall: Survival Roguelite Mobile Game (Survivor.io-like)

<aside>
📌

**Product Requirements Document (PRD)**

| Field | Value |
| --- | --- |
| Nama Produk (working title) | **Project Nightfall** |
| Genre | Survival Roguelite / Bullet-Heaven / Horde Survival |
| Platform | Android (primary), iOS (primary), WebGL (opsional, marketing demo) |
| Engine | Unity 2022 LTS (URP 2D) |
| Orientasi | Portrait (9:16), locked |
| Mode | Single-player (online-assisted), asynchronous social |
| Model Bisnis | Free-to-Play + IAP + Rewarded Ads + Battle Pass |
| Versi Dokumen | 1.0 (Draft) |
| Tanggal | 31 Agustus 2026 |
| Owner | abigalhebeevie3 (Product) |
| Status | 🟡 In Review |
</aside>

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
> 

## 2.2 Tujuan Bisnis

| # | Tujuan | Metrik | Target (6 bulan pasca-launch) |
| --- | --- | --- | --- |
| G1 | Akuisisi pemain | Total Install | 2.000.000 |
| G2 | Retensi jangka pendek | D1 Retention | ≥ 42% |
| G3 | Retensi jangka menengah | D7 Retention | ≥ 18% |
| G4 | Retensi jangka panjang | D30 Retention | ≥ 7% |
| G5 | Monetisasi | ARPDAU | ≥ $0,085 |
| G6 | Konversi pembayar | Payer Conversion Rate | ≥ 3,5% |
| G7 | Engagement | Avg. Session Length | ≥ 12 menit |
| G8 | Engagement | Sessions/DAU/day | ≥ 4,5 |
| G9 | Kualitas | Crash-free session rate | ≥ 99,5% |
| G10 | Rating | Store Rating | ≥ 4,4 ⭐ |

## 2.3 Tujuan Pemain (Player Goals)

- Merasakan sensasi "membabat ribuan musuh" dengan effort input minimal.
- Menemukan kombinasi build baru yang overpowered.
- Menyelesaikan chapter demi chapter dan melihat karakter makin kuat.
- Bersaing di leaderboard mingguan.

## 2.4 Non-Goals (Yang TIDAK dikerjakan di v1.0)

<aside>
🚫

Hal berikut **secara sengaja dikecualikan** dari scope rilis 1.0 untuk menjaga fokus:

- Real-time PvP / co-op multiplayer sinkron
- Mode landscape
- Console / PC port
- Character voice acting penuh
- Player-generated content / level editor
- Cross-platform account merging (di luar Google/Apple/Guest)
- Trading antar pemain
</aside>

---

# 3. Target Audiens & Persona

## 3.1 Demografi Target

| Atribut | Detail |
| --- | --- |
| Usia | 16–34 tahun (core: 18–28) |
| Gender | 65% pria / 35% wanita |
| Wilayah Prioritas | Tier 1: Indonesia, Filipina, Vietnam, Thailand
Tier 2: Brasil, Meksiko, India
Tier 3: US, Jepang, Korea |
| Device | Android mid-range (RAM 3–4GB, Snapdragon 6xx setara), iPhone 8+ |
| Koneksi | 4G tidak stabil → **wajib bisa dimainkan offline untuk gameplay inti** |
| Waktu bermain | Commute, istirahat kerja, sebelum tidur |

## 3.2 Persona

### 👤 Persona 1 — "Rian, si Casual Commuter" (55% populasi)

| Aspek | Detail |
| --- | --- |
| Usia / Pekerjaan | 24 / karyawan swasta |
| Motivasi | Hiburan cepat tanpa berpikir keras, relaksasi |
| Perilaku | Main 3–5 sesi/hari, @10 menit. Tidak baca guide. |
| Spending | Rp0 – Rp50.000/bulan (beli Battle Pass kalau murah) |
| Kebutuhan Desain | Onboarding super jelas, auto-progress, reward harian, tidak butuh koordinasi |
| Pain Point | Benci jika kalah karena tidak paham sistem |

### 👤 Persona 2 — "Dita, si Optimizer" (30% populasi)

| Aspek | Detail |
| --- | --- |
| Usia / Pekerjaan | 21 / mahasiswa |
| Motivasi | Mencari build terkuat, menaklukkan stage tersulit |
| Perilaku | Main 6–10 sesi/hari, nonton YouTube guide, ikut komunitas Discord |
| Spending | Rp100.000 – Rp400.000/bulan |
| Kebutuhan Desain | Damage number detail, stat sheet lengkap, endgame content, leaderboard |
| Pain Point | Benci RNG yang tidak bisa dimitigasi & konten habis |

### 👤 Persona 3 — "Bagus, si Whale Kompetitif" (5% populasi, 60% revenue)

| Aspek | Detail |
| --- | --- |
| Usia / Pekerjaan | 31 / wiraswasta |
| Motivasi | Jadi yang terkuat, ranking #1 leaderboard, koleksi lengkap |
| Perilaku | Login 8×/hari, selalu beli semua paket event |
| Spending | Rp1.000.000+/bulan |
| Kebutuhan Desain | Leaderboard global & guild, konten eksklusif, skin prestise, gear S-tier |
| Pain Point | Benci jika uangnya tidak menghasilkan keunggulan yang terlihat |

### 👤 Persona 4 — "Sari, si Collector Santai" (10% populasi)

| Aspek | Detail |
| --- | --- |
| Motivasi | Mengumpulkan semua karakter, skin lucu, pet |
| Kebutuhan Desain | Kosmetik, galeri koleksi, event bertema, pet system |

---

# 4. Analisis Kompetitor & Diferensiasi

## 4.1 Lanskap Kompetitor

| Game | Developer | Kekuatan | Kelemahan | Pelajaran yang Diambil |
| --- | --- | --- | --- | --- |
| [**Survivor.io**](http://Survivor.io) | Habby | Polish tinggi, evolusi senjata memuaskan, monetisasi halus | Late-game grind berat, paywall gear S | Ambil: evolusi & feel. Perbaiki: kurva grind |
| **Vampire Survivors** | poncle | Depth luar biasa, harga sekali bayar, secret berlimpah | UI mobile kurang optimal, tidak F2P-native | Ambil: hidden unlock & secret |
| **Archero** | Habby | Skill draft, room-based progression | Energy system membatasi, RNG kejam | Hindari: hard energy gate |
| **Soul Knight** | ChillyRoom | Co-op, variasi senjata besar | Grafik kurang modern | Ambil: variasi senjata |
| **Magic Survival** | LEME | Sistem sinergi elemen kuat | Produksi rendah | Ambil: sinergi elemen |
| **Dead Cells Mobile** | Motion Twin | Kualitas premium | Bukan F2P, sulit | Ambil: game feel |

## 4.2 Diferensiasi Utama (USP)

<aside>
⭐

**5 Pembeda Project Nightfall:**

1. **Sistem Elemen & Sinergi** — 5 elemen (Fire/Ice/Lightning/Poison/Void) dengan status effect yang saling bereaksi (mis. Ice + Lightning = *Superconduct*, radius damage besar).
2. **Dual-Evolution** — satu senjata bisa punya **2 jalur evolusi berbeda** tergantung item pasif yang dipilih → variasi build 2× lipat.
3. **Loadout Pre-Run** — pemain memilih 2 senjata "starter" sebelum run, mengurangi frustrasi RNG.
4. **Zero Energy System** — tidak ada stamina/energy gate. Main sepuasnya. Monetisasi lewat progression speed & kosmetik.
5. **Offline-First** — semua gameplay inti berjalan offline; sinkronisasi saat online.
</aside>

---

# 5. Core Gameplay Loop

## 5.1 Loop Mikro (dalam 1 detik)

```
Gerakkan joystick → Karakter bergerak → Senjata auto-fire ke musuh terdekat/arah hadap
→ Musuh mati → Drop XP gem / koin / item → Pemain menyerap dengan mendekat
→ XP bar naik
```

## 5.2 Loop Menengah (dalam 1 run, 15 menit)

```
Start run (Lv.1, 1 senjata)
  ↓
Bunuh musuh → kumpulkan XP
  ↓
LEVEL UP → Muncul 3 kartu pilihan (senjata baru / upgrade senjata / item pasif)
  ↓
Pilih 1 → Build makin kuat
  ↓
Buka Chest dari Elite → dapat upgrade gratis / evolusi
  ↓
[Menit 5 / 10] Mini-Boss muncul
  ↓
Lanjut farming, evolve senjata
  ↓
[Menit 15] BOSS AKHIR
  ↓
Menang → Reward Screen → Kembali ke Lobby
Kalah → Opsi Revive (ads/gem) atau Kembali ke Lobby (reward parsial)
```

## 5.3 Loop Makro (harian / mingguan)

```
Lobby
  ↓
Cek Daily Mission & Login Reward
  ↓
Upgrade Gear / Tech Parts / Skill Tree dengan resource hasil run
  ↓
Pilih Chapter / Event Stage
  ↓
RUN (loop menengah)
  ↓
Dapat: Koin, EXP Karakter, Gear Drop, Tech Parts, Event Token
  ↓
Gear naik level → Power Rating naik → Buka stage lebih sulit
  ↓
Selesaikan Battle Pass tier & Event
  ↓
(ulangi)
```

## 5.4 Diagram Waktu Sesi (Session Pacing)

| Menit | Level Pemain (perkiraan) | Musuh/detik | Event | Emosi Target |
| --- | --- | --- | --- | --- |
| 0:00–1:00 | 1–5 | 2–4 | Tutorial spawn ringan | Tenang, belajar |
| 1:00–3:00 | 5–14 | 5–10 | Spawn grup pertama | Mulai sibuk |
| 3:00–5:00 | 14–24 | 10–18 | Elite #1 + Chest | Tegang |
| 5:00–7:00 | 24–32 | 18–28 | **Mini-Boss #1** | Puncak kecil |
| 7:00–9:00 | 32–40 | 25–35 | Swarm wave, evolusi pertama | Lega + kuat |
| 9:00–11:00 | 40–48 | 35–50 | **Mini-Boss #2**  • Elite ×3 | Tegang tinggi |
| 11:00–13:00 | 48–55 | 50–70 | Horde massal, screen penuh | Power fantasy |
| 13:00–14:30 | 55–60 | 70–90 | Pre-boss buildup, musuh berhenti spawn | Antisipasi |
| 14:30–15:00 | 60+ | Boss saja | **BOSS AKHIR** | Klimaks |

---

# 6. Mekanik Gameplay Inti (Detail Teknis)

## 6.1 Kontrol

| Elemen | Spesifikasi |
| --- | --- |
| Input utama | **Floating virtual joystick** — muncul di titik sentuh pertama di sisi kiri/manapun layar bawah |
| Dead zone | 8% dari radius joystick |
| Radius joystick | 120 px @ referensi 1080×1920 |
| Respons | Analog penuh (0–1), kecepatan proporsional terhadap jarak dari center |
| Smoothing | Lerp 0,12 detik untuk menghindari jitter |
| Auto-attack | **Selalu aktif**, tidak bisa dimatikan |
| Tombol aktif | Maks. 2 tombol skill aktif (dari Equipment Skill), pojok kanan bawah |
| Pause | Tombol ☰ pojok kanan atas → membuka menu pause (stats, settings, quit) |
| Haptic | Getaran ringan saat level up, dapat chest, dan kena damage (bisa dimatikan) |

## 6.2 Kamera

| Properti | Nilai |
| --- | --- |
| Tipe | Orthographic, top-down |
| Follow | Smooth damp, `smoothTime = 0,15s` |
| Look-ahead | 1,2 unit ke arah gerakan pemain |
| Ortho size default | 6,5 unit (≈ 13 unit tinggi dunia) |
| Zoom-out otomatis | Saat boss aktif → ortho size 8,0 (lerp 1,5 detik) |
| Screen shake | Trauma-based (Perlin noise), max 0,35 unit, decay 1,8/detik |
| Batas | Tidak ada (map infinite scrolling) |

## 6.3 Karakter Pemain — Stat Dasar

| Stat | Simbol | Nilai Dasar | Cap | Keterangan |
| --- | --- | --- | --- | --- |
| Max HP | `HP` | 100 | — | Naik dari gear & level karakter |
| HP Regen | `REG` | 0,0/detik | 50/s | Dari item pasif |
| Move Speed | `SPD` | 4,0 unit/detik | 12,0 | +% dari item |
| Attack (ATK) | `ATK` | 10 | — | Multiplier utama damage |
| Crit Rate | `CR` | 5% | 100% |  |
| Crit Damage | `CD` | 150% | 1000% |  |
| Armor | `ARM` | 0 | 90% DR | Damage reduction |
| Dodge | `DGE` | 0% | 60% | Peluang hindar total |
| Pickup Radius | `PR` | 1,5 unit | 12,0 | Radius serap XP/koin |
| Cooldown Reduction | `CDR` | 0% | 70% | Mempercepat semua senjata |
| Area Size | `AoE` | 100% | 400% | Ukuran hitbox skill |
| Projectile Speed | `PS` | 100% | 300% |  |
| Duration | `DUR` | 100% | 300% | Lama efek bertahan |
| Amount (+Proj) | `AMT` | +0 | +8 | Jumlah proyektil tambahan |
| Luck | `LCK` | 0% | 200% | Pengaruh drop rate & kualitas kartu |
| Growth (EXP Gain) | `GRW` | 100% | 300% |  |
| Greed (Coin Gain) | `GRD` | 100% | 500% |  |
| Revive | `RVV` | 0 | 5 | Auto-revive dalam run |
| Magnet | `MAG` | — | — | Konsumabel, tarik semua XP di layar |

## 6.4 Formula Damage

```
// 1. Base damage per hit
BaseDamage = WeaponBaseDamage × (1 + WeaponLevelBonus) × (1 + ATK_Percent) + ATK_Flat

// 2. Critical
isCrit = random() < CritRate
CritMultiplier = isCrit ? (1 + CritDamage) : 1

// 3. Elemental bonus & resistance musuh
ElementMultiplier = 1 + ElementBonus - EnemyElementResist

// 4. Damage tipe musuh (Normal / Elite / Boss)
TypeMultiplier = 1 + DamageVsType

// 5. Armor musuh
ArmorReduction = EnemyArmor / (EnemyArmor + 100 + 10 × PlayerLevel)

// 6. Final
FinalDamage = BaseDamage
            × CritMultiplier
            × ElementMultiplier
            × TypeMultiplier
            × (1 - ArmorReduction)
            × (1 + AllDamageBonus)
            × RandomVariance(0.95 .. 1.05)
```

**Formula damage yang diterima pemain:**

```
IncomingDamage = EnemyATK × (1 - min(0.90, PlayerArmor / (PlayerArmor + 200)))
                × (1 - DamageReductionPercent)
if random() < DodgeRate → IncomingDamage = 0  // tampilkan teks "MISS"
```

**Invincibility Frame (i-frame):** 0,5 detik setelah menerima damage kontak. Damage area (DoT) mengabaikan i-frame tetapi memiliki tick rate sendiri (0,5 detik).

## 6.5 Sistem XP & Level Up

| Aspek | Spesifikasi |
| --- | --- |
| Sumber XP | Gem yang di-drop musuh (hijau = 1, biru = 5, ungu = 25, emas = 100) |
| Drop rate gem | Normal: 100% hijau; Elite: 1× biru + 3× hijau; Mini-Boss: 1× ungu; Boss: 1× emas |
| Radius serap | `PickupRadius`, gem terbang ke pemain dengan easing saat masuk radius |
| Formula XP butuh | `XPRequired(n) = floor(5 + 8×n + 0.55×n²)` untuk level n → n+1 |
| Contoh | Lv1→2: 13 XP · Lv10→11: 140 XP · Lv30→31: 740 XP · Lv60→61: 2.463 XP |
| Level maksimum | Tidak ada hard cap (praktis ±70 dalam 15 menit) |
| Saat level up | Game **pause total (timescale 0)**, tampil 3 kartu pilihan |
| Antrean level up | Jika naik beberapa level sekaligus, kartu ditampilkan berurutan |

## 6.6 Sistem Kartu Level-Up (Draft)

**Aturan pemilihan kartu:**

1. Slot senjata maksimal **6**, slot item pasif maksimal **6**.
2. Jika slot senjata penuh → kartu senjata **baru** tidak lagi muncul, hanya upgrade senjata yang dimiliki.
3. Kartu upgrade senjata muncul hanya jika senjata belum Lv.MAX (Lv.5).
4. Sistem **weighted random** dengan bobot:

| Tipe Kartu | Bobot Dasar | Modifier |
| --- | --- | --- |
| Senjata baru | 30 | ×0 jika slot penuh |
| Upgrade senjata | 40 | ×1,5 jika senjata mendekati evolusi |
| Item pasif baru | 20 | ×0 jika slot penuh |
| Upgrade item pasif | 25 | — |
| Kartu HP instan (+30% HP) | 5 | ×3 jika HP < 40% |
| Kartu Koin (+150 coin) | 5 | — |
1. **Pity system:** jika 3 kali level up berturut-turut tidak muncul kartu untuk senjata yang bisa di-evolve, paksa munculkan pada level up ke-4.
2. **Reroll:** pemain punya 1 reroll gratis per run (bisa +2 dari gear). Reroll tambahan: tonton iklan (maks. 3×/run).
3. **Banish:** menghapus 1 kartu dari pool selamanya dalam run tersebut. 2 charge dari gear.
4. **Skip:** melewati level up dan mendapat +20 koin.
5. Kartu **Luck**: setiap 10% Luck menambah 1% peluang kartu berkualitas lebih tinggi (rarity kartu: Common → Rare → Epic dengan nilai upgrade lebih besar).

## 6.7 Sistem Spawn Musuh

**Spawn di luar layar**, pada elips dengan radius 1,3× dimensi kamera, sudut acak.

```
SpawnRate(t) = BaseRate × (1 + 0.09 × t_minutes)^1.35 × StageMultiplier
MaxAliveEnemies = min(320, 40 + 18 × t_minutes)   // hard cap untuk performa
```

| Aturan | Detail |
| --- | --- |
| Culling | Musuh yang berjarak > 2,2× layar dari pemain akan di-despawn & di-respawn di depan |
| Object pooling | Semua musuh, proyektil, damage number, dan VFX menggunakan pool |
| Formasi | Ada 8 pola spawn: Random Ring, Line Charge, Pincer (2 sisi), Circle Trap, Snake, Grid Wall, Corner Rush, Spiral |
| Wave scripted | Setiap 30 detik ada 1 wave khusus dengan pola tertentu (didefinisikan di ScriptableObject per stage) |
| Elite spawn | Menit 3, 6, 9, 12 (+random 1–2 tambahan) — ditandai aura merah & healthbar |
| Chest drop | Elite selalu drop chest; chest memberi 1–5 upgrade acak gratis |

## 6.8 Sistem Drop

| Item | Peluang Drop | Efek |
| --- | --- | --- |
| XP Gem | 100% dari semua musuh | Menambah XP |
| Koin | 25% (normal), 100% (elite) | Mata uang, dikonversi di akhir run |
| ❤️ Heal Pack | 1,2% | Memulihkan 30% Max HP |
| 🧲 Magnet | 0,8% | Menarik semua XP gem di layar |
| 💣 Bomb | 0,6% | Membunuh semua musuh normal di layar |
| ⏱️ Freeze | 0,4% | Membekukan semua musuh 3 detik |
| 🎁 Chest | 100% dari Elite | 1–5 upgrade acak (dipengaruhi Luck) |
| 🔧 Tech Part | 3% dari Elite/Boss | Material meta-progression |
| ⚙️ Gear | Lihat tabel drop stage | Equipment |

## 6.9 Kondisi Menang / Kalah

| Kondisi | Trigger | Hasil |
| --- | --- | --- |
| **Menang** | Membunuh Boss akhir sebelum timer 15:00 habis | Reward penuh (100%) + first-clear bonus |
| **Survive** | Bertahan sampai 15:00 tapi boss belum mati | Reward 70% |
| **Kalah** | HP = 0 dan tidak revive | Reward parsial berdasarkan waktu bertahan: `reward% = waktu_detik / 900 × 60%` |
| **Quit** | Pemain keluar manual | Reward 50% dari yang terkumpul, run tidak dihitung untuk mission |
| **Revive** | HP = 0 | Opsi: tonton iklan (1×/run gratis) atau bayar 60 gem (harga naik 2× tiap pakai). Revive memulihkan 50% HP + membunuh semua musuh di layar + i-frame 3 detik |

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
| W02 | **Baseball Bat** | Neutral | Melee arc | 20 | 1,4s | Forward | Ayunan busur 120° di depan |
| W03 | **Force Field** | Void | Aura persisten | 6/tick | 0,5s tick | Aura | Aura radius 2 unit di sekitar pemain |
| W04 | **Drone** | Lightning | Orbit + tembak | 9 | 0,8s | Orbit | 1 drone mengorbit & menembak musuh terdekat |
| W05 | **RPG Launcher** | Fire | Proyektil ledak | 45 | 2,6s | Nearest | Roket meledak radius 2,5 unit |
| W06 | **Lightning Emitter** | Lightning | Chain instan | 18 | 1,8s | Random | Petir menyambar 3 musuh berantai |
| W07 | **Molotov** | Fire | Ground DoT | 8/tick | 2,2s | Random | Lempar botol, area api 4 detik |
| W08 | **Frost Blade** | Ice | Melee slash | 16 | 1,1s | Nearest | Tebasan yang memperlambat 30% |
| W09 | **Toxic Grenade** | Poison | AoE DoT | 10/tick | 2,4s | Nearest | Awan racun radius 3, durasi 5 detik |
| W10 | **Shuriken Storm** | Neutral | Proyektil menyebar | 8 | 1,6s | Forward | 3 shuriken menyebar kipas 45° |
| W11 | **Laser Beam** | Void | Sinar kontinu | 14/tick | 3,0s (channel 1,5s) | Nearest | Sinar menembus semua musuh dalam garis |
| W12 | **Guardian Drone** | Neutral | Orbit shield | 25 | Pasif | Orbit | 2 bola mengorbit, damage kontak |
| W13 | **Boomerang** | Neutral | Proyektil balik | 22 | 1,9s | Forward | Terbang & kembali, kena 2× |
| W14 | **Ice Nova** | Ice | AoE burst | 30 | 3,2s | Aura | Ledakan es radius 3,5, freeze 1 detik |
| W15 | **Flame Thrower** | Fire | Cone kontinu | 7/tick | 0,2s tick | Forward | Kerucut api 90°, jarak 4 unit |
| W16 | **Landmine** | Fire | Trap | 60 | 3,5s | Drop | Jatuhkan ranjau, meledak saat diinjak |
| W17 | **Chain Whip** | Neutral | Melee sweep | 18 | 1,3s | Nearest | Cambuk memutar 360° radius 2,5 |
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

<aside>
📐

**Aturan skala umum antar level senjata:**

- Damage: **+18% s.d. +25%** per level (kumulatif ×2,1 dari Lv1 ke Lv5)
- Cooldown: **-8% s.d. -12%** pada level 3 dan 5 saja
- Jumlah proyektil / hit: +1 pada level 2 dan 4 (untuk senjata proyektil)
- Area: +15% pada level 3 dan 5 (untuk senjata AoE)
</aside>

---

# 8. Item Pasif (Passive Items)

Maksimal **6 item pasif** per run, masing-masing Lv.1–Lv.5.

| # | Item | Efek per Level | Total di Lv.5 | Fungsi Evolusi |
| --- | --- | --- | --- | --- |
| P01 | **Sharp Blade** | +10% ATK | +50% ATK | Kunai → **Shadow Blade** |
| P02 | **Whistle** | +8% Area | +40% Area | Baseball Bat → **Quantum Bat** |
| P03 | **Energy Cube** | +8% CDR | +40% CDR | Force Field → **Void Barrier** |
| P04 | **Circuit Board** | +1 Drone / 2 lvl | +2 Drone | Drone → **Swarm Legion** |
| P05 | **Ammo Pack** | +12% Proj. Damage | +60% | RPG → **Nuke Launcher** |
| P06 | **Capacitor** | +10% Lightning DMG | +50% | Lightning Emitter → **Thunder God** |
| P07 | **Oil Can** | +15% Burn duration | +75% | Molotov → **Inferno Field** |
| P08 | **Cryo Core** | +12% Slow effect | +60% | Frost Blade → **Absolute Zero** |
| P09 | **Bio Filter** | +15% Poison DMG | +75% | Toxic Grenade → **Plague Bloom** |
| P10 | **Running Shoes** | +6% Move Speed | +30% | Shuriken → **Sonic Storm** |
| P11 | **Magnet Core** | +25% Pickup Radius | +125% | — (utility) |
| P12 | **Lucky Coin** | +12% Luck | +60% | — (utility) |
| P13 | **Piggy Bank** | +20% Coin Gain | +100% | — (utility) |
| P14 | **Vitamin** | +12% Max HP | +60% | — (survival) |
| P15 | **Bandage** | +0,6 HP Regen/s | +3,0 HP/s | — (survival) |
| P16 | **Kevlar Vest** | +6 Armor | +30 Armor | — (survival) |
| P17 | **Reflex Chip** | +3% Dodge | +15% Dodge | — (survival) |
| P18 | **Hourglass** | +10% Duration | +50% | Void Orb → **Singularity** |
| P19 | **Scope** | +8% Crit Rate | +40% Crit Rate | Laser → **Death Ray** |
| P20 | **Adrenaline** | +10% Crit DMG | +50% Crit DMG | Boomerang → **Reaper's Return** |
| P21 | **Textbook** | +10% EXP Gain | +50% | — (utility) |
| P22 | **Spare Battery** | +1 Revive (maks 2) | +2 Revive | — (survival) |

---

# 9. Sistem Evolusi (Evolution)

## 9.1 Syarat Evolusi

<aside>
⚡

Sebuah senjata dapat berevolusi ketika **SEMUA** kondisi terpenuhi:

1. Senjata mencapai **Lv.5 (MAX)**
2. Item pasif pasangannya mencapai **Lv.3 atau lebih**
3. Pemain membuka **Chest dari Elite/Boss** setelah kedua syarat terpenuhi
4. Waktu run sudah melewati **menit ke-5** (mencegah evolusi terlalu dini)
</aside>

Saat evolusi terjadi: layar flash putih, slow-motion 0,8 detik, VFX khusus, SFX "EVOLVED!", dan senjata lama diganti oleh versi evolusi (slot tetap 1).

## 9.2 Tabel Evolusi Lengkap

| Senjata Dasar |   • Item Pasif | = Senjata Evolusi | Efek Evolusi |
| --- | --- | --- | --- |
| Kunai | Sharp Blade | **Shadow Blade** | 6 bilah bayangan menembus tanpa batas, homing, DMG 65 |
| Baseball Bat | Whistle | **Quantum Bat** | Ayunan 360°, knockback besar, menciptakan shockwave DMG 90 |
| Force Field | Energy Cube | **Void Barrier** | Aura radius 5, DMG 28/tick, memantulkan proyektil musuh |
| Drone | Circuit Board | **Swarm Legion** | 6 drone mengorbit, tembak rentetan, DMG 30 ×3 burst |
| RPG Launcher | Ammo Pack | **Nuke Launcher** | Ledakan radius 7, DMG 320, screen shake besar |
| Lightning Emitter | Capacitor | **Thunder God** | Petir menyambar 12 musuh, stun 0,5s, DMG 85 |
| Molotov | Oil Can | **Inferno Field** | Lautan api permanen mengikuti pemain, DMG 40/tick |
| Frost Blade | Cryo Core | **Absolute Zero** | Membekukan total 2 detik, DMG ×3 ke musuh beku |
| Toxic Grenade | Bio Filter | **Plague Bloom** | Racun menular antar musuh, DMG 45/tick |
| Shuriken Storm | Running Shoes | **Sonic Storm** | 12 shuriken memantul, kecepatan ×3, DMG 55 |
| Laser Beam | Scope | **Death Ray** | Sinar permanen berputar, DMG 70/tick, crit +100% |
| Boomerang | Adrenaline | **Reaper's Return** | 4 sabit terbang, lifesteal 3%, DMG 110 |
| Void Orb | Hourglass | **Singularity** | Black hole 8 detik, tarik kuat, DMG 60/tick, instakill musuh <10% HP |

## 9.3 Dual-Evolution (USP)

Beberapa senjata memiliki **jalur evolusi kedua**, ditentukan oleh item pasif alternatif:

| Senjata | Jalur A | Jalur B (alternatif) |
| --- | --- | --- |
| Kunai |   • Sharp Blade → **Shadow Blade** (single-target tinggi) |   • Lucky Coin → **Fortune Kunai** (drop koin ×3, DMG sedang) |
| RPG Launcher |   • Ammo Pack → **Nuke Launcher** (burst besar) |   • Energy Cube → **Barrage Cannon** (CD 0,6s, DMG 90 spam) |
| Force Field |   • Energy Cube → **Void Barrier** (defensif) |   • Sharp Blade → **Razor Aura** (radius kecil, DMG 65/tick) |
| Frost Blade |   • Cryo Core → **Absolute Zero** (kontrol) |   • Adrenaline → **Frostbite Edge** (crit ×4 ke musuh melambat) |

Jika kedua item pasangan aktif, pemain **diberi pilihan** lewat popup saat membuka chest.

## 9.4 Sistem Elemen & Reaksi (USP)

| Elemen | Status Effect | Durasi | Efek |
| --- | --- | --- | --- |
| 🔥 Fire | **Burn** | 3s | 5% ATK/detik DoT, stack maks 5 |
| ❄️ Ice | **Chill / Freeze** | 2s / 1s | -40% speed / diam total |
| ⚡ Lightning | **Shock** | 2s | +15% damage diterima, chain ke 2 musuh |
| ☠️ Poison | **Toxin** | 5s | 3% Max HP musuh/detik, menembus armor |
| 🌀 Void | **Fragile** | 4s | -30% armor musuh, tarik ke pusat |

**Reaksi Elemen (Combo):**

| Kombinasi | Nama Reaksi | Efek |
| --- | --- | --- |
| Fire + Ice | **Thermal Shock** | Ledakan DMG = 250% ATK, radius 3 |
| Ice + Lightning | **Superconduct** | Radius 4, -50% armor semua musuh, 6 detik |
| Fire + Lightning | **Overload** | Ledakan berantai, DMG 180% ATK ke 5 musuh |
| Poison + Fire | **Toxic Combustion** | Racun menyebar radius 5, DMG ×2 |
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
| Lv.1–10 | Character Shard ×10 per level | +2% ATK & +2% HP per level |
| Lv.11–20 | Shard ×25 + Coin | +3% per level, buka slot gear ke-5 |
| Lv.21–30 | Shard ×50 + Tech Part | +4% per level, buka Star Rating |
| ⭐ Star 1–6 | Duplikat karakter | Setiap bintang: +8% semua stat + 1 perk unik |

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

```
EnemyHP(t, chapter) = BaseHP × (1 + 0.16 × t_minutes)^1.42 × ChapterMultiplier
EnemyATK(t, chapter) = BaseATK × (1 + 0.11 × t_minutes)^1.25 × ChapterMultiplier
ChapterMultiplier = 1.0 × (1.35)^(chapter - 1)
```

## 11.3 Elite Enemy

| Properti | Nilai |
| --- | --- |
| HP | 12× musuh normal setara |
| Ukuran | 1,6× |
| Visual | Aura merah menyala + outline, healthbar di atas kepala |
| Reward | Chest (1–5 upgrade) + 1 gem biru + 3% Tech Part |
| Modifier acak | 1 dari: *Enraged* (+50% speed), *Armored* (+30 armor), *Regenerating* (+2%HP/s), *Explosive* (meledak saat mati), *Summoner* (spawn 5 swarmling tiap 3 detik) |

## 11.4 Boss — Struktur Umum

Setiap boss memiliki **3 fase** yang dipicu oleh threshold HP (100–66%, 66–33%, 33–0%). Setiap fase menambah 1 pola serangan baru.

### Daftar Boss (Launch: 10 Boss)

| # | Boss | Chapter | Tema | Pola Serangan Utama |
| --- | --- | --- | --- | --- |
| B01 | **Colossus** | 1 | Zombie raksasa | Ground slam (AoE lingkaran telegraf 1,2s) · Charge lurus · Summon 20 walker |
| B02 | **Hive Queen** | 2 | Serangga | Spawn swarm terus-menerus · Semburan asam kerucut · Terbang & jatuh |
| B03 | **Iron Warden** | 3 | Mech | Laser sweep 180° · Roket homing ×6 · Shield phase (harus hancurkan 4 node) |
| B04 | **Pyroclast** | 4 | Api | Lantai lava menyebar · Meteor telegraf · Ring api mengecil |
| B05 | **Glacier Titan** | 5 | Es | Ice spike dari tanah · Blizzard (-60% speed) · Membekukan area, harus terus bergerak |
| B06 | **The Butcher** | 6 | Slasher | Dash 3× beruntun · Bear trap · Enrage di 33% (speed ×2) |
| B07 | **Storm Caller** | 7 | Petir | Chain lightning menandai posisi · Tornado bergerak · EMP (nonaktifkan drone 3 detik) |
| B08 | **Plague Lord** | 8 | Racun | Kabut racun menyebar · Spawn Splitter · Zona aman mengecil |
| B09 | **Void Devourer** | 9 | Void | Black hole menarik pemain · Membalik kontrol 2 detik · Clone bayangan |
| B10 | **Omega Prime** | 10 | Final | Gabungan semua pola sebelumnya, 4 fase, HP 5× |

### Aturan Desain Boss

<aside>
⚔️

- Semua serangan boss **wajib memiliki telegraph visual** minimal **0,8 detik** sebelum eksekusi (indikator merah di tanah).
- Boss tidak boleh membunuh pemain dari full HP dengan 1 serangan (max 45% Max HP per hit).
- Selama fase boss, spawn musuh normal berhenti (kecuali boss yang memang summon).
- Boss punya **enrage timer 90 detik** — setelah itu ATK ×1,5 setiap 15 detik untuk mencegah stall.
- Boss immune terhadap Freeze & Stun total; hanya menerima **50% efek slow** dan **duration reduction** dari CC.
</aside>

---

# 12. Desain Stage & Chapter

## 12.1 Struktur Chapter

Setiap **Chapter** terdiri dari **10 Stage**. Stage 1–9 adalah stage normal (5 menit), Stage 10 adalah **Boss Stage** (15 menit).

| Chapter | Nama | Tema Visual | Musuh Utama | Boss | Power Rating Disarankan | Unlock |
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
| 9 | Zona anti-gravitasi | Speed ×1,5 tapi kontrol sulit |
| 10 | Laser grid bergerak | DMG 30% Max HP |

## 12.3 Tipe Stage Tambahan

| Tipe Stage | Durasi | Tujuan | Reward Khusus |
| --- | --- | --- | --- |
| **Normal Stage** | 5 menit | Bertahan sampai timer habis | Coin, EXP, Gear drop |
| **Boss Stage** | 15 menit | Kalahkan boss | Gear rarity tinggi, Tech Part |
| **Elite Rush** | 3 menit | Bunuh 15 elite | Chest Key, Tech Part ×5 |
| **Endless Mode** | ∞ | Bertahan selama mungkin | Leaderboard, Coin scaling |
| **Daily Dungeon** | 4 menit | Rotasi harian per material | Material upgrade spesifik |
| **Weekly Raid Boss** | 6 menit | Damage race (shared HP guild) | Guild Coin, Gear Legendary |
| **Time Attack** | Sampai boss mati | Bunuh boss secepat mungkin | Leaderboard mingguan, Gem |
| **Nightmare Mode** | 15 menit | Versi ×3 sulit dari boss stage | Gear S-tier, Title eksklusif |
| **Event Stage** | Variabel | Mekanik unik per event | Event Token |

## 12.4 Sistem Difficulty Tier

Setelah menyelesaikan Chapter 10, terbuka **Difficulty Tier**:

| Tier | Nama | Enemy HP | Enemy ATK | Reward Multiplier | Modifier Khusus |
| --- | --- | --- | --- | --- | --- |
| I | Normal | ×1,0 | ×1,0 | ×1,0 | — |
| II | Hard | ×2,5 | ×1,6 | ×1,8 | Elite +50% |
| III | Nightmare | ×6,0 | ×2,4 | ×3,2 | Musuh punya 1 modifier acak |
| IV | Hell | ×15,0 | ×3,5 | ×5,5 | Tidak ada Heal Pack drop |
| V | Apocalypse | ×40,0 | ×5,0 | ×9,0 | Revive dinonaktifkan |
| VI | Void | ×110,0 | ×7,0 | ×15,0 | Timer 12 menit saja |

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

| Rarity | Warna | Jumlah Sub-Stat | Level Max | Multiplier Stat Utama | Drop Rate (Ch.5) |
| --- | --- | --- | --- | --- | --- |
| Common (C) | Abu-abu | 0 | 20 | ×1,0 | 55% |
| Uncommon (B) | Hijau | 1 | 30 | ×1,4 | 25% |
| Rare (A) | Biru | 2 | 40 | ×2,0 | 13% |
| Epic (S) | Ungu | 3 | 60 | ×3,2 | 5,5% |
| Legendary (SS) | Emas | 4 | 80 | ×5,5 | 1,3% |
| Mythic (SSS) | Merah | 4 + 1 Set Effect | 100 | ×9,0 | 0,2% |

### Mekanik Gear

<aside>
⚙️

**Operasi yang bisa dilakukan pada gear:**

1. **Enhance (Upgrade Level)** — biaya Coin + Enhance Stone. Setiap 5 level = milestone bonus tambahan.
2. **Ascend (Naik Rarity)** — gabungkan 3 gear rarity sama untuk naik 1 tingkat. Stat utama dipertahankan.
3. **Reforge (Re-roll Sub-Stat)** — mengacak ulang sub-stat, biaya Reforge Dust. Bisa mengunci 1 sub-stat (biaya 2×).
4. **Awaken** — pada Lv.Max, buka efek pasif unik (contoh: "Setiap 10 detik, senjata berikutnya crit pasti").
5. **Dismantle** — hancurkan gear jadi material (60% nilai kembali).
6. **Lock** — kunci gear agar tidak terkena auto-dismantle.
7. **Set Effect** — memakai 2/4/6 gear dari set yang sama memberi bonus bertingkat.
</aside>

### Set Effect (6 Set di Launch)

| Set | Bonus 2 Piece | Bonus 4 Piece | Bonus 6 Piece |
| --- | --- | --- | --- |
| **Berserker** | ATK +12% | ATK +25%, Crit Rate +8% | Di bawah 50% HP, ATK +60% |
| **Guardian** | Max HP +15% | Armor +40, DR +10% | Sekali per run: bertahan dari damage fatal dengan 1 HP |
| **Swiftness** | Move Speed +10% | CDR +15% | Setiap 5 detik bergerak, serangan berikutnya ×2 |
| **Elementalist** | Elemental DMG +15% | Semua status effect +50% durasi | Reaksi elemen memicu 2× |
| **Fortune** | Luck +20% | Coin Gain +50% | Chest memberi 2 upgrade tambahan |
| **Void Walker** | Void DMG +20% | Dodge +12% | Setelah dodge, i-frame +1 detik dan ATK +30% (3 detik) |

## 13.2 Tech Parts (Sistem Modul)

Sistem sekunder yang memodifikasi **senjata dalam run**.

| Tech Part | Efek | Material Upgrade |
| --- | --- | --- |
| **Overclock Chip** | Semua senjata mulai run di Lv.2 | Circuit Fragment ×30 |
| **Ammo Expander** | Amount +1 untuk semua senjata proyektil | Ammo Core ×25 |
| **Evolution Catalyst** | Syarat evolusi turun: item pasif cukup Lv.2 | Catalyst Shard ×50 |
| **Reroll Module** | Reroll gratis +2 per run | Data Chip ×20 |
| **Banish Module** | Banish charge +2 per run | Data Chip ×20 |
| **Magnet Core** | Pickup Radius +60% | Magnet Ore ×15 |
| **Starter Kit** | Mulai run dengan 1 item pasif acak Lv.2 | Kit Fragment ×35 |
| **Revive Cell** | +1 auto-revive per run | Bio Cell ×60 |
| **Greed Engine** | Coin Gain +80% | Gold Fragment ×40 |
| **Boss Slayer** | Damage ke boss +25% | Slayer Mark ×45 |

Setiap Tech Part punya **Lv.1–Lv.10**; efeknya meningkat linear per level.

## 13.3 Skill Tree (Talent Board)

Skill tree global (berlaku untuk semua karakter), terbuka setelah Chapter 3. Mata uang: **Talent Point** (didapat dari Account Level).

| Cabang | Fokus | Contoh Node |
| --- | --- | --- |
| 🔴 **Offense** (18 node) | Damage | ATK +2%/node · Crit Rate +1%/node · Boss DMG +3%/node · Elemental DMG +2%/node |
| 🔵 **Defense** (15 node) | Bertahan | Max HP +2%/node · Armor +3/node · Dodge +0,5%/node · HP Regen +0,2/s |
| 🟢 **Utility** (14 node) | Kualitas hidup | Pickup Radius +5% · Luck +2% · EXP Gain +2% · Reroll +1 (node akhir) |
| 🟡 **Economy** (12 node) | Resource | Coin Gain +3%/node · Gear drop rate +1%/node · Tech Part drop +0,5%/node |
| 🟣 **Mastery** (10 node, endgame) | Spesialisasi | Pilih 1 elemen untuk +25% damage · Weapon slot ke-7 (node terakhir, butuh 80 TP) |

Aturan: node harus dibuka berurutan; ada **keystone node** di setiap 5 node yang memberi efek besar. Respec biaya 500 Gem (gratis 1× per season).

## 13.4 Collectibles (Koleksi)

Sistem koleksi pasif yang memberi bonus permanen.

| Kategori | Jumlah Item | Bonus Set Lengkap |
| --- | --- | --- |
| Kartu Senjata | 33 (20 dasar + 13 evolusi) | ATK +10% |
| Kartu Musuh | 12 arketipe + 10 boss | HP +10% |
| Kartu Chapter | 10 | Coin Gain +15% |
| Kartu Karakter | 8 | Semua stat +5% |
| Kartu Artefak (event) | 20 | Luck +25% |

Kartu didapat dari: membunuh musuh (auto-unlock), chest, event, dan gacha. Kartu duplikat menaikkan level kartu (Lv.1–5) yang menambah bonus individual kecil.

## 13.5 Pet System (Companion)

| Pet | Rarity | Efek Pasif | Skill Aktif (auto) |
| --- | --- | --- | --- |
| 🐕 **Buddy** | Common | Pickup Radius +20% | Menyalak: knockback musuh sekitar (CD 15s) |
| 🦅 **Falcon** | Rare | Crit Rate +5% | Menukik: DMG 200% ATK ke 5 musuh (CD 12s) |
| 🐺 **Fenrir** | Epic | ATK +12% | Auman: musuh sekitar ketakutan 2 detik (CD 20s) |
| 🐉 **Draco** | Legendary | Fire DMG +25%, HP +10% | Napas api kerucut, DMG 400% ATK (CD 18s) |
| 👻 **Wisp** | Legendary | HP Regen +1,5/s, Luck +15% | Menyembuhkan 25% Max HP (CD 45s) |

Pet punya level (1–50) dan makanan (Pet Food) sebagai material. Pet aktif hanya 1 per run.

## 13.6 Account Level & Power Rating

```jsx
// Power Rating dihitung dari semua sumber meta-progression
PowerRating = (TotalGearScore × 1.0)
            + (CharacterLevel × 45)
            + (TalentPointsSpent × 30)
            + (TechPartLevelSum × 25)
            + (CollectibleBonus × 15)
            + (PetLevel × 20)

GearScore(item) = (MainStatValue × RarityMultiplier)
                + Σ(SubStatValue × 0.6)
                + (EnhanceLevel × 12)
```

Power Rating ditampilkan di lobby dan dipakai untuk:

- Rekomendasi stage ("Power kamu terlalu rendah untuk stage ini")
- Matchmaking leaderboard bracket
- Syarat masuk event tertentu

---

# 14. Ekonomi & Mata Uang

## 14.1 Daftar Mata Uang

| Ikon | Mata Uang | Tipe | Sumber Utama | Kegunaan | Cap |
| --- | --- | --- | --- | --- | --- |
| 🪙 | **Coin** | Soft | Semua run, mission | Enhance gear, reforge | Tidak ada |
| 💎 | **Gem** | Hard | IAP, achievement, event, BP | Beli gacha, revive, energy-free boost, karakter | Tidak ada |
| 🎟️ | **Summon Ticket** | Premium | Event, BP, shop | Gacha gear/karakter | Tidak ada |
| 🔑 | **Chest Key** | Sekunder | Elite Rush, daily | Membuka chest di lobby | 99 |
| 🔧 | **Tech Part** | Material | Elite/Boss drop | Upgrade Tech Parts | Tidak ada |
| 🪨 | **Enhance Stone** | Material | Daily Dungeon | Naikkan level gear | Tidak ada |
| ✨ | **Reforge Dust** | Material | Dismantle gear | Re-roll sub-stat | Tidak ada |
| 🧩 | **Character Shard** | Material | Gacha, event, shop | Naikkan level/bintang karakter | Tidak ada |
| 🏅 | **Guild Coin** | Sosial | Guild raid, donasi | Guild shop | Tidak ada |
| 🎗️ | **Event Token** | Temporer | Event stage | Event shop (hangus saat event berakhir) | Tidak ada |
| 🧠 | **Talent Point** | Progresi | Account Level up | Skill tree | Tidak ada |

## 14.2 Faucet & Sink (Keseimbangan Ekonomi)

| Mata Uang | Faucet (Masuk / hari, pemain aktif) | Sink (Keluar) | Target Rasio |
| --- | --- | --- | --- |
| Coin | ±180.000 | Enhance gear (±150.000), Reforge (±40.000) | Sink/Faucet ≈ 1,05 (sedikit defisit) |
| Gem | ±180 (F2P) | Gacha (300/pull), revive (60) | Defisit terkontrol untuk mendorong IAP |
| Enhance Stone | ±400 | Enhance (±380) | ≈ 0,95 |
| Tech Part | ±25 | Upgrade Tech (±30) | Defisit → mendorong farming |

<aside>
⚠️

**Prinsip ekonomi:** pemain F2P harus bisa menyelesaikan **Chapter 10 dalam ±35 hari** tanpa membayar. Pemain berbayar bisa mempercepat menjadi ±10 hari, tapi **tidak boleh melewati konten yang belum di-unlock** (no pay-to-skip content, hanya pay-to-accelerate).

</aside>

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
| P001 | Starter Pack | 15.000 | 500 Gem + Gear A + 10.000 Coin | 1× seumur akun |
| P002 | Gem Kecil | 15.000 | 300 Gem (+60 bonus pertama) | — |
| P003 | Gem Sedang | 79.000 | 1.700 Gem | — |
| P004 | Gem Besar | 159.000 | 3.600 Gem | — |
| P005 | Gem Jumbo | 399.000 | 9.800 Gem | — |
| P006 | Gem Mega | 799.000 | 21.000 Gem | — |
| P007 | Monthly Card | 45.000 | 300 Gem instan + 100 Gem/hari (30 hari) + 2 reroll gratis | 1 aktif |
| P008 | Growth Fund | 129.000 | Reward bertahap saat mencapai Chapter 3/5/7/10 (total 4.500 Gem) | 1× |
| P009 | Battle Pass Premium | 89.000 | 60 tier reward + karakter eksklusif | Per season |
| P010 | Battle Pass Elite | 199.000 | Premium + skip 15 tier + skin | Per season |
| P011 | Weekly Deal | 29.000 | Rotasi mingguan (material paket) | 1×/minggu |
| P012 | Event Bundle | 99.000–499.000 | Gear event + token | Per event |
| P013 | Remove Ads | 69.000 | Hilangkan iklan paksa (rewarded tetap ada) | 1× |

## 15.3 Iklan (Ads)

| Penempatan | Tipe | Frekuensi | Reward |
| --- | --- | --- | --- |
| Revive dalam run | Rewarded | 1× per run | Revive gratis |
| Reroll kartu | Rewarded | Maks 3× per run | Reroll ekstra |
| Double reward akhir run | Rewarded | 1× per run | Reward ×2 |
| Free chest lobby | Rewarded | 3× per hari | Chest gratis |
| Coin doubler daily | Rewarded | 1× per hari | Coin ×2 selama 30 menit |
| Gem gratis | Rewarded | 5× per hari | 10 Gem |
| Interstitial | Full-screen | Maks 1 per 3 run, tidak pernah di 3 hari pertama | — |

<aside>
🧭

**Aturan iklan:**

- Tidak ada iklan sama sekali dalam **3 hari pertama** pemain baru (menjaga D1/D3 retention).
- Interstitial tidak pernah muncul saat pemain **kalah** (menghindari frustrasi ganda).
- Semua rewarded ads bersifat **opsional** dan selalu memberi nilai jelas.
- Ad mediation: AppLovin MAX dengan waterfall (Google AdMob, Unity Ads, ironSource, Meta).
</aside>

## 15.4 Battle Pass

| Aspek | Spesifikasi |
| --- | --- |
| Durasi season | 35 hari |
| Jumlah tier | 60 |
| Sumber XP BP | Daily mission (100 XP), weekly mission (500 XP), stage clear (10 XP), event |
| XP per tier | 1.000 (tetap) |
| Estimasi waktu selesai F2P | ±28 hari main normal (45 menit/hari) |
| Jalur gratis | Coin, Enhance Stone, 1 Gear A, 300 Gem total |
| Jalur premium | 2.500 Gem, Gear SS, 1 karakter eksklusif, skin, Pet Food, 5.000 Enhance Stone |
| Tier setelah 60 | Infinite tier: setiap 2.000 XP = 50 Gem |

---

# 16. Live Ops & Event

## 16.1 Kalender Event (Siklus 4 Minggu)

| Minggu | Event Utama | Event Sampingan | Durasi |
| --- | --- | --- | --- |
| 1 | **Boss Rush Festival** — semua boss berturut-turut | Login 7 hari | 7 hari |
| 2 | **Gear Fever** — drop rate gear ×2 | Guild Raid | 5 hari |
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
| **Puzzle Event** | Stage dengan modifier aneh (mis. tanpa senjata utama) | Gem, Tech Part |
| **Guild War** | Guild vs Guild, total skor anggota | Guild Coin, ranking |

## 16.3 Daily & Weekly Mission

| Misi Harian | Reward |
| --- | --- |
| Selesaikan 3 stage | 50 Gem + 100 BP XP |
| Bunuh 1.000 musuh | 10.000 Coin |
| Kalahkan 1 elite | 5 Enhance Stone |
| Tonton 1 iklan | 20 Gem |
| Enhance gear 1× | 100 BP XP |
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
| Guild Level | 1–20, naik dari kontribusi anggota. Tiap level: buff pasif untuk semua anggota (mis. +1% ATK/level) |
| Guild Raid | Boss mingguan dengan HP bersama, semua anggota menyerang. Reward berdasarkan kontribusi + total |
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

- **Friend List** — maks 50 teman, kirim/terima 5 stamina-free gift/hari (berupa Coin).
- **Profile Card** — avatar, frame, title, Power Rating, karakter favorit, statistik seumur hidup.
- **Replay Sharing** — rekam 30 detik terakhir run terbaik, bagikan sebagai video (pendorong UGC/marketing).
- **Build Sharing** — bagikan kode build (loadout + gear + talent) lewat kode 8 karakter.

---

# 18. Onboarding & FTUE (First-Time User Experience)

## 18.1 Menit-per-Menit 10 Menit Pertama

| Waktu | Yang Terjadi | Tujuan Desain |
| --- | --- | --- |
| 0:00–0:15 | Splash + logo, langsung masuk gameplay (**tanpa login wall**) | Time-to-first-action < 15 detik |
| 0:15–0:45 | Cinematic singkat 8 detik (bisa di-skip) + tutorial gerak: "Geser untuk bergerak" | Ajarkan 1 kontrol saja |
| 0:45–1:30 | Musuh mudah datang, pemain melihat auto-attack bekerja | "Oh, saya tidak perlu menekan tombol serang" |
| 1:30–2:00 | Level up pertama — highlight kartu dengan glow + tangan pointer | Ajarkan sistem draft |
| 2:00–3:00 | Musuh lebih banyak, level up 3–4× | Rasakan power growth |
| 3:00–3:30 | Elite pertama muncul + chest | Ajarkan chest |
| 3:30–4:30 | Mini-boss scripted (mudah, pasti menang) | Beri kemenangan awal |
| 4:30–5:00 | Reward screen dengan animasi mewah, dapat gear pertama | Dopamine hit |
| 5:00–6:00 | Tutorial lobby: equip gear (dipandu) | Ajarkan meta-progression |
| 6:00–6:30 | Tutorial enhance gear (gratis 1×) | Ajarkan upgrade |
| 6:30–7:00 | Klaim daily reward + starter pack ditampilkan (**tanpa hard sell**) | Perkenalkan ekonomi |
| 7:00–10:00 | Stage 2 & 3, level up sampai buka evolusi pertama (scripted) | **Momen "WOW"** — evolusi pertama harus terjadi di 10 menit pertama |

<aside>
🎯

**Aturan FTUE:**

- **Tidak ada** pop-up IAP dalam 10 menit pertama.
- **Tidak ada** iklan dalam 3 hari pertama.
- **Tidak ada** login/registrasi wajib — guest account otomatis, prompt link akun di hari ke-3.
- Pemain **tidak boleh kalah** di 3 stage pertama (HP diberi "safety floor" tersembunyi).
- Setiap fitur baru diperkenalkan **satu per satu**, tidak sekaligus. Gunakan gating: Gear (Ch.1), Tech Part (Ch.2), Skill Tree (Ch.3), Guild (Ch.4), Pet (Ch.5), Event (Ch.3).
</aside>

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

```jsx
[Splash] → [Loading] → [LOBBY / MAIN MENU]
                            │
   ┌────────────┬───────────┼───────────┬────────────┬──────────┐
   │            │           │           │            │          │
[Character] [Equipment]  [PLAY]     [Shop]      [Guild]    [Event]
   │            │           │           │            │          │
   │       ┌────┴────┐  [Chapter    ┌───┴───┐   [Raid]   [Event Stage]
   │       │         │   Select]    │       │   [Chat]   [Event Shop]
[Level] [Gear    [Tech      │    [Gacha] [IAP]  [Ranking]
[Star]   List]   Parts]     │    [Daily] [Pass]
[Skin]  [Enhance][Skill     │
         Reforge] Tree]     │
                            ↓
                    [Loadout Select]
                            ↓
                      [IN-GAME HUD]
                            ↓
                    [Result Screen]
                            ↓
                        [LOBBY]
```

## 19.2 In-Game HUD (Layout Portrait)

| Posisi | Elemen | Catatan |
| --- | --- | --- |
| Atas-tengah | Timer (MM:SS) besar | Font tebal, selalu terbaca |
| Atas-tengah bawah timer | XP Bar + Level | Bar penuh lebar layar, tipis |
| Atas-kiri | Kill count + Coin count | Ikon kecil |
| Atas-kanan | Tombol Pause ☰ | Hit area minimal 48×48 dp |
| Di bawah HUD atas | Ikon senjata & item aktif (2 baris × 6) | Dengan indikator level |
| Tengah | Area gameplay | 70% layar |
| Bawah-kiri | Area joystick (invisible sampai disentuh) | Zona 40% kiri bawah |
| Bawah-kanan | Tombol skill aktif (maks 2) dengan radial cooldown | Zona 30% kanan bawah |
| Atas boss | Boss health bar + nama + indikator fase | Muncul hanya saat boss aktif |
| Tepi layar | Indikator panah untuk musuh/item di luar layar | Warna sesuai tipe |

## 19.3 Prinsip UX yang Wajib

<aside>
📱

1. **Thumb-zone friendly** — semua tombol yang sering ditekan berada di 1/3 bawah layar.
2. **Safe area** — hormati notch, punch-hole, dan gesture bar (padding minimal 24 dp).
3. **Hit area minimal 48×48 dp** untuk semua tombol interaktif.
4. **Maksimal 3 tap** dari lobby ke mulai bermain.
5. **Feedback instan** — setiap tap harus punya respons visual < 100 ms.
6. **Red dot notification** pada semua menu yang punya reward belum diklaim.
7. **One-tap upgrade** — tombol "Auto Equip Best" dan "Enhance Max" untuk mengurangi friksi.
8. **Damage number** bisa dimatikan di settings (untuk device rendah & preferensi).
9. **Loading screen** maksimal 4 detik; tampilkan tips gameplay.
10. **Tidak ada teks kecil** — minimum font size 12 sp.
</aside>

## 19.4 Daftar Layar Lengkap (Screen Inventory)

| # | Layar | Elemen Utama |
| --- | --- | --- |
| S01 | Splash / Logo | Logo, versi, progress bar |
| S02 | Loading | Tips, art, progress |
| S03 | Lobby | Karakter 3D/2D, Power Rating, tombol Play, nav bar, red dots |
| S04 | Chapter Select | Peta chapter, stage node, bintang, rekomendasi power |
| S05 | Loadout Select | Pilih karakter, 2 senjata starter, pet, tampilan stat |
| S06 | In-Game HUD | (lihat 19.2) |
| S07 | Level-Up Card | 3 kartu, tombol reroll/banish/skip |
| S08 | Pause Menu | Stat detail run, settings, resume, quit |
| S09 | Death / Revive | Countdown 10 detik, tombol iklan/gem, quit |
| S10 | Result Screen | Reward animasi, statistik run, tombol double reward, share |
| S11 | Character List | Grid karakter, filter, indikator shard |
| S12 | Character Detail | Stat, skill, level up, star up, skin |
| S13 | Equipment | 6 slot, inventory list, filter, sort, auto-equip |
| S14 | Gear Detail | Stat utama & sub, enhance, ascend, reforge, awaken, lock |
| S15 | Tech Parts | Grid modul, level, material |
| S16 | Skill Tree | Board node interaktif, zoom/pan, talent point counter |
| S17 | Collection | Galeri kartu, progress set |
| S18 | Pet | List pet, level, feed, pilih aktif |
| S19 | Shop — Gacha | Banner, rate display, pull 1×/10×, pity counter |
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
| S32 | First-time Tutorial Overlay | Highlight + pointer + teks |

---

# 20. Art Direction & Audio

## 20.1 Arahan Visual

| Aspek | Spesifikasi |
| --- | --- |
| Gaya | 2D stylized semi-realistis, outline tipis, warna saturasi tinggi (mudah dibaca di layar kecil) |
| Perspektif | Top-down 3/4 view (seperti isometrik ringan) |
| Palet | Latar gelap (nilai kecerahan rendah) agar musuh & efek menonjol |
| Kontras | Pemain selalu paling terang di layar; musuh outline merah tipis |
| Resolusi sprite | Karakter 128×128 px, musuh normal 96×96 px, boss 512×512 px |
| Animasi | Skeletal (Spine 2D / Unity 2D Animation), 12–24 fps |
| Frame animasi minimal | Idle (4), Walk (8), Attack (6), Hurt (2), Death (8) |
| Tileset | Modular 256×256 px, seamless tiling untuk map infinite |
| VFX | Particle system + shader; harus punya versi "Low" untuk device rendah |
| UI Style | Flat modern + neon accent, rounded corner 12 dp, glassmorphism ringan |

## 20.2 Aturan Readability (Sangat Kritis)

<aside>
👁️

Karena layar bisa dipenuhi 300+ musuh dan ratusan efek, aturan berikut **wajib**:

1. VFX pemain berwarna **terang & cerah** (cyan, putih, kuning); VFX musuh berwarna **merah/ungu gelap**.
2. Semua telegraph serangan musuh berwarna **merah dengan alpha 40%**, tidak pernah warna lain.
3. Batas maksimal partikel di layar: **1.500** (Low: 400, Medium: 800, High: 1.500).
4. Damage number di-**pool** dan digabung (damage merge) jika > 30 angka/detik.
5. Karakter pemain memiliki **outline putih permanen** agar tidak pernah hilang di kerumunan.
6. Ada opsi settings: "Kurangi Efek Visual" dan "Sembunyikan Damage Number".
7. Musuh yang berbahaya (Bomber, Sniper) punya **indikator ikon di atas kepala**.
</aside>

## 20.3 Audio

| Kategori | Spesifikasi |
| --- | --- |
| Musik lobby | 1 track loop, tenang, 2–3 menit |
| Musik stage | 1 track per chapter (10 track), tempo naik seiring waktu (layered music: bass → drum → lead) |
| Musik boss | 1 track per boss tier (3 track), intens |
| SFX senjata | Unik per senjata (20+ SFX), dengan variasi pitch ±10% agar tidak monoton |
| SFX musuh | Spawn, hurt, death per arketipe |
| SFX UI | Tap, confirm, cancel, error, level up, evolve, chest open, reward |
| SFX prioritas | Sistem voice limiting: maks 24 SFX bersamaan, prioritas: UI > Boss > Player > Enemy |
| Format | OGG Vorbis (Android), AAC (iOS), 44,1 kHz |
| Ukuran total audio | Target < 45 MB |
| Settings | Slider terpisah untuk BGM, SFX, Haptic. Auto-mute saat app di background |

---

# 21. Kebutuhan Teknis (Technical Requirements)

## 21.1 Stack Teknologi

| Layer | Teknologi | Alasan |
| --- | --- | --- |
| Engine | **Unity 2022.3 LTS**, URP 2D | Ekosistem mobile terbaik, DOTS opsional untuk crowd |
| Bahasa | C# | Standar Unity |
| Crowd simulation | **Unity DOTS / ECS + Burst + Jobs** untuk sistem musuh | Wajib untuk 300+ musuh @60 fps |
| Rendering | GPU Instancing + Sprite Atlas + SRP Batcher | Kurangi draw call |
| UI | UI Toolkit (menu) + uGUI (HUD in-game) | Performa & fleksibilitas |
| Animasi | Spine 2D atau Unity 2D Animation | Skeletal ringan |
| Backend | **PlayFab** atau **Firebase + Cloud Functions** | Save cloud, leaderboard, remote config |
| Database | Firestore / PlayFab Entities | Data pemain |
| Analytics | Firebase Analytics + GameAnalytics + Adjust (attribution) | Funnel & UA |
| Crash | Firebase Crashlytics | Stabilitas |
| Remote Config | Firebase Remote Config / PlayFab Title Data | Live balancing tanpa update |
| A/B Testing | Firebase A/B Testing | Optimasi monetisasi & retensi |
| Ads | AppLovin MAX (mediation) | eCPM optimal |
| IAP | Unity IAP + server-side receipt validation | Anti-fraud |
| Push | Firebase Cloud Messaging | Retensi |
| CI/CD | GitHub Actions + Unity Cloud Build + Fastlane | Otomasi build |
| Version Control | Git + Git LFS (atau Perforce jika asset besar) | Kolaborasi |
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
| Ukuran total setelah download asset | < 600 MB | — | — |
| Konsumsi baterai | < 12%/jam | < 10%/jam | < 8%/jam |
| Suhu device | Tidak melebihi 42°C setelah 30 menit | — | — |

## 21.3 Optimasi Wajib

<aside>
⚡

1. **Object pooling** untuk SEMUA: musuh, proyektil, VFX, damage number, XP gem, koin, audio source.
2. **Spatial hashing / grid partitioning** untuk collision detection (bukan Physics2D bawaan untuk musuh).
3. **Sprite atlas** per kategori, maks 2048×2048.
4. **Texture compression:** ASTC 6×6 (Android & iOS), fallback ETC2.
5. **Zero GC allocation** di Update loop — wajib diverifikasi dengan Unity Profiler.
6. **LOD musuh:** musuh jauh dari kamera memakai animasi frame-rate rendah (6 fps).
7. **Batching damage number** — gabungkan damage yang terjadi dalam window 0,2 detik.
8. **Fixed timestep 0,02s** untuk simulasi gameplay agar deterministik.
9. **Addressables** — chapter asset di-download on-demand.
10. **Async scene loading** dengan progress bar nyata.
11. **Audio streaming** untuk BGM, **decompress on load** untuk SFX pendek.
12. **Adaptive quality** — deteksi FPS drop > 3 detik, otomatis turunkan kualitas partikel.
</aside>

## 21.4 Arsitektur Kode (Struktur Modul)

```jsx
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
| **ScriptableObject-driven data** | Semua data senjata, musuh, gear, stage → memudahkan balancing tanpa coding |
| **Event Bus** | Komunikasi antar sistem tanpa coupling (mis. `OnEnemyKilled`, `OnLevelUp`) |
| **Service Locator / DI (VContainer)** | Akses service global |
| **State Machine** | Boss phase, game state, UI screen state |
| **Command Pattern** | Aksi meta (enhance, purchase) agar bisa di-queue offline |
| **ECS/DOTS** | Hanya untuk sistem musuh & proyektil (bagian paling berat) |

---

# 22. Backend, Save Data & Anti-Cheat

## 22.1 Save Data

| Aspek | Spesifikasi |
| --- | --- |
| Local save | JSON ter-enkripsi (AES-256), disimpan di `Application.persistentDataPath` |
| Auto-save | Setiap akhir run, setiap transaksi, dan setiap 60 detik di lobby |
| Cloud save | Sinkron saat login, saat app pause, dan setiap 5 menit online |
| Konflik | Bandingkan `lastModified`  • `totalPowerRating`; tampilkan dialog pilihan ke pemain |
| Offline mode | Semua gameplay bisa offline; transaksi di-queue dan dikirim saat online |
| Backup | Server menyimpan 3 snapshot terakhir (rollback support untuk CS) |
| Account link | Guest → Google Play Games / Apple Sign-In / Email |

### Struktur Data Pemain (Player Profile Schema)

```jsx
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
  "missions": { "dailyResetAt": "ISO8601", "daily": [...], "weekly": [...] },
  "guildId": "gld_1234",
  "settings": { "bgm": 0.7, "sfx": 1.0, "haptic": true, "lang": "id", "quality": "medium" },
  "flags": { "tutorialCompleted": true, "starterPackBought": false },
  "checksum": "hmac-sha256"
}
```

## 22.2 Anti-Cheat

| Ancaman | Mitigasi |
| --- | --- |
| Memory editing (GameGuardian) | Enkripsi nilai penting di memori (`ObscuredInt` / Anti-Cheat Toolkit), checksum berkala |
| Speed hack | Deteksi anomali `Time.deltaTime` vs `Stopwatch` sistem; validasi durasi run di server |
| Save file editing | HMAC-SHA256 checksum pada save; server-side validation saat sync |
| IAP fraud | Server-side receipt validation (Google Play Developer API & App Store Server API) |
| Reward injection | Semua reward penting divalidasi server; run result dikirim dengan telemetry (kill count, damage, waktu) untuk sanity check |
| Leaderboard cheating | Server memvalidasi: skor vs Power Rating vs durasi vs kill count. Outlier ditandai untuk review manual |
| APK modding | Integritas: Play Integrity API (Android), DeviceCheck (iOS); obfuscation dengan Obfuscator Pro / IL2CPP |
| Emulator farming | Deteksi emulator, batasi reward event untuk akun mencurigakan |

### Aturan Validasi Run Result

```jsx
// Server menolak hasil run jika:
killCount > (durationSeconds × MaxKillRatePerSecond × 1.3)
|| coinsEarned > (killCount × MaxCoinPerKill × 1.2)
|| durationSeconds < MinPossibleDuration(stageId)
|| playerLevel > MaxTheoreticalLevel(durationSeconds)
|| damageDealt > (powerRating × DamageCoefficient × durationSeconds)
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

<aside>
🔐

**Aturan keamanan:** Gacha, reward, dan semua transaksi currency **harus server-authoritative**. Client tidak pernah menentukan hasil gacha atau jumlah reward.

</aside>

---

# 23. Analytics & KPI

## 23.1 Event yang Wajib Di-track

| Kategori | Event | Parameter |
| --- | --- | --- |
| Lifecycle | `app_open`, `session_start`, `session_end` | duration, source |
| FTUE | `tutorial_step` | step_id, time_since_install, completed |
| Gameplay | `run_start` | stage_id, character_id, power_rating, loadout |
| Gameplay | `run_end` | result (win/lose/quit), duration, level_reached, kills, damage, coins |
| Gameplay | `level_up_choice` | level, options_shown, option_chosen, rerolled, banished |
| Gameplay | `weapon_evolved` | weapon_id, evolution_id, time_in_run |
| Gameplay | `player_death` | stage_id, time, cause (enemy_type), hp_at_last_10s |
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
| Engagement | DAU, MAU, DAU/MAU (stickiness), Session/DAU, Session length, Playtime/DAU |
| Progresi | Avg. chapter at D1/D7/D30, Funnel drop-off per stage, Avg. Power Rating per hari |
| Monetisasi | ARPDAU, ARPPU, ARPU, LTV (D7/D30/D180), Conversion rate, Repeat purchase rate |
| Iklan | Ad impressions/DAU, eCPM, Ad ARPDAU, Fill rate |
| Ekonomi | Currency inflow/outflow, Balance median per segment, Sink efficiency |
| Kualitas | Crash-free rate, ANR rate, Avg FPS per device tier, Load time p95 |
| Sosial | Guild join rate, Guild retention lift, Leaderboard participation |

## 23.3 Funnel Kritis yang Dimonitor

```jsx
Install → Tutorial Start (target 95%)
  → Tutorial Complete (target 85%)
  → First Run Complete (target 80%)
  → First Gear Equipped (target 75%)
  → Chapter 1 Cleared (target 60%)
  → Day 2 Return (target 42%)
  → Chapter 3 Cleared (target 30%)
  → First Purchase (target 3,5%)
```

Setiap penurunan > 10% dari target memicu investigasi dan A/B test.

---

# 24. Kerangka Balancing

## 24.1 Prinsip Balancing

<aside>
⚖️

1. **Tidak ada build "wajib"** — minimal 6 build viable untuk menyelesaikan konten terkini.
2. **Win rate target** per stage: Stage 1–5 → 95%, Stage 6–9 → 80%, Boss Stage → 60% pada attempt pertama dengan power rekomendasi.
3. **TTK (Time To Kill)** musuh normal di menit 1 harus ≤ 0,8 detik; boss 45–90 detik.
4. **Power creep terkontrol** — gear baru maksimal +15% lebih kuat dari yang tertinggi saat itu per season.
5. **Semua senjata harus punya niche** — dievaluasi lewat pick rate & win rate.
6. **Balancing lewat Remote Config** — semua angka kunci bisa diubah tanpa update app.
</aside>

## 24.2 Metrik Balancing Per Senjata

| Metrik | Target Sehat | Tindakan jika di luar range |
| --- | --- | --- |
| Pick rate | 3%–12% | >18% → nerf 10–15%; <2% → buff 15–20% |
| Win rate saat dipakai | 45%–65% | >75% → nerf; <35% → buff |
| Kontribusi damage rata-rata | 10%–25% dari total | Sesuaikan base damage |
| Evolution rate | >40% dari run yang memakainya | Terlalu rendah → permudah syarat |

## 24.3 Kurva Progresi (Time Gate)

| Milestone | Target Waktu F2P | Target Waktu Payer |
| --- | --- | --- |
| Chapter 1 clear | 25 menit | 25 menit |
| Chapter 3 clear | Hari 2 | Hari 1 |
| Chapter 5 clear | Hari 6 | Hari 2 |
| Chapter 7 clear | Hari 15 | Hari 5 |
| Chapter 10 clear | Hari 35 | Hari 10 |
| Tier III (Nightmare) | Hari 60 | Hari 20 |
| Tier VI (Void) | Hari 150+ | Hari 60 |
| Gear SS pertama | Hari 12 | Hari 1 |
| Gear SSS pertama | Hari 75 | Hari 15 |

## 24.4 Parameter yang Wajib Ada di Remote Config

```jsx
{
  "enemy_hp_multiplier": 1.0,
  "enemy_atk_multiplier": 1.0,
  "enemy_spawn_rate_multiplier": 1.0,
  "xp_curve_coefficient": 0.55,
  "coin_drop_multiplier": 1.0,
  "gear_drop_rates": { "C": 0.55, "B": 0.25, "A": 0.13, "S": 0.055, "SS": 0.013, "SSS": 0.002 },
  "gacha_rates": { ... },
  "gacha_pity_threshold": { "standard": 60, "character": 80 },
  "weapon_base_damage": { "W01": 12, "W02": 20, ... },
  "evolution_requirements": { "passive_min_level": 3, "min_run_minutes": 5 },
  "revive_gem_cost_base": 60,
  "ad_frequency_cap": { "interstitial_per_runs": 3, "rewarded_daily": 15 },
  "ftue_no_ads_days": 3,
  "battle_pass_xp_per_tier": 1000,
  "feature_flags": { "guild_enabled": true, "pet_enabled": true }
}
```

---

# 25. Retensi & Notifikasi

## 25.1 Mekanisme Retensi

| Mekanisme | Detail | Target |
| --- | --- | --- |
| Daily Login Reward | 28 hari siklus, hari ke-7/14/21/28 reward besar (Gem 500, Tiket, Gear A) | D1–D30 |
| Daily Mission | 6 misi, reset 04:00 waktu lokal | Harian |
| Weekly Mission | 4 misi besar | Mingguan |
| Offline Reward (Idle) | Akumulasi Coin & EXP saat offline, maks 8 jam. Bisa ×2 dengan iklan | Comeback harian |
| Free Chest Timer | 1 chest gratis tiap 4 jam (maks 3 tersimpan) | Multi-session |
| Battle Pass | Progress harian yang terlihat | 35 hari |
| Event Berkala | Selalu ada minimal 2 event aktif | Mingguan |
| Guild Check-in | Reward untuk seluruh guild | Sosial |
| Comeback Reward | Pemain kembali setelah 7+ hari: paket welcome-back | Resurrection |
| Milestone Reward | Reward saat total playtime / total kill mencapai angka tertentu | Jangka panjang |

## 25.2 Strategi Push Notification

| Trigger | Waktu Kirim | Contoh Copy (ID) |
| --- | --- | --- |
| Energy-free reminder harian | 19:00 waktu lokal | "Zombie makin banyak! Ayo bertahan lagi malam ini 🧟" |
| Free chest siap | Saat chest penuh | "3 peti gratis menunggumu! 🎁" |
| Offline reward penuh | Setelah 8 jam offline | "Hadiah offline sudah penuh, klaim sekarang!" |
| Daily mission belum selesai | 21:00 | "Misi harian tinggal 2 lagi. Selesaikan sebelum reset!" |
| Event baru dimulai | Saat event mulai | "Event Boss Rush dimulai! Hadiah Gear SS menanti ⚔️" |
| Event akan berakhir | 12 jam sebelum berakhir | "Event berakhir 12 jam lagi. Jangan lewatkan!" |
| Battle Pass akan berakhir | 3 hari sebelum | "Season berakhir 3 hari lagi — kamu di tier 42/60" |
| Guild raid aktif | Saat raid dimulai | "Guild-mu sedang menyerang boss. Ikut sekarang!" |
| Comeback (D3 lapsed) | 3 hari tidak login | "Kami merindukanmu! Ada 500 Gem menunggu 💎" |

<aside>
🔔

**Aturan notifikasi:** maksimal **2 push per hari**. Tidak pernah antara 22:00–08:00 waktu lokal. Semua bisa dimatikan per kategori di settings. Wajib meminta izin notifikasi **setelah** sesi pertama selesai, bukan saat pertama buka app.

</aside>

---

# 26. Lokalisasi & Aksesibilitas

## 26.1 Bahasa (Launch: 10 Bahasa)

| Prioritas | Bahasa | Kode |
| --- | --- | --- |
| P0 | Inggris | en |
| P0 | Indonesia | id |
| P0 | Portugis (Brasil) | pt-BR |
| P1 | Spanyol | es |
| P1 | Vietnam | vi |
| P1 | Thai | th |
| P1 | Filipina (Tagalog) | tl |
| P2 | Jepang | ja |
| P2 | Korea | ko |
| P2 | Tionghoa Sederhana | zh-Hans |

### Aturan Lokalisasi

- Semua teks di file eksternal (CSV/JSON), **tidak ada hardcoded string**.
- UI harus toleran terhadap teks **40% lebih panjang** (bahasa Jerman/Portugis) — gunakan auto-shrink & wrapping.
- Format angka, tanggal, dan mata uang mengikuti locale.
- Harga IAP menggunakan harga lokal store, bukan konversi manual.
- Font harus mendukung Latin, CJK, dan Thai (gunakan Noto Sans family + fallback).

## 26.2 Aksesibilitas

| Fitur | Detail |
| --- | --- |
| Colorblind mode | 3 preset (Protanopia, Deuteranopia, Tritanopia) — mengubah warna telegraph & VFX |
| Ukuran teks | Slider 3 tingkat (Normal / Besar / Sangat Besar) |
| Reduce motion | Kurangi screen shake, flash, dan efek partikel |
| Reduce flashing | Nonaktifkan flash layar (penting untuk pemain fotosensitif/epilepsi) |
| Haptic toggle | On/Off |
| Left-handed mode | Cerminkan posisi joystick & tombol skill |
| Joystick sensitivity | Slider |
| Fixed joystick option | Alternatif dari floating joystick |
| Auto-aim assist | Sudah default (auto-attack) |
| Subtitle | Untuk semua dialog/cinematic |

---

# 27. QA & Testing Plan

## 27.1 Cakupan Testing

| Jenis Test | Cakupan | Tools |
| --- | --- | --- |
| Unit Test | Formula damage, XP curve, ekonomi, gacha rate, save/load | NUnit + Unity Test Framework |
| Integration Test | Alur run lengkap, sinkronisasi save, IAP flow | Unity Test Framework |
| Automated Playtest | Bot AI menjalankan 1.000 run untuk validasi balance & crash | Custom bot framework |
| Performance Test | FPS, memori, baterai, thermal di 15 device matrix | Unity Profiler, Firebase Test Lab |
| Device Compatibility | 30+ device (Android 8–15, iOS 14–18) | BrowserStack / Firebase Test Lab |
| Network Test | Koneksi lambat, putus, airplane mode, packet loss | Charles Proxy, Network Link Conditioner |
| Security Test | Memory editing, save tampering, API fuzzing | GameGuardian (internal), Burp Suite |
| Localization Test | Overflow teks, karakter rusak, konteks salah | Manual + screenshot automation |
| Store Compliance | Kebijakan Google Play & Apple (lootbox disclosure, privacy, rating) | Checklist manual |
| Accessibility Test | Semua opsi aksesibilitas berfungsi | Manual |
| Soak Test | Main 4 jam terus-menerus tanpa crash / memory leak | Automated |

## 27.2 Device Test Matrix

| Tier | Device | OS | Prioritas |
| --- | --- | --- | --- |
| Low | Redmi 9A, Samsung A03, Infinix Hot 10 | Android 10–11 | P0 |
| Low | iPhone 8, iPhone SE 2020 | iOS 15–16 | P0 |
| Mid | Redmi Note 11/12, Samsung A54, Poco X5 | Android 12–13 | P0 |
| Mid | iPhone 11, iPhone 12 | iOS 16–17 | P0 |
| High | Samsung S23/S24, Pixel 8 | Android 14–15 | P1 |
| High | iPhone 14/15/16 | iOS 17–18 | P1 |
| Tablet | iPad 9th gen, Galaxy Tab A8 | — | P2 |
| Foldable | Galaxy Z Fold/Flip | Android 14 | P2 |

## 27.3 Definition of Done (per fitur)

<aside>
✅

Sebuah fitur dianggap **selesai** hanya jika:

1. Kode di-review dan di-merge ke `develop`
2. Unit test lulus (coverage ≥ 70% untuk logika inti)
3. Berfungsi di minimal 3 device tier berbeda
4. Tidak menurunkan FPS > 5% pada device low-end
5. Semua string sudah dilokalisasi (minimal en + id)
6. Analytics event sudah terpasang dan terverifikasi
7. Tidak ada memory leak (verifikasi profiler 10 menit)
8. UI mengikuti design system & safe area
9. Kasus offline & error sudah ditangani
10. Didokumentasikan di wiki internal
11. QA sign-off
</aside>

## 27.4 Klasifikasi Bug

| Severity | Definisi | SLA Perbaikan |
| --- | --- | --- |
| S0 — Blocker | Crash saat boot, kehilangan save, IAP gagal namun terpotong | < 4 jam (hotfix) |
| S1 — Critical | Progression blocked, exploit ekonomi, crash sering | < 24 jam |
| S2 — Major | Fitur tidak berfungsi, balance rusak parah | < 3 hari |
| S3 — Minor | Bug visual, teks salah, animasi aneh | Sprint berikutnya |
| S4 — Trivial | Typo, polish | Backlog |

---

# 28. Roadmap & Milestone

## 28.1 Timeline Pengembangan (±12 Bulan)

| Fase | Durasi | Milestone | Deliverable |
| --- | --- | --- | --- |
| **M0 — Pre-production** | Bulan 1 | Konsep terkunci | GDD, PRD, art style guide, tech spike (300 musuh @60fps) |
| **M1 — Prototype** | Bulan 2–3 | **Fun Test** | Gameplay loop inti: gerak, auto-attack, 5 senjata, 3 musuh, level up, 1 stage. Playtest internal 20 orang |
| **M2 — Vertical Slice** | Bulan 4–5 | **Greenlight** | Chapter 1 lengkap dengan art final, 10 senjata, 5 evolusi, 1 boss, lobby dasar, gear system |
| **M3 — Alpha** | Bulan 6–8 | **Feature Complete** | Semua sistem inti: 20 senjata, 22 pasif, 13 evolusi, Chapter 1–5, gear, tech part, skill tree, shop, IAP |
| **M4 — Beta** | Bulan 9–10 | **Content Complete** | Chapter 1–10, 10 boss, 8 karakter, guild, event, battle pass, semua lokalisasi |
| **M5 — Soft Launch** | Bulan 11 | **Metrics Validation** | Rilis di 3 negara (Filipina, Vietnam, Peru). Iterasi berdasarkan D1/D7 & ARPDAU |
| **M6 — Global Launch** | Bulan 12 | **Ship** | Rilis global + kampanye UA |
| **M7 — Live Ops** | Berkelanjutan | Season | Update konten tiap 5 minggu |

## 28.2 Gerbang Kualitas Soft Launch

<aside>
🚦

Global launch **hanya dilakukan** jika soft launch mencapai:

- D1 Retention ≥ 40%
- D7 Retention ≥ 16%
- D30 Retention ≥ 6%
- ARPDAU ≥ $0,07
- Crash-free session ≥ 99,3%
- Tutorial completion ≥ 82%
- Store rating ≥ 4,2

Jika tidak tercapai → perpanjang soft launch 4–8 minggu dan iterasi.

</aside>

## 28.3 Roadmap Konten Pasca-Launch

| Season | Tema | Konten Baru |
| --- | --- | --- |
| S1 (bulan 1–2) | Awakening | Chapter 11–12, 2 karakter, 5 senjata, event Halloween |
| S2 (bulan 3–4) | Frozen Dawn | Chapter 13–14, mode co-op asinkron, 1 set gear baru |
| S3 (bulan 5–6) | Machine War | Chapter 15–16, sistem Mech mount, guild war v2 |
| S4 (bulan 7–8) | Void Rising | Chapter 17–18, elemen ke-6 (Chaos), endgame roguelite mode |
| S5+ | — | Evaluasi berdasarkan data |

---

# 29. Tim, Resource & Estimasi Anggaran

## 29.1 Komposisi Tim Minimum

| Peran | Jumlah | Fase Terlibat | Catatan |
| --- | --- | --- | --- |
| Game Producer / PM | 1 | Semua | Owner roadmap & prioritas |
| Game Designer (Systems) | 2 | Semua | 1 fokus combat, 1 fokus ekonomi/meta |
| Level / Content Designer | 1 | M2–M7 | Stage, wave, boss pattern |
| Unity Engineer (Gameplay) | 3 | Semua | 1 senior wajib paham DOTS |
| Unity Engineer (Meta/UI) | 2 | M2–M7 | Sistem meta & UI |
| Backend Engineer | 1 | M3–M7 | API, anti-cheat, live ops |
| Technical Artist | 1 | M2–M6 | Shader, VFX, optimasi |
| 2D Artist (Character) | 2 | M1–M6 | Karakter, musuh, boss |
| 2D Artist (Environment/UI) | 2 | M2–M6 | Tileset, UI, ikon |
| Animator | 1 | M2–M6 | Spine 2D |
| VFX Artist | 1 | M2–M6 | Efek senjata & boss |
| Sound Designer | 1 (part-time) | M2–M6 | SFX & musik (bisa outsource) |
| QA Lead + QA Tester | 1 + 2 | M2–M7 | Manual + automation |
| Data Analyst | 1 | M4–M7 | Dashboard & A/B test |
| Live Ops Manager | 1 | M5–M7 | Event & konten |
| UA / Marketing | 1–2 | M4–M7 | Kampanye & kreatif |
| Community Manager | 1 | M5–M7 | Discord, sosmed, support |

**Total: ±22–25 orang** pada puncak produksi.

## 29.2 Estimasi Anggaran (Kasar, USD)

| Kategori | Estimasi | Catatan |
| --- | --- | --- |
| Gaji tim (12 bulan) | $650.000 – $1.100.000 | Sangat bergantung lokasi tim |
| Outsourcing art tambahan | $60.000 – $120.000 | Boss, skin, marketing art |
| Audio (musik + SFX) | $20.000 – $40.000 | ±15 track + 200 SFX |
| Tools & lisensi | $15.000 | Unity Pro, Spine, plugin, Perforce |
| Backend & infrastruktur | $18.000/tahun | Scaling sesuai DAU |
| QA device farm | $8.000 | Pembelian device + layanan |
| Soft launch UA | $50.000 | Uji metrik |
| Global launch UA | $500.000 – $2.000.000 | Skala sesuai LTV terbukti |
| Legal, compliance, entitas | $20.000 | Privacy policy, ToS, rating |
| Buffer 15% | — | Wajib |
| **Total pra-UA global** | **≈ $850.000 – $1.400.000** | — |

<aside>
💡

**Versi hemat (tim kecil / indie):** tim 5–7 orang, scope dipangkas ke 5 chapter, 12 senjata, 3 karakter, tanpa guild & pet di v1.0 → estimasi **$80.000 – $180.000** dan **7–9 bulan**. Sisanya ditambahkan sebagai update pasca-launch.

</aside>

## 29.3 Prioritas Fitur (MoSCoW)

| Prioritas | Fitur |
| --- | --- |
| **Must Have (v1.0)** | Kontrol joystick, auto-attack, 20 senjata, 22 item pasif, 13 evolusi, sistem XP & kartu level-up, spawn & scaling musuh, 10 arketipe musuh, elite, 5 boss minimal, Chapter 1–5, gear 6 slot + enhance, save lokal & cloud, IAP dasar, rewarded ads, daily mission, login reward, tutorial, 3 bahasa |
| **Should Have (v1.0–1.2)** | Chapter 6–10 + 5 boss, tech parts, skill tree, battle pass, gacha, 8 karakter, event system, leaderboard, 10 bahasa, collectibles |
| **Could Have (v1.3+)** | Guild, guild raid, guild war, pet system, endless mode, difficulty tier, replay sharing, build sharing, skin/kosmetik |
| **Won't Have (v1.x)** | Real-time co-op/PvP, level editor, PC/console port, trading |

---

# 30. Risiko & Mitigasi

| # | Risiko | Kemungkinan | Dampak | Mitigasi |
| --- | --- | --- | --- | --- |
| R1 | Performa buruk di device low-end (musuh terlalu banyak) | Tinggi | Kritis | Tech spike di M0; wajib DOTS/ECS; adaptive quality; cap musuh per tier device |
| R2 | Genre sudah jenuh, sulit menonjol | Tinggi | Tinggi | Fokus pada USP (elemen & dual-evolution); soft launch untuk validasi sebelum UA besar |
| R3 | Retensi D1 di bawah target | Sedang | Kritis | FTUE diuji A/B sejak alpha; evolusi pertama dalam 10 menit; tanpa iklan 3 hari pertama |
| R4 | Monetisasi terlalu agresif merusak review | Sedang | Tinggi | Tidak ada energy gate; ads opsional; harga lokal wajar; monitoring sentimen review |
| R5 | Balance rusak (build overpowered) | Tinggi | Sedang | Semua angka di Remote Config; bot playtest 1.000 run; monitoring pick/win rate mingguan |
| R6 | Cheating merusak leaderboard & ekonomi | Sedang | Tinggi | Server-authoritative reward & gacha; validasi run result; Play Integrity API |
| R7 | Scope creep menunda rilis | Tinggi | Tinggi | MoSCoW ketat; feature freeze di M4; review scope tiap sprint |
| R8 | Biaya UA melebihi LTV | Sedang | Kritis | Validasi LTV D30 di soft launch sebelum spend besar; target ROAS D7 ≥ 25% |
| R9 | Konten habis → churn pemain lama | Tinggi | Sedang | Roadmap season 5 mingguan; endless & difficulty tier sebagai konten tak terbatas |
| R10 | Penolakan store (lootbox/rating) | Rendah | Tinggi | Tampilkan rate gacha; age rating 12+; siapkan dokumen compliance sejak awal |
| R11 | Kehilangan save data pemain | Rendah | Kritis | Cloud save + 3 snapshot backup + checksum + tool CS untuk rollback |
| R12 | Ketergantungan pada 1 sumber UA | Sedang | Sedang | Diversifikasi: Meta, Google, TikTok, Unity, influencer, ASO organik |
| R13 | Kunci personel keluar (bus factor) | Sedang | Tinggi | Dokumentasi wajib; code review; tidak ada sistem yang hanya dipahami 1 orang |

---

# 31. Lampiran (Appendix)

## 31.1 Glosarium

| Istilah | Arti |
| --- | --- |
| **Run** | Satu sesi permainan dari mulai sampai menang/kalah |
| **Roguelite** | Genre dengan RNG per run + progresi permanen antar run |
| **Draft** | Memilih 1 dari beberapa opsi acak saat naik level |
| **Evolve / Evolusi** | Menggabungkan senjata MAX + item pasif menjadi versi kuat |
| **Meta-progression** | Progresi permanen di luar run (gear, level, talent) |
| **Power Rating** | Angka gabungan kekuatan akun |
| **Pity** | Jaminan mendapat item langka setelah N pull |
| **Faucet / Sink** | Sumber masuk / keluar mata uang |
| **TTK** | Time To Kill — waktu membunuh musuh |
| **i-frame** | Invincibility frame, periode kebal setelah terkena damage |
| **Telegraph** | Indikator visual sebelum serangan musuh dieksekusi |
| **Culling** | Menghapus objek yang tidak terlihat untuk performa |
| **FTUE** | First Time User Experience |
| **ARPDAU** | Average Revenue Per Daily Active User |
| **LTV** | Lifetime Value — total pendapatan per pemain |
| **ROAS** | Return On Ad Spend |
| **DOTS/ECS** | Arsitektur data-oriented Unity untuk performa tinggi |

## 31.2 Naming Convention

| Tipe | Format | Contoh |
| --- | --- | --- |
| Senjata | `W##_NamaSenjata` | `W01_Kunai` |
| Evolusi | `WE##_NamaEvolusi` | `WE01_ShadowBlade` |
| Item Pasif | `P##_NamaItem` | `P01_SharpBlade` |
| Musuh | `E##_NamaMusuh` | `E01_Walker` |
| Boss | `B##_NamaBoss` | `B01_Colossus` |
| Karakter | `C##_NamaKarakter` | `C01_Rex` |
| Stage | `ST_Ch#_St#` | `ST_Ch1_St10` |
| Gear | `GR_SLOT_###` | `GR_WEAPON_012` |
| Layar UI | `S##_NamaLayar` | `S03_Lobby` |
| Prefab | `PF_Kategori_Nama` | `PF_Enemy_Walker` |
| ScriptableObject | `SO_Kategori_Nama` | `SO_Weapon_Kunai` |
| Analytics Event | `snake_case` | `weapon_evolved` |

## 31.3 Contoh Skema ScriptableObject Senjata

```jsx
[CreateAssetMenu(menuName = "Nightfall/Weapon")]
public class WeaponSO : ScriptableObject {
    public string weaponId;          // "W01"
    public string displayNameKey;    // key lokalisasi
    public Sprite icon;
    public ElementType element;      // Neutral, Fire, Ice, Lightning, Poison, Void
    public AttackType attackType;    // Projectile, Melee, Aura, Orbit, Deployable, Ground
    public TargetingMode targeting;  // Nearest, Random, Forward, Aura, Orbit, Drop

    [Header("Level Data (index 0 = Lv.1)")]
    public WeaponLevelData[] levels; // 5 entri

    [Header("Evolution")]
    public PassiveItemSO evolutionPassiveA;
    public WeaponSO evolutionResultA;
    public PassiveItemSO evolutionPassiveB;   // opsional (dual-evolution)
    public WeaponSO evolutionResultB;

    [Header("VFX & Audio")]
    public GameObject projectilePrefab;
    public GameObject hitVfxPrefab;
    public AudioClip fireSfx;
    public AudioClip hitSfx;
}

[System.Serializable]
public class WeaponLevelData {
    public float baseDamage;
    public float cooldown;
    public int projectileCount;
    public int pierce;
    public float areaScale;
    public float duration;
    public float knockback;
    public string descriptionKey;   // key lokalisasi untuk teks kartu
}
```

## 31.4 Checklist Sebelum Rilis (Launch Checklist)

- [ ]  Semua S0/S1 bug tertutup
- [ ]  Crash-free session ≥ 99,5% selama 7 hari terakhir soft launch
- [ ]  Semua analytics event terverifikasi di dashboard
- [ ]  IAP diuji di sandbox dan production untuk semua SKU
- [ ]  Receipt validation server aktif dan diuji
- [ ]  Remote Config terhubung dan bisa mengubah balance tanpa update
- [ ]  Cloud save & restore diuji lintas device dan lintas platform
- [ ]  Semua string dilokalisasi dan diuji overflow
- [ ]  Privacy Policy, Terms of Service, dan GDPR/CCPA consent flow siap
- [ ]  Age rating diperoleh (IARC / ESRB / PEGI)
- [ ]  Rate gacha ditampilkan di dalam game
- [ ]  Store listing siap: ikon, screenshot (5 per bahasa), video preview, deskripsi ASO
- [ ]  Push notification campaign terjadwal
- [ ]  Server load test untuk 10× DAU proyeksi
- [ ]  Rencana rollback build & hotfix pipeline siap
- [ ]  Discord + kanal support pemain aktif
- [ ]  Dokumen customer support (FAQ, tool rollback akun) siap
- [ ]  Kill switch untuk fitur bermasalah (feature flag) berfungsi
- [ ]  Kampanye UA & kreatif siap tayang
- [ ]  Tim on-call ditentukan untuk 72 jam pertama pasca-launch

## 31.5 Referensi & Bacaan Lanjutan

| Topik | Referensi |
| --- | --- |
| Game feel & juice | "Game Feel" — Steve Swink; talk "Juice it or lose it" |
| Roguelite design | Analisis desain Vampire Survivors & Hades |
| Unity DOTS untuk crowd | Dokumentasi Unity Entities + sampel Boss Room |
| F2P ekonomi | "Game Economy Design" oleh Deconstructor of Fun |
| Mobile UX | Google Material Design (mobile game guidelines), Apple HIG |
| LiveOps | Studi kasus Habby, Supercell, dan Playrix |

---

<aside>
📝

**Catatan revisi**

| Versi | Tanggal | Perubahan | Penulis |
| --- | --- | --- | --- |
| 1.0 | 31 Agustus 2026 | Draft awal lengkap | abigalhebeevie3 |
</aside>

[GAP Analysis & Rencana Eksekusi — Repo SALDOKUGAME (Zomburst) vs PRD](https://app.notion.com/p/GAP-Analysis-Rencana-Eksekusi-Repo-SALDOKUGAME-Zomburst-vs-PRD-d051f771a8c24d82a97a6ec110bdb9e6?pvs=21)