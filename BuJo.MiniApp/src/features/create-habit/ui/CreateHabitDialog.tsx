import { useState, type FormEvent } from 'react'
import { PlusIcon } from 'lucide-react'
import { Button } from '@/shared/ui/button'
import { Input } from '@/shared/ui/input'
import { Label } from '@/shared/ui/label'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/shared/ui/dialog'
import { useCreateHabit } from '../model/use-create-habit'

const MAX_NAME_LENGTH = 100

/** Диалог создания привычки. Self-contained: триггер-кнопка + форма. */
export function CreateHabitDialog() {
  const [open, setOpen] = useState(false)
  const [name, setName] = useState('')
  const { mutate, isPending } = useCreateHabit()

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    const trimmed = name.trim()
    if (!trimmed) return

    mutate(trimmed, {
      onSuccess: () => {
        setName('')
        setOpen(false)
      },
    })
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="gap-2">
          <PlusIcon className="size-4" />
          Новая привычка
        </Button>
      </DialogTrigger>

      <DialogContent>
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Новая привычка</DialogTitle>
            <DialogDescription>
              Например: «Пить воду», «Зарядка», «Чтение 20 минут».
            </DialogDescription>
          </DialogHeader>

          <div className="grid gap-2 py-4">
            <Label htmlFor="habit-name">Название</Label>
            <Input
              id="habit-name"
              value={name}
              onChange={(event) => setName(event.target.value)}
              placeholder="Название привычки"
              maxLength={MAX_NAME_LENGTH}
              autoFocus
            />
          </div>

          <DialogFooter>
            <Button type="submit" disabled={isPending || name.trim().length === 0}>
              {isPending ? 'Создание…' : 'Создать'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
