import type { StatsPeriod } from '../model/types'
import type { GetHabitLogsInput } from './habitApi'

/** Фабрика query-ключей для react-query (домен привычек). */
export const habitKeys = {
  all: ['habits'] as const,
  lists: () => [...habitKeys.all, 'list'] as const,
  list: (includeArchived: boolean) =>
    [...habitKeys.lists(), { includeArchived }] as const,
  stats: (habitId: string, period: StatsPeriod) =>
    [...habitKeys.all, 'stats', habitId, period] as const,
  logs: (input: GetHabitLogsInput) =>
    [...habitKeys.all, 'logs', input] as const,
}
