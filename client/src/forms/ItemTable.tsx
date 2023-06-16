import { ActionIcon, Anchor, Table } from '@mantine/core'
import { useContext, useMemo } from 'react'
import { SquareX } from 'tabler-icons-react'
import { DbItem } from '../db/db'
import { DbContext, IDbContext } from '../DbWrapper'
import { ItemCollection } from '../entities/ICollection'

export interface ItemTableColumnDef<T extends DbItem> {
    name: string
    header?: string | JSX.Element
    value?: (item: T, col: ItemCollection<T>, ctx: IDbContext) => string | JSX.Element
}

export type ItemTableProps<T extends DbItem> = {
    deleteItem: (id: string) => Promise<void>,
    onItemClick: (id: string) => void,
    collection: ItemCollection<T>
    filters?: any
}

export const ItemTable = <T extends DbItem>({
    onItemClick,
    deleteItem,
    collection,
    filters,
} : ItemTableProps<T>) => {
    const items = useMemo(() => (filters
        ? collection.items.filter(i => collection.applyFilter(i, filters))
        : collection.items
    ), [collection, filters])

    const ctx = useContext(DbContext)

    const ths = <tr>
          <th>type</th>
          <th>name</th>
          {collection.tblColumns.map(c => <th key={c.name}>{c.header || c.name}</th>)}
          <th></th>
        </tr>

    const trs = items.map(item => (
        <tr key={item.id}>
            <td>{collection.getType(item).short}</td>
            <td><Anchor onClick={() => onItemClick?.(item.id)}>{item.name}</Anchor></td>
            {collection.tblColumns.map(c => <td key={c.name}>{c.value ? c.value(item, collection, ctx) : (item as any)[c.name]}</td>)}
            <td width={32}>
                <ActionIcon onClick={() => deleteItem?.(item.id)} color='red'>
                    <SquareX />
                </ActionIcon>
            </td>
        </tr>
    ))

    return <Table>
        <thead>{ths}</thead>
        <tbody>{trs}</tbody>
    </Table>
}