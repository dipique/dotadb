import { FC } from 'react'
import { Button } from '@mantine/core'
import { useSpotlight } from '@mantine/spotlight'

export const LogoutButton: FC<{ disabled?: boolean}> = ({
  disabled = false
}) => {
  const { triggerAction } = useSpotlight()
  return <Button disabled={disabled} onClick={() => triggerAction('logout')}>Log Out</Button>
}