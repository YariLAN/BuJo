import {
  init as initSdk,
  initData,
  isTMA,
  restoreInitData,
} from '@telegram-apps/sdk-react'
import { appConfig } from '@/shared/config'

let initialized = false

/**
 * Инициализирует Telegram Mini Apps SDK. Реальная инициализация выполняется только внутри
 * среды Telegram; в обычном браузере вызов безопасен и ничего не делает.
 */
export function initTelegram(): void {
  if (initialized) return
  initialized = true

  try {
    if (!isTMA()) return
    initSdk()
    restoreInitData()
  } catch (error) {
    console.warn('[telegram] SDK init skipped (non-Telegram environment):', error)
  }
}

/**
 * Возвращает Telegram ID текущего пользователя (из initData) либо dev-фолбэк из .env.
 * Используется axios-интерсептором для заголовка `X-Telegram-Id` вне React-дерева.
 */
export function getTelegramId(): string | null {
  try {
    const user = initData.user()
    if (user?.id != null) return String(user.id)
  } catch {
    // Вне Telegram сигнал недоступен — используем фолбэк ниже.
  }
  return appConfig.devTelegramId ?? null
}
