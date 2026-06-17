# PROJECT_CONTEXT_API.md

Bu dosya, yeni Codex ortamında backend projesini hızlıca devralmak için hazırlanmış handoff özetidir. Kod değişirse bu doküman yeniden doğrulanmalıdır; kaynak gerçeklik her zaman koddur.

## 1. Projenin amacı
- `FridgeApp`, kullanıcıların buzdolabı ürünlerini tutabildiği bir backend/API projesidir.
- MVP odağı:
  - kullanıcı oluşturma
  - dolap oluşturma
  - ürün ekleme ve listeleme
  - son kullanma tarihi takibi
  - hızlı tüketim / hızlı güncelleme
  - alışveriş listesi
  - temel tarif önerileri
  - favori hızlı ekle
- Yakın dönem hedefi: backend'i GitHub, Neon PostgreSQL ve Koyeb'e hazırlamak.

## 2. Backend mimarisi
- Mimari sade tutulmuş durumda: `Controller -> Service -> AppDbContext`.
- `Controllers` HTTP ve route yönetiyor.
- `Services` iş kurallarını ve EF Core çağrılarını topluyor.
- `Entities` veritabanı tablolarını temsil ediyor.
- `Models` request/response modellerini içeriyor.
- `AppDbContext`, EF Core veri erişim katmanı.
- Auth/JWT henüz yok.
- `UserId` ve `FridgeId` halen client tarafından taşınan alanlar.

## 3. Klasör yapısı
- `Controllers`: API endpoint dosyaları
- `Data`: `AppDbContext`
- `Entities`: kalıcı veri modelleri
- `Enums`: takip ve aksiyon enum'ları
- `Interfaces`: service interface'leri
- `Migrations`: EF Core migration dosyaları
- `Models`: request/response modelleri
- `Services`: business logic servisleri
- `Program.cs`: DI, CORS, Swagger, DbContext provider kaydı
- `appsettings.json`: secret içermeyen boş `DefaultConnection` placeholder'ı.
- `Dockerfile`: Koyeb/container deploy için multi-stage .NET 9 build
- `.dockerignore`: image dışında kalacak local/temp/secret dosyaları

## 4. Controller listesi
- `UsersController`
- `FridgesController`
- `ItemsController`
- `ProductCatalogController`
- `FridgeQuickAddItemsController`
- `RecipesController`
- `ShoppingListController`

## 5. Service listesi
- `UserService`
- `FridgeService`
- `ItemService`
- `ProductCatalogService`
- `FridgeQuickAddService`
- `RecipeSuggestionService`
- `ShoppingListService`

## 6. Model/entity listesi

### Entities
- `User`
- `Fridge`
- `Item`
- `ProductDefinition`
- `ShoppingListItem`
- `ItemActivityLog`
- `FridgeQuickAddItem`

### Models
- `CreateUserRequest`
- `CreateFridgeRequest`
- `CreateItemRequest`
- `CreateShoppingListItemRequest`
- `CreateFridgeQuickAddItemRequest`
- `ProductCatalogItemResponse`
- `RecipeSuggestionResponse`
- `ItemQuickUpdateResponse`

## 7. Enum listesi
- `TrackingType`
  - `Countable`
  - `Approximate`
- `ApproximateItemStatus`
  - `Unknown`
  - `Low`
  - `Half`
  - `AlmostFinished`
  - `Finished`
- `ItemType`
  - `Unknown`
  - `OpenProduct`
  - `ClosedProduct`
- `ItemActivityActionType`
  - `Added`
  - `Increased`
  - `Decreased`
  - `MarkedLow`
  - `MarkedHalf`
  - `MarkedAlmostFinished`
  - `Finished`
  - `Deleted`

## 8. DbContext yapısı
`AppDbContext` içinde şu `DbSet`'ler var:
- `Items`
- `ProductDefinitions`
- `ShoppingListItems`
- `ItemActivityLogs`
- `Fridges`
- `FridgeQuickAddItems`
- `Users`

