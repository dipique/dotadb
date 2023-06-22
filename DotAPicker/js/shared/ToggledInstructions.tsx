import { useState, FC } from 'react'

export class InstructionSection {
    text: string = ''
    title?: string
}

export const ToggledInstructions:FC<{instructions: InstructionSection[], defaultOpen?: boolean }> = ({ instructions, defaultOpen = false }) => {
    const [ showInstructions, setShowInstructions ] = useState(defaultOpen)
    const onToggleClick = () => setShowInstructions(!showInstructions)
    return <div>
        <a onClick={onToggleClick}>Show/Hide Instructions</a>
        {showInstructions && instructions.map(({ title, text }, index) => <div key={index} style={{ marginBottom: 4 }}>
            { title && <span><strong>{title}: </strong></span>}
            <span>{text}</span>
        </div>)}
    </div>
}