# Коротка документація (українською) — де що знаходиться

Це пояснення для тебе: що за що відповідає в коді. Проєкт — **FitTrack**,
система організації фітнес-тренувань (Zadanie 9), на **C# / ASP.NET Core MVC**
(.NET 8) з базою **SQLite** і **Entity Framework Core**.

## ASP.NET Core MVC — це справжній MVC

На відміну від Django (де патерн називається MTV), ASP.NET Core MVC напряму
реалізує **Model–View–Controller**. Якщо тебе питатимуть «що таке Models,
що таке Controllers» — відповідай так:

- **Models (`FitTrack/Models/` + `FitTrack/Data/`)** — це **дані**.
  Класи описують структуру (`Workout`, `Category`, `Exercise`), а також
  правила перевірки (атрибути `[Required]`, `[StringLength]`, `[Range]`).
  У папці `Data/` лежить `FitTrackContext` — «міст» між класами і базою даних.

- **Controllers (`FitTrack/Controllers/`)** — це **логіка**.
  Приймають запит від браузера, беруть або зберігають дані через модель
  і віддають готову сторінку. Кожен метод (Action) = одна дія: показати
  список, показати деталі, додати, редагувати, видалити. Тут же пошук,
  фільтрація і перевірка «чи залогінений».

- **Views (`FitTrack/Views/`)** — це **зовнішній вигляд** (HTML/Razor `.cshtml`).
  Контролер передає в них дані, а вони малюють таблиці, форми, кнопки.

## Файли і за що вони відповідають

| Файл / папка | За що відповідає |
|---|---|
| `FitTrack.sln` | Файл рішення — відкривається у Visual Studio |
| `FitTrack/Program.cs` | Старт і налаштування застосунку (база, логін, маршрути) |
| `FitTrack/appsettings.json` | Налаштування: рядок підключення до БД + логін/пароль адміна |
| `FitTrack/Models/Workout.cs` | **МОДЕЛЬ** — головна сутність «тренування» |
| `FitTrack/Models/Category.cs` | **МОДЕЛЬ** (додаткова) — категорія/тип тренування |
| `FitTrack/Models/Exercise.cs` | **МОДЕЛЬ** (додаткова) — вправа в тренуванні |
| `FitTrack/Models/Intensity.cs` | Перелік (enum) інтенсивності: Niska/Średnia/Wysoka |
| `FitTrack/Models/WorkoutFilterViewModel.cs` | ViewModel для списку + пошуку/фільтра |
| `FitTrack/Data/FitTrackContext.cs` | Контекст EF Core — доступ до бази, зв'язки |
| `FitTrack/Data/SeedData.cs` | Створення БД і початкові приклади даних |
| `FitTrack/Controllers/WorkoutsController.cs` | **КОНТРОЛЕР** тренувань: список, деталі, CRUD, пошук/фільтр |
| `FitTrack/Controllers/CategoriesController.cs` | **КОНТРОЛЕР** категорій (CRUD) |
| `FitTrack/Controllers/AccountController.cs` | **КОНТРОЛЕР** логіну (вхід/вихід) |
| `FitTrack/Controllers/HomeController.cs` | Головна сторінка + сторінка помилки |
| `FitTrack/Views/Workouts/` | **ВИГЛЯД** тренувань: Index, Details, Create, Edit, Delete |
| `FitTrack/Views/Categories/` | **ВИГЛЯД** категорій |
| `FitTrack/Views/Account/Login.cshtml` | Сторінка входу |
| `FitTrack/Views/Shared/_Layout.cshtml` | Спільний шаблон (меню, шапка, підвал) |
| `FitTrack/wwwroot/` | Статика: CSS, JS, Bootstrap, jQuery (валідація) |
| `FitTrack.Tests/` | Тести (xUnit) — перевіряють контролер тренувань |
| `Dockerfile`, `docker-compose.yml` | Запуск у Docker |
| `README.md` | Офіційна документація для здачі (польською) |

## Як працює один запит (приклад: відкрити список тренувань)

1. Браузер заходить на `/` → маршрут із `Program.cs` направляє до
   `WorkoutsController.Index` (**контролер**).
2. Контролер бере тренування з бази через `FitTrackContext` (**модель**),
   застосовує пошук/фільтр/сортування.
3. Контролер передає дані у `Views/Workouts/Index.cshtml` (**вигляд**),
   який малює таблицю, що бачить користувач.

## Що зроблено на вищу оцінку (≥2 пункти з інструкції)

1. **Дві додаткові моделі + зв'язки** — `Category` і `Exercise` пов'язані
   з головною моделлю `Workout` (зовнішні ключі `CategoryId`, `WorkoutId`).
2. **Пошук, фільтрація і сортування** — по назві, категорії, інтенсивності.
3. **Валідація** — на сервері (атрибути в моделях + `ModelState`) і на
   клієнті (jQuery Validation на основі тих самих атрибутів).
4. **Логін / сесія** — додавати/редагувати/видаляти можна лише після входу
   (`[Authorize]` + cookie-аутентифікація).
5. **Тести** — у `FitTrack.Tests` (xUnit + база в пам'яті).
6. **Docker** — `Dockerfile` + `docker-compose.yml`.
7. **Стилізація** — Bootstrap 5, кольорові бейджі інтенсивності.

## Швидкий запуск (нагадування)

Потрібен **.NET SDK 8.0** (встановити з dotnet.microsoft.com).

```bash
cd Project
dotnet restore
dotnet run --project FitTrack
```

Або просто відкрий `FitTrack.sln` у Visual Studio і натисни **F5**.
База даних і приклади створюються самі при першому запуску.

Логін для додавання тренувань: **admin / Admin123!**

## Запуск тестів

```bash
cd Project
dotnet test
```

## Важлива примітка

Я не зміг скомпілювати проєкт у цьому середовищі (немає .NET SDK і доступу
до інтернету для встановлення). Код написаний строго за стандартними
шаблонами ASP.NET Core MVC 8 (як у курсовому MvcMovie). Перед здачею
просто відкрий його у Visual Studio і запусти (`F5`) або `dotnet run` —
якщо щось загориться, напиши мені, поправлю.
