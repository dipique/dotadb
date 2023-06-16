import { DbItem } from '../db/db'

export class Item extends DbItem  {
    type: ItemTypeKey = DefaultItemType
    location?: string = ''
    description?: string = ''
    notes?: string = ''
}

export const ItemTypes = {
    weapon:  {
        display: 'weapon',
        short: 'weapon',
    },
} as const

export type ItemTypeKey = keyof typeof ItemTypes
export const DefaultItemType: ItemTypeKey = 'weapon'