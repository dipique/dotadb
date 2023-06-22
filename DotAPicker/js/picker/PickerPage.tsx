import { useState, useEffect } from 'react'
import { createRoot } from 'react-dom/client'
import { ToggledInstructions } from '../shared/ToggledInstructions'
import { Picker } from './Picker'

export const PickerPage = () => {
    const [ testText, setTestText ] = useState('')

    useEffect(() => {
        fetch('/Picker/Heroes')
            .then(response => response.json())
            .then(data => {
                console.log(data)
                
                setTestText(data
                    ? data.length
                        ? `${data.length} heroes received from api`
                        : 'Items property empty or undefined'
                    : 'Empty response received'
                )
            })

    }, []) // first load

    return <div>
        <ToggledInstructions instructions={[
            { text: 'This is the picking screen where you can choose 5 allies and 5 enemies so you can see all the notes you spent all that time writing.' },
            { text: 'You can search by name, preference, and any labels you assigned. When you find the hero you were looking for, click the "allies" or "enemies" link to add them.' },
            { text: 'From the hero name search box, you can add the top hero to enemies by pressing Enter, or to allies by pressing Shift-Enter.', title: 'Tip' },
            { text: 'Once added, you can remove a hero by clicking the red X next to the hero name. But... do it gently. Heroes have feelings, too.' },
            { text: 'Lastly, if you\'re wondering why this app is so ugly, it\'s because I suck at this front end shit. Sorry about that. Believe it or not even this took me a pretty long time.' },
        ]} />
        <Picker />
        <div>{testText}</div>
    </div>
}

createRoot(
    document.getElementById('pickerRoot')!
).render(<PickerPage />)

