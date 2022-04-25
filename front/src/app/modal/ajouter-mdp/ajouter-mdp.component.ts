import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Static/Aes';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { ExportMdp } from 'src/app/Types/Export/ExportMdp';

@Component({
  selector: 'app-ajouter-mdp',
  templateUrl: './ajouter-mdp.component.html',
  styleUrls: ['./ajouter-mdp.component.scss']
})
export class AjouterMdpComponent implements OnInit 
{
  voirMdp: boolean = false;

  constructor(
    private outilServ: OutilService, 
    private mdpServ: MdpService, 
    private dialogRef: MatDialogRef<AjouterMdpComponent>,
    private datePipe: DatePipe) { }

  ngOnInit(): void {
  }

  Ajouter(_form: NgForm): void
  {
    _form.value.DateExpiration = this.datePipe.transform(_form.value.DateExpiration, "yyyy-MM-dd");
    _form.value.IdCompteCreateur = VariableStatic.compte.Id;

    const DATA = this.ChiffrerDonnee(_form.value);

    this.mdpServ.Ajouter(DATA).subscribe({
      next: (retour: number) =>
      {
        this.outilServ.ToastOK("Votre mot de passe a été enregistré");

        _form.value.Id = retour;
        this.dialogRef.close(_form.value);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  AfficherMdp()
  {
    this.voirMdp = !this.voirMdp;
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

}
