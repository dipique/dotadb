import { InputWrapper } from '@mantine/core'
import { useContext } from 'react'
import { MentionsInput, Mention } from 'react-mentions'
import { DbItem } from '../db/db'
import { DbContext } from '../DbWrapper'
import './MentionInput.css'

const toMentionItem = (item: DbItem) => ({
    id: item.id, display: item.name
})

export const MentionInput = ({ value, placeholder, required, onChange, style, ...rest}: any) => {
    // const { peopleCol, placesCol } = useContext(DbContext) // TODO: make this part of the configurations
    const { cols } = useContext(DbContext)

    return <InputWrapper {...rest}>
        <div className='mention-div'>
            <MentionsInput
                value={value}
                placeholder={placeholder}
                required={required}
                onChange={({target}) => onChange(target.value)}
                className='mention-input'
                style={{
                    fontSize: '14px',
                    paddingTop: '6px',
                    fontFamily: '-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Helvetica,Arial,sans-serif,Apple Color Emoji,Segoe UI Emoji',
                    
                }}
            >
                {
                    ...cols.map(col => <Mention
                        trigger='@'
                        // data={col.items.map(toMentionItem)}
                        data={[].map(toMentionItem)}
                        style={{ backgroundColor: '#087F5B' }} // this should be defined in collection
                    />)
                }
            </MentionsInput>
        </div>
    </InputWrapper>
}