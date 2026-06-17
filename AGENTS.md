# AGENTS.md

## Proje Kimliği
- Bu klasör `FridgeApp` backend/API projesidir.
- Teknoloji: ASP.NET Core Web API + Entity Framework Core.
- Mevcut veritabanı sağlayıcısı PostgreSQL / Npgsql'dir.
- Hedef production yönü: Neon PostgreSQL + Koyeb deploy.
- Flutter mobil uygulama ayrı projedir: `fridge_app_mobile`.

## Temel Çalışma Kuralları
- Büyük refactor yapma. Mevcut yapıyı adım adım geliştir.
- Mevcut endpoint route'larını, request shape'lerini ve temel response davranışını kullanıcı açıkça istemedikçe bozma.
- Kod ile doküman çelişirse önce kodu doğrula; doküman yardımcı bağlamdır.
- Mevcut controller -> service -> AppDbContext akışını gereksiz yere yeniden kurma.
- Gereksiz abstraction, generic repository veya büyük mimari sıçrama ekleme.

## Secrets ve Config
- Gerçek connection string, şifre, token, API key, server IP veya local ağ bilgisini Git'e yazma.
- Config dosyasında gerçek secret görürsen değeri tekrar yazma; sadece temizlenmesi gerektiğini raporla.
- Production secret'ları environment variable / secret store üzerinden yönet.

## Build ve Raporlama
- Kod değişikliğinden sonra `dotnet build` çalıştır.
- Migration veya veritabanı şeması değiştirirsen final yanıtta açıkça şunları yaz:
  - hangi dosyalar değişti
  - hangi migration oluştu veya güncellendi
  - hangi komutun çalıştırılması gerektiği
  - veritabanına uygulanıp uygulanmadığı
- Davranış değiştiren riskleri ve mevcut uyumluluk etkilerini saklama.

## Korunması Gerekenler
- Mevcut API davranışını kırma.
- Mevcut mobil istemcinin kullandığı endpointleri keyfi olarak yeniden adlandırma.
- Veritabanı sağlayıcısını kullanıcı açıkça istemedikçe sessizce değiştirme.
- Kullanıcının istemediği kod temizliği/refactor turuna girme.

## Deploy Yönü
- Kısa vadeli hedef: repo'yu GitHub'a temiz ve secrets'sız taşımak.
- Sonraki hedef: PostgreSQL initial migration üretimi ve Neon üzerinde smoke test.
- Production hedefi: Koyeb üzerinde API deploy + Neon PostgreSQL.
