import { Relationship } from "./Relationship"
import { Tip } from "./Tip"

export enum HeroPreference {
    Hated       = 0,
    Disliked    = 1,
    Indifferent = 2,
    Preferred   = 3,
    Favorite    = 4,
}

export const HeroPreferences = Object.entries(HeroPreference)
    .filter(e => !isNaN(e[0]as any))
    .map(e => ({ name: e[1] as string, id: Number(e[0]) }));


export class Hero {
    Name             : string = ''
    AltNames         : string = ''
    Notes            : string = ''
    Preference       : HeroPreference = HeroPreference.Indifferent
    NameSet          : string = '' // includes alternative names for the hero
    Initials         : string = ''
    Labels           : string = '' // labels as pipe-delimited string
    DescriptionLabels: string[] = []
    Tips             : Tip[] = []
    Relationships    : Relationship[] = []
    Id               : number = 0
    UserId           : number = 0
  }