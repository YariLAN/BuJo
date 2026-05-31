# BuJo — Цифровой Bullet Journal

Сервис для ведения задач и привычек с основным взаимодействием через Telegram-бота
и REST API для будущего веб-дашборда. Бэкенд написан на **.NET 10** (ASP.NET Core),
данные хранятся в **PostgreSQL** через **EF Core**, интеграция с Telegram — через
**Telegram.Bot SDK** (long polling).

---

## Структура репозитория

```
BuJo/
└── BuJo.Backend/
    ├── BuJo.Backend.sln
    ├── docker-compose.yml          # host + postgres
    ├── docker-compose.override.yml
    └── src/
        ├── Host/                   # Composition Root (Program.cs, appsettings, Dockerfile)
        ├── Web/                    # ASP.NET Core контроллеры
        ├── Application/            # Use cases / сервисы (CQRS-style commands & queries)
        ├── Contracts/              # DTO / Request / Response, ApiRoutesV1
        ├── Domain/                 # Доменные сущности (Accounting, Habits, Tasks)
        ├── Data/                   # EF Core: DataContext, Configurations, Migrations, Repositories
        ├── TelegramBot/            # Обработчики команд, callback-ов, polling worker
        ├── Integrations/Telegram/  # Планируется как обёртка над Telegram.Bot SDK (см. ниже)
        ├── HostJobs/               # Зарезервировано под фоновые джобы (Hangfire), пока пусто
        └── Common/                 # IHaveConfigSection и прочие cross-cutting расширения
```

Solution-конфиг: [BuJo.Backend.sln](BuJo.Backend/BuJo.Backend.sln).
Точка входа: [Host/Program.cs](BuJo.Backend/src/Host/Program.cs).

> Архитектура — Clean Architecture с разделением на слои `Domain → Application → Contracts → Web/Host`.
> Внешние интеграции (Telegram, PostgreSQL) изолированы в `Integrations` и `Data`.
> Веб-дашборд (React) из исходного дизайн-документа в этом репозитории **пока не реализован** —
> только бэкенд + Telegram-бот.

---

## Доменная модель

### Accounting

[User](BuJo.Backend/src/Domain/Accounting/User.cs) — пользователь Telegram.

| Поле | Тип | Описание |
|---|---|---|
| `Id` | `Guid` | PK |
| `TelegramId` | `string` | Уникальный, индекс |
| `Name` | `string` | Имя/username из Telegram |
| `ReminderMorningTime` | `TimeOnly?` | Время утреннего напоминания |
| `ReminderEveningTime` | `TimeOnly?` | Время вечернего напоминания |
| `CreatedAt`, `UpdatedAt` | `DateTimeOffset` | |
| `Habits` | nav 1-N | Список привычек |
| `Tasks` | nav 1-N | Список задач |

Фабрика: `User.Create(telegramId, name)`. Методы: `SetReminderMorning`, `SetReminderEvening`.

### Habits

[Habit](BuJo.Backend/src/Domain/Habits/Habit.cs) — привычка пользователя.

| Поле | Тип |
|---|---|
| `Id` | `Guid` |
| `UserId` | `Guid` (FK → `User`) |
| `Name` | `string` |
| `IsArchived` | `bool` (default `false`) |
| `CreatedAt`, `UpdatedAt` | `DateTimeOffset` |
| `Logs` | nav 1-N (`HabitLog`) |

Фабрика: `Habit.Create(userId, name)`.

[HabitLog](BuJo.Backend/src/Domain/Habits/HabitLog.cs) — отметка выполнения за день.

| Поле | Тип |
|---|---|
| `Id` | `Guid` |
| `HabitId` | `Guid` (FK → `Habit`) |
| `Date` | `DateOnly` |
| `IsCompleted` | `bool` |

Фабрика: `HabitLog.Create(habitId, date, isCompleted)`.

### Tasks

[Task](BuJo.Backend/src/Domain/Tasks/Task.cs) — задача пользователя.

| Поле | Тип |
|---|---|
| `Id` | `Guid` |
| `UserId` | `Guid` (FK → `User`) |
| `Title` | `string` |
| `Description` | `string?` |
| `Status` | `TaskStatus` enum — `ToDo`, `InProgress`, `Done`, `Cancelled` |
| `StartDateTime` | `DateTimeOffset?` |
| `DueDateTime` | `DateTimeOffset?` |
| `ReminderAt` | `DateTimeOffset?` |
| `IsSentReminder` | `bool` |
| `CreatedAt` | `DateTimeOffset` |

