import { useContext, useMemo } from 'react'
import { Search } from 'tabler-icons-react'

import { AppShell, MantineProvider } from '@mantine/core'
import { SpotlightProvider, SpotlightAction } from '@mantine/spotlight'
import { NotificationsProvider } from '@mantine/notifications'
import { getActions, slIconSize } from './SpotlightActions'

import { AppContext } from './App'
import { AuthContext } from './AuthWrapper'
import { AppNavbar } from './AppNavbar'
import { AppHeader } from './AppHeader'

export const UIWrapper = (props: any) => {
    const appCtx = useContext(AppContext)
    const authCtx = useContext(AuthContext)

    const actions: SpotlightAction[] = useMemo(() => getActions(appCtx, authCtx), [appCtx])

    return <>
        <MantineProvider
            theme={{ colorScheme: 'dark'}}
            withGlobalStyles
            withNormalizeCSS
        >
            <NotificationsProvider>
                <SpotlightProvider
                    actions={actions}
                    searchIcon={<Search size={slIconSize} />}
                    searchPlaceholder='Search...'
                    nothingFoundMessage='Action not found'
                    highlightQuery
                >
                    <AppShell
                        padding="md"
                        navbar={<AppNavbar />}
                        header={<AppHeader />}
                        children={props.children}
                    />
                </SpotlightProvider>
            </NotificationsProvider>
        </MantineProvider>
    </>
}