Ek notlar:
- `FridgeQuickAddItem -> Fridge` ilişkisi cascade delete.
- `FridgeQuickAddItem -> ProductDefinition` ilişkisi nullable ve `SetNull`.
- `ProductDefinition` için seed data var.
- Seed edilen temel katalog ürünleri:
  - `Yumurta`
  - `Sut`
  - `Yogurt`
  - `Peynir`
  - `Tereyagi`
  - `Tavuk`
  - `Kiyma`
  - `Domates`

## 9. Mevcut endpointler

### Users
- `POST /api/users`
- `GET /api/users`

### Fridges
- `POST /api/fridges`
- `GET /api/fridges`

### Items
- `GET /api/items`
- `GET /api/items/expiring?fridgeId={id}&filter={expired|next3Days|next7Days}`
- `POST /api/items`
- `PATCH /api/items/{id}/quantity?delta={int}`
- `PATCH /api/items/{id}/approximate-status?status={Low|Half|AlmostFinished|Finished}`
- `PATCH /api/items/{id}/finish`
- `DELETE /api/items/{id}`

### Product Catalog
- `GET /api/productcatalog/quick-add`
- `GET /api/productcatalog/search?query=&category=`
- `GET /api/productcatalog/categories`

### Fridge Quick Add
- `GET /api/fridges/{fridgeId}/quick-add-items`
- `POST /api/fridges/{fridgeId}/quick-add-items`
- `DELETE /api/fridges/{fridgeId}/quick-add-items/{quickAddItemId}`

### Recipes
- `GET /api/recipes/suggestions?fridgeId={id}`

### Shopping List
- `GET /api/shopping-list?userId={id}`
- `POST /api/shopping-list`
- `PATCH /api/shopping-list/{id}/complete`
- `DELETE /api/shopping-list/{id}`

### Health
- `GET /health`

## 10. Mevcut request/response modelleri

### Request modelleri
- `CreateUserRequest`
  - `Email`
  - `PasswordHash`
- `CreateFridgeRequest`
  - `UserId`
  - `Name`
- `CreateItemRequest`
  - `FridgeId`
  - `ProductDefinitionId?`
  - `Name?`
  - `Quantity`
  - `Unit?`
  - `ExpirationDate?`
  - `TrackingType?`
  - `ItemType`
  - `IsOpened`
  - Kural: `ProductDefinitionId` veya `Name` alanlarından en az biri dolu olmalı.
- `CreateShoppingListItemRequest`
  - `UserId`
  - `Name`
  - `Source?`
- `CreateFridgeQuickAddItemRequest`
  - `ProductDefinitionId?`
  - `Name?`
  - `DefaultUnit?`
  - `TrackingType?`
  - `QuickAmounts?`

### Response modelleri
- `ProductCatalogItemResponse`
  - `Id`
  - `ProductDefinitionId?`
  - `QuickAddItemId?`
  - `Name`
  - `Category`
  - `SubCategory?`
  - `DefaultUnit`
  - `QuickAmounts`
  - `TrackingType`
  - `IsQuickAdd`
  - `IsCustom`
- `RecipeSuggestionResponse`
  - `RecipeName`
  - `Description?`
  - `Category?`
  - `EstimatedPrepTimeMinutes?`
  - `MatchedItems`
  - `MissingItems`
  - `Ingredients`
  - `PreparationSteps`
  - `Score`
  - `CanMakeNow`
- `ItemQuickUpdateResponse`
  - `Id`
  - `Name`
  - `Quantity`
  - `Unit`
  - `TrackingType`
  - `ApproximateStatus`
  - `IsFinished`

### Doğrudan entity dönen akışlar
- `User`
- `Fridge`
- `Item`
- `ShoppingListItem`

## 11. ItemsController endpointleri

### `GET /api/items`
- Tüm aktif item'ları döndürür.
- `IsDeleted = true` olanlar gelmez.
- Şu anda `fridgeId` bazlı filtre yok.

### `GET /api/items/expiring`
- Query:
  - `fridgeId`
  - `filter`
- Desteklenen filtreler:
  - `expired`
  - `next3Days`
  - `next7Days`
- `ExpirationDate = null` item'lar bu listelere dahil edilmez.
- Sonuçlar tarihe göre sıralanır.

### `POST /api/items`
- `CreateItemRequest` alır.
- `ProductDefinitionId` verilirse:
  - katalogdan ürün aranır
  - `Name` boşsa katalog adı kullanılır
  - `Unit` boşsa katalog `DefaultUnit` kullanılır
  - `TrackingType` katalogdan gelir
