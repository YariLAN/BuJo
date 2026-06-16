/** Доменные типы привычек. Соответствуют контрактам BuJo Backend (Contracts/V1/Habits). */

export interface Habit {
  id: string
  name: string
  isArchived: boolean
  /** ISO datetime. */
  createdAt: string
}

export interface HabitLog {
  id: string
  habitId: string
  habitName: string | null
  /** Дата в формате YYYY-MM-DD. */
  date: string
  isCompleted: boolean
}

export interface MonthlyStats {
  year: number
  /** Месяц 1–12. */
  month: number
  completedDays: number
  totalDays: number
}

export interface CalendarDay {
  /** Дата в формате YYYY-MM-DD. */
  date: string
  isCompleted: boolean
}

export interface HabitStats {
  currentStreak: number
  bestStreak: number
  /** Процент выполнения за период (0–100). */
  completionRate: number
  totalCompleted: number
  monthlyStats: MonthlyStats[]
  calendarDays: CalendarDay[]
}

/** Период статистики (значения соответствуют enum StatsPeriod на бэкенде). */
export const STATS_PERIODS = ['Week', 'Month', 'Quarter', 'All'] as const
export type StatsPeriod = (typeof STATS_PERIODS)[number]

export const STATS_PERIOD_LABELS: Record<StatsPeriod, string> = {
  Week: 'Неделя',
  Month: 'Месяц',
  Quarter: 'Квартал',
  All: 'Всё время',
}
