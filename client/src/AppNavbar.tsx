import { ActionIcon, Navbar, Title } from '@mantine/core'
import { FC, useContext, useState } from 'react'
import { AppContext } from './App'
import { DbContext } from './DbWrapper'

const hoverHighlight = 'rgba(147, 147, 147, 0.1)'
const optionBaseStyle = { padding: '8px' }
export const AppNavbar = () => {
    const { activePage, setActivePage } = useContext(AppContext)
    const [ hoverPage, setHoverPage ] = useState('')
    const { cols } = useContext(DbContext)

    const NavbarOption: FC<{ icon: JSX.Element, label: string }> = ({ icon, label }) =>
        <tr
            style={(hoverPage === label || activePage === label) ? { ...optionBaseStyle, background: hoverHighlight } : optionBaseStyle}
            onMouseOver={() => setHoverPage(label)}
            onMouseOut={() => setHoverPage('')}
            onClick={() => setActivePage(label)}
        >
            <td><ActionIcon size='xl'>{icon}</ActionIcon></td>
            <td><Title style={{ marginLeft: '6px'}} order={5}>{label}</Title></td>
        </tr>

    return <Navbar width={{ base: 180 }} p='md'><table style={{ borderSpacing: '0' }}><tbody>
        {cols?.map(col => <NavbarOption key={col.name} icon={col.icon({size: 40})} label={col.name} />)}
    </tbody></table></Navbar>
}