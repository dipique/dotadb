import { createContext, useMemo } from 'react'
import { useQuery } from 'react-query'
import { showNotification } from '@mantine/notifications'
import { CalendarEvent, Location, MoodBoy, Swords } from 'tabler-icons-react'
import { DbItem, useItemDb } from './db/db'
import {
    ItemCollection, ICollection
} from './entities/ICollection'
import { Tip, NoteTypes, DefaultTipType } from './entities/Tip'
import { TipForm, TipFormGrpCfg } from './forms/TipForm'

export interface IFindItemResult<T extends DbItem> {
    item: T
    id: string
    collection: string
}

export const DbContext = createContext<IDbContext>({} as IDbContext)

export interface IDbContext {
    tipCol: ItemCollection<Tip>
    cols: ICollection[]
    findItemById: <T extends DbItem>(id: string) => IFindItemResult<T> | undefined
}

export const DbWrapper = (props: any) => {
    const getItems = <T extends DbItem>(getAll: () => Promise<T[]>, colName: string) => async () => {
        try {
            return await getAll()
        } catch (err) {
            console.log(err)
            showNotification({
                title: 'Error',
                message: `Failed to fetch ${colName}`
            })
            return []
        }
    }
    
    const getQry = <T extends DbItem>(name: string, getNew: () => T) =>
        useQuery(name, getItems(useItemDb<T>(name, getNew).getAll, name))

    const tipQry = getQry('tip', () => new Tip())
    const tipCol = useMemo(() => new ItemCollection<Tip>({
        name: 'tips',
        singular: 'tip',
        getNew: () => new Tip(),
        icon: props => <MoodBoy {...props} />,
        types: NoteTypes,
        defType: DefaultTipType,
        formGrpCfg: TipFormGrpCfg,
        tblColumns: [ { name: 'text' } ],
        renderForm: TipForm,
    }, tipQry), [ tipQry ])
    
    const findItemById = useMemo(() =>
        <T extends DbItem>(id: string): IFindItemResult<T> | undefined => {
            if (!id) return undefined
            const tip = tipCol.items.find(p => p.id === id)
            if (tip) return { item: tip as T, id, collection: 'tip' }
            // const place = placesCol.items.find(p => p.id === id)
            // if (place) return { item: place as T, id, collection: 'places' }
            // const encounter = encountersCol.items.find(p => p.id === id)
            // if (encounter) return { item: encounter as T, id, collection: 'encounters' }
            return undefined
        }
    , [ tipCol ])

    return <>
        <DbContext.Provider
            value={{
                tipCol,
                cols: [tipCol],
                findItemById
            }}
            children={props.children}
        />
    </>
}