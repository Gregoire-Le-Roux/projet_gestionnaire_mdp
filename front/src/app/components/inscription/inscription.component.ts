import { Component, EventEmitter, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AES } from 'crypto-js';
import { ConnexionService } from 'src/app/services/connexion.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Static/Aes';
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
    private connexionServ: ConnexionService, 
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

    console.log(formValide);

    if(!formValide) return

    let aes: Aes = new Aes(this.CLE_SECRETE); 

    const DATA = {
      Prenom: aes.Chiffrer(_form.value.Prenom),
      Nom: aes.Chiffrer(_form.value.Nom),
      Mail: aes.Chiffrer(_form.value.Mail),
      Mdp: aes.Chiffrer(_form.value.Mdp),
    }

    aes = null;

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
