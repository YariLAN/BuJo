import { create } from 'zustand'
import type { StatsPeriod } from './types'

interface HabitFilterState {
  /** Показывать ли архивные привычки в списке. */
  showArchived: boolean
  /** Выбранный период на экране статистики (запоминается между переходами). */
  statsPeriod: StatsPeriod
  setShowArchived: (value: boolean) => void
  toggleShowArchived: () => void
  setStatsPeriod: (period: StatsPeriod) => void
}

/** Доменный UI-store фильтров привычек (Zustand). */
export const useHabitFilterStore = create<HabitFilterState>((set) => ({
  showArchived: false,
  statsPeriod: 'Month',
  setShowArchived: (value) => set({ showArchived: value }),
  toggleShowArchived: () =>
    set((state) => ({ showArchived: !state.showArchived })),
  setStatsPeriod: (period) => set({ statsPeriod: period }),
}))
