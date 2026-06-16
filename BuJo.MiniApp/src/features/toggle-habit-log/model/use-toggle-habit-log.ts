import { useMutation, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { habitKeys, logHabit } from '@/entities/habit'
import { getApiErrorMessage } from '@/shared/api'
import { getUtcTodayIso } from '@/shared/lib/date'

interface ToggleInput {
  habitId: string
  /** Целевое состояние после нажатия. */
  isCompleted: boolean
}

/** Отметка выполнения привычки за сегодня с инвалидацией логов/статистики. */
export function useToggleHabitLog() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ habitId, isCompleted }: ToggleInput) =>
      logHabit({ habitId, isCompleted, date: getUtcTodayIso() }),
    onSuccess: () => {
      // Обновляем логи (отметки за сегодня) и статистику.
      queryClient.invalidateQueries({ queryKey: habitKeys.all })
    },
    onError: (error) => {
      toast.error(getApiErrorMessage(error))
    },
  })
}
