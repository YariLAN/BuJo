import { createBrowserRouter } from 'react-router-dom'
import { HabitsPage } from '@/pages/habits'
import { HabitStatsPage } from '@/pages/habit-stats'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <HabitsPage />,
  },
  {
    path: '/habits/:habitId/stats',
    element: <HabitStatsPage />,
  },
])
