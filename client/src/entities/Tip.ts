import { DbItem } from '../db/db'

export class Tip extends DbItem  {    
    text?: string = ''
}

export const NoteTypes = {
    other:  {
        display: 'other',
        short: 'other',
    },
} as const

export type TipTypeKey = keyof typeof NoteTypes
export const DefaultTipType: TipTypeKey = 'other'