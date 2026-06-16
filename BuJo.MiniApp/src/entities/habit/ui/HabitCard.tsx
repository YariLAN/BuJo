import type { ReactNode } from 'react'
import { ArchiveIcon, ChevronRightIcon } from 'lucide-react'
import { Card } from '@/shared/ui/card'
import { Badge } from '@/shared/ui/badge'
import { cn } from '@/shared/lib/utils'
import type { Habit } from '../model/types'

interface HabitCardProps {
  habit: Habit
  /** Действие справа (например, кнопка отметки за сегодня). */
  action?: ReactNode
  /** Клик по карточке (например, открыть статистику). */
  onClick?: () => void
  className?: string
}

/** Презентационная карточка привычки (entity-уровень, без бизнес-логики). */
export function HabitCard({ habit, action, onClick, className }: HabitCardProps) {
  return (
    <Card
      className={cn(
        'flex-row items-center gap-3 p-4',
        onClick && 'hover:bg-accent/40 cursor-pointer transition-colors',
        className,
      )}
      onClick={onClick}
    >
      <div className="flex min-w-0 flex-1 items-center gap-2">
        <span className="truncate font-medium">{habit.name}</span>
        {habit.isArchived && (
          <Badge variant="secondary" className="gap-1">
            <ArchiveIcon className="size-3" />в архиве
          </Badge>
        )}
      </div>

      {action && (
        // Клики по действию не должны открывать статистику.
        <div onClick={(event) => event.stopPropagation()}>{action}</div>
      )}

      {onClick && !action && (
        <ChevronRightIcon className="text-muted-foreground size-4 shrink-0" />
      )}
    </Card>
  )
}