Фабрика: `Task.Create(userId, title, description?, startDate?, dueDate?, reminderAt?)`. Метод: `ChangeStatus(status)`.

> Важно: в текущей реализации статус задачи имеет 4 значения (`Cancelled` добавлен относительно дизайн-документа),
> у задачи есть отдельное `ReminderAt` + флаг `IsSentReminder` — отметка, что напоминание уже было отправлено.

---

## Сервисы / Use Cases (Application слой)

CQRS-light: команды и запросы — `record`-ы, сервисы их принимают на вход.
Доступ к данным — через репозитории + `Ardalis.Specification`.

### Accounting

[IUserService](BuJo.Backend/src/Application/Accounting/IUserService.cs) / [UserService](BuJo.Backend/src/Application/Accounting/UserService.cs)

| Метод | Параметры | Возвращает | Поведение |
|---|---|---|---|
| `GetOrDefaultAsync` | `GetUserQuery` | `UserResponse?` | Поиск по `UserId` / `TelegramId` / `UserName` через [GetUserSpecification](BuJo.Backend/src/Application/Accounting/GetUserSpecification.cs). Возвращает `null`, если пользователь не найден. |
| `CreateAsync` | `CreateUserCommand` | `UserResponse` | Проверяет уникальность по `TelegramId`, создаёт `User` через фабрику, сохраняет, мапит в `UserResponse`. |

Команды/запросы:
- [CreateUserCommand](BuJo.Backend/src/Application/Accounting/CreateUserCommand.cs) — `(string TelegramId, string? Username)`
- [GetUserQuery](BuJo.Backend/src/Application/Accounting/GetUserQuery.cs) — `(Guid? UserId, string? TelegramId, string? UserName)`

Маппер: [UserMapper](BuJo.Backend/src/Application/Accounting/UserMapper.cs) — extension `User.ToResponse()`.

### Habits / Tasks

Сервисы для привычек и задач **пока не реализованы** — есть только доменные сущности и таблицы.
Регистрация в DI: [Application/ServiceRegistry.cs](BuJo.Backend/src/Application/ServiceRegistry.cs).

---

## REST API (Web слой)

Контроллеры лежат в [Web/Controllers/](BuJo.Backend/src/Web/Controllers/),
константы путей — в [Contracts/V1/ApiRoutesV1.cs](BuJo.Backend/src/Contracts/V1/ApiRoutesV1.cs).
В Development подключён Scalar (`/scalar`) + OpenAPI (`/openapi`).

### Реализовано

| Метод | Путь | Контроллер / Handler | Описание |
|---|---|---|---|
| `POST` | `/api/v1/users/register` | [UserController.RegisterAsync](BuJo.Backend/src/Web/Controllers/UserController.cs) | Регистрация по `UserRegisterRequest(TelegramId, Username)`. **На данный момент — заглушка** (возвращает `Ok()`). |
| `GET` | `/WeatherForecast` | `WeatherForecastController` | Тестовый sample-endpoint из шаблона. |

### Подразумевается (из дизайн-документа, ещё не реализовано)

```
GET    /api/v1/habits              — список привычек
POST   /api/v1/habits              — создать
PUT    /api/v1/habits/{id}         — обновить
DELETE /api/v1/habits/{id}         — архивировать
POST   /api/v1/habits/{id}/log     — отметить за дату
GET    /api/v1/habits/stats        — статистика

GET    /api/v1/tasks               — список (фильтры)
POST   /api/v1/tasks               — создать
PUT    /api/v1/tasks/{id}          — обновить
PATCH  /api/v1/tasks/{id}/status   — сменить статус
DELETE /api/v1/tasks/{id}          — удалить

PUT    /api/v1/users/settings      — настройки напоминаний
```

DTO-контракты лежат в [Contracts/V1/](BuJo.Backend/src/Contracts/V1/), пример: [UserResponse](BuJo.Backend/src/Contracts/V1/Accounting/UserResponse.cs).

---

## Telegram-бот

