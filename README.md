# InquirySample

سرویس استعلام کالا بر اساس **کد تعرفه (HsCode)**. سرویس اصلی درخواست را دریافت می‌کند، از بین چند سرویس‌دهنده‌ی خارجی (Provider) به ترتیب اولویت استعلام می‌گیرد و نتیجه را همراه با سابقه‌ی تلاش‌ها (Attempt) در دیتابیس ذخیره می‌کند.

## ۱. روش اجرای پروژه

### پیش‌نیازها

- .NET SDK 10.0
- Docker و Docker Compose
- SQL Server (در تنظیمات پیش‌فرض: instance محلی با Windows Authentication)
- گواهی توسعه‌ی HTTPS (فقط یک بار):
  ```bash
  dotnet dev-certs https --trust
  ```

### مرحله ۱: بالا آوردن Redis

```bash
docker-compose up -d
```

- Redis روی پورت `6379` بالا می‌آید.
- RedisInsight (رابط گرافیکی) روی `http://localhost:8080` در دسترس است (اختیاری).

### مرحله ۲: ساخت دیتابیس‌ها

دو اسکریپت زیر (در ریشه‌ی پروژه) را روی SQL Server اجرا کنید. هر دو خودشان دیتابیس را می‌سازند و داده‌ی تستی اضافه می‌کنند.

| فایل | دیتابیس | محتوا |
|---|---|---|
| `InquirySampleDbScript.sql` | `InquirySampleDb` | جدول‌های سرویس اصلی، ۲۰۰ کالا (HsCode از `100000` تا `100199`) و دو Provider (آدرس‌ها و اولویت‌ها) |
| `ProviderDbScript.sql` | `ProvidersDb` | جدول کالاهای Providerها با ۲۰۰ HsCode و وضعیت مجوز تصادفی (`IsLicenseValid`) |

اجرا با SSMS یا خط فرمان:

```bash
sqlcmd -S . -E -i InquirySampleDbScript.sql
sqlcmd -S . -E -i ProviderDbScript.sql
```

> اگر SQL Server شما Windows Authentication ندارد یا روی instance دیگری است، connection stringها را در `appsettings.Development.json` هر سه پروژه‌ی WebAPI تغییر دهید.

### مرحله ۳: اجرای سرویس‌ها

هر دستور در یک ترمینال جدا، از ریشه‌ی پروژه. ابتدا Providerها:

```bash
dotnet run --project InquirySample.ProviderA.WebAPI --launch-profile https
dotnet run --project InquirySample.ProviderB.WebAPI --launch-profile https
```

سپس سرویس اصلی:

```bash
dotnet run --project InquirySample.WebAPI --launch-profile https
```

| سرویس | آدرس HTTPS |
|---|---|
| سرویس اصلی (Swagger) | `https://localhost:7216/swagger` |
| Provider A | `https://localhost:7021` |
| Provider B | `https://localhost:7022` |

> آدرس Providerها در جدول `Providers` دیتابیس `InquirySampleDb` ذخیره شده و باید با پورت‌های بالا یکی باشد. لیست Providerها هنگام بالا آمدن سرویس اصلی یک‌بار خوانده می‌شود؛ با تغییر آن‌ها سرویس اصلی را دوباره اجرا کنید.

### نمونه درخواست

```bash
curl -k -X POST https://localhost:7216/api/inquiries \
  -H "Content-Type: application/json" \
  -d '{"hsCode":"100000"}'
```

نمونه پاسخ:

```json
{
  "id": 1,
  "productId": 1,
  "hsCode": "100000",
  "providerId": 1,
  "createdAt": "2026-10-17T10:00:00Z",
  "completedAt": "2026-10-17T10:00:00Z",
  "output": "True",
  "inquiryStatus": 0,
  "status": "Success"
}
```

| وضعیت HTTP | معنی |
|---|---|
| `200` | استعلام انجام شد (وضعیت نهایی در فیلد `status`: `Success` یا `Failed`) |
| `404` | کالایی با این HsCode وجود ندارد |
| `400` | خطای اعتبارسنجی دامنه |
| `500` | خطای پیش‌بینی‌نشده یا خطای دیتابیس |

### اجرای تست‌ها

```bash
dotnet test
```

### تست بار (اختیاری)

