import { FC, useContext } from 'react'

import { FormGroupCfg } from './FormGroupCfg'
import { Tip } from '../entities/Tip'
import { ItemForm, ItemFormProps } from './ItemForm'
import { DbContext } from '../DbWrapper'

export const TipFormGrpCfg: FormGroupCfg<Tip> = {
   text:        { placeholder: 'enter tip text', initFocus: true, required: true },
}

export const TipForm: FC<ItemFormProps<Tip>> = props => {
  const { tipCol } = useContext(DbContext)
  return <ItemForm<Tip>
      {...props}
      col={tipCol}
  />
}