Запускается как `BackgroundService` через long polling
([TelegramPollingWorker](BuJo.Backend/src/TelegramBot/Workers/TelegramPollingWorker.cs)).
Маршрутизация — [UpdateDispatcher](BuJo.Backend/src/TelegramBot/UpdateDispatcher.cs):
текст, начинающийся с `/`, попадает в соответствующий `ICommandHandler`;
остальные типы апдейтов логируются как `Unhandled`.

`ReceiverOptions`: `AllowedUpdates = [Message, CallbackQuery]`, `DropPendingUpdates = true`.

Конфиг: [TelegramOptions](BuJo.Backend/src/TelegramBot/TelegramOptions.cs)
(секция `Telegram:Token` в `appsettings.json` / user-secrets / env).

### Прослойка над Telegram.Bot SDK

В целевой архитектуре доступ к Telegram должен идти не напрямую к `Telegram.Bot`,
а через тонкую обёртку — проект **[Integrations/Telegram/](BuJo.Backend/src/Integrations/Telegram/)**.
Он задумывался как слой адаптации SDK: контракты вроде
[TelegramMessage](BuJo.Backend/src/Integrations/Telegram/Contracts/TelegramMessage.cs) и
[TelegramUpdate](BuJo.Backend/src/Integrations/Telegram/Contracts/TelegramUpdate.cs) уже там лежат —
это будущий «наш» API, на который должны опираться `TelegramBot`-обработчики.

**Текущее состояние:** проект `TelegramBot` использует `Telegram.Bot` SDK **напрямую** —
`UpdateDispatcher` и хендлеры работают с `Telegram.Bot.Types.Update` / `Message`,
а `ITelegramBotClient` инжектится прямо в воркер и хендлеры. Контракты из `Integrations/Telegram`
пока не задействованы. Переход на прослойку — отдельная задача рефакторинга:
`TelegramBot` должен зависеть только от абстракций `Integrations/Telegram`, а сам SDK —
жить за их реализацией внутри `Integrations/Telegram`.

### Реализованные команды

| Команда | Handler | Поведение |
|---|---|---|
| `/start` | [StartCommandHandler](BuJo.Backend/src/TelegramBot/Handlers/Commands/StartCommandHandler.cs) | Проверяет `UserService.GetOrDefaultAsync` по `TelegramId`. Если новый — создаёт пользователя и шлёт welcome-сообщение. Если существующий — шлёт "С возвращением!". |

### Подразумевается (из дизайн-документа)

Команды:
```
/habits         — управление привычками
/tasks          — список задач на сегодня
/add            — быстрое добавление задачи (followup-вопрос: Сегодня / Завтра / Без даты)
/stats          — статистика за месяц
/settings       — настройки напоминаний
```

Callback-handlers (inline-кнопки) — ещё не реализованы. По дизайну:
- `HabitLogCallbackHandler` — обработка ✅/❌ для отметки привычки за день
- `TaskStatusCallbackHandler` — смена статуса задачи (`Todo → InProgress → Done`)

Message-handlers (свободный текст):
- `QuickAddTaskHandler` — превращает произвольное сообщение в задачу

### Расширение бота

Чтобы добавить новую команду:
1. Создать класс в `TelegramBot/Handlers/Commands/`, реализовать `ICommandHandler`
   ([интерфейс](BuJo.Backend/src/TelegramBot/Handlers/ICommandHandler.cs): `Command` + `HandleAsync(Message, CancellationToken)`).
2. Зарегистрировать в [TelegramBot/ServiceRegistry.cs](BuJo.Backend/src/TelegramBot/ServiceRegistry.cs)
   как `ICommandHandler` (DI разрешит коллекцию в `UpdateDispatcher`).

Для callback-ов / нетекстовых апдейтов потребуется расширить `UpdateDispatcher.DispatchAsync`
(сейчас в `switch` только ветка `Message`-команды).

---

## Data слой (EF Core + PostgreSQL)

[DataContext](BuJo.Backend/src/Data/DataContext.cs):
- `DbSet<User>`, `DbSet<Habit>`, `DbSet<Task>`
- `ApplyConfigurationsFromAssembly` — конфигурации из [Data/Configurations/](BuJo.Backend/src/Data/Configurations/)
- `UseEnumConvention` — enum-ы хранятся как `string`
- `MigrateAsync()` вызывается на старте из `Program.MigrateDatabase`

