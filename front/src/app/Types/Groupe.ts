import { Mdp } from "./Mdp"

export type Groupe =
{
    Id: number,
    IdCompteCreateur: number,
    Titre: string,
    NbCompte: number,
    NbMdp: number,
    ListeMdp: Mdp[]
}