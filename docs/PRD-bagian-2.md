# PRD — Project Nightfall (Bagian 2: Bab 23–31)

Lanjutan dari [`PRD.md`](./PRD.md) (Bab 1–22).

Bab 23 diulang lengkap di sini karena file bagian 1 terpotong di tengah bab ini.

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

```
Install -> Tutorial Start (target 95%)
  -> Tutorial Complete (target 85%)
  -> First Run Complete (target 80%)
  -> First Gear Equipped (target 75%)
  -> Chapter 1 Cleared (target 60%)
  -> Day 2 Return (target 42%)
  -> Chapter 3 Cleared (target 30%)
  -> First Purchase (target 3,5%)
```

Setiap penurunan > 10% dari target memicu investigasi dan A/B test.

---

# 24. Kerangka Balancing

## 24.1 Prinsip Balancing

> 1. **Tidak ada build "wajib"** — minimal 6 build viable untuk menyelesaikan konten terkini.
> 2. **Win rate target** per stage: Stage 1–5 -> 95%, Stage 6–9 -> 80%, Boss Stage -> 60% pada attempt pertama dengan power rekomendasi.
> 3. **TTK (Time To Kill)** musuh normal di menit 1 harus <= 0,8 detik; boss 45–90 detik.
> 4. **Power creep terkontrol** — gear baru maksimal +15% lebih kuat dari yang tertinggi saat itu per season.
> 5. **Semua senjata harus punya niche** — dievaluasi lewat pick rate & win rate.
> 6. **Balancing lewat Remote Config** — semua angka kunci bisa diubah tanpa update app.

## 24.2 Metrik Balancing Per Senjata

| Metrik | Target Sehat | Tindakan jika di luar range |
| --- | --- | --- |
| Pick rate | 3%–12% | >18% -> nerf 10–15%; <2% -> buff 15–20% |
| Win rate saat dipakai | 45%–65% | >75% -> nerf; <35% -> buff |
| Kontribusi damage rata-rata | 10%–25% dari total | Sesuaikan base damage |
| Evolution rate | >40% dari run yang memakainya | Terlalu rendah -> permudah syarat |

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

