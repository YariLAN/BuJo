import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeftIcon } from 'lucide-react'
import {
  STATS_PERIOD_LABELS,
  STATS_PERIODS,
  useHabitFilterStore,
  useHabitStatsQuery,
  useHabitsQuery,
  type CalendarDay,
  type StatsPeriod,
} from '@/entities/habit'
import { getApiErrorMessage } from '@/shared/api'
import { formatDayMonth } from '@/shared/lib/date'
import { cn } from '@/shared/lib/utils'
import { Button } from '@/shared/ui/button'
import { Card } from '@/shared/ui/card'
import { Progress } from '@/shared/ui/progress'
import { Skeleton } from '@/shared/ui/skeleton'
import { Tabs, TabsList, TabsTrigger } from '@/shared/ui/tabs'

function Metric({ label, value }: { label: string; value: string | number }) {
  return (
    <Card className="gap-1 p-4">
      <span className="text-muted-foreground text-xs">{label}</span>
      <span className="text-2xl font-bold">{value}</span>
    </Card>
  )
}

function StatCalendar({ days }: { days: CalendarDay[] }) {
  if (days.length === 0) return null

  return (
    <div className="grid grid-cols-7 gap-1">
      {days.map((day) => (
        <div
          key={day.date}
          title={`${formatDayMonth(day.date)} — ${day.isCompleted ? 'выполнено' : 'нет'}`}
          className={cn(
            'aspect-square rounded-sm',
            day.isCompleted ? 'bg-emerald-500' : 'bg-muted',
          )}
        />
      ))}
    </div>
  )
}

export function HabitStatsPage() {
  const navigate = useNavigate()
  const { habitId = '' } = useParams()

  const statsPeriod = useHabitFilterStore((state) => state.statsPeriod)
  const setStatsPeriod = useHabitFilterStore((state) => state.setStatsPeriod)

  const { data: stats, isPending, isError, error, refetch } =
    useHabitStatsQuery(habitId, statsPeriod)

  // Имя привычки берём из кэша списка (включая архивные).
  const { data: habits } = useHabitsQuery(true)
  const habitName = habits?.find((habit) => habit.id === habitId)?.name

  return (
    <main className="mx-auto flex min-h-dvh w-full max-w-md flex-col gap-4 p-4">
      <header className="flex items-center gap-2">
        <Button
          variant="ghost"
          size="icon"
          aria-label="Назад"
          onClick={() => navigate('/')}
        >
          <ArrowLeftIcon className="size-5" />
        </Button>
        <h1 className="truncate text-xl font-bold">
          {habitName ?? 'Статистика'}
        </h1>
      </header>

      <Tabs
        value={statsPeriod}
        onValueChange={(value) => setStatsPeriod(value as StatsPeriod)}
      >
        <TabsList className="w-full">
          {STATS_PERIODS.map((period) => (
            <TabsTrigger key={period} value={period}>
              {STATS_PERIOD_LABELS[period]}
            </TabsTrigger>
          ))}
        </TabsList>
      </Tabs>

      {isPending && (
        <div className="flex flex-col gap-4">
          <div className="grid grid-cols-2 gap-3">
            {Array.from({ length: 4 }).map((_, index) => (
              <Skeleton key={index} className="h-20 rounded-xl" />
            ))}
          </div>
          <Skeleton className="h-32 rounded-xl" />
        </div>
      )}

      {isError && (
        <div className="flex flex-col items-center gap-3 rounded-xl border border-dashed p-6 text-center">
          <p className="text-muted-foreground text-sm">
            {getApiErrorMessage(error, 'Не удалось загрузить статистику')}
          </p>
          <Button variant="outline" size="sm" onClick={() => refetch()}>
            Повторить
          </Button>
        </div>
      )}

      {!isPending && !isError && (
        <div className="flex flex-col gap-4">
          <div className="grid grid-cols-2 gap-3">
            <Metric label="Текущая серия" value={`${stats.currentStreak} дн.`} />
            <Metric label="Лучшая серия" value={`${stats.bestStreak} дн.`} />
            <Metric label="Выполнено" value={stats.totalCompleted} />
            <Metric
              label="Процент"
              value={`${Math.round(stats.completionRate)}%`}
            />
          </div>

          <div className="flex flex-col gap-2">
            <div className="flex items-center justify-between text-sm">
              <span className="text-muted-foreground">Выполнение за период</span>
              <span className="font-medium">
                {Math.round(stats.completionRate)}%
              </span>
            </div>
            <Progress value={Math.round(stats.completionRate)} />
          </div>

          <StatCalendar days={stats.calendarDays} />
        </div>
      )}
    </main>
  )
}
