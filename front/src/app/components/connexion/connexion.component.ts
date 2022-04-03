import { Component, OnInit } from '@angular/core';
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
export class ConnexionComponent implements OnInit 
{
  private readonly CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

  constructor(
    private connexionServ: ConnexionService, 
    private outilServ: OutilService,
    private router: Router) { }

  ngOnInit(): void 
  {
  }
  
  SeConnecter(_form: NgForm): void
  {
    let aes: Aes = new Aes(this.CLE_SECRETE); 

    let mdpChiffrer = aes.Chiffrer(_form.value.mdp);
    let loginChiffrer = aes.Chiffrer(_form.value.login);

    aes = null;

    this.connexionServ.Connexion({ login: loginChiffrer, mdp: mdpChiffrer }).subscribe({
      next: (retour: string | Compte) =>
      { 
        if(typeof(retour) == "string")
          this.outilServ.ToastErreur(retour);
        else
        {
          VariableStatic.compte = retour as Compte;
          this.DechiffrerCompte();
        }
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  private DechiffrerCompte(): void
  {
    // base64 to string
    VariableStatic.compte.HashCle = atob(VariableStatic.compte.HashCle);

    let aes: Aes = new Aes(VariableStatic.compte.HashCle);

    VariableStatic.compte.Mail = aes.Dechiffrer(VariableStatic.compte.Mail);
    VariableStatic.compte.Nom = aes.Dechiffrer(VariableStatic.compte.Nom);
    VariableStatic.compte.Prenom = aes.Dechiffrer(VariableStatic.compte.Prenom);

    aes = null;
  }
}
