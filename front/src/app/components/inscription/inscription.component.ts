import { Component, EventEmitter, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Cache } from 'src/app/enum/Cache';
import { CompteService } from 'src/app/services/compte.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Classes/Aes';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { Compte } from 'src/app/Types/Compte';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-inscription',
  templateUrl: './inscription.component.html',
  styleUrls: ['./inscription.component.scss']
})
export class InscriptionComponent
{
  @Output() ChangerForm: EventEmitter<boolean> = new EventEmitter();

  voirMdp: boolean = false;
  voirMdpConfirmation: boolean = false;  
  btnClicker: boolean = false;

  patternMail: RegExp = environment.patternMail;
  patternVide: RegExp = environment.patternVide;

  private readonly CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

  constructor(
    private compteServ: CompteService, 
    private outilServ: OutilService,
    private router: Router) { }

  Inscription(_form: NgForm): void
  {
    if(_form.invalid || this.btnClicker)
      return;

    let formValide = true;
    this.btnClicker = true;

    if(_form.value.Prenom.replace(this.patternVide, "") == "")
    {
      this.outilServ.ToastErreur("Prénom vide");
      formValide = false;
    }
    if(_form.value.Nom.replace(this.patternVide, "") == "")
    {
      this.outilServ.ToastErreur("Nom vide");
      formValide = false;
    }
    if(_form.value.Mail.replace(this.patternVide, "") == "")
    {
      this.outilServ.ToastErreur("Mail vide");
      formValide = false;
    }
    if(_form.value.Mdp.replace(this.patternVide, "") == "")
    {
      this.outilServ.ToastErreur("Mot de passe vide");
      formValide = false;
    }

    if(_form.value.Mdp != _form.value.MdpConfirmation) 
    {
      this.outilServ.ToastErreur("Les mots de passe ne correspondent pas");
      formValide = false;
    }

    if(!formValide) 
    {      
      this.btnClicker = false;
      return
    }
    
    let aes: Aes = new Aes(this.CLE_SECRETE); 

    const DATA = {
      Prenom: aes.Chiffrer(_form.value.Prenom),
      Nom: aes.Chiffrer(_form.value.Nom),
      Mail: aes.Chiffrer(_form.value.Mail),
      Mdp: aes.Chiffrer(_form.value.Mdp),
    }

    aes = null;

    this.compteServ.Inscription(DATA).subscribe({
      next: (retour: string | Compte) =>
      {
        this.btnClicker = false;
        if(typeof(retour) == "string")
          this.outilServ.ToastErreur(retour);
        else {
          const DATA_COMPTE: Compte = 
          {
            Id: retour.Id,
            Prenom: _form.value.Prenom,
            Nom: _form.value.Nom,
            Mail: _form.value.Mail,
            HashCle: retour.HashCle,
            Jwt: retour.Jwt
          }
          VariableStatic.compte = DATA_COMPTE
          sessionStorage.setItem(Cache.INFO_COMPTE, JSON.stringify(DATA_COMPTE));
          this.router.navigate(["/mdp"]);
        }
      },
      error: () =>
      {
        this.btnClicker = false;
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  AfficherFormInscription(): void
  {
    this.ChangerForm.emit(false);
  }

  AfficherMdp(): void
  {
    this.voirMdp = !this.voirMdp;
  }

  AfficherMdpConfirmation(): void
  {
    this.voirMdpConfirmation = !this.voirMdpConfirmation;
  }

}
