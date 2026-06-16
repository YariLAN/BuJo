import { RouterProvider } from 'react-router-dom'
import { Toaster } from '@/shared/ui/sonner'
import { Providers } from './providers'
import { router } from './router'

export function App() {
  return (
    <Providers>
      <RouterProvider router={router} />
      <Toaster />
    </Providers>
  )
}
