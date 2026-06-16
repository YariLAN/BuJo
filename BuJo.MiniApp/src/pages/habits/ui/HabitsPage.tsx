import { MoonIcon, SunIcon } from 'lucide-react'
import { useHabitFilterStore, useHabitsQuery } from '@/entities/habit'
import { CreateHabitDialog } from '@/features/create-habit'
import { HabitList } from '@/widgets/habit-list'
import { getApiErrorMessage } from '@/shared/api'
import { useUiStore } from '@/shared/store'
import { Button } from '@/shared/ui/button'
import { Skeleton } from '@/shared/ui/skeleton'

export function HabitsPage() {
  const showArchived = useHabitFilterStore((state) => state.showArchived)
  const toggleShowArchived = useHabitFilterStore((state) => state.toggleShowArchived)

  const theme = useUiStore((state) => state.theme)
  const toggleTheme = useUiStore((state) => state.toggleTheme)

  const { data: habits, isPending, isError, error, refetch } =
    useHabitsQuery(showArchived)

  return (
    <main className="mx-auto flex min-h-dvh w-full max-w-md flex-col gap-4 p-4">
      <header className="flex items-center justify-between gap-2">
        <h1 className="text-2xl font-bold tracking-tight">Привычки</h1>
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon"
            aria-label="Переключить тему"
            onClick={toggleTheme}
          >
            {theme === 'dark' ? (
              <SunIcon className="size-5" />
            ) : (
              <MoonIcon className="size-5" />
            )}
          </Button>
          <CreateHabitDialog />
        </div>
      </header>

      <div className="flex items-center justify-end">
        <Button variant="ghost" size="sm" onClick={toggleShowArchived}>
          {showArchived ? 'Скрыть архив' : 'Показать архив'}
        </Button>
      </div>

      {isPending && (
        <div className="flex flex-col gap-3">
          {Array.from({ length: 4 }).map((_, index) => (
            <Skeleton key={index} className="h-[68px] w-full rounded-xl" />
          ))}
        </div>
      )}

      {isError && (
        <div className="flex flex-col items-center gap-3 rounded-xl border border-dashed p-6 text-center">
          <p className="text-muted-foreground text-sm">
            {getApiErrorMessage(error, 'Не удалось загрузить привычки')}
          </p>
          <Button variant="outline" size="sm" onClick={() => refetch()}>
            Повторить
          </Button>
        </div>
      )}

      {!isPending && !isError && habits.length === 0 && (
        <div className="flex flex-col items-center gap-2 rounded-xl border border-dashed p-8 text-center">
          <p className="font-medium">Пока нет привычек</p>
          <p className="text-muted-foreground text-sm">
            Создайте первую — и отмечайте выполнение каждый день.
          </p>
        </div>
      )}

      {!isPending && !isError && habits.length > 0 && (
        <HabitList habits={habits} />
      )}
    </main>
  )
}
