import { createRoot } from 'react-dom/client'
import { PickerPage } from './picker/PickerPage'

createRoot(
    document.getElementById('app-root')!
).render(
    <div>
        <PickerPage />
    </div>
)