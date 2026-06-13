# Спецификация: Создание привычек и их вывод

## Цель

Позволить пользователю добавлять именованные привычки и просматривать их список через Telegram-бот и REST API, чтобы в будущем ежедневно отмечать их выполнение.

## Скоуп

### Входит
- Создание привычки: Telegram-бот (через кнопку главного меню) + `POST /api/v1/habits`
- Список привычек: Telegram-бот (через кнопку главного меню) + `GET /api/v1/habits`
- Замена заглушек `OpenStubAsync` для `HabitCreate` и `HabitsList` на реальную логику

### Не входит (отложено)
- Архивирование, переименование привычек
- Отметка выполнения (`HabitLog`)
- Статистика (`GET /api/v1/habits/stats`)
- Пагинация списка привычек

---

## Пользовательские сценарии

### Telegram-бот — создание привычки

Точка входа: кнопка **«➕ Привычка»** в главном меню (`MenuCallbacks.HabitCreate`).

```
Пользователь нажимает «➕ Привычка»
Бот (редактирует сообщение):
  "✏️ Введите название привычки:"
  [❌ Отмена]

Пользователь отправляет текст: "Читать книги"
Бот (редактирует сообщение):
  "✅ Привычка «Читать книги» добавлена!"
  [📋 Мои привычки] [🏠 Главное меню]

-- Отмена --
Пользователь нажимает «❌ Отмена»
Бот: возвращается в главное меню
```

**PendingAction:** `HabitNameInput` — бот переходит в это состояние после показа промпта, ждёт следующего текстового сообщения.

Валидация:
- Пустая строка / только пробелы → бот редактирует на: "⚠️ Название не может быть пустым. Введите название привычки:" + кнопка [❌ Отмена]. PendingAction не сбрасывается.
- Длина > 100 символов → бот редактирует на: "⚠️ Название слишком длинное (максимум 100 символов). Введите другое:" + [❌ Отмена].

---

### Telegram-бот — список привычек

Точка входа: кнопка **«📋 Привычки»** в главном меню (`MenuCallbacks.HabitsList`).

```
-- Есть привычки --
Пользователь нажимает «📋 Привычки»
Бот (редактирует сообщение):
  "📋 Ваши привычки:
   1. Читать книги
   2. Зарядка
   3. Медитация"
  [➕ Добавить] [🏠 Главное меню]

-- Список пуст --
Пользователь нажимает «📋 Привычки»
Бот (редактирует сообщение):
  "📋 У вас пока нет привычек."
  [➕ Добавить] [🏠 Главное меню]
```

Список отображает только неархивированные привычки (`IsArchived = false`), отсортированные по `CreatedAt ASC`.

---

### REST API — для веб-дашборда

Оба эндпойнта требуют идентификации пользователя. На текущем этапе — `X-Telegram-Id` header (строка, TelegramId пользователя). В будущем — JWT-аутентификация.

---

## API-контракты

### POST /api/v1/habits

Создать привычку для текущего пользователя.

**Request header:** `X-Telegram-Id: <string>`

**Request body:**
```json
{
  "name": "string — название привычки, 1–100 символов, обязательно"
}
```

**Response 201 Created:**
```json
{
  "id": "guid",
  "name": "string",
  "isArchived": false,
  "createdAt": "2024-01-15T10:00:00Z"
}
```

**Ошибки:**
- `400` — пустое или слишком длинное имя (валидация)
- `404` — пользователь с указанным `X-Telegram-Id` не найден
- `409` — привычка с таким именем у пользователя уже существует (неархивированная)

---

### GET /api/v1/habits

Получить список привычек текущего пользователя (только неархивированные).

**Request header:** `X-Telegram-Id: <string>`

**Query params:**
- `includeArchived=true` — включить архивированные (по умолчанию `false`)

**Response 200 OK:**
```json
[
  {
    "id": "guid",
    "name": "string",
    "isArchived": false,
    "createdAt": "2024-01-15T10:00:00Z"
  }
]
```

**Ошибки:**
- `404` — пользователь с указанным `X-Telegram-Id` не найден

---

## Доменные операции

Новый метод фабрики уже есть: `Habit.Create(userId, name)`. Добавлять ничего не нужно.

### Новый интерфейс: `IHabitService`

```csharp
public interface IHabitService
{
    Task<HabitResponse> CreateAsync(CreateHabitCommand command, CancellationToken ct = default);
    Task<IReadOnlyList<HabitResponse>> GetListAsync(GetHabitsQuery query, CancellationToken ct = default);
}
```

### Команды и запросы

```csharp
// Application/Habits/CreateHabitCommand.cs
public record CreateHabitCommand(Guid UserId, string Name);

// Application/Habits/GetHabitsQuery.cs
public record GetHabitsQuery(Guid UserId, bool IncludeArchived = false);
```

### DTO

```csharp
// Contracts/V1/Habits/HabitResponse.cs
public sealed class HabitResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public bool IsArchived { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
```

---

## Слой данных

### Новый интерфейс: `IHabitRepository`

```csharp
// Application/Habits/IHabitRepository.cs
public interface IHabitRepository
{
    Task AddAsync(Habit habit, CancellationToken ct = default);
    Task<IReadOnlyList<Habit>> GetByUserIdAsync(Guid userId, bool includeArchived, CancellationToken ct = default);
}
```

### Спецификация (Ardalis.Specification)

```csharp
// Application/Habits/GetHabitsSpecification.cs
// Фильтр по UserId + IsArchived, сортировка по CreatedAt ASC
```

### Репозиторий

```csharp
// Data/Repositories/Habits/HabitRepository.cs
// Наследует RepositoryBase<Habit>, реализует IHabitRepository
```

---

## Слой Telegram-бота

### Иерархия callback-хендлеров

По аналогии с `Settings` — раздел привычек образует собственный домен с отдельным префиксом:

| Хендлер | Префикс | Отвечает за |
|---|---|---|
| `MenuCallbackHandler` | `menu` | Точки входа из главного меню: `menu:habits`, `menu:habit:create` |
| `HabitsCallbackHandler` | `habits` | Вся навигация внутри раздела привычек |

`MenuCallbackHandler` открывает раздел и передаёт управление. Всё, что происходит внутри раздела (кнопки внутри списка, кнопки после создания), — через `HabitsCallbackHandler`.

### Новый PendingAction

В `Domain/Accounting/UserBotState.cs` добавить в enum:
```csharp
AwaitingHabitName = 10
```

### Новый `HabitCallbacks`

```csharp
// TelegramBot/Menus/Habits/HabitCallbacks.cs
public static class HabitCallbacks
{
    public const string Prefix = "habits";

    // Кнопка «➕ Добавить» внутри экрана списка привычек
    public const string Add = Prefix + ":add";
}
```

> `MenuCallbacks.HabitsList = "menu:habits"` и `MenuCallbacks.HabitCreate = "menu:habit:create"` — остаются без изменений, это точки входа из главного меню.

### Новый `IHabitsMenuService`

```csharp
public interface IHabitsMenuService
{
    Task OpenListAsync(Guid userId, long chatId, CancellationToken ct);
    Task OpenCreatePromptAsync(Guid userId, long chatId, CancellationToken ct);
    Task ShowCreatedAsync(Guid userId, long chatId, string habitName, CancellationToken ct);
    Task ShowValidationErrorAsync(Guid userId, long chatId, string error, CancellationToken ct);
}
```

### Новый `HabitsCallbackHandler`

```csharp
// TelegramBot/Handlers/Callbacks/HabitsCallbackHandler.cs
public sealed class HabitsCallbackHandler : CallbackHandlerBase
{
    public override string Prefix => HabitCallbacks.Prefix;

    protected override async Task HandleCallbackAsync(Guid userId, CallbackQuery callbackQuery, CancellationToken ct)
    {
        switch (callbackQuery.Data)
        {
            case HabitCallbacks.Add:
                await habitsMenuService.OpenCreatePromptAsync(userId, chatId, ct);
                return;
        }
    }
}
```

### Обновление `MenuCallbackHandler`

Заменить `OpenStubAsync` для `HabitsList` и `HabitCreate`:
```csharp
case MenuCallbacks.HabitsList:
    await habitsMenuService.OpenListAsync(userId, chatId, ct);
    return;

case MenuCallbacks.HabitCreate:
    await habitsMenuService.OpenCreatePromptAsync(userId, chatId, ct);
    return;
```

### Новый `IPendingInputHandler`: `HabitNameInputHandler`

Обрабатывает `PendingAction.AwaitingHabitName`: вызывает `IHabitService.CreateAsync`, показывает результат через `IHabitsMenuService`.

---

## Валидация и граничные случаи

| Случай | Поведение |
|---|---|
| Название пустое / только пробелы | 400 в API; в боте — сообщение об ошибке, PendingAction сохраняется |
| Название длиннее 100 символов | 400 в API; в боте — сообщение об ошибке |
| Пользователь не зарегистрирован (API) | 404 |
| Пользователь не зарегистрирован (бот) | Игнорируется — уже обрабатывается `PendingActionMessageHandler` |
| Дублирующееся название привычки | 409 в API; в боте — сообщение "⚠️ Привычка с таким названием уже существует" |
| Список привычек пуст | Специальное сообщение без нумерации |

---

## Зависимости

- Доменные сущности `Habit` и `HabitLog` — уже реализованы ✅
- Конфигурации EF Core для `Habit` — уже реализованы ✅
- Система `PendingAction` и `UserBotState` — уже реализована ✅
- Кнопки `HabitCreate` и `HabitsList` в главном меню — уже есть ✅
- Заглушки в `MenuCallbackHandler` — нужно заменить на реальные вызовы ⬜
