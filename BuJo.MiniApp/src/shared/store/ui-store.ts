import { create } from 'zustand'

export type ThemeMode = 'light' | 'dark'

interface UiState {
  /** Текущая тема оформления (синхронизируется с темой Telegram / системы). */
  theme: ThemeMode
  setTheme: (theme: ThemeMode) => void
  toggleTheme: () => void
}

/**
 * Глобальный store UI-состояния (Zustand). Лежит в shared, т.к. читается
 * и из shared/ui (например, Toaster), и из верхних слоёв.
 */
export const useUiStore = create<UiState>((set) => ({
  theme: 'light',
  setTheme: (theme) => set({ theme }),
  toggleTheme: () =>
    set((state) => ({ theme: state.theme === 'dark' ? 'light' : 'dark' })),
}))
