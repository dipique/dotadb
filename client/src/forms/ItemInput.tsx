import { useContext, useMemo } from 'react'
import { Select } from '@mantine/core'
import { DbItem } from '../db/db'
import { DbContext } from '../DbWrapper'
import { ICollection, ItemCollection } from '../entities/ICollection'

export const ItemInput = ({ collection = '', readOnly = false, idsToExclude = [], ...rest }: any) => {
    // const { placesCol, peopleCol } = useContext(DbContext)
    const { } = useContext(DbContext)

    // return collection items IF there is no collection filter OR the filter matches the collection
    const includeColResults = (col: ICollection) => !collection || collection === col.name

    const getColItems = <T extends DbItem>(col: ItemCollection<T>) =>
        includeColResults(col) ? col.items.filter(i => !idsToExclude.includes(i.id)).map(i => ({
            collection: col.name,
            group: `${col.name} - ${i.type}`,
            value: i.id,
            label: col.getTitle(i),
        })) : []

    const items = useMemo(() => [
        // ...getColItems(placesCol),
        // ...getColItems(peopleCol),
    // ], [collection, placesCol, peopleCol])
    ], [collection])

    return <Select
        searchable
        data={items}
        disabled={readOnly}
        {...rest}
    />
}