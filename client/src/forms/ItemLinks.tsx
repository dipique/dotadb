import { ActionIcon, InputWrapper, Stack, TextInput } from '@mantine/core'
import { FC, useContext, useState } from 'react'
import { SquareX } from 'tabler-icons-react'
import { DbItem, ILink } from '../db/db'
import { DbContext } from '../DbWrapper'
import { ItemInput } from './ItemInput'

export const ItemLink: FC<{
    link: ILink,
    onChange?: (id: string, desc?: string) => void
    readOnly: boolean
    deleteLink: (link: ILink) => void
    idsToExclude: string[]
}> = ({ link, onChange, deleteLink, idsToExclude = [], readOnly = false }) => link.id
  ? <div style={ { borderLeft: '3px white solid' }}>
        <div style={{ display: 'flex' }}>
            <div style={{ width: '91%' }}>
                <ItemInput
                    collection={link.collection}
                    value={link.id}
                    onChange={(id: string) => onChange?.(id, link.description)}
                    readOnly={readOnly}
                />
            </div>
            <div style={{ width: '9%' }}>
                {link.id && <ActionIcon
                        style={{ marginLeft: '5px', borderRight: '0' }}
                        color='red'
                        size={36}
                        onClick={() => deleteLink(link)}
                >
                    <SquareX size={36} />
                </ActionIcon>}
            </div>
        </div>
        <TextInput
            placeholder='describe how this item is related/linked'
            value={link.description}
            disabled={!link.id}
            onChange={e => onChange?.(link.id, e.target.value)}
            style={{ paddingTop: '6px' }}
        />
    </div>
  : <ItemInput
        onChange={(id: string) => onChange?.(id)}
        placeholder={'link new item'}
        idsToExclude={idsToExclude}
    />

export interface LinkListProps<TItem extends DbItem, TListItem> {
    item: TItem,
    updateList: (items: TListItem[]) => void,
    [k: string]: any
}

export const ItemLinks = <TItem extends DbItem>({ item, updateList, ...rest }: LinkListProps<TItem, ILink>) => {
    const [ links, setLinks ] = useState(item.links || [])
    const { findItemById } = useContext(DbContext)
    const handleItemChange = (collection: string, id: string, description: string) => {
        if (!id) return

        const matchIdx = links.findIndex(l => l.id === id)
        const isNew = matchIdx < 0
        let newLinks: ILink[] = []
        if (isNew) {
            collection ||= findItemById(id)?.collection || ''
            if (!collection)
                throw new Error('Could not find item in any collection')
            newLinks = [...links, { collection, id, description }]
        } else {
            const match = links[matchIdx]
            if (match.description === description)
                return // no change
            
            newLinks = [...links]
            newLinks[matchIdx] = { collection, id, description}
        }

        updateList(newLinks)
        setLinks(newLinks)
    }

    const handleItemDelete = (link: ILink) => {
        const newLinks = links.filter(l => l.id !== link.id)
        if (newLinks.length < links.length) {
            updateList(newLinks)
            setLinks(newLinks)
        }
    }

    return <InputWrapper label='links' {...rest}>
        <Stack>{[ ...links, {} as ILink].map((l, idx) =>
            <ItemLink
                key={l.id || idx}
                link={l}
                readOnly={!!l.id} // readonly if this already has a value
                onChange={(id: string, desc?: string) => handleItemChange(l.collection, id, desc || '')} 
                deleteLink={handleItemDelete}
                idsToExclude={links.map(l => l.id || '')}
            />
        )}</Stack>
    </InputWrapper>
}