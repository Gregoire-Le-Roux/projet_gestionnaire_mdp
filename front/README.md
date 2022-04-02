# Tuto
Angular se lance par defaut sur le port 4200
http://localhost:4200

Bien penser a typer le plus possible pour qu'on sache se que c'est.

# Convention nommage (je suis un peu chiant avec sa dsl)

constante, readonly => MAJUSCULE avec des _ (RESULTAT_API)
fonction, class, type => Commence par une Maj
fonction => termine par un type de retour

Essaye de ne pas utiliser le type any qui veut dire nimporte quel type

accolade sur la meme colonne
exemple:

fonction(_text: string): void
{
    if()
    {

    }
}

sa a banir:
fonction(_text: string): void {

}

parametre de fonction commence par un _ et se termine par un type
pense a bien aérer le code ne fait pas des pattés STP

Les fonctions ou varibles qui ne sont pas utilisées et ne doivent pas etre dans HTML passe les en private
et a la fin du reste

exemple:

listeCompte: compte[];
listetag: Tag[];

private cle: string;

Fonction(): void
{

}

private Fonction2(): void
{

}

## Commandes (dans le dossier /front)

ng serve => demaré angular
ng serve --o => démarer angular + ouvrir dans navigateur
ng build => compilé angular pour le deploiement sur serve web

### Génération les plus utile

Générer component (page)
ng g c components/[nom du component]

Générer service (partie dialogue avec API principalement. Un service par thème SVP)
ng g s services/[nom du service] (ne pas ajouter le mot service dans le nom c'est automatique)

Générer Guard (protection des URLs)
ng g g guard/[nom du guard] (ne pas ajouter le mot guard dans le nom c'est automatique)

## Form (type NgForm)

#e="ngForm" => Attribut angular (une sorte id unique a chaque page HTML)
               sert a donnée un nom au formaulaire pour le récuperer.

ngModel => sert à indiquer au formulaire que l'on veut recupérer se champs
           renvoie un JSON sous forme: nameInput: valeur;

Angular gere lui meme la validité d'un formulaire avec les attributs HTML comme: required, max, min, pattern, etc.

Dans le TS il suffi d'utiliser .invalid / .valid

Exemple:

Ajouter(_form: ngForm): void
{
    if(_form.invalid)
        // faire des trucs
        return;
}

## Binding

Le binding sert à faire passer des valeurs de:

HTML => TS (utilisé très rarement, par xp 2 fois utilisé hors events click, change, etc)
TS => HTML: avec les: [] (initialiser un champs HTML input exemple: [ngModel]="nomVariable")

Actualise la valeur de la variable et nous l'affiche en même temps que nous changeons sa valeur
HTML <=> TS (les deux sens simultané, exemple [(ngModel)]="nomVariable")

## event

le eventListener faut l'oublié

onClick => (click)
onChange => (change)

etc...