- `ProductDefinitionId` yoksa manuel ürün adı ile çalışabilir.
- Ek olarak `ItemActivityLog` içine `Added` kaydı düşülür.

### `PATCH /api/items/{id}/quantity`
- Sadece `TrackingType = Countable` item'larda anlamlıdır.
- `delta` pozitifse artırır, negatifse azaltır.
- Negatif miktar oluşmaz, taban `0`'dır.
- `delta = 0` geçersizdir.
- Aktivite logu:
  - `Increased`
  - `Decreased`

### `PATCH /api/items/{id}/approximate-status`
- Sadece approximate mantığı içindir.
- Durumlar:
  - `Low`
  - `Half`
  - `AlmostFinished`
  - `Finished`
- `Finished` gelirse item soft delete olur.
- Aktivite logu:
  - `MarkedLow`
  - `MarkedHalf`
  - `MarkedAlmostFinished`
  - `Finished`

### `PATCH /api/items/{id}/finish`
- Ürünü bitti olarak işaretler.
- `Quantity = 0`
- `IsDeleted = true`
- `DeletedAt = now`
- Approximate item ise `ApproximateStatus = Finished`
- Aktivite logu: `Finished`

### `DELETE /api/items/{id}`
- Fiziksel silme yapmaz.
- Soft delete yapar:
  - `IsDeleted = true`
  - `DeletedAt = now`
- Aktivite logu: `Deleted`

## 12. ShoppingList endpointleri

### `GET /api/shopping-list?userId={id}`
- Kullanıcının liste öğelerini döndürür.
- Önce tamamlanmamışlar, sonra yeniler.

### `POST /api/shopping-list`
- Basit alışveriş hatırlatma öğesi oluşturur.
- Quantity ve unit zorunlu değildir.
- `Source` string alanı opsiyoneldir.

### `PATCH /api/shopping-list/{id}/complete`
- Öğeyi tamamlandı olarak işaretler.
- `IsCompleted = true`

### `DELETE /api/shopping-list/{id}`
- Fiziksel siler.
- Shopping list için şu an soft delete yok.

## 13. Recipes endpointleri

### `GET /api/recipes/suggestions?fridgeId={id}`
- Kural bazlı öneri üretir.
- Ayrı tarif tablosu yok.
- Tarif tanımları `RecipeSuggestionService` içinde in-memory tutulur.
- Mevcut durumda 24 sabit tarif tanımı vardır.

## 14. Quick Add / FridgeQuickAdd endpointleri

### Product catalog tabanlı endpointler
- `GET /api/productcatalog/quick-add`
  - `ProductDefinition.IsQuickAdd = true` olan katalog ürünlerini döndürür.
- `GET /api/productcatalog/search?query=&category=`
  - isim arama + kategori filtresi
  - fuzzy search yok
- `GET /api/productcatalog/categories`
  - aktif ürünlerden distinct kategori listesi üretir

### Dolap bazlı favori hızlı ekle endpointleri
- `GET /api/fridges/{fridgeId}/quick-add-items`
  - seed katalog quick add ürünleri ile dolaba özel favorileri birleştirir
- `POST /api/fridges/{fridgeId}/quick-add-items`
  - katalog ürününden veya manuel girişten favori hızlı ekle öğesi oluşturur
- `DELETE /api/fridges/{fridgeId}/quick-add-items/{quickAddItemId}`
  - fiziksel silmez
  - `IsActive = false` yapar

## 15. Home dashboard'ın hangi endpointlerden veri topladığı
- Backend tarafında özel bir `home dashboard` endpoint'i yok.
- Mevcut yapıdaki dashboard verisi client tarafında birleştirilmeye uygundur.
- Mobil tarafın bugün toplayabileceği ana kaynaklar:
  - `GET /api/items/expiring?fridgeId=...&filter=expired`
  - `GET /api/items/expiring?fridgeId=...&filter=next3Days`
  - `GET /api/items/expiring?fridgeId=...&filter=next7Days`
  - `GET /api/recipes/suggestions?fridgeId=...`
  - `GET /api/shopping-list?userId=...`
  - `GET /api/fridges/{fridgeId}/quick-add-items`
  - isteğe bağlı olarak `GET /api/items`
  - isteğe bağlı olarak `GET /api/productcatalog/quick-add`
