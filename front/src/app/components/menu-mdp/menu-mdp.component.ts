import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Cache } from 'src/app/enum/Cache';
import { AjouterMdpComponent } from 'src/app/modal/ajouter-mdp/ajouter-mdp.component';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Classes/Aes';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { Mdp } from 'src/app/Types/Mdp';
import { ListingMdpComponent } from '../listing-mdp/listing-mdp.component';

@Component({
  selector: 'app-menu-mdp',
  templateUrl: './menu-mdp.component.html',
  styleUrls: ['./menu-mdp.component.scss']
})
export class MenuMdpComponent implements OnInit 
{
  // recupere les deux components de la page HTML sous forme de liste
  @ViewChildren(ListingMdpComponent) component = new QueryList<ListingMdpComponent>();

  private listeMdpPartagerAvecMoi: Mdp[] = [];
  private listeMesMdp: Mdp[] = [];

  constructor(
    private mdpServ: MdpService, 
    private outilServ: OutilService,
    private dialog: MatDialog) { }

  ngOnInit(): void 
  {
    this.ListerMesMdp();
    this.ListerMdpPartagerAvecMoi();
  }

  OuvrirModalAjouterMdp(): void
  {
    const DIALOG_REF = this.dialog.open(AjouterMdpComponent);

    DIALOG_REF.afterClosed().subscribe({
      next: (retour: Mdp) =>
      {
        if(retour)
        {
          this.listeMesMdp.push(retour);
          sessionStorage.setItem(Cache.LISTE_MDP, JSON.stringify(this.listeMesMdp));
          this.component.toArray()[0].dataSource.data = this.listeMesMdp;
        }
      }
    });
  }

  private ListerMesMdp(): void
  {
    this.mdpServ.ListerLesMiens().subscribe({
      next: (retour: Mdp[]) =>
      {
        if(retour.length > 0)
        { 
          let aes: Aes = new Aes(VariableStatic.compte.HashCle);

          this.listeMesMdp = aes.DechiffrerMdp(retour);
          
          sessionStorage.setItem(Cache.LISTE_MDP, JSON.stringify(this.listeMesMdp));

          this.component.toArray()[0].InitListe(this.listeMesMdp);
        }
        else
          this.outilServ.ToastInfo("Vous n'avez encore aucun mot de passe");
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  private ListerMdpPartagerAvecMoi(): void
  {
    this.mdpServ.ListerPartagerAvecMoi().subscribe({
      next: (retour: Mdp[]) =>
      {
        if(retour.length > 0)
        {
          let aes: Aes = new Aes(VariableStatic.compte.HashCle);

          this.listeMdpPartagerAvecMoi = aes.DechiffrerMdp(retour);
          this.component.toArray()[1].InitListe(this.listeMdpPartagerAvecMoi);
        }
        else
          this.outilServ.ToastInfo("Aucun mot passe n'a été partagé avec vous");
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }
}
