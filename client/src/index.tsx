import { createRoot } from 'react-dom/client'
import { App } from './App'
import { AuthWrapper } from './AuthWrapper'
import { QueryClient, QueryClientProvider } from 'react-query'

const queryClient = new QueryClient()

const AuthProvider = ({ children }: { children: React.ReactNode }) => <>{children}</>
const DbWrapper = ({ children }: { children: React.ReactNode }) => <>{children}</>

createRoot(
    document.getElementById('app-root')!
).render(
    <AuthProvider>
        <AuthWrapper>
            <QueryClientProvider client={queryClient}>
                <DbWrapper>
                    <App />
                </DbWrapper>
            </QueryClientProvider>
        </AuthWrapper>
    </AuthProvider>
)

