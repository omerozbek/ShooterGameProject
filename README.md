> Not: Kodlardaki yorum satırlarının tamamı ve README, AI tarafından yazılmıştır.

# ShooterGame — Unity Atış Simülasyonu

Staj değerlendirmesi kapsamında Unity ile geliştirilmiş bir atış poligonu simülasyonu. Proje; **Kalıtım (Inheritance)**, **Soyut Sınıflar (Abstract Classes)** ve **Arayüzler (Interfaces)** kullanılarak tasarlanmıştır.

---

## Kontroller

| Tuş | Eylem |
|---|---|
| Ok Tuşları | Karakter hareketi |
| Fare | Kameraya bakış yönü |
| S | Ateş et |
| Space | Silah değiştir |
| R | Şarjör doldur |
| T | Hedefi sıfırla |

---

## Silahlar

| Özellik | Desert Eagle | AK-47 |
|---|---|---|
| Ateş modu | Yarı otomatik | Tam otomatik |
| Ateş hızı | 60 atış/dak | 400 atış/dak |
| Şarjör | 10 mermi | 30 mermi |
| İsabet oranı | %90 | %60 |
| Hasar | 40 | 5 |
| Şarj süresi | 4 sn | 5 sn |

---

## Proje Yapısı

```
Assets/Scripts/
├── Interfaces/
│   ├── IShootable.cs       # Ateş etme arayüzü
│   ├── IReloadable.cs      # Şarj etme arayüzü
│   └── IDamageable.cs      # Hasar alma arayüzü
├── Weapons/
│   ├── WeaponBase.cs       # Soyut silah sınıfı (IShootable, IReloadable)
│   ├── DesertEagle.cs      # Desert Eagle implementasyonu
│   └── AK47.cs             # AK-47 implementasyonu
├── Core/
│   ├── PlayerController.cs # Hareket ve kamera kontrolü
│   ├── WeaponSystem.cs     # Silah yönetimi ve girdi işleme
│   └── Target.cs           # Hedef (IDamageable, 100 HP)
└── UI/
    └── UIManager.cs        # TextMeshPro ile arayüz
```

---

## OOP Tasarımı

- **`IShootable`** — `Shoot()` metodunu zorunlu kılar
- **`IReloadable`** — `Reload()` metodunu zorunlu kılar
- **`IDamageable`** — `TakeDamage()` metodunu zorunlu kılar
- **`WeaponBase`** — `MonoBehaviour`'dan türeyen, `IShootable` ve `IReloadable`'ı uygulayan soyut temel sınıf; isabet hesabı ve ateş hızını yönetir
- **`DesertEagle` / `AK47`** — `WeaponBase`'den türeyen somut silah sınıfları

---

