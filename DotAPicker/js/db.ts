export const getHeroes = async () => {
    try {
        const resp = await fetch('/Picker/Heroes')
        const data = await resp.json()
        return data
    } catch (err) {
        console.error(err)
        return err
    }
}