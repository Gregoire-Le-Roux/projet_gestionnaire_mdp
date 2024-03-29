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

    this.compteServ.Existe(_form.value.Mail).subscribe({
      next: (retour: boolean) =>
      {
        if(retour == false)
          this.EnvoyerDemandeInscription(_form.value.Prenom, _form.value.Nom, _form.value.Mail, _form.value.Mdp);
        else
        {
          this.btnClicker = false;
          this.outilServ.ToastInfo(`Le mail ${_form.value.Mail} existe déjà`);
        }   
      },
      error: () =>
      {
        this.btnClicker = false;
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

  private EnvoyerDemandeInscription(_prenom: string, _nom: string, _mail: string, _mdp: string): void
  {
    let aes: Aes = new Aes(this.CLE_SECRETE); 

    const DATA = {
      Prenom: aes.Chiffrer(_prenom),
      Nom: aes.Chiffrer(_nom),
      Mail: aes.Chiffrer(_mail),
      Mdp: aes.Chiffrer(_mdp),
    }

    this.compteServ.DemanderInscription(DATA).subscribe({
      next: (retour: string) =>
      {
        this.outilServ.ToastOK(retour);
        this.btnClicker = false;
      },
      error: () =>
      {
        this.btnClicker = false;
      }
    });
  }
}
