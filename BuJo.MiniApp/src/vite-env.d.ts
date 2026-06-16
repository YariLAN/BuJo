/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Базовый URL REST API. В dev по умолчанию '/api/v1' (проксируется Vite на бэкенд). */
  readonly VITE_API_BASE_URL?: string
  /** Telegram ID для локальной разработки вне Telegram (см. shared/config). */
  readonly VITE_DEV_TELEGRAM_ID?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
