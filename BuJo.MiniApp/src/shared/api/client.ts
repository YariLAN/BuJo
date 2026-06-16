import axios from 'axios'
import { appConfig } from '@/shared/config'
import { getTelegramId } from '@/shared/lib/telegram'

/** Единый axios-инстанс для общения с BuJo Backend. */
export const apiClient = axios.create({
  baseURL: appConfig.apiBaseUrl,
})

// Бэкенд идентифицирует пользователя по заголовку X-Telegram-Id на каждом запросе.
apiClient.interceptors.request.use((config) => {
  const telegramId = getTelegramId()
  if (telegramId) {
    config.headers.set('X-Telegram-Id', telegramId)
  }
  return config
})