```json
{
  "enemy_hp_multiplier": 1.0,
  "enemy_atk_multiplier": 1.0,
  "enemy_spawn_rate_multiplier": 1.0,
  "xp_curve_coefficient": 0.55,
  "coin_drop_multiplier": 1.0,
  "gear_drop_rates": { "C": 0.55, "B": 0.25, "A": 0.13, "S": 0.055, "SS": 0.013, "SSS": 0.002 },
  "gacha_pity_threshold": { "standard": 60, "character": 80 },
  "weapon_base_damage": { "W01": 12, "W02": 20 },
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
| Daily Login Reward | 28 hari siklus, hari ke-7/14/21/28 reward besar | D1–D30 |
| Daily Mission | 6 misi, reset 04:00 waktu lokal | Harian |
| Weekly Mission | 4 misi besar | Mingguan |
| Offline Reward (Idle) | Akumulasi Coin & EXP saat offline, maks 8 jam. Bisa x2 dengan iklan | Comeback harian |
| Free Chest Timer | 1 chest gratis tiap 4 jam (maks 3 tersimpan) | Multi-session |
| Battle Pass | Progress harian yang terlihat | 35 hari |
| Event Berkala | Selalu ada minimal 2 event aktif | Mingguan |
| Guild Check-in | Reward untuk seluruh guild | Sosial |
| Comeback Reward | Pemain kembali setelah 7+ hari: paket welcome-back | Resurrection |
| Milestone Reward | Reward saat total playtime / total kill mencapai angka tertentu | Jangka panjang |

## 25.2 Strategi Push Notification

| Trigger | Waktu Kirim | Contoh Copy (ID) |
| --- | --- | --- |
| Reminder harian | 19:00 waktu lokal | "Zombie makin banyak! Ayo bertahan lagi malam ini" |
| Free chest siap | Saat chest penuh | "3 peti gratis menunggumu!" |
| Offline reward penuh | Setelah 8 jam offline | "Hadiah offline sudah penuh, klaim sekarang!" |
| Daily mission belum selesai | 21:00 | "Misi harian tinggal 2 lagi. Selesaikan sebelum reset!" |
| Event baru dimulai | Saat event mulai | "Event Boss Rush dimulai! Hadiah Gear SS menanti" |
| Event akan berakhir | 12 jam sebelum berakhir | "Event berakhir 12 jam lagi. Jangan lewatkan!" |
| Battle Pass akan berakhir | 3 hari sebelum | "Season berakhir 3 hari lagi — kamu di tier 42/60" |
| Guild raid aktif | Saat raid dimulai | "Guild-mu sedang menyerang boss. Ikut sekarang!" |
| Comeback (D3 lapsed) | 3 hari tidak login | "Kami merindukanmu! Ada 500 Gem menunggu" |

> **Aturan notifikasi:** maksimal **2 push per hari**. Tidak pernah antara 22:00–08:00 waktu lokal. Semua bisa dimatikan per kategori di settings. Wajib meminta izin notifikasi **setelah** sesi pertama selesai, bukan saat pertama buka app.

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
- UI harus toleran terhadap teks **40% lebih panjang** — gunakan auto-shrink & wrapping.
- Format angka, tanggal, dan mata uang mengikuti locale.
- Harga IAP menggunakan harga lokal store, bukan konversi manual.
- Font harus mendukung Latin, CJK, dan Thai (Noto Sans family + fallback).

## 26.2 Aksesibilitas

| Fitur | Detail |
| --- | --- |
| Colorblind mode | 3 preset (Protanopia, Deuteranopia, Tritanopia) |
| Ukuran teks | Slider 3 tingkat (Normal / Besar / Sangat Besar) |
| Reduce motion | Kurangi screen shake, flash, dan efek partikel |
| Reduce flashing | Nonaktifkan flash layar (penting untuk pemain fotosensitif) |
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
| Device Compatibility | 30+ device (Android 8–15, iOS 14–18) | Firebase Test Lab |
| Network Test | Koneksi lambat, putus, airplane mode, packet loss | Charles Proxy |
| Security Test | Memory editing, save tampering, API fuzzing | Burp Suite |
| Localization Test | Overflow teks, karakter rusak, konteks salah | Manual + screenshot automation |
| Store Compliance | Kebijakan Google Play & Apple | Checklist manual |
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

> Sebuah fitur dianggap **selesai** hanya jika:
>
> 1. Kode di-review dan di-merge ke `develop`
> 2. Unit test lulus (coverage >= 70% untuk logika inti)
> 3. Berfungsi di minimal 3 device tier berbeda
> 4. Tidak menurunkan FPS > 5% pada device low-end
> 5. Semua string sudah dilokalisasi (minimal en + id)
> 6. Analytics event sudah terpasang dan terverifikasi
> 7. Tidak ada memory leak (verifikasi profiler 10 menit)
> 8. UI mengikuti design system & safe area
> 9. Kasus offline & error sudah ditangani
> 10. Didokumentasikan di wiki internal
> 11. QA sign-off

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

## 28.1 Timeline Pengembangan (~12 Bulan)

| Fase | Durasi | Milestone | Deliverable |
| --- | --- | --- | --- |
| **M0 — Pre-production** | Bulan 1 | Konsep terkunci | GDD, PRD, art style guide, tech spike (300 musuh @60fps) |
| **M1 — Prototype** | Bulan 2–3 | **Fun Test** | Gameplay loop inti: gerak, auto-attack, 5 senjata, 3 musuh, level up, 1 stage |
| **M2 — Vertical Slice** | Bulan 4–5 | **Greenlight** | Chapter 1 lengkap dengan art final, 10 senjata, 5 evolusi, 1 boss, lobby, gear |
| **M3 — Alpha** | Bulan 6–8 | **Feature Complete** | 20 senjata, 22 pasif, 13 evolusi, Chapter 1–5, gear, tech part, skill tree, shop, IAP |
| **M4 — Beta** | Bulan 9–10 | **Content Complete** | Chapter 1–10, 10 boss, 8 karakter, guild, event, battle pass, lokalisasi |
| **M5 — Soft Launch** | Bulan 11 | **Metrics Validation** | Rilis di 3 negara (Filipina, Vietnam, Peru) |
| **M6 — Global Launch** | Bulan 12 | **Ship** | Rilis global + kampanye UA |
| **M7 — Live Ops** | Berkelanjutan | Season | Update konten tiap 5 minggu |

## 28.2 Gerbang Kualitas Soft Launch

> Global launch **hanya dilakukan** jika soft launch mencapai:
>
> - D1 Retention >= 40%
> - D7 Retention >= 16%
> - D30 Retention >= 6%
> - ARPDAU >= $0,07
> - Crash-free session >= 99,3%
> - Tutorial completion >= 82%
> - Store rating >= 4,2
>
> Jika tidak tercapai, perpanjang soft launch 4–8 minggu dan iterasi.

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

**Total: ~22–25 orang** pada puncak produksi.

## 29.2 Estimasi Anggaran (Kasar, USD)

| Kategori | Estimasi | Catatan |
| --- | --- | --- |
| Gaji tim (12 bulan) | $650.000 – $1.100.000 | Sangat bergantung lokasi tim |
| Outsourcing art tambahan | $60.000 – $120.000 | Boss, skin, marketing art |
| Audio (musik + SFX) | $20.000 – $40.000 | ~15 track + 200 SFX |
| Tools & lisensi | $15.000 | Unity Pro, Spine, plugin |
| Backend & infrastruktur | $18.000/tahun | Scaling sesuai DAU |
| QA device farm | $8.000 | Pembelian device + layanan |
| Soft launch UA | $50.000 | Uji metrik |
| Global launch UA | $500.000 – $2.000.000 | Skala sesuai LTV terbukti |
| Legal, compliance, entitas | $20.000 | Privacy policy, ToS, rating |
| Buffer 15% | — | Wajib |
| **Total pra-UA global** | **~$850.000 – $1.400.000** | — |

> **Versi hemat (tim kecil / indie):** tim 5–7 orang, scope dipangkas ke 5 chapter, 12 senjata, 3 karakter, tanpa guild & pet di v1.0 -> estimasi **$80.000 – $180.000** dan **7–9 bulan**. Sisanya ditambahkan sebagai update pasca-launch.
>
> Untuk repo ini (satu orang), bahkan versi hemat pun masih terlalu besar. Lihat rekomendasi "Zomburst Edition" di catatan implementasi PRD.md: **10 senjata, 5 chapter.**

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
| R2 | Genre sudah jenuh, sulit menonjol | Tinggi | Tinggi | Fokus pada USP (elemen & dual-evolution); soft launch sebelum UA besar |
| R3 | Retensi D1 di bawah target | Sedang | Kritis | FTUE diuji A/B sejak alpha; evolusi pertama dalam 10 menit; tanpa iklan 3 hari pertama |
| R4 | Monetisasi terlalu agresif merusak review | Sedang | Tinggi | Tidak ada energy gate; ads opsional; harga lokal wajar; monitoring sentimen |
| R5 | Balance rusak (build overpowered) | Tinggi | Sedang | Semua angka di Remote Config; bot playtest 1.000 run; monitoring pick/win rate |
| R6 | Cheating merusak leaderboard & ekonomi | Sedang | Tinggi | Server-authoritative reward & gacha; validasi run result; Play Integrity API |
| R7 | Scope creep menunda rilis | Tinggi | Tinggi | MoSCoW ketat; feature freeze di M4; review scope tiap sprint |
| R8 | Biaya UA melebihi LTV | Sedang | Kritis | Validasi LTV D30 di soft launch; target ROAS D7 >= 25% |
| R9 | Konten habis -> churn pemain lama | Tinggi | Sedang | Roadmap season 5 mingguan; endless & difficulty tier |
| R10 | Penolakan store (lootbox/rating) | Rendah | Tinggi | Tampilkan rate gacha; age rating 12+; dokumen compliance sejak awal |
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

```csharp
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

