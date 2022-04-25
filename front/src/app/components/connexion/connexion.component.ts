import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ConnexionService } from 'src/app/services/connexion.service';
import { OutilService } from 'src/app/services/outil.service';
import { Compte } from 'src/app/Types/Compte';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { Aes } from 'src/app/Static/Aes';
import { Router } from '@angular/router';

@Component({
  selector: 'app-connexion',
  templateUrl: './connexion.component.html',
  styleUrls: ['./connexion.component.scss']
})
export class ConnexionComponent
{
  voirMdp: boolean = false;
  private readonly CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

  constructor(
    private connexionServ: ConnexionService, 
    private outilServ: OutilService,
    private router: Router) { }
  
  SeConnecter(_form: NgForm): void
  {
    if(_form.invalid)
      return;

    let aes: Aes = new Aes(this.CLE_SECRETE); 

    let mdpChiffrer = aes.Chiffrer(_form.value.mdp);
    let loginChiffrer = aes.Chiffrer(_form.value.login);

    aes = null;

    this.connexionServ.Connexion({ login: loginChiffrer, mdp: mdpChiffrer }).subscribe({
      next: (retour: string | Compte) =>
      { 
        console.log(retour);
        
        if(typeof(retour) == "string")
          this.outilServ.ToastErreur(retour);
        else
        {
          this.DechiffrerCompte(retour);
          this.router.navigate(["/mdp"]);
        }
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  AfficherMdp(): void
  {
    this.voirMdp = !this.voirMdp;
  }

  private DechiffrerCompte(_retour: Compte): void
  {
    _retour.HashCle = atob(_retour.HashCle);

    let aes: Aes = new Aes(_retour.HashCle);

    const DATA: Compte =
    {
      Id: _retour.Id,
      HashCle: _retour.HashCle,
      Mail: aes.Dechiffrer(_retour.Mail),
      Nom: aes.Dechiffrer(_retour.Nom),
      Prenom: aes.Dechiffrer(_retour.Prenom)
    };

    aes = null;

    VariableStatic.compte = DATA;
  }
}
