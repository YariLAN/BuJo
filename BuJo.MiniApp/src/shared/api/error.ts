import { AxiosError } from 'axios'

/**
 * Извлекает человекочитаемое сообщение об ошибке из ответа бэкенда.
 * Контроллеры BuJo возвращают как простые строки (`BadRequest("...")`),
 * так и ProblemDetails (`{ title, detail }`).
 */
export function getApiErrorMessage(
  error: unknown,
  fallback = 'Что-то пошло не так',
): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data
    if (typeof data === 'string' && data.trim()) return data
    if (data && typeof data === 'object') {
      const obj = data as Record<string, unknown>
      if (typeof obj.detail === 'string') return obj.detail
      if (typeof obj.title === 'string') return obj.title
    }
    return error.message || fallback
  }
  if (error instanceof Error) return error.message
  return fallback
}
