import { FC, useState, useEffect } from 'react'
import { Hero, HeroPreference, HeroPreferences } from '../model/Hero'
import { MultiSelect } from '@mantine/core'

export class HeroFilter {
    heroName: string = ''
    preference: HeroPreference[] = []
    label: string[] = []
}

export const PickerFilterBar: FC<{
    heroes: Hero[]
    updateFilter: (filter: HeroFilter) => void
}> = ({ heroes, updateFilter }) => {
    const [ filter, setFilter ] = useState<HeroFilter>(new HeroFilter())

    useEffect(() => {
        filter && updateFilter(filter)
    }, [ filter ])

    return <div>
        <MultiSelect
            data={HeroPreferences.map(hp => ({ value: hp.id.toString(), label: hp.name }))}
            onChange={val => setFilter({ ...filter, preference: val.map(v => parseInt(v) as HeroPreference) })}
        />
    </div>
}