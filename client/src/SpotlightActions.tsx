import { SpotlightAction } from '@mantine/spotlight'
import { Link, Login, Logout } from 'tabler-icons-react'
import { IAppContext } from './App'
import { IAuthContext } from './AuthWrapper'

export const slIconSize = 18

export interface ISpotlightAction extends SpotlightAction {
    hidden: boolean
}

export const getActions = (appCtx: IAppContext, authCtx: IAuthContext): ISpotlightAction[] => [
    {
        id: 'tips',
        title: 'Go to Tips',
        description: 'Open Tips page',
        onTrigger: () => appCtx.setActivePage('tips'),
        icon: <Link size={slIconSize} />,
        hidden: !authCtx.isAuthenticated || appCtx.activePage == 'tips'
    },
].filter(a => !a.hidden)