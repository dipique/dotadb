import { useMemo } from 'react'

export const apiUri = 'http://localhost:8000'

export interface ILink {
    id: string
    collection: string
    description: string
 }

export class DbItem {
    id: string = ''
    name: string = ''
    type: string = ''
    image: string = ''

    links: ILink[] = []
}

export interface IDbActions<T extends DbItem> {
    save: (item: T) => Promise<T>
    get: (id: string) => Promise<T>
    getAll: () => Promise<T[]>
    remove: (id: string) => Promise<void>
}

export const getAccessTokenSilently = async (options: any) => 'token'

export const useItemDb = <T extends DbItem>(colName: string, createNew: () => T) => {
    const getToken = useMemo(() => async () => await getAccessTokenSilently({}), [])

    return {
        save: async (item: T) => {
            const idParam = item.id ? `?id=${item.id}` : ''
            const response = await fetch(`${apiUri}/${colName}${idParam}`, {
                method: item.id ? 'PATCH' : 'POST',
                headers: {
                    Authorization: `Bearer ${await getToken()}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(item)
            })
            return await response.json() as T
        },
        getAll: async () => {
            const response = await fetch(`${apiUri}/${colName}`, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${await getToken()}`
                }
            })
            return (await response.json() || []) as T[]
        },
        get: async (id?: string) => {
            if (!id)
                return createNew()

            const response = await fetch(`${apiUri}/${colName}?id=${id}`, {
                method: 'GET',
                headers: { Authorization: `Bearer ${await getToken()}` }
            })
            return await response.json() as T
        },
        remove: async (id: string) => {
            await fetch(`${apiUri}/${colName}?id=${id}`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${await getToken()}` }
            })
        }
    } as IDbActions<T>
}