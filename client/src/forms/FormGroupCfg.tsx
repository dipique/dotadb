import { Grid, TextInput } from '@mantine/core'

export class FldOpts {
    render?: (props: any) => JSX.Element = props => <TextInput {...props} />
    placeholder?: string
    label?: string
    span?: number = 6
    initFocus?: boolean = false
    required?: boolean

    constructor(defaultSpan: number = 6) {
        this.span = defaultSpan
    }
}

export type FormGroupCfg<T> = { [Key in keyof T]?: FldOpts | null }