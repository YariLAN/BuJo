import { useNavigate } from 'react-router-dom'
import { HabitCard, useHabitLogsQuery, type Habit } from '@/entities/habit'
import { ToggleHabitButton } from '@/features/toggle-habit-log'
import { getUtcTodayIso } from '@/shared/lib/date'

interface HabitListProps {
  habits: Habit[]
}

/**
 * Список привычек: карточка + кнопка отметки за сегодня.
 * Состояние «отмечено сегодня» берётся из логов за текущую дату.
 */
export function HabitList({ habits }: HabitListProps) {
  const navigate = useNavigate()
  const today = getUtcTodayIso()
  const { data: todayLogs } = useHabitLogsQuery({ fromDate: today, toDate: today })

  const completedToday = new Set(
    (todayLogs ?? []).filter((log) => log.isCompleted).map((log) => log.habitId),
  )

  return (
    <div className="flex flex-col gap-3">
      {habits.map((habit) => (
        <HabitCard
          key={habit.id}
          habit={habit}
          onClick={() => navigate(`/habits/${habit.id}/stats`)}
          action={
            <ToggleHabitButton
              habitId={habit.id}
              isCompleted={completedToday.has(habit.id)}
            />
          }
        />
      ))}
    </div>
  )
}
