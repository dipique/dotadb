import { FC } from 'react'
import { UseQueryResult } from 'react-query'
import { DbItem, IDbActions, useItemDb } from '../db/db'
import { FormGroupCfg } from '../forms/FormGroupCfg'
import { ItemFormProps } from '../forms/ItemForm'
import { ItemTableColumnDef } from '../forms/ItemTable'
import { IItemType, ItemTypes } from './ItemTypes'

export interface ICollection {
    name        : string
    singular    : string
    icon        : (props?: any) => JSX.Element
    types       : ItemTypes
    defType     : string
    dbStatus?   : string
    dbFetching? : boolean
    getNew      : () => DbItem
    renderForm  : FC<ItemFormProps<any>>
}

export interface RequiredItemCollectionProps<T extends DbItem> extends ICollection {
    name        : string
    singular    : string
    icon        : (props?: any) => JSX.Element
    types       : ItemTypes
    getNew      : () => T
    formGrpCfg  : FormGroupCfg<T>
    tblColumns  : ItemTableColumnDef<T>[]
    renderForm  : FC<ItemFormProps<T>>
}

export type ItemCollectionProps<T extends DbItem> = RequiredItemCollectionProps<T> & Partial<ItemCollection<T>>

export class ItemCollection<T extends DbItem>
       implements ICollection, RequiredItemCollectionProps<T>
{
    name        : string
    singular    : string
    icon        : (props?: any) => JSX.Element
    types       : ItemTypes
    defType     : string
    getNew      : () => T
    formGrpCfg  : FormGroupCfg<T>
    tblColumns     : ItemTableColumnDef<T>[]
    renderForm  : FC<ItemFormProps<T>>
    items       : T[]
    dbStatus    : string
    dbFetching  : boolean
    useDb       : () => IDbActions<T>

    getId       : (item?: T)              => string    = i => i ? `${i.type}_${i.name}` : ''
    getTitle    : (item?: T)              => string    = i => i ? `${this.types[i.type].short}: ${i.name}` : '[unknown]'
    getType     : (item?: T)              => IItemType = i => this.types[i?.type || this.defType]
    applyFilter : (item : T, filter: any) => boolean   = (i, filter) => !filter?.type || i.type === filter.type

    constructor(props: ItemCollectionProps<T>, qryResult: UseQueryResult<T[], unknown>) {
        this.name       = props.name
        this.singular   = props.singular
        this.icon       = props.icon
        this.types      = props.types
        this.defType    = props.defType
        this.getNew     = props.getNew
        this.formGrpCfg = props.formGrpCfg
        this.tblColumns = props.tblColumns
        this.renderForm = props.renderForm
        this.items      = qryResult.data || []
        this.dbStatus   = qryResult.status
        this.dbFetching = qryResult.isFetching

        this.useDb      = () => useItemDb<T>(this.name, this.getNew)

        Object.assign(this, props)
    }
}