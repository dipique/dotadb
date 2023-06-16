import 'reflect-metadata'

// used to track types associated with each property

export const ASS_PROPS_KEY = 'ASSOCIATED_PROPS'
type PropAssociations = { [key: string | symbol]: string[] }
const getAssMeta = (target: object): PropAssociations => Reflect.getMetadata(ASS_PROPS_KEY, target.constructor)
const setAssMeta = (target: object, values: PropAssociations) => Reflect.defineMetadata(ASS_PROPS_KEY, values, target.constructor)

export function association(...types: string[]): PropertyDecorator {
    return (target: object, propertyKey: string | symbol) => {
        const props = getAssMeta(target) || {}
        props[propertyKey] = types
        setAssMeta(target, props)
    }
}

export const getPropAssociations = (propName: string, target: object): string[] => {
    if (!target) return []
    const props = getAssMeta(target)
    if (!props) return []
    return props[propName] || []
}
