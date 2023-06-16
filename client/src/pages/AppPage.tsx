import { useEffect, useState } from 'react'
import { useQueryClient } from 'react-query'
import { ActionIcon, Box, Dialog, Group, Loader, Title } from '@mantine/core'
import { showNotification } from '@mantine/notifications'
import { useHotkeys } from '@mantine/hooks'
import { useSpotlight } from '@mantine/spotlight'
import { SquarePlus } from 'tabler-icons-react'

import { DbItem } from '../db/db'
import { ItemTable } from '../forms/ItemTable'
import { ItemForm } from '../forms/ItemForm'
import { ItemFilters } from '../forms/ItemFilters'
import { ItemCollection } from '../entities/ICollection'

export const AppPage = <T extends DbItem>({
    col
} : { col: ItemCollection<T> }) => {
    const { name, singular, useDb, getTitle, getId, icon, items } = col
    const [ showDialog, setShowDialog ] = useState(false)
    const [ itemId, setItemId ] = useState('')
    const { save, getAll, remove } = useDb()
    const [ filters, setFilters ] = useState<any>({ type: '' })

    const closeForm = () => setShowDialog(false)
    const showForm = () => setShowDialog(true)

    const hk = useHotkeys([
        ['c', () => {
            if (showDialog) return
            showForm()
            setItemId('')
        }],
    ])

    const qc = useQueryClient()
    const saveItem = async (item: T) => {
        try {
            await save(item)
            setShowDialog(false)
            showNotification({
                title: 'Success!',
                message: `${item.id ? 'Update' : 'Create'} ${singular} succeeded`
            })
            qc.invalidateQueries(name)
        } catch (err) {
            console.log(err)
            showNotification({
                title: 'Error',
                message: `${item.id ? 'Update' : 'Create'} ${singular} failed`
            })
        }
    }

    const deleteItem = async (id: string) => {
        if (!id) return

        try {
            await remove(id)
            qc.invalidateQueries(name)
            setShowDialog(false)
        } catch (err) {
            console.log(err)
            showNotification({
                title: 'Error',
                message: `Failed to delete ${singular}`
            })
            throw err
        }
    }

    const { registerActions, removeActions } = useSpotlight()
    useEffect(() => {
        if (!items?.length) return

        const actions = items.map(p => ({
            id: getId(p),
            title: getTitle(p),
            description: `View details for this ${singular}`,
            onTrigger: () => {
                setItemId(p.id)
                showForm()
            },
            icon: icon(),
        }))
        registerActions(actions)
        return () => removeActions(actions.map(p => p.id))
    }, [ items ])

    return <>
        <Title>{name}</Title>
        {showDialog &&
        <Dialog
            opened={showDialog}
            withCloseButton
            onClose={() => setShowDialog(false)}
            size='xl'
            radius='md'
        >
            <ItemForm<T>
                col={col}
                item={col.items.find(p => p.id === itemId) || col.getNew()}
                closeForm={closeForm}
                saveItem={saveItem}
                deleteItem={deleteItem}
            />
        </Dialog>}
        {col.dbStatus == 'success'
            ? <Box sx={{ maxWidth: 600 }}>
                <Group position='right'>
                    <ItemFilters<T> filters={filters} setFilters={setFilters} types={col.types} />
                    <ActionIcon size='lg' disabled={col.dbStatus != 'success' || col.dbFetching} onClick={() => {
                        setItemId('')
                        setShowDialog(true)
                    }}>
                        <SquarePlus width={32} height={32} color='green' />
                    </ActionIcon>
                </Group>
                <ItemTable<T>
                    deleteItem={deleteItem}
                    onItemClick={(id: string) => {
                        setItemId(id)
                        setShowDialog(true)
                    }}
                    collection={col}
                    filters={filters}
                />
              </Box>
            : <Loader />}
    </>
}