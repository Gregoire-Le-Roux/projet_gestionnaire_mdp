import { Component, Inject, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Aes } from 'src/app/Classes/Aes';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { Cache } from 'src/app/enum/Cache';
import { GroupeService } from 'src/app/services/groupe.service';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { GroupeMdpCompte } from 'src/app/Types/GroupeMdpCompte';
import { Mdp } from 'src/app/Types/Mdp';

@Component({
  selector: 'app-ajout-mdp-groupe',
  templateUrl: './ajout-mdp-groupe.component.html',
  styleUrls: ['./ajout-mdp-groupe.component.scss']
})
export class AjoutMdpGroupeComponent implements OnInit {

  infoGroupe: GroupeMdpCompte;
  listeMdp: Mdp[] = [];

  private idGroupe: number;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private groupeServ: GroupeService,
    private dialogRef: MatDialogRef<AjoutMdpGroupeComponent>,
    private outilServ: OutilService,
    private mdpServ: MdpService,
    ) { }

  ngOnInit(): void {    
    this.infoGroupe = this.data.infoGroupe;
    this.idGroupe = this.data.idGroupe;
    if(sessionStorage.getItem(Cache.LISTE_MDP))
    {
      this.listeMdp = JSON.parse(sessionStorage.getItem(Cache.LISTE_MDP));
      for(let i = 0; i < this.infoGroupe.listeMdp.length; i++)
      {
        const INDEX_MDP = this.listeMdp.findIndex(x => x.Id == this.infoGroupe.listeMdp[i].Id);
        this.listeMdp.splice(INDEX_MDP, 1);
      }
      return;
    }
  
    this.mdpServ.ListerLesMiens().subscribe({
      next: (liste: Mdp[]) =>
      {
        let aes: Aes = new Aes(VariableStatic.compte.HashCle);
        this.listeMdp = aes.DechiffrerMdp(liste);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  Ajouter(_form: NgForm): void
  {
    if(_form.invalid || _form.value.ListeIdMdp == "")
      return;

    let aes: Aes = new Aes(VariableStatic.compte.HashCle);

    this.groupeServ.AjouterMdp({ ListeIdMdp: _form.value.ListeIdMdp, IdGroupe: this.idGroupe }).subscribe({
      next: (retour: boolean) =>
      {
        if(retour) {
          let listeMdp: Mdp[] = [];

          for (const element of _form.value.ListeIdMdp) 
          {
            const MDP = this.listeMdp.find(m => +m.Id == +element);
  
            listeMdp.push(MDP);
          }
  
          const DATA_RETOUR: any = 
          {
            ListeMdp: listeMdp
          };
  
          this.dialogRef.close(DATA_RETOUR);
        }        
      }
    })
  }

}
