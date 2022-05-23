export class GenerateurMDP 
{
    private longueur : number
    private contientMinuscule : boolean
    private contientMajuscule : boolean
    private contientNombre : boolean
    private contientCaracteresSpeciaux : boolean
    constructor(_longueur : number, _contientMinuscule : boolean, _contientMajuscule : boolean,  _contientNombre : boolean, _contientCaracteresSpeciaux : boolean ) {
        this.longueur = _longueur;
        this.contientMinuscule = _contientMinuscule;
        this.contientMajuscule = _contientMajuscule;
        this.contientNombre = _contientNombre;
        this.contientCaracteresSpeciaux = _contientCaracteresSpeciaux;
    }

    Generer () : string {
        // On définit toutes les valeurs possibles pour l'alphabet, les chiffres et les caractères spéciaux
        const ALPHABET = "abcdefghijklmnopqrstuvwxyz";
        const CHIFFRE = "0123456789";
        const SPECIAUX = "@$!%*?&";

        let mdp : string = "";
        let contientArray : Array<string> = [];
        let randomContient : number = 0, randomLettre : number = 0;

        // Selon si la case est cochée ou non, on ajoute la possibilité d'avoir un type de caractère
        if(this.contientMinuscule) contientArray.push("minuscule");
        if(this.contientMajuscule) contientArray.push("majuscule");
        if(this.contientNombre) contientArray.push("nombre");
        if(this.contientCaracteresSpeciaux) contientArray.push("speciaux");

        // Si aucune case n'est cochée, le mot de passe contiendra par défaut des minuscules, majuscules et des nombres.
        if(contientArray.length == 0) contientArray = ["minuscule", "majuscule", "nombre"];

        // On boucle un nombre de fois selon la longueur définit
        for(let i = 0; i < this.longueur; i++) {
            // On sélectionne un type de caractère aléatoire selon majuscule, minuscule, nombre et caractère spécial
            randomContient = Math.floor((Math.random() * contientArray.length));
            // Selon le type de caractère, on en choisit un au hasard dans sa liste 
            switch(contientArray[randomContient]) {
                case "minuscule":
                    randomLettre = Math.floor((Math.random() * ALPHABET.length));
                    mdp = mdp + ALPHABET[randomLettre];
                    break;
                case "majuscule":
                    randomLettre = Math.floor((Math.random() * ALPHABET.length));
                    mdp = mdp + ALPHABET[randomLettre].toUpperCase();
                    break;
                case "nombre":
                    randomLettre = Math.floor((Math.random() * CHIFFRE.length));
                    mdp = mdp + CHIFFRE[randomLettre];
                    break;
                case "speciaux":
                    randomLettre = Math.floor((Math.random() * SPECIAUX.length));
                    mdp = mdp + SPECIAUX[randomLettre];
                    break;
                default:
                    break;
            }
        }

        return mdp;
    }
}