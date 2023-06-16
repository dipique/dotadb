import { createContext, FC, ReactNode } from 'react'

export interface IAuthContext {
    isAuthenticated: boolean
}

export const AuthContext = createContext<IAuthContext>({} as IAuthContext)

export const AuthWrapper: FC<{children: ReactNode}> = props => {
    return <>
        <AuthContext.Provider
            value={{
                isAuthenticated: true, // makes things load quick during development
            }}
            children={props.children}
        />
    </>
}