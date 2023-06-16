import { FC } from 'react'
import { Button } from '@mantine/core'
import { useSpotlight } from '@mantine/spotlight'

export const LoginButton: FC<{ disabled?: boolean}> = ({
    disabled = false
}) => {
  const { triggerAction } = useSpotlight()
  return <Button disabled={disabled} onClick={() => triggerAction('login')}>Log In</Button>
}