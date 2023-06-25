import { HeroCanvas } from './HeroCanvas'
import { HeroList } from './HeroList'
import { PickerFilterBar } from './PickerFilterBar'

export const Picker = () => {
    return <div>
        <PickerFilterBar heroes={[]} updateFilter={console.log} />
        <HeroList />
        <HeroCanvas />
    </div>
}