اسکریپت‌های [k6](https://k6.io) در ریشه‌ی پروژه هستند. سرویس‌ها باید بالا باشند:

```bash
k6 run test.js     # تا ۵۰۰ کاربر همزمان
k6 run -e SLEEP=1 test2.js    # تا ۱۰۰۰ کاربر همزمان
```

با تغییر فایل ورودی در اسکریپت از `hscodes.txt` (کدهای متفاوت) به `hscodesSame.txt` (کدهای تکراری) می‌توان رفتار کش و قفل را زیر بار هم‌زمان درخواست‌های یکسان بررسی کرد.

## ۲. توضیح مختصر معماری

پروژه از سه سرویس مستقل و یک کتابخانه‌ی مشترک تشکیل شده است:

```
InquirySample.WebAPI              سرویس اصلی (Controller)
InquirySample.Application         Use caseها (MediatR)، Behaviorها، Interfaceها
InquirySample.Domain              موجودیت‌ها و قواعد دامنه
InquirySample.Infrastruture       EF Core، Redis، ارتباط با Providerها
InquirySample.BuildingBlocks      DTOها و Enumهای مشترک بین سرویس اصلی و Providerها
InquirySample.ProviderA.WebAPI    شبیه‌ساز سرویس‌دهنده A
InquirySample.ProviderB.WebAPI    شبیه‌ساز سرویس‌دهنده B
InquirySample.Application.Test    تست‌های واحد (xUnit, Moq)
```

وابستگی لایه‌ها: `WebAPI → Application → Domain` و `Infrastruture → Application`. لایه‌ی Application فقط به Interface (مثل `IRedisService` و `IInquiryExecutionService`) وابسته است و پیاده‌سازی در Infrastruture قرار دارد.

### جریان یک استعلام

1. `POST /api/inquiries` با HsCode دریافت می‌شود و `InquiryCommand` از طریق MediatR اجرا می‌شود.
2. کالا با `GetProductByHsCodeQuery` پیدا می‌شود (این Query از طریق `CachingBehavior` در Redis کش می‌شود). اگر کالا نباشد، خطای `NotFound` برمی‌گردد.
3. یک `Inquiry` با وضعیت `Pending` ذخیره می‌شود.
4. `InquiryExecutionService` اجرا می‌شود:
   - برای هر HsCode یک قفل داخل‌پردازشی گرفته می‌شود تا درخواست‌های هم‌زمانِ یکسان، یک‌بار به Provider برود.
   - اگر نتیجه‌ی همان HsCode در Redis باشد، از آن استفاده می‌شود و Attemptهای درخواست اول به‌صورت «اشتراکی» (`SharedInquiryId`) برای این استعلام ثبت می‌شود.
   - در غیر این صورت Providerهای فعال به ترتیب `Priority` صدا زده می‌شوند. اگر یکی موفق نباشد، نوبت بعدی است و هر تلاش به‌صورت یک `ProviderAttempt` ثبت می‌شود.
5. وضعیت `Inquiry` به `Success` یا `Failed` تغییر می‌کند و ذخیره می‌شود.

### نقش Redis

- کش نتیجه‌ی استعلام هر HsCode (کلید `inquiry:{hsCode}`، ماندگاری ۱ ثانیه)
- کش Query پیدا کردن کالا (ماندگاری ۵۰۰ میلی‌ثانیه)

### رفتار Providerها

| | Provider A | Provider B |
|---|---|---|
| اولویت | ۱ | ۲ |
| قاعده‌ی HsCode | ۶ رقم است و رقم آخر نباید `4`، `6` یا `9` باشد، وگرنه `400` | بدون محدودیت |
| خروجی | وضعیت مجوز کالا (`True`/`False`) | وضعیت مجوز کالا (`True`/`False`) |

### وضعیت هر تلاش (`RequestStatus`)

| وضعیت | شرط |
|---|---|
| `Success` | پاسخ `200` معتبر |
| `BussinessFailed` | پاسخ `400` از Provider |
| `Timeout` | گذشتن از `TimeoutMs` یا پاسخ `408`/`504` |
| `TechnicalError` | بقیه‌ی خطاها (قطع بودن سرویس، پاسخ نامعتبر، `404`، `500` و ...) |

## ۳. تصمیمات مهم فنی

**CQRS با MediatR و Pipeline Behavior**
منطق هر use case در یک Handler جدا قرار گرفته و موضوعات مشترک به‌جای تکرار، در Behaviorها پیاده شده‌اند: `ExceptionHandlingBehavior` برای تبدیل استثناها به خطا و ثبت در `ErrorLogs`، و `CachingBehavior` برای کش Queryها (هر Query که `ICacheableQuery` را پیاده کند خودکار کش می‌شود).

**ErrorOr به‌جای استثنا برای خطاهای قابل‌انتظار**
خطاهایی مثل «کالا یافت نشد» جزو مسیر عادی برنامه‌اند و با `ErrorOr` برگردانده می‌شوند. Controller آن‌ها را به کد HTTP مناسب نگاشت می‌کند. استثنا فقط برای وضعیت‌های غیرمنتظره می‌ماند.

**جدا کردن `IReadDbContext` و `IWriteDbContext`**
مسیر خواندن فقط `IQueryable` با `AsNoTracking` می‌دهد. نوشتن از مسیر جدا انجام می‌شود. هر دو توسط یک `ApplicationDbContext` پیاده شده‌اند، پس بدون پیچیدگی دو دیتابیس، مرز خواندن/نوشتن مشخص است.

**Domain با رفتار برای `Inquiry` و `ProviderAttempt`**
سازنده‌ها private هستند و تغییر وضعیت فقط از متدهایی مثل `MarkAsSuccess` و `MarkAsFailed` ممکن است که انتقال‌های نامعتبر را با `DomainException` رد می‌کنند. موجودیت `Provider` عمداً ساده است، چون داده‌ی پیکربندی است و با اسکریپت وارد می‌شود.

**کش نتیجه و قفل به ازای هر HsCode**
وقتی چند درخواست هم‌زمان برای یک HsCode می‌رسد، فقط اولی به Provider می‌رود و بقیه از نتیجه‌ی کش‌شده استفاده می‌کنند. این کار فشار روی سرویس‌دهنده‌های خارجی را کم می‌کند. برای اینکه سابقه‌ی استعلام کامل بماند، Attemptهای درخواست اول برای درخواست‌های بعدی هم با `SharedInquiryId` ثبت می‌شود.
*محدودیت شناخته‌شده:* قفل `SemaphoreSlim` فقط در یک پروسه معتبر است. با چند نمونه از سرویس اصلی باید با قفل توزیع‌شده (مثلاً روی Redis) جایگزین شود.

**Fallback بین Providerها با Timeout مستقل**
هر Provider `TimeoutMs` مخصوص خودش را دارد (با `CancellationTokenSource` مرتبط). خطای فنی، Timeout یا شکست تجاری یک Provider، سرویس را متوقف نمی‌کند و نوبت به Provider بعدی می‌رسد. شمارش وضعیت هر تلاش (جدول بالا) برای گزارش‌گیری و عیب‌یابی ذخیره می‌شود.

**بارگذاری Providerها در حافظه هنگام راه‌اندازی**
لیست Providerها در `IHostedService` یک‌بار از دیتابیس خوانده و در یک Singleton نگه داشته می‌شود تا در هر درخواست کوئری اضافه نخورد.

**ثبت خطا در دیتابیس**
خطاهای پیش‌بینی‌نشده در جدول `ErrorLogs` ذخیره می‌شوند و کاربر پیام عمومی می‌گیرد، نه جزئیات داخلی.

**تست‌های واحد**
تست Handlerها با xUnit، Moq (برای وابستگی‌هایی مثل سرویس اجرای استعلام) و EF Core InMemory و FluentAssertions نوشته شده است.

## ۴. فرضیاتی که در انجام تسک داشته‌ام

- HsCode به‌صورت رشته‌ی عددی ۶ رقمی است و کالاها از قبل در دیتابیس وجود دارند. استعلام برای کد ناموجود، `404` می‌دهد.
- Providerها سرویس‌های خارجی‌اند و با یک endpoint ثابت (`POST /api/inquiry`) و قرارداد مشترک (`ProviderRequestDto` / `ProviderResponseDto`) صدا زده می‌شوند.
- Provider A برای HsCodeهایی که رقم آخرشان `4`، `6` یا `9` است، رد می‌کند (شکست تجاری)؛ در این حالت استعلام به Provider B می‌رسد.
- اولویت و زمان انتظار هر Provider از دیتابیس خوانده می‌شود و پیش‌فرض `2000` میلی‌ثانیه است.
- نتیجه‌ی استعلام هر HsCode (چه موفق چه ناموفق) فقط ۱ ثانیه معتبر است؛ یعنی هدف کش، کنترل درخواست‌های هم‌زمان یکسان است، نه نگهداری بلندمدت نتیجه.
- داده‌ی Providerها و کالاها تستی است و با اسکریپت SQL وارد می‌شود؛ در محیط واقعی مدیریت آن‌ها خارج از این سرویس است.
- محیط اجرا Development است و SQL Server روی همان ماشین با Windows Authentication در دسترس است.
