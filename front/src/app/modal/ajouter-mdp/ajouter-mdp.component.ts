import { DatePipe } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Classes/Aes';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { ExportMdp } from 'src/app/Types/Export/ExportMdp';
import { environment } from 'src/environments/environment';
import { GenerateurMDPComponent } from '../generateur-mdp/generateur-mdp.component';

@Component({
  selector: 'app-ajouter-mdp',
  templateUrl: './ajouter-mdp.component.html',
  styleUrls: ['./ajouter-mdp.component.scss']
})
export class AjouterMdpComponent implements OnInit 
{
  @ViewChild ("inputMdp") inputMdp : ElementRef;

  voirMdp: boolean = false;

  private regexVide: RegExp = environment.patternVide;

  constructor(
    private outilServ: OutilService,
    private mdpServ: MdpService,
    private dialogRef: MatDialogRef<AjouterMdpComponent>,
    private dialog: MatDialog,
    private datePipe: DatePipe) { }

  ngOnInit(): void {
  }

  Ajouter(_form: NgForm): void
  {
    if(_form.invalid)
    {
      this.outilServ.ToastWarning("Veuillez completer tous les champs");
      return;
    }

    if(!this.FormValide(_form.value))
      return;

    if(_form.value.DateExpiration != "")
      _form.value.DateExpiration = this.datePipe.transform(_form.value.DateExpiration, "yyyy-MM-dd");

    _form.value.IdCompteCreateur = VariableStatic.compte.Id;

    const DATA = this.ChiffrerDonnee(_form.value);

    this.mdpServ.Ajouter(DATA).subscribe({
      next: (retour: number) =>
      {
        this.outilServ.ToastOK("Votre mot de passe a été enregistré");

        _form.value.Id = retour;
        _form.value.Mdp = this.inputMdp.nativeElement.value;
        this.dialogRef.close(_form.value);
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

  OuvrirModalGenerateurMdp(): void
  {
    const DIALOG_REF = this.dialog.open(GenerateurMDPComponent);

    DIALOG_REF.afterClosed().subscribe({
      next: (retour: string) =>
      {
        if(retour)
        {
          this.inputMdp.nativeElement.value = retour;
        }
      }
    });
  }

  private ChiffrerDonnee(_donnee: ExportMdp): ExportMdp
  {
    let aes: Aes = new Aes(VariableStatic.compte.HashCle);

    const DATA: ExportMdp =
    {
      Titre: aes.Chiffrer(_donnee.Titre),
      Login: aes.Chiffrer(_donnee.Login),
      Mdp: aes.Chiffrer(_donnee.Mdp),
      Url: aes.Chiffrer(_donnee.Url),
      Description: aes.Chiffrer(_donnee.Description),
      DateExpiration: aes.Chiffrer(_donnee.DateExpiration),
      IdCompteCreateur: _donnee.IdCompteCreateur
    };

    return DATA;
  }

  private FormValide(_donnee): boolean
  {
    let formValide: boolean = true;

    if(_donnee.Titre.replace(this.regexVide, "") == "")
    {
      this.outilServ.ToastWarning("Le champ titre est vide");
      formValide = false;
    }

    if(_donnee.Login.replace(this.regexVide, "") == "")
    {
      this.outilServ.ToastWarning("Le champ login est vide");
      formValide = false;
    }

    if(_donnee.Mdp.replace(this.regexVide, "") == "")
    {
      this.outilServ.ToastWarning("Le champ mot de passe est vide");
      formValide = false;
    }

    if(_donnee.Url.replace(this.regexVide, "") == "")
    {
      this.outilServ.ToastWarning("Le champ url est vide");
      formValide = false;
    }

    return formValide;
  }
}
