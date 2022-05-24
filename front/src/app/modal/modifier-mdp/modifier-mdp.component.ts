import { DatePipe } from '@angular/common';
import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Classes/Aes';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { ExportMdp } from 'src/app/Types/Export/ExportMdp';
import { Mdp } from 'src/app/Types/Mdp';
import { GenerateurMDPComponent } from '../generateur-mdp/generateur-mdp.component';

@Component({
  selector: 'app-modifier-mdp',
  templateUrl: './modifier-mdp.component.html',
  styleUrls: ['./modifier-mdp.component.scss']
})
export class ModifierMdpComponent implements OnInit 
{
  @ViewChild ("inputMdp") inputMdp : ElementRef;
  voirMdp: boolean = false;
  mdp: Mdp;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private mdpServ: MdpService, 
    private outilServ: OutilService,
    private datePipe: DatePipe,
    private dialogRef: MatDialogRef<ModifierMdpComponent>,    
    private dialog: MatDialog,
    ) { }

  ngOnInit(): void
  {
    this.mdp = this.data.mdp;
  }

  Modifier(_form: NgForm): void
  {
    if(_form.invalid)
    {
      this.outilServ.ToastInfo("Veuillez compléter tous les champs");
      return;
    }

    _form.value.IdCompteCreateur = VariableStatic.compte.Id;
    _form.value.Id = this.mdp.Id;
    _form.value.Mdp = this.inputMdp.nativeElement.value;
    _form.value.DateExpiration = this.datePipe.transform(_form.value.DateExpiration, "yyyy-MM-dd");

    let data: ExportMdp = this.ChiffrerDonnee(_form.value);
    
    this.mdpServ.Modifier(data).subscribe({
      next: (retour: boolean) =>
      {
        if(retour == true)
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

  private ChiffrerDonnee(_donnee: Mdp): ExportMdp
  {
    const AES = new Aes(VariableStatic.compte.HashCle);

    const DATA: ExportMdp =
    {
      Id: _donnee.Id,
      IdCompteCreateur: _donnee.IdCompteCreateur,

      Login: AES.Chiffrer(_donnee.Login),
      Description: AES.Chiffrer(_donnee.Description),
      DateExpiration: AES.Chiffrer(_donnee.DateExpiration),
      Mdp: AES.Chiffrer(_donnee.Mdp),
      Titre: AES.Chiffrer(_donnee.Titre),
      Url: AES.Chiffrer(_donnee.Url)
    }

    return DATA;
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
}
