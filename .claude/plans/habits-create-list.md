# План реализации: Создание привычек и их вывод

Спецификация: [.claude/specs/habits-create-list.md](../specs/habits-create-list.md)

Каждый шаг — атомарная единица изменений: один pull-request или одна логическая ревью-сессия.  
Шаги выполняются последовательно. Перед каждым шагом жду апрув.

---

## Шаг 1 — Application слой: сервис и контракты

**Создаём:**

| Файл | Описание |
|---|---|
| `Application/Habits/IHabitRepository.cs` | Интерфейс репозитория: `AddAsync`, `GetListAsync` |
| `Application/Habits/CreateHabitCommand.cs` | `record CreateHabitCommand(Guid UserId, string Name)` |
| `Application/Habits/GetHabitsQuery.cs` | `record GetHabitsQuery(Guid UserId, bool IncludeArchived = false)` |
| `Contracts/V1/Habits/HabitResponse.cs` | DTO: `Id`, `Name`, `IsArchived`, `CreatedAt` |
| `Application/Habits/HabitMapper.cs` | Extension `Habit.ToResponse()` |
| `Application/Habits/IHabitService.cs` | `CreateAsync`, `GetListAsync` |
| `Application/Habits/HabitService.cs` | Реализация: создание (с дедупликацией по имени), список |

**Изменяем:**

| Файл | Изменение |
|---|---|
| `Application/ServiceRegistry.cs` | `services.AddTransient<IHabitService, HabitService>()` |

**Валидация в `HabitService.CreateAsync`:**
- `name.Trim()` пустой → `ArgumentException`
- длина > 100 → `ArgumentException`
- неархивированная привычка с таким именем уже есть → `InvalidOperationException`

---

## Шаг 2 — Data слой: репозиторий и спецификация

**Создаём:**

| Файл | Описание |
|---|---|
| `Application/Habits/GetHabitsSpecification.cs` | Ardalis-спецификация: фильтр по `UserId` + `IsArchived`, сортировка `CreatedAt ASC` |
| `Data/Repositories/Habits/HabitRepository.cs` | `RepositoryBase<Habit>` + `IHabitRepository`; `BaseQuery` без `Include` (логи не нужны) |

**Изменяем:**

| Файл | Изменение |
|---|---|
| `Data/ServiceRegistry.cs` | `services.AddScoped<IHabitRepository, HabitRepository>()` |

---

## Шаг 3 — PendingAction + Telegram: callbacks и меню привычек

**Изменяем:**

| Файл | Изменение |
|---|---|
| `Domain/Accounting/UserBotState.cs` | Добавить `AwaitingHabitName = 10` в enum `PendingAction` |

**Создаём:**

| Файл | Описание |
|---|---|
| `TelegramBot/Menus/Habits/HabitCallbacks.cs` | `Prefix = "habits"`, `Add = "habits:add"` — кнопка «➕ Добавить» внутри раздела |
| `TelegramBot/Menus/Habits/HabitsListMenuBuilder.cs` | `Build(IReadOnlyList<HabitResponse>)` → `MenuView` со списком + [➕ Добавить (`HabitCallbacks.Add`)] [🏠 Главное меню] |
| `TelegramBot/Menus/Habits/HabitCreatePromptBuilder.cs` | `Build()` → `MenuView` "✏️ Введите название привычки:" + [❌ Отмена (`MenuCallbacks.Main`)] |
| `TelegramBot/Menus/Habits/HabitCreatedMenuBuilder.cs` | `Build(string habitName)` → `MenuView` "✅ Привычка «...» добавлена!" + [📋 Мои привычки (`HabitCallbacks.Add` → список)] [🏠 Главное меню] |
| `TelegramBot/Services/Habits/IHabitsMenuService.cs` | Интерфейс: `OpenListAsync`, `OpenCreatePromptAsync`, `ShowCreatedAsync`, `ShowValidationErrorAsync` |
| `TelegramBot/Services/Habits/HabitsMenuService.cs` | Реализация через `MenuRenderer` + `IHabitService` + `IUserBotStateService` |

> Кнопка "📋 Мои привычки" после создания должна открывать список — используем `MenuCallbacks.HabitsList` (точка входа через `MenuCallbackHandler`).

---

## Шаг 4 — Telegram: хендлеры ввода и callbacks

**Создаём:**

| Файл | Описание |
|---|---|
| `TelegramBot/Handlers/Messages/HabitNameInputHandler.cs` | `IPendingInputHandler` для `PendingAction.AwaitingHabitName`: валидирует текст, вызывает `IHabitService.CreateAsync`, показывает результат через `IHabitsMenuService`; при ошибке валидации — показывает ошибку без сброса `PendingAction` |
| `TelegramBot/Handlers/Callbacks/HabitsCallbackHandler.cs` | `ICallbackHandler` с `Prefix = HabitCallbacks.Prefix`; обрабатывает `habits:add` → `IHabitsMenuService.OpenCreatePromptAsync`; по аналогии с `SettingCallbackHandler` |

**Изменяем:**

| Файл | Изменение |
|---|---|
| `TelegramBot/Handlers/Callbacks/MenuCallbackHandler.cs` | Заменить `OpenStubAsync` для `HabitsList` и `HabitCreate` на `IHabitsMenuService.OpenListAsync` / `OpenCreatePromptAsync` |

---

## Шаг 5 — REST API + регистрация всего

**Создаём:**

| Файл | Описание |
|---|---|
| `Contracts/V1/Habits/CreateHabitRequest.cs` | `record CreateHabitRequest(string Name)` |
| `Web/Controllers/HabitController.cs` | `POST /api/v1/habits` и `GET /api/v1/habits`; идентификация через `X-Telegram-Id` header → `IUserService.GetOrDefaultAsync` → `IHabitService` |

**Изменяем:**

| Файл | Изменение |
|---|---|
| `Contracts/V1/ApiRoutesV1.cs` | Добавить константы: `Habits = "/api/v1/habits"` |
| `TelegramBot/ServiceRegistry.cs` | `AddScoped<IHabitsMenuService, HabitsMenuService>()`, `AddScoped<ICallbackHandler, HabitsCallbackHandler>()`, `AddScoped<IPendingInputHandler, HabitNameInputHandler>()` |

---

## Порядок проверки (после шага 5)

1. Telegram: `/start` → [➕ Привычка] → ввести имя → видим подтверждение
2. Telegram: [📋 Привычки] → видим созданную привычку
3. Telegram: [➕ Привычка] → ввести пустую строку → видим ошибку, можно снова ввести
4. REST: `POST /api/v1/habits` с `X-Telegram-Id` → `201`
5. REST: `GET /api/v1/habits` → список привычек
6. REST: `POST /api/v1/habits` с дублирующимся именем → `409`
7. REST: `POST /api/v1/habits` без заголовка / с несуществующим ID → `404`
