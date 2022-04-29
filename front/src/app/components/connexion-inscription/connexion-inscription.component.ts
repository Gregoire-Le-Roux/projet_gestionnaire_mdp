import { Component } from '@angular/core';

@Component({
  selector: 'app-connexion-inscription',
  templateUrl: './connexion-inscription.component.html',
  styleUrls: ['./connexion-inscription.component.scss']
})
export class ConnexionInscriptionComponent {

  afficherFormInscription: boolean = false;
  
  constructor() { }

  ChangerForm(_etat: boolean): void
  {
    this.afficherFormInscription = _etat;
  }

}
