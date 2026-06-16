/**
 * Конфигурация приложения, собранная из переменных окружения Vite.
 * - `apiBaseUrl` — база REST API; в dev по умолчанию '/api/v1' и проксируется на бэкенд.
 * - `devTelegramId` — Telegram ID для запуска вне Telegram (локальная разработка).
 */
export const appConfig = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? '/api/v1',
  devTelegramId: import.meta.env.VITE_DEV_TELEGRAM_ID || undefined,
} as const
