import { Component, Inject, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GroupeService } from 'src/app/services/groupe.service';
import { OutilService } from 'src/app/services/outil.service';
import { GroupeMdpCompte } from 'src/app/Types/GroupeMdpCompte';

@Component({
  selector: 'app-info-groupe',
  templateUrl: './info-groupe.component.html',
  styleUrls: ['./info-groupe.component.scss']
})
export class InfoGroupeComponent implements OnInit 
{
  nomGroupe: string = "";
  infoGroupe: GroupeMdpCompte;

  private idGroupe: number;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private groupeServ: GroupeService,
    private outilServ: OutilService
    ) { }

  ngOnInit(): void 
  {
    this.nomGroupe = this.data.nomGroupe;
    this.infoGroupe = this.data.infoGroupe;
    this.idGroupe = this.data.idGroupe;
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
        if(retour == true)
        {
          
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
        console.log(retour);
        
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

}
