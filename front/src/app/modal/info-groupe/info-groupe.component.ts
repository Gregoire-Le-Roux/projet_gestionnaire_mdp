import { Component, ElementRef, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Aes } from 'src/app/Classes/Aes';
import { CompteService } from 'src/app/services/compte.service';
import { GroupeService } from 'src/app/services/groupe.service';
import { OutilService } from 'src/app/services/outil.service';
import { Compte } from 'src/app/Types/Compte';
import { GroupeMdpCompte } from 'src/app/Types/GroupeMdpCompte';
import { environment } from 'src/environments/environment';
import { AjoutMdpGroupeComponent } from '../ajout-mdp-groupe/ajout-mdp-groupe.component';

@Component({
  selector: 'app-info-groupe',
  templateUrl: './info-groupe.component.html',
  styleUrls: ['./info-groupe.component.scss']
})
export class InfoGroupeComponent implements OnInit, OnDestroy
{
  @ViewChild("inputMail") inputMail: ElementRef;

  nomGroupe: string = "";
  infoGroupe: GroupeMdpCompte;

  private idGroupe: number;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private groupeServ: GroupeService,
    private outilServ: OutilService,
    private dialogRef: MatDialogRef<InfoGroupeComponent>,
    private compteServ: CompteService,
    private dialog: MatDialog
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

  ModifierListeMailGroupe(_mail: string): void
  {
    if(_mail.replace(environment.patternVide, "") == "")
      return;

    _mail = _mail.toLowerCase();
    
    if(_mail.match(environment.patternMail) == null)
    {
      this.outilServ.ToastErreur("Veuillez indiquer une adresse mail");
      return;
    }

    this.compteServ.Existe(_mail).subscribe({
      next: (retour: boolean) =>
      {
        if(retour == true)
        {
          this.groupeServ.ExisteCompte(_mail, this.idGroupe).subscribe({
            next: (retour: boolean) =>
            {
              if(retour !== true)
                this.AjouterCompteGrp(_mail);
              else
                this.outilServ.ToastInfo(`L'adresse: ${_mail} est déjà dans le groupe`);
            },
          });
        }
        else
          this.outilServ.ToastInfo(`L'adresse: ${_mail} n'existe pas`);
      }
    });
  }

  private AjouterCompteGrp(_mail: string): void
  {
    this.groupeServ.AjouterCompte({ mail: _mail, idGroupe: this.idGroupe }).subscribe({
      next: (retour: boolean | Compte) =>
      { 
        if(retour !== false)
        {
          retour = retour as Compte;

          const AES = new Aes(retour.HashCle);

          let ajoutCompte = { 
            Id: retour.Id, 
            Prenom: AES.Dechiffrer(retour.Prenom), 
            Nom: AES.Dechiffrer(retour.Nom), 
            Mail: retour.Mail 
          };

          this.infoGroupe.listeCompte.push(ajoutCompte);
          this.inputMail.nativeElement.value = "";
        }
        else
        {
          this.outilServ.ToastErreur("Vous n'avez pas la propriéter du groupe");
        }
      },
    });
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

  OuvrirModalAjouterMdp(): void
  {
    const DIALOG_REF = this.dialog.open(AjoutMdpGroupeComponent, { minWidth: "40%", data: { infoGroupe: this.infoGroupe, idGroupe: this.idGroupe }});

    DIALOG_REF.afterClosed().subscribe({
      next: (retour: any) =>
      {
        console.log(retour);
        

        if(retour != undefined && retour != false)
        {
          for (const element of retour.ListeMdp) 
            this.infoGroupe.listeMdp.push(element);
        }  
      }
    })
  }

}