- Yani dashboard aggregation şu anda backend'de değil, istemcidedir.

## 16. Son yapılan önemli geliştirmeler
- Controller'lardan `DbContext` kullanımının service layer'a taşınması
- `ProductDefinition` katalog yapısının eklenmesi
- Quick Add ve tüm ürünlerin tek katalog kaynağından beslenmesi
- `Item` için `TrackingType`, `ItemType`, `IsOpened` ve soft delete desteği
- Countable / Approximate hızlı güncelleme endpointleri
- `ShoppingListItem` yapısı ve servis/controller akışı
- `ItemActivityLog` temelinin eklenmesi
- Kural bazlı tarif öneri servisi
- Dolap bazlı favori hızlı ekle yapısı

## 17. Öncelikli Tüketim mantığı
- Ayrı bir "priority consumption" tablosu veya servisi yok.
- Mevcut mantık `ExpirationDate` üzerinden yürür.
- Öncelik üretimi için mevcut backend dayanakları:
  - `expired`
  - `next3Days`
  - `next7Days`
- `ExpirationDate = null` ürünler öncelikli tüketim listelerine girmez.
- Client, bu sonuçları UI tarafında gruplandırarak "önce bunları tüket" deneyimi oluşturabilir.

## 18. Alışveriş Uyarıları mantığı
- Arka planda çalışan otomatik bildirim/job motoru henüz yok.
- Bu aşamada altyapı iki parçadan oluşur:
  - `ShoppingListItem`
  - item tüketim / bitiş sinyalleri
- `ShoppingListItem.Source` alanı gelecekte şu değerleri taşıyabilecek serbest string alandır:
  - `Manual`
  - `LowStock`
  - `Expired`
  - `RecipeMissing`
- Düşük stok veya biten ürün algısı bugün client tarafından üretilebilir ve shopping list'e yazılabilir.

## 19. Tarif Önerileri mantığı
- Yapay zeka yok, tamamen kural bazlıdır.
- Tarifler `RecipeSuggestionService` içinde sabit tanımlanır.
- Eşleşme mantığı:
  - ürün adları normalize edilir
  - büyük/küçük harf farkı yok sayılır
  - basit Türkçe karakter sadeleştirmesi yapılır
- Öneri çıkması için:
  - en az 1 eşleşen ürün olmalı
  - eksik ürün sayısı en fazla 2 olmalı
- `score = matched / required * 100`
- `CanMakeNow = missingItems.Count == 0`
- Süresi geçmiş veya soft-deleted ürünler öneriye girmez.

## 20. Favori Hızlı Ekle mantığı
- İki kaynak bir araya getirilir:
  - seed katalog quick add ürünleri
  - dolaba özel custom favoriler
- `ProductCatalogItemResponse` hem katalog hem de favori quick add için ortak response modelidir.
- Custom favori eklerken:
  - katalog ürününden türetilebilir
  - tamamen manuel ürün olarak da eklenebilir
- Silme işlemi `IsActive = false` ile yapılır.

## 21. Sesle/Manuel/Bulk ekleme backend etkileri
- Manuel ekleme bugün hazırdır:
  - `POST /api/items`
- Sesli ekleme için ayrı endpoint yok.
  - Beklenen yaklaşım: mobil taraf ses girdisini çözümler ve yine `CreateItemRequest` ile aynı endpoint'e vurur.
- Bulk ekleme endpoint'i yok.
  - İleride gerekiyorsa batch request veya toplu import endpoint'i eklenmeli.
- Katalog arama ve quick add yapısı, sesli/manüel akışları destekleyecek temel zemini sağlar.

## 22. Mevcut DB bağlantı yapısı
- `Program.cs` içinde:
  - `builder.Configuration.GetConnectionString("DefaultConnection")`
  - `options.UseNpgsql(...)`
