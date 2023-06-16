import { Header } from '@mantine/core'

import { LoginButton } from './auth/LoginButton'
import { LogoutButton } from './auth/LogoutButton'

const useAuth = () => ({ isAuthenticated: false, isLoading: false})

export const AppHeader = () => {
    const { isAuthenticated, isLoading } = useAuth()
    return <Header height={60} p="xs">
            <LoginButton disabled={isLoading || isAuthenticated} />
            <LogoutButton disabled={isLoading || !isAuthenticated} />
    </Header>
}