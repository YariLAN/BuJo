import { useEffect, type ReactNode } from 'react'
import { useUiStore } from '@/shared/store'

/**
 * Синхронизирует тему оформления:
 * 1) определяет светлую/тёмную по `prefers-color-scheme` (внутри Telegram это отражает тему клиента);
 * 2) применяет класс `.dark` к <html> для dark-варианта Tailwind.
 */
export function ThemeProvider({ children }: { children: ReactNode }) {
  const theme = useUiStore((state) => state.theme)
  const setTheme = useUiStore((state) => state.setTheme)

  useEffect(() => {
    const media = window.matchMedia('(prefers-color-scheme: dark)')
    setTheme(media.matches ? 'dark' : 'light')

    const onChange = (event: MediaQueryListEvent) =>
      setTheme(event.matches ? 'dark' : 'light')
    media.addEventListener('change', onChange)
    return () => media.removeEventListener('change', onChange)
  }, [setTheme])

  useEffect(() => {
    document.documentElement.classList.toggle('dark', theme === 'dark')
  }, [theme])

  return <>{children}</>
}
