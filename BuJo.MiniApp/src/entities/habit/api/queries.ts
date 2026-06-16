import { useQuery } from '@tanstack/react-query'
import {
  getHabitLogs,
  getHabits,
  getHabitStats,
  type GetHabitLogsInput,
} from './habitApi'
import { habitKeys } from './keys'
import type { StatsPeriod } from '../model/types'

/** Список привычек пользователя. */
export function useHabitsQuery(includeArchived = false) {
  return useQuery({
    queryKey: habitKeys.list(includeArchived),
    queryFn: () => getHabits(includeArchived),
  })
}

/** Статистика конкретной привычки за период. */
export function useHabitStatsQuery(habitId: string, period: StatsPeriod) {
  return useQuery({
    queryKey: habitKeys.stats(habitId, period),
    queryFn: () => getHabitStats(habitId, period),
    enabled: Boolean(habitId),
  })
}

/** Логи привычек (например, отметки за сегодня). */
export function useHabitLogsQuery(input: GetHabitLogsInput) {
  return useQuery({
    queryKey: habitKeys.logs(input),
    queryFn: () => getHabitLogs(input),
  })
}
