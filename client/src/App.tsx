import { createContext, useContext, useState } from 'react'
import { LoggedOut } from './pages/LoggedOut'
import { UIWrapper } from './UIWrapper'
import { DbContext } from './DbWrapper'
import { AppPage } from './pages/AppPage'
import { AuthContext } from './AuthWrapper'

export interface IAppContext {
    activePage: string
    setActivePage: (s: string) => void
}

export const AppContext = createContext<IAppContext>({} as IAppContext)

export const App = () => {
    const { isAuthenticated } = useContext(AuthContext)
    const { cols } = useContext(DbContext)
    
    const [ activePage, setActivePage ] = useState(cols?.find(() => true)?.name || '')

    const ActivePage = () => {
        if (!isAuthenticated)
            return <LoggedOut />
        
        const col = cols?.find(c => c.name === activePage) as any
        return col?.renderForm ? <AppPage<any> col={col} /> : <div>Invalid page</div>
    }

    return <>
        <AppContext.Provider
            value={{  activePage, setActivePage }}
        >
            <UIWrapper>
                <ActivePage />
            </UIWrapper>
        </AppContext.Provider>
    </>
}