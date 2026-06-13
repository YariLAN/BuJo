# BJ-3 CRUD для задач: вывод списка задач и их создание

## Контекст
В проекте уже есть базовая структура для задач:
- [Domain модель](src/Domain/Tasks/Task.cs) — Task с полями Id, UserId, Title, Description, Status, StartDateTime, DueDateTime, ReminderAt, IsSentReminder, CreatedAt
- [Статусы](src/Domain/Tasks/TaskStatus.cs) — ToDo, InProgress, Done, Archived
- [EF Core конфигурация](src/Data/Configurations/TaskConfiguration.cs)
- [Telegram-заглушки](src/TelegramBot/Handlers/Commands/TasksCommandHandler.cs) — TasksCommandHandler и CreateTaskCommandHandler, которые пока ничего не делают
- [Заготовка ITaskService](src/Application/Tasks/Abstractions/ITaskService.cs)

При этом отсутствует Application-слой для работы с задачами, Web API контроллеры, DTO для запросов/ответов, репозиторий для задач в Data-слое, и Telegram-бот не умеет реально создавать или показывать задачи.

## Цели
- Пользователь может создать новую задачу через Telegram-бота (название, описание, приоритет, дедлайн)
- Пользователь может получить список своих задач через Telegram-бота
- Реализован Web API: GET /api/v1/tasks (список) и POST /api/v1/tasks (создание)
- Данные сохраняются в БД
- Работает фильтрация по статусу
- Application-слой покрыт юнит-тестами

## Сценарии использования

### Сценарий 1: Telegram — просмотр списка задач
1. Пользователь вводит команду `/tasks` в Telegram-боте
2. Бот запрашивает список задач пользователя через Application-слой
3. Если задач нет — бот показывает сообщение «У вас нет задач» с кнопкой «Создать задачу»
4. Если задачи есть — бот показывает нумерованный список задач (название, статус, дедлайн если есть)

### Сценарий 2: Telegram — создание задачи
1. Пользователь нажимает кнопку «Создать задачу» (или вводит команду создания)
2. Бот запрашивает название задачи (обязательно)
3. Бот запрашивает описание (опционально)
4. Бот предлагает выбрать приоритет (Low/Medium/High/Critical)
5. Бот запрашивает дедлайн (опционально, в формате даты)
6. Бот создаёт задачу через Application-слой и подтверждает создание

### Сценарий 3: Web API — получение списка задач
1. Клиент отправляет GET /api/v1/tasks?status=active
2. API возвращает JSON-массив задач пользователя, отфильтрованных по статусу

### Сценарий 4: Web API — создание задачи
1. Клиент отправляет POST /api/v1/tasks с телом {title, description, priority, dueDate}
2. API валидирует входные данные
3. API создаёт задачу и возвращает TaskResponse (id, title, description, priority, dueDate, status, createdAt)

## Out of scope
- Редактирование задачи (Update) — будет в отдельной задаче
- Удаление задачи (Delete) — будет в отдельной задаче
- Пагинация списка задач
- Назначение задач другим пользователям
- Поиск по задачам

## Требования

### Функциональные требования
1. **Domain**: Существующая модель Task не требует изменений. В модель нужно добавить поле Priority (enum).
2. **Application**:
   - Создать `ITaskRepository` в Application/Abstractions
   - Создать `TaskService` с методами `GetTasksAsync(Guid userId, TaskStatus? status)` и `CreateTaskAsync(Guid userId, CreateTaskRequest request)`
   - Создать DTO: `CreateTaskRequest` (title, description, priority, dueDate), `TaskResponse` (id, title, description, priority, dueDate, status, createdAt)
   - Валидация: title не пустой и не длиннее 500 символов
   - Привязка пользователя через TelegramUserId/UserId
   - Логирование через ILogger
3. **Data**:
   - Создать `TaskRepository`, реализующий `ITaskRepository`
   - Зарегистрировать в ServiceRegistry
4. **Web API**:
   - GET /api/v1/tasks — возвращает список задач пользователя, опциональная фильтрация по статусу
   - POST /api/v1/tasks — создаёт новую задачу, возвращает TaskResponse
5. **TelegramBot**:
   - Доработать TasksCommandHandler для реального получения списка задач
   - Доработать CreateTaskCommandHandler / PendingActionMessageHandler для создания задачи через форму (название → описание → приоритет → дедлайн)

### Нефункциональные требования
- Логирование ключевых операций (создание задачи, получение списка)
- Валидация входных данных на уровне Application
- Controller не тянет Domain, DbContext
- Controller не тянет IRepository. Взаимодействие через DI сервисов 
- Unit-тесты для Application-слоя (TaskService)

## Статус
Draft — 2026-06-11

## Branch
`BJ-3-crud-tasks-list-create`