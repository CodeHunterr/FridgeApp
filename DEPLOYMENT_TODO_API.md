# DEPLOYMENT_TODO_API.md

Bu dosya, backend projesini GitHub + Neon PostgreSQL + Koyeb hattına taşımak için yapılacak işleri sade bir checklist olarak toplar.

## 1. Mevcut durum özeti
- Backend: ASP.NET Core Web API
- ORM: EF Core
- Şu anki provider: PostgreSQL / Npgsql
- Hedef provider: PostgreSQL / Neon
- Hedef deploy: Koyeb
- Not: Repo içinde gerçek DB secret'ı olmamalı. Dokümana veya Git history'sine secret yazılmamalı.

## 2. GitHub'a hazırlık yapılacaklar
- `appsettings.json` içindeki gerçek connection string'i temizle.
- Gerekirse geçmiş commit'lerde secret varsa repo public olmadan önce ayrıca değerlendir.
- `README` ve handoff dokümanlarını repo kökünde bırak.
- Local makineye özgü geçici dosyaların commit edilmediğini kontrol et.
- Deploy öncesi en az bir temiz `dotnet build` al.

Local geliştirme için gerçek connection string'i repoya yazma. Güvenli örnek için
`appsettings.example.json` dosyasını referans al; kendi değerlerini `dotnet user-secrets`,
environment variable veya Git'e girmeyen local config üzerinden ver.

## 3. `.gitignore` kontrolü
Şu an var olan temel ignore'lar:
- `bin/`
- `obj/`
- `.vs/`
- `appsettings.Development.json`
- `*.user`
- `*.suo`

Ek kontrol önerileri:
- `appsettings.Local.json`
- `appsettings.*.local.json`
- `.env`
- `.env.*`
- publish / temp klasörleri

Not:
- `appsettings.json` doğrudan ignore edilmek zorunda değil.
- Ama içindeki gerçek secret temizlenmeli ve placeholder ya da boş yapı ile bırakılmalı.

## 4. `appsettings` / secret temizliği
- Repo'da gerçek connection string kalmamalı.
- Lokal geliştirme için tercih sırası:
  1. `dotnet user-secrets`
  2. environment variable
  3. local ignore edilen config dosyası
- Koyeb production için connection string doğrudan environment variable olarak verilmeli.
- Production/Koyeb/Neon tarafında connection string dosyaya yazılmamalı.
- Kullanılacak environment variable:
  - `ConnectionStrings__DefaultConnection=<connection-string>`
- ASP.NET Core bu değeri otomatik olarak `ConnectionStrings:DefaultConnection` config yoluna map eder.
- İleride production için başlangıç env değişkenleri:
  - `ASPNETCORE_ENVIRONMENT=Production`
  - `ConnectionStrings__DefaultConnection=<Neon PostgreSQL connection string>`
- Doküman, issue veya PR açıklamasında gerçek secret paylaşılmamalı.

## 5. `Npgsql.EntityFrameworkCore.PostgreSQL` geçiş durumu
- `Npgsql.EntityFrameworkCore.PostgreSQL` ana provider olarak kullanılmalıdır.
- SQL Server provider production hedefinden çıkarılmıştır.
- Yeni provider ile temiz build alınmalıdır.
- Sıfırdan Postgres DB üzerinde migration smoke test yapılmalıdır.

Not:
- SQL Server provider ile üretilmiş migration'ların Neon üzerinde birebir sorunsuz çalışacağı varsayılmamalı.
- En güvenli yaklaşım, provider geçişini ayrı bir kontrollü turda yapmak.

## 6. `UseNpgsql` bağlantı yapısı
- `Program.cs` içinde `options.UseNpgsql(...)` kullanılmalıdır.
- `ConnectionStrings:DefaultConnection` yapısı korunur.
- Kod tarafında connection string anahtarı aynı kaldığı için mobil/backend config katmanında daha az sürtünme olur.

Ek kontrol noktaları:
- `DateTime` davranışları
- seed data migration'ları
- collation / string karşılaştırma farkları
- nullable ve numeric alanların provider davranışı

## 7. Neon connection string env variable planı
Önerilen yaklaşım:
- Runtime API için:
  - `ConnectionStrings__DefaultConnection`
- İsteğe bağlı migration/admin ayrımı için:
  - `ConnectionStrings__MigrationConnection`