> **Catatan repo:** implementasi nyata di `Assets/Scripts/Data/SenjataSO.cs` memakai nama berbahasa Indonesia (`namaTampil`, `jedaSerang`, `jumlahProyektil`, `hasilEvolusi`) tetapi mengikuti struktur yang sama.

## 31.4 Checklist Sebelum Rilis (Launch Checklist)

- [ ] Semua S0/S1 bug tertutup
- [ ] Crash-free session >= 99,5% selama 7 hari terakhir soft launch
- [ ] Semua analytics event terverifikasi di dashboard
- [ ] IAP diuji di sandbox dan production untuk semua SKU
- [ ] Receipt validation server aktif dan diuji
- [ ] Remote Config terhubung dan bisa mengubah balance tanpa update
- [ ] Cloud save & restore diuji lintas device dan lintas platform
- [ ] Semua string dilokalisasi dan diuji overflow
- [ ] Privacy Policy, Terms of Service, dan GDPR/CCPA consent flow siap
- [ ] Age rating diperoleh (IARC / ESRB / PEGI)
- [ ] Rate gacha ditampilkan di dalam game
- [ ] Store listing siap: ikon, screenshot (5 per bahasa), video preview, deskripsi ASO
- [ ] Push notification campaign terjadwal
- [ ] Server load test untuk 10x DAU proyeksi
- [ ] Rencana rollback build & hotfix pipeline siap
- [ ] Discord + kanal support pemain aktif
- [ ] Dokumen customer support (FAQ, tool rollback akun) siap
- [ ] Kill switch untuk fitur bermasalah (feature flag) berfungsi
- [ ] Kampanye UA & kreatif siap tayang
- [ ] Tim on-call ditentukan untuk 72 jam pertama pasca-launch

## 31.5 Referensi & Bacaan Lanjutan

| Topik | Referensi |
| --- | --- |
| Game feel & juice | "Game Feel" — Steve Swink; talk "Juice it or lose it" |
| Roguelite design | Analisis desain Vampire Survivors & Hades |
| Unity DOTS untuk crowd | Dokumentasi Unity Entities + sampel Boss Room |
| F2P ekonomi | "Game Economy Design" oleh Deconstructor of Fun |
| Mobile UX | Google Material Design, Apple HIG |
| LiveOps | Studi kasus Habby, Supercell, dan Playrix |

---

### Catatan revisi

| Versi | Tanggal | Perubahan | Penulis |
| --- | --- | --- | --- |
| 1.0 | 31 Agustus 2026 | Draft awal lengkap | abigalhebeevie3 |
