import { useMutation, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { createHabit, habitKeys } from '@/entities/habit'
import { getApiErrorMessage } from '@/shared/api'

/** Создание привычки с инвалидацией списка и тостами. */
export function useCreateHabit() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (name: string) => createHabit(name),
    onSuccess: (habit) => {
      queryClient.invalidateQueries({ queryKey: habitKeys.lists() })
      toast.success(`Привычка «${habit.name}» создана`)
    },
    onError: (error) => {
      toast.error(getApiErrorMessage(error))
    },
  })
}