Конфигурации сущностей:
- [UserConfiguration](BuJo.Backend/src/Data/Configurations/Accounting/UserConfiguration.cs) — уникальный индекс на `TelegramId`
- [HabitConfiguration](BuJo.Backend/src/Data/Configurations/HabitConfiguration.cs) — FK `User.Habits`, default `IsArchived = false`
- [HabitLogConfiguration](BuJo.Backend/src/Data/Configurations/HabitLogConfiguration.cs) — FK `Habit.Logs`
- [TaskConfiguration](BuJo.Backend/src/Data/Configurations/TaskConfiguration.cs) — FK `User.Tasks`

Репозитории — [Data/Repositories/](BuJo.Backend/src/Data/Repositories/), наследуют `RepositoryBase<T>`,
работают с `ISpecification<T>` через `Ardalis.Specification`. Сейчас есть только
[UserRepository](BuJo.Backend/src/Data/Repositories/Accounting/UserRepository.cs)
(в `BaseQuery` подтягивает `Tasks` и `Habits` через `Include`).

Подключение: Npgsql, `UseSnakeCaseNamingConvention`, `EnableRetryOnFailure`. Строка соединения — секция
`ConnectionOptions` (см. [ConnectionOptions](BuJo.Backend/src/Data/ConnectionOptions.cs) и
[Data/ServiceRegistry.cs](BuJo.Backend/src/Data/ServiceRegistry.cs)).

Миграции: [Data/Migrations/](BuJo.Backend/src/Data/Migrations/) — `Initial`, `Edit_Db`.

---

## Инфраструктура / запуск

`docker-compose` поднимает два сервиса (см. [docker-compose.yml](BuJo.Backend/docker-compose.yml)):

| Сервис | Что |
|---|---|
| `host` | ASP.NET Core API + встроенный Telegram polling worker. Билдится из [Host/Dockerfile](BuJo.Backend/src/Host/Dockerfile). |
| `db` | `postgres` (`bujo_db` / `postgres` / `postgres`), порт `5447 → 5432`, volume `postgres-data`. |

В отличие от исходного дизайн-документа — **Telegram-бот и API живут в одном процессе** (`Host`),
отдельного `bujo_bot` контейнера нет. Веб-дашборд тоже отсутствует.

Конфигурация — `appsettings.json` + user-secrets (`UserSecretsId` указан в `Host.csproj`).
Минимальный набор ключей:
```
Telegram:Token
ConnectionOptions:ConnectionString   // см. ConnectionOptions
```

---

## Composition Root

[Host/Program.cs](BuJo.Backend/src/Host/Program.cs) собирает приложение через extension-методы из
`ServiceRegistry`-ов каждого слоя:

```csharp
builder.Services
    .AddWeb()
    .AddApplication()
    .AddData(builder.Configuration)
    .AddTelegramBot(builder.Configuration);
```

На старте:
1. Применяются миграции (`DataContext.MigrateAsync`).
2. В Development подключается OpenAPI + Scalar UI.
3. Запускается HTTP-pipeline (`MapControllers`) и `TelegramPollingWorker` как `IHostedService`.

---

## Текущее состояние (важно при планировании задач)

Реализовано:
- Доменная модель `User` / `Habit` / `HabitLog` / `Task` + миграции БД.
- `UserService` (создание / поиск).
- Telegram polling, dispatcher и команда `/start` (поверх `Telegram.Bot` SDK напрямую).

Не реализовано (но описано в дизайн-документе):
- Прослойка `Integrations/Telegram` как обёртка SDK — лежит каркас контрактов, но `TelegramBot`
  всё ещё работает с `Telegram.Bot` напрямую.
- Сервисы и API для `Habit` / `Task` (CRUD, отметка выполнения, статистика, смена статуса).
- Эндпойнт `PUT /api/v1/users/settings` для настройки времени напоминаний.
- Все Telegram-команды кроме `/start`, все callback- и message-handlers.
- Фоновые джобы (`HostJobs` пуст) — отправка утренних/вечерних напоминаний и напоминаний по задачам
  (`Task.ReminderAt` + `IsSentReminder` уже в схеме под это).
- React-дашборд.
