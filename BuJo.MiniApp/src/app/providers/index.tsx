import type { ReactNode } from 'react'
import { QueryProvider } from './query-provider'
import { TelegramProvider } from './telegram-provider'
import { ThemeProvider } from './theme-provider'

/** Композиция глобальных провайдеров приложения. */
export function Providers({ children }: { children: ReactNode }) {
  return (
    <TelegramProvider>
      <ThemeProvider>
        <QueryProvider>{children}</QueryProvider>
      </ThemeProvider>
    </TelegramProvider>
  )
}
