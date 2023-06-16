import { KeyboardEvent, MutableRefObject, useEffect, useMemo, useRef, useState } from 'react'

import { Box, Button, Group, SegmentedControl, Center, Grid } from '@mantine/core'
import { HotkeyItem, useForm, useHotkeys } from '@mantine/hooks'
import { UseForm } from '@mantine/hooks/lib/use-form/use-form'

import { getPropAssociations } from '../meta/TypeAssociation'
import { FldOpts } from './FormGroupCfg'
import { DbItem } from '../db/db'
import { ItemCollection } from '../entities/ICollection'
import { ItemLinks } from './ItemLinks'

export type ItemFormProps<T extends DbItem> = {
    item: T
    col: ItemCollection<T>
    saveItem: (item: T) => Promise<void>
    deleteItem: (id: string) => Promise<void>
    closeForm: () => void
}

const inputBreakoutKeyCombo = 'ctrl+alt+'
const toBreakoutHotkey = (key: string) => `${inputBreakoutKeyCombo}${key[0].toLowerCase()}`

export const ItemForm = <T extends DbItem>({
    item, col,
    saveItem, deleteItem,
    closeForm
}: ItemFormProps<T>) => {
    const [ itemType, setItemType ] = useState(col.getNew().type as keyof T)
    const [ saving, setSaving ] = useState(false)
    const [ formType ] = useState<'create' | 'update'>(item?.id ? 'update' : 'create')

    const fieldCfg = useMemo(() => Object.entries(col.formGrpCfg).map(([key, cfg]) => {
        const prop = key as keyof T
        const fldOpts = { ...new FldOpts, ...(cfg as any)} as FldOpts
        fldOpts.label = fldOpts.label ?? key
        let { render, span, label, placeholder, required } = fldOpts
        const blank = col.getNew()
        const typeAss = getPropAssociations(key, blank)

        return (form: UseForm<T>, type: string, ref?: MutableRefObject<any>) => {
            const shown = !typeAss.length || typeAss.includes(type)
            if (!shown) {
                // reset hidden fields
                if (form.values[prop] !== blank[prop])
                form.setFieldValue(prop, blank[prop])
                return undefined
            }
            return <Grid.Col key={`col_${key}`} span={span}>
                {render!({
                    key: prop,
                    ...form.getInputProps(prop),
                    label, placeholder, required,
                    ref: ((cfg as any)?.initFocus ? ref : undefined),
                })}
            </Grid.Col>
        }
    }), [])

    const form = useForm<T>({
        initialValues: {
            ...col.getNew(),
            ...item,
        }
    })

    const hks = useMemo(() => [
        ...Object.entries(col.types).map(([key]) => [
            toBreakoutHotkey(key), () => onTypeChange(key as keyof T)
        ]),
        ['Escape', closeForm]
    ] as HotkeyItem[], [])

    useHotkeys(hks)

    const onTypeChange = (v: keyof T) => {
        setItemType(v)
        form.setFieldValue('type', v as string)
        form.getInputProps('type').onChange(v)
    }

    const validateItem = (item: T) => !!item // TODO: validation

    const onSaveItem = async (item: T) => {
        if (!validateItem(item))
            return
        setSaving(true)
        const result = await saveItem(item)
    }

    const initFocusRef = useRef<any>()

    useEffect(() => {
        if (initFocusRef.current)
            initFocusRef.current.focus()
        }, [])

    const handleFormHotkeys = useMemo(() => (e: KeyboardEvent<HTMLFormElement>) => {
        const tgt = e.target as any
        // do we not need to override in this case?
        if (tgt.nodeName === 'INPUT') {
            if (e.key === 'Escape') {
                closeForm()
                return
            } else if (!(e.ctrlKey && e.altKey && e.key.length === 1))
                return
        }

        // does this match a current hotkey?
        const bohk = toBreakoutHotkey(e.key)
        const hk = hks.find(hk => hk[0] === bohk)
        if (!hk) return

        // run the event
        hk[1](e as any)
    }, [])

    return <Box sx={{ maxWidth: 400 }}>
        <form onKeyDown={handleFormHotkeys} onSubmit={form.onSubmit(onSaveItem)}>
            <Center>
                <SegmentedControl
                    size='md'
                    data={Object.entries(col.types).map(([key, pt]) => ({
                        value: key,
                        label: (pt as any).short.toLowerCase()
                    }))}
                    {...form.getInputProps('type')}
                    onChange={v => onTypeChange(v as keyof T)}
                />
            </Center>
            <Grid gutter='xs'>
                {fieldCfg.map(f => f(form, itemType as string, initFocusRef))}
            </Grid>
            <ItemLinks item={item} updateList={links => form.setFieldValue('links', links)} style={{ paddingTop: '8px' }} />
            <Group position="apart" mt="md">
                <Button
                    disabled={formType === 'create'}
                    color='red'
                    loading={saving}
                    type="button"
                    onClick={(e: any) => {
                        e.preventDefault()
                        deleteItem?.(item?.id!)
                    }}
                >
                    Delete
                </Button>
                <Button loading={saving} type="submit">Submit</Button>
            </Group>
        </form>
    </Box>
}