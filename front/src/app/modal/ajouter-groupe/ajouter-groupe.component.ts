import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { Cache } from 'src/app/enum/Cache';
import { CompteService } from 'src/app/services/compte.service';
import { GroupeService } from 'src/app/services/groupe.service';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Static/Aes';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { ExportGroupe } from 'src/app/Types/Export/ExportGroupe';
import { Groupe } from 'src/app/Types/Groupe';
import { Mdp } from 'src/app/Types/Mdp';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-ajouter-groupe',
  templateUrl: './ajouter-groupe.component.html',
  styleUrls: ['./ajouter-groupe.component.scss']
})
export class AjouterGroupeComponent implements OnInit 
{
  @ViewChild("inputMail") inputMail: ElementRef;

  listeMail: string[] = [];
  listeMdp: Mdp[] = [];

  constructor(
    private dialogRef: MatDialogRef<AjouterGroupeComponent>,
    private groupeServ: GroupeService, 
    private outilServ: OutilService,
    private compteServ: CompteService,
    private mdpServ: MdpService
    ) { }

  ngOnInit(): void 
  {
    if(sessionStorage.getItem(Cache.LISTE_MDP))
    {
      this.listeMdp = JSON.parse(sessionStorage.getItem(Cache.LISTE_MDP));
      return;
    }
  
    this.mdpServ.ListerLesMiens().subscribe({
      next: (liste: Mdp[]) =>
      {
        let aes: Aes = new Aes(VariableStatic.compte.HashCle);
        this.listeMdp = aes.DechiffrerMdp(liste);

        sessionStorage.setItem(Cache.LISTE_MDP, JSON.stringify(this.listeMdp));
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });

  }

  AjouterListeMail(_mail: string): void
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
          this.listeMail.push(_mail);
          this.inputMail.nativeElement.value = "";
        }
        else
          this.outilServ.ToastInfo(`L'adresse: ${_mail} n'existe pas`);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  Supprimer(_index: number): void
  {
    this.listeMail.splice(_index, 1);
  }

  Ajouter(_form: NgForm): void
  {
    if(_form.invalid)
      return;

    let aes: Aes = new Aes(VariableStatic.compte.HashCle);
    let listeMailChiffrer: string[] = [];

    for (const element of this.listeMail) 
      listeMailChiffrer.push(aes.Chiffrer(element));

    const DATA: ExportGroupe =
    {
      IdCreateur: VariableStatic.compte.Id,
      Titre: aes.Chiffrer(_form.value.Titre),
      ListeMail: listeMailChiffrer,
      ListeIdMdp: _form.value.ListeIdMdp == "" ? [] : _form.value.ListeIdMdp
    };

    this.groupeServ.Ajouter(DATA).subscribe({
      next: (retour: number) =>
      {
        let listeMdp: Mdp[] = [];

        for (const element of _form.value.ListeIdMdp) 
        {
          const MDP = this.listeMdp.find(m => +m.Id == +element);

          listeMdp.push(MDP);
        }

        const DATA_RETOUR: Groupe = 
        {
          Id: retour,
          IdCompteCreateur: VariableStatic.compte.Id,
          Titre: _form.value.Titre,
          NbCompte: this.listeMail.length,
          NbMdp: listeMdp.length,
          ListeMdp: listeMdp
        };

        this.dialogRef.close(DATA_RETOUR);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    })
  }
}
