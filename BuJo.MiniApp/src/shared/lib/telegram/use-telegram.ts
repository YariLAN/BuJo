import { useMemo } from 'react'
import { initData, useSignal } from '@telegram-apps/sdk-react'
import { appConfig } from '@/shared/config'

export interface TelegramUser {
  /** Telegram ID пользователя или dev-фолбэк; null, если не удалось определить. */
  telegramId: string | null
  /** true, если приложение реально запущено внутри Telegram. */
  isTelegramEnv: boolean
}

/**
 * Реактивно отдаёт Telegram-пользователя. Внутри Telegram берёт id из initData,
 * иначе — `VITE_DEV_TELEGRAM_ID` из окружения.
 */
export function useTelegramUser(): TelegramUser {
  const user = useSignal(initData.user)

  return useMemo<TelegramUser>(() => {
    const realId = user?.id != null ? String(user.id) : null
    return {
      telegramId: realId ?? appConfig.devTelegramId ?? null,
      isTelegramEnv: realId !== null,
    }
  }, [user])
}
