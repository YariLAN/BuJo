import { useEffect, type ReactNode } from 'react'
import { initTelegram } from '@/shared/lib/telegram'

/** Инициализирует Telegram Mini Apps SDK один раз при старте приложения. */
export function TelegramProvider({ children }: { children: ReactNode }) {
  useEffect(() => {
    initTelegram()
  }, [])

  return <>{children}</>
}
