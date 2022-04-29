export type GroupeMdpCompte =
{
    listeMdp: MdpGroupe[],
    listeCompte: CompteGroupe[]
}

type MdpGroupe =
{
    Id: number,
    Titre: string,
    Login: string,
    Mdp: string
}

type CompteGroupe =
{
    Id: number,
    Nom: string,
    Prenom: string,
    Mail: string
}