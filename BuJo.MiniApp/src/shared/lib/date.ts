/**
 * Сегодняшняя дата в UTC в формате `YYYY-MM-DD`.
 * Совпадает с дефолтом бэкенда (`DateOnly.FromDateTime(DateTime.UtcNow)`),
 * поэтому используется и для записи отметок, и для определения «сегодня» в UI.
 */
export function getUtcTodayIso(): string {
  return new Date().toISOString().slice(0, 10)
}

const dayMonthFormatter = new Intl.DateTimeFormat('ru-RU', {
  day: 'numeric',
  month: 'long',
})

/** Форматирует ISO-дату `YYYY-MM-DD` как «5 июня». */
export function formatDayMonth(iso: string): string {
  return dayMonthFormatter.format(new Date(`${iso}T00:00:00Z`))
}

const monthFormatter = new Intl.DateTimeFormat('ru-RU', {
  month: 'long',
  year: 'numeric',
})

/** Форматирует пару (год, месяц 1–12) как «июнь 2026 г.». */
export function formatMonth(year: number, month: number): string {
  return monthFormatter.format(new Date(Date.UTC(year, month - 1, 1)))
}