- `appsettings.json` içinde gerçek connection string yok; `DefaultConnection` boş bırakılmıştır.
- `appsettings.example.json` Neon/PostgreSQL için placeholder bağlantı örneği içerir.
- Production ve Neon bağlantısı `ConnectionStrings__DefaultConnection` environment variable'ı üzerinden verilmelidir.
- `appsettings.Development.json` şu an sadece logging içeriyor.
- Production ortamında uygulama `PORT` env variable'ı varsa onu dinler; yoksa `8080` kullanır.
- Development ortamında launchSettings/local davranış korunur.
- `GET /health` DB bağımsızdır ve Koyeb health check için kullanılabilir.

## 23. PostgreSQL/Neon için dikkat edilmesi gerekenler
- Ana provider `Npgsql.EntityFrameworkCore.PostgreSQL` olarak ayarlanmıştır.
- `Program.cs` içindeki DbContext kaydı `UseNpgsql` kullanır.
- SQL Server provider'ı ile üretilen eski migration'lar Neon için doğrudan doğru kabul edilmemeli.
- Neon tarafında runtime için pooled connection string düşünülebilir.
- Migration ve admin işleri için direct connection string tercih edilmeli.
- Neon tüm bağlantılarda SSL/TLS bekler. Connection string console'dan alınmalı.
- String karşılaştırma/collation davranışları SQL Server ile birebir aynı olmayabilir.
- Tarih/saat ve identity davranışları PostgreSQL tarafında yeniden test edilmeli.
- Seed data ve migration'lar sıfırdan ayağa kalkan Postgres DB üzerinde smoke test edilmelidir.

## 24. Migration durumu
Eski SQL Server migration'ları `Migrations.SqlServerArchive/` içine taşınmıştır:
- `20260415054121_AddProductCatalogSupport`
- `20260415093638_AddItemTrackingSupport`
- `20260415111445_AddShoppingListAndItemActivity`
- `20260604143324_AddFridgeQuickAddItems`

Notlar:
- Arşiv içinde eski `AppDbContextModelSnapshot.cs` de bulunur.
- Aktif PostgreSQL migration: `InitialPostgres`.
- Neon ortamında `database update` çalıştırmadan önce hangi DB'ye uygulanacağı net şekilde teyit edilmelidir.

## 25. Bilinen riskler
- Git history içinde geçmiş secret kalmış olabilir; repo public olmadan önce ayrıca kontrol edilmelidir.
- Auth/authorization yok.
- `UserId` ve `FridgeId` client'tan geliyor; ownership kontrolü zayıf.
- `GET /api/items` tüm aktif item'ları döndürüyor, fridge bazlı değil.
- `Program.cs` içinde `UseHttpsRedirection()` şu anda kapalı yorum satırı halinde.
- CORS policy şu an `AllowAnyOrigin/AllowAnyMethod/AllowAnyHeader`.
- Shopping list delete fiziksel silme yapıyor.
- Arka planda çalışan bildirim/job altyapısı yok.
- Bazı string içeriklerde encoding/mojibake izleri var.

## 26. Bilinen teknik borçlar
- Otomatik test yok.
- CI/CD workflow yok.
- Bazı endpointler entity'yi doğrudan döndürüyor.
- Global soft delete query filter yok; filtreler servis bazında manuel uygulanıyor.
- Shopping list `Source` string tutuluyor, enum değil.
- Tarif listesi kod içine gömülü.
- Pagination ve gelişmiş filtreleme yok.
- Auth gelmeden multi-user davranışı tam güvenli sayılmaz.
- Neon/Koyeb production smoke test ayrıca yapılmalıdır.

## 27. Sonraki plan: GitHub + Neon PostgreSQL + Koyeb deploy
1. Repo içindeki gerçek secret'ları temizle.
2. `.gitignore` ve config politikasını production'a uygun hale getir.
3. Dockerfile ve `/health` ile Koyeb hazırlığını doğrula.
4. Postgres uyumlu migration stratejisini Neon temiz DB üzerinde doğrula.
5. Neon veritabanını ayağa kaldır ve connection string'i env variable olarak bağla.
6. Koyeb üzerinde GitHub bağlantılı deploy kur.
7. Deploy sonrası temel endpoint smoke testlerini çalıştır.
8. Flutter mobil proje ile yeni backend URL uyumluluğunu kontrol et.
