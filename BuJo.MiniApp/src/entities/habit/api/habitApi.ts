import { apiClient } from '@/shared/api'
import type { Habit, HabitLog, HabitStats, StatsPeriod } from '../model/types'

const HABITS = '/habits'

/** GET /api/v1/habits — список привычек пользователя. */
export async function getHabits(includeArchived = false): Promise<Habit[]> {
  const { data } = await apiClient.get<Habit[]>(HABITS, {
    params: { includeArchived },
  })
  return data
}

/** POST /api/v1/habits — создать привычку. */
export async function createHabit(name: string): Promise<Habit> {
  const { data } = await apiClient.post<Habit>(HABITS, { name })
  return data
}

export interface LogHabitInput {
  habitId: string
  isCompleted: boolean
  /** YYYY-MM-DD; если не задано — бэкенд использует сегодняшнюю дату (UTC). */
  date?: string
}

/** POST /api/v1/habits/{id}/log — отметить выполнение привычки. */
export async function logHabit({
  habitId,
  isCompleted,
  date,
}: LogHabitInput): Promise<HabitLog> {
  const { data } = await apiClient.post<HabitLog>(`${HABITS}/${habitId}/log`, {
    date,
    isCompleted,
  })
  return data
}

/** GET /api/v1/habits/{id}/stats — статистика привычки за период. */
export async function getHabitStats(
  habitId: string,
  period: StatsPeriod,
): Promise<HabitStats> {
  const { data } = await apiClient.get<HabitStats>(
    `${HABITS}/${habitId}/stats`,
    { params: { period } },
  )
  return data
}

export interface GetHabitLogsInput {
  habitId?: string
  /** YYYY-MM-DD */
  fromDate?: string
  /** YYYY-MM-DD */
  toDate?: string
}

/** GET /api/v1/habits/logs — логи привычек (с фильтрами). */
export async function getHabitLogs(
  input: GetHabitLogsInput = {},
): Promise<HabitLog[]> {
  const { data } = await apiClient.get<HabitLog[]>(`${HABITS}/logs`, {
    params: input,
  })
  return data
}
