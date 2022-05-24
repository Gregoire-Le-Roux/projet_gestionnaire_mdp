import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Cache } from 'src/app/enum/Cache';
import { AjouterGroupeComponent } from 'src/app/modal/ajouter-groupe/ajouter-groupe.component';
import { InfoGroupeComponent } from 'src/app/modal/info-groupe/info-groupe.component';
import { GroupeService } from 'src/app/services/groupe.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Classes/Aes';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { Groupe } from 'src/app/Types/Groupe';
import { GroupeMdpCompte } from 'src/app/Types/GroupeMdpCompte';
import { CompteImport } from 'src/app/Types/Import/CompteImport';
import { Mdp } from 'src/app/Types/Mdp';

@Component({
  selector: 'app-listing-groupe',
  templateUrl: './listing-groupe.component.html',
  styleUrls: ['./listing-groupe.component.scss']
})
export class ListingGroupeComponent implements OnInit, AfterViewInit
{
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataSource: MatTableDataSource<Groupe>;

  displayedColumns: string[] = ['Titre', 'NbCompte', 'NbMdp', 'action'];

  constructor(
    private groupeServ: GroupeService, 
    private outilServ: OutilService,
    private dialog: MatDialog) { }

  ngOnInit(): void
  {
    this.dataSource = new MatTableDataSource();
    this.ListerGroupe();
  }

  ngAfterViewInit(): void 
  {
    this.dataSource.paginator = this.paginator;
    this.dataSource.paginator._intl.itemsPerPageLabel = "Groupe par ligne";
    this.dataSource.sort = this.sort;
  }

  ConfirmerSupprimerGroupe(_titreGroupe: string, _idGroupe: number, _index: number, _event: Event): void
  {
    _event.stopPropagation();

    const TITRE = `Confirmation suppression groupe: ${_titreGroupe}`;
    const MESSAGE = `Confirmer vous la suppression du groupe: ${_titreGroupe} ?`;

    this.outilServ.OuvrirModalConfirmation(TITRE, MESSAGE);

    this.outilServ.reponseConfirmation.subscribe({
      next: (retour: boolean) =>
      {
        if(retour == true)
        {
          this.SupprimerGroupe(_idGroupe, _index);
        }
        else
          this.outilServ.ToastInfo("Suppression annulée");
      }
    });
  }

  OuvrirModalAjouterGroupe(): void
  {
    const DIALOG_REF = this.dialog.open(AjouterGroupeComponent);

    DIALOG_REF.afterClosed().subscribe({
      next: (retour: Groupe) =>
      {
        if(retour)
        {
          this.dataSource.data.push(retour);
          this.dataSource.data = this.dataSource.data;
        }
      }
    });
  }

  OuvrirModalModifierGroupe(_event: Event): void
  {
    _event.stopPropagation();
  }

  OuvrirModalInfoGroupe(_idGroupe: number, _nomGroupe: string, _listeMdp: Mdp[], _event: Event): void
  {
    _event.stopPropagation();

    this.groupeServ.ListerCompte(_idGroupe).subscribe({
      next: (retour: CompteImport[]) =>
      {
        let mdpCompteGroupe = this.InitInfoGroupe(retour, _listeMdp);
        
        this.dialog.open(InfoGroupeComponent, 
          { minWidth: "40%", data: { nomGroupe: _nomGroupe, idGroupe: _idGroupe, infoGroupe: mdpCompteGroupe }});
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }
    
  applyFilter(event: Event): void
  {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  private InitInfoGroupe(_listeCompte: CompteImport[], _listeMdp: Mdp[]): GroupeMdpCompte
  {
    let mdpCompteGroupe: GroupeMdpCompte =
    {
      listeCompte: [],
      listeMdp: []
    };

    const AES = new Aes(VariableStatic.compte.HashCle);

    // dechiffrer les comptes
    for (const element of _listeCompte) 
    {
      mdpCompteGroupe.listeCompte.push({
        Id: element.Id,
        Nom: AES.Dechiffrer(element.Nom),
        Prenom: AES.Dechiffrer(element.Prenom),
        Mail: AES.Dechiffrer(element.Mail)
      });
    }

    // ajouter les MDPs
    mdpCompteGroupe.listeMdp = _listeMdp;

    return mdpCompteGroupe;
  }

  private SupprimerGroupe(_idGroupe: number, _index: number): void
  {
    this.groupeServ.Supprimer(_idGroupe).subscribe({
      next: (retour: boolean) =>
      {
        if(retour == true)
        {
          this.dataSource.data.splice(_index, 1);
          this.dataSource.data = this.dataSource.data;

          this.outilServ.ToastOK("Le groupe a bien été supprimeé");
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

  private ListerGroupe(): void
  {
    this.groupeServ.Lister().subscribe({
      next: (liste: Groupe[]) =>
      {  
        let aes = new Aes(VariableStatic.compte.HashCle);

        for (let groupe of liste)
        {
          groupe.Titre = aes.Dechiffrer(groupe.Titre);
          groupe.ListeMdp = aes.DechiffrerMdp(groupe.ListeMdp);
        }
        
        this.dataSource.data = liste;
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }
}
