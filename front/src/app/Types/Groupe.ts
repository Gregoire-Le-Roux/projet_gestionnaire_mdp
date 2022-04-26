export type Groupe =
{
    Id: number,
    IdCompteCreateur: number,
    Titre: string,

    listeCompte: [{
        IdGroupe: number,
        IdCompte: number
    }]
}