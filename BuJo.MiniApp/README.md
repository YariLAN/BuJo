# BuJo MiniApp

Telegram Mini App для BuJo — клиент к [BuJo.Backend](../BuJo.Backend) для ведения привычек.
Живёт отдельно от бэкенда, общается с ним по REST.

## Стек

- **React 19 + TypeScript**, сборка **Vite**
- Архитектура **Feature-Sliced Design (FSD)**
- UI: **shadcn/ui** (new-york) + **Tailwind CSS v4**
- HTTP: **Axios**; серверное состояние: **TanStack Query**; клиентское UI-состояние: **Zustand**
- Маршрутизация: **react-router-dom**
- Telegram: **@telegram-apps/sdk-react**

## Архитектура (FSD)

```
src/
├─ app/        # провайдеры (Query, Telegram, Theme), роутер, глобальные стили, точка входа
├─ pages/      # habits (список), habit-stats (статистика привычки)
├─ widgets/    # habit-list (сборка карточек + отметка за сегодня)
├─ features/   # create-habit (диалог создания), toggle-habit-log (отметка ✅)
├─ entities/   # habit (типы, api, react-query хуки, filter-store, HabitCard)
└─ shared/     # api (axios), config (env), lib (telegram, date, cn), store (UI), ui (shadcn)
```

Импорты — строго сверху вниз; публичный API каждого слайса — `index.ts`. Алиас `@` → `src`.

## Требования

- Node.js 20+ (проверено на 24).
- Запущенный **BuJo.Backend** (по умолчанию `http://localhost:5129`).
- Пользователь должен существовать в БД бэкенда — он создаётся ботом при команде `/start`.

## Запуск (dev)

```bash
npm install
cp .env.example .env   # и укажите VITE_DEV_TELEGRAM_ID
npm run dev            # http://localhost:5173
```

Vite проксирует `/api/*` на бэкенд (см. `vite.config.ts`), поэтому CORS в dev не нужен.

### Переменные окружения (`.env`)

| Переменная | По умолчанию | Назначение |
|---|---|---|
| `VITE_API_BASE_URL` | `/api/v1` | База REST API. В dev проксируется на бэкенд; менять обычно нужно только для прод-сборки. |
| `VITE_DEV_TELEGRAM_ID` | — | Telegram ID для запуска вне Telegram. Должен соответствовать существующему пользователю БД. |

## Как работает аутентификация

Бэкенд идентифицирует пользователя по заголовку **`X-Telegram-Id`** на каждом запросе.
- Внутри Telegram заголовок берётся из `initData` (Telegram SDK).
- Вне Telegram (локальная разработка) — из `VITE_DEV_TELEGRAM_ID`.

Логика изолирована в `shared/lib/telegram` и `shared/api` (axios-интерсептор).

## Скрипты

| Команда | Действие |
|---|---|
| `npm run dev` | Dev-сервер Vite |
| `npm run build` | Проверка типов (`tsc -b`) + production-сборка в `dist/` |
| `npm run lint` | ESLint |
| `npm run preview` | Локальный предпросмотр собранного `dist/` |

## Деплой в Telegram

1. `npm run build` → залить `dist/` на HTTPS-хостинг.
2. Указать URL в настройках бота через **@BotFather** (Menu Button / Web App).
3. На бэкенде настроить **CORS** для домена приложения (в dev не требуется благодаря Vite-proxy).
4. Для прод-сборки задать `VITE_API_BASE_URL` (абсолютный URL API).
