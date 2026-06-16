import { CheckIcon } from 'lucide-react'
import { Button } from '@/shared/ui/button'
import { cn } from '@/shared/lib/utils'
import { useToggleHabitLog } from '../model/use-toggle-habit-log'

interface ToggleHabitButtonProps {
  habitId: string
  /** Отмечена ли привычка за сегодня. */
  isCompleted: boolean
}

/** Кнопка отметки выполнения привычки за сегодня (✅ / пусто). */
export function ToggleHabitButton({ habitId, isCompleted }: ToggleHabitButtonProps) {
  const { mutate, isPending } = useToggleHabitLog()

  return (
    <Button
      type="button"
      size="icon"
      variant={isCompleted ? 'default' : 'outline'}
      disabled={isPending}
      aria-pressed={isCompleted}
      aria-label={isCompleted ? 'Снять отметку за сегодня' : 'Отметить за сегодня'}
      className={cn(
        'size-10 rounded-full',
        isCompleted && 'bg-emerald-600 text-white hover:bg-emerald-600/90',
      )}
      onClick={() => mutate({ habitId, isCompleted: !isCompleted })}
    >
      <CheckIcon className={cn('size-5', !isCompleted && 'opacity-30')} />
    </Button>
  )
}
