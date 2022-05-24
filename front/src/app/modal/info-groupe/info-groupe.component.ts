import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GroupeService } from 'src/app/services/groupe.service';
import { OutilService } from 'src/app/services/outil.service';
import { GroupeMdpCompte } from 'src/app/Types/GroupeMdpCompte';

@Component({
  selector: 'app-info-groupe',
  templateUrl: './info-groupe.component.html',
  styleUrls: ['./info-groupe.component.scss']
})
export class InfoGroupeComponent implements OnInit, OnDestroy
{
  nomGroupe: string = "";
  infoGroupe: GroupeMdpCompte;

  private idGroupe: number;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private groupeServ: GroupeService,
    private outilServ: OutilService,
    private dialogRef: MatDialogRef<InfoGroupeComponent>
    ) { }

  ngOnInit(): void 
  {
    this.nomGroupe = this.data.nomGroupe;
    this.infoGroupe = this.data.infoGroupe;
    this.idGroupe = this.data.idGroupe;
  }

  ngOnDestroy(): void {
    this.dialogRef.close({ NbCompte: this.infoGroupe.listeCompte.length, NbMdp: this.infoGroupe.listeMdp.length });
  }

  SupprimerCompteGroupe(_form: NgForm): void
  {
    if(_form.value.listeIdCompte == "" || _form.value.listeIdCompte == [])
    {
      this.outilServ.ToastInfo("Veuillez choisir au moins un compte");
      return;
    }
    
    _form.value.idGroupe = this.idGroupe;
    
    this.groupeServ.SupprimerCompte(_form.value).subscribe({
      next: (retour: boolean) =>
      {
        if(retour)
        {
          for(let i = 0; i < _form.value.listeIdCompte.length; i++)
          {
            const INDEX_COMPTE = this.infoGroupe.listeCompte.findIndex(x => x.Id == _form.value.listeIdCompte[i]);
            this.infoGroupe.listeCompte.splice(INDEX_COMPTE, 1);
          }
        }
        else
          this.outilServ.ToastErreurHttp();
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  SupprimerMdpGroupe(_form: NgForm): void
  {
    if(_form.value.listeIdMdp == "" || _form.value.listeIdMdp == [])
    {
      this.outilServ.ToastInfo("Veuillez choisir au moins un mot de passe");
      return;
    }

    _form.value.idGroupe = this.idGroupe;

    this.groupeServ.SupprimerMdp(_form.value).subscribe({
      next: (retour: boolean) =>
      {
        if(retour)
        {
          for(let i = 0; i < _form.value.listeIdMdp.length; i++)
          {
            const INDEX_MDP = this.infoGroupe.listeMdp.findIndex(x => x.Id == _form.value.listeIdMdp[i]);
            this.infoGroupe.listeMdp.splice(INDEX_MDP, 1);
          }
        }
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

}