Pratik öneri:
- Uygulama runtime'da Neon pooled connection string kullanabilir.
- `dotnet ef database update` gibi migration/admin işlerinde direct connection string tercih edilmeli.

Sebep:
- Neon docs, web uygulamaları için pooled bağlantının uygun olduğunu söylüyor.
- Aynı docs, schema migration gibi işlemler için direct connection tercih edilmesini öneriyor.

Ek not:
- Connection string elle yazılmamalı.
- Neon console'un ürettiği string baz alınmalı.
- SSL/TLS parametreleri korunmalı.

## 8. Dockerfile planı
Bu repo için öneri:
- Koyeb'e Dockerfile ile deploy etmek en güvenli yol.

Sebep:
- Koyeb git build dokümantasyonunda yerleşik dil akışları var.
- `.NET` için Dockerfile tabanlı deploy daha deterministik olur.

Önerilen Docker yaklaşımı:
- multi-stage build
- `mcr.microsoft.com/dotnet/sdk:9.0` ile build/publish
- `mcr.microsoft.com/dotnet/aspnet:9.0` ile runtime
- çalışma portu sabit olsun
- container içinde `FridgeApp.dll` ayağa kalksın

Port planı:
- `8000` gibi sabit bir port seç
- `ASPNETCORE_URLS=http://0.0.0.0:8000`
- Koyeb exposed port olarak aynı portu kullan

## 9. Koyeb env variable listesi
Başlangıç için yeterli olabilecek env'ler:
- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://0.0.0.0:8000`
- `ConnectionStrings__DefaultConnection=<Neon runtime connection string>`

Opsiyonel log ayarları:
- `Logging__LogLevel__Default=Information`
- `Logging__LogLevel__Microsoft.AspNetCore=Warning`

Not:
- Koyeb web service'lerde `PORT` env'i otomatik tanımlayabiliyor.
- Buna bağımlı dinamik çözüm de kurulabilir.
- Bu repo için ilk deploy'da sabit port yaklaşımı daha sade olur.

## 10. Health check endpoint planı
Şu an:
- Ayrı bir `/health` endpoint yok.
- Koyeb varsayılan TCP health check ile ilk aşamada ayağa kaldırabilir.

Öneri:
- Deploy öncesi veya hemen sonrasında bir HTTP health endpoint ekle:
  - `/health`
  - veya `/api/health`
- Sonra Koyeb health check'i HTTP path bazlı yapılandır.

Minimum beklenti:
- `200 OK`
- DB bağımlı olmayan hafif bir cevap

## 11. `dotnet build` / `dotnet ef database update` komutları

### Build
```powershell
dotnet build
```

### Migration ekleme
```powershell
./.config-tools/dotnet-ef migrations add <MigrationName>
```

### Database update
```powershell
./.config-tools/dotnet-ef database update
```

Notlar:
- Eğer runtime için pooled Neon string kullanılacaksa, migration komutlarında direct connection ile çalışmak daha güvenlidir.
- Migration çalıştırmadan önce hangi DB'ye uygulandığı net şekilde teyit edilmeli.

## 12. Koyeb deploy sonrası test edilecek endpointler
Swagger production'da şu an açık olmayabilir, bu yüzden smoke test raw endpoint üzerinden yapılmalı.

Önerilen test sırası:
1. `GET /api/fridges`
2. `GET /api/productcatalog/quick-add`
3. `GET /api/productcatalog/categories`
4. `POST /api/users`
5. `POST /api/fridges`
6. `POST /api/items`
7. `GET /api/items`
8. `GET /api/items/expiring?fridgeId={id}&filter=next7Days`
9. `PATCH /api/items/{id}/quantity?delta=-1`
10. `GET /api/recipes/suggestions?fridgeId={id}`
11. `GET /api/shopping-list?userId={id}`
12. `POST /api/shopping-list`
13. `GET /api/fridges/{fridgeId}/quick-add-items`
14. `POST /api/fridges/{fridgeId}/quick-add-items`

## 13. Deploy öncesi ek kontrol notları
- `UseHttpsRedirection()` şu an kapalı. Production davranışı ayrıca değerlendirilmelidir.
- CORS şu an çok geniş açık. Mobil uygulama netleştiğinde kısıtlanması planlanmalı.
- Auth yok. Production'a çıkmadan önce risk kabulü net olmalı.
- `appsettings.json` secrets temizliği yapılmadan repo paylaşılmamalı.
