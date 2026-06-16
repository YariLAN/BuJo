// Публичный API сущности «привычка».

export type {
  Habit,
  HabitLog,
  HabitStats,
  MonthlyStats,
  CalendarDay,
  StatsPeriod,
} from './model/types'
export { STATS_PERIODS, STATS_PERIOD_LABELS } from './model/types'

export { useHabitFilterStore } from './model/filter-store'

export { HabitCard } from './ui/HabitCard'

export { habitKeys } from './api/keys'
export {
  getHabits,
  createHabit,
  logHabit,
  getHabitStats,
  getHabitLogs,
  type LogHabitInput,
  type GetHabitLogsInput,
} from './api/habitApi'
export {
  useHabitsQuery,
  useHabitStatsQuery,
  useHabitLogsQuery,
} from './api/queries'
