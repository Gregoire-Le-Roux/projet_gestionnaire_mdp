import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AjouterMdpComponent } from 'src/app/modal/ajouter-mdp/ajouter-mdp.component';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Static/Aes';
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
          this.component.toArray()[0].dataSource.data = this.listeMesMdp;
        }
      }
    });
  }

  private DechiffrerInfoRecu(_liste: Mdp[]): Mdp[]
  {
    let aes: Aes = new Aes("$2a$11$5FPfTSv/dy3XWMDx9d7wPuHiBUuyfSsDEnXNnmlh04ChKFHdTZgU.");

    for (const element of _liste) 
    {
      element.DateExpiration =  aes.Dechiffrer(element.DateExpiration);  
      element.Login = aes.Dechiffrer(element.Login);
      element.Titre = aes.Dechiffrer(element.Titre);
      element.Url = aes.Dechiffrer(element.Url);
      element.Mdp = aes.Dechiffrer(element.Mdp);
    }

    return _liste;
  }

  private ListerMesMdp(): void
  {
    this.mdpServ.ListerLesMiens(1).subscribe({
      next: (retour: Mdp[]) =>
      {
        if(retour.length > 0)
        {
          this.listeMesMdp = this.DechiffrerInfoRecu(retour); 
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
    this.mdpServ.ListerPartagerAvecMoi(1).subscribe({
      next: (retour: Mdp[]) =>
      {
        if(retour.length > 0)
        {
          this.listeMdpPartagerAvecMoi = this.DechiffrerInfoRecu(retour);
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
