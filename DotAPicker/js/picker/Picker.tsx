import { HeroCanvas } from './HeroCanvas'
import { HeroList } from './HeroList'
import { PickerFilterBar } from './PickerFilterBar'

export const Picker = () => {
    return <div>
        <PickerFilterBar />
        <HeroList />
        <HeroCanvas />
    </div>
}