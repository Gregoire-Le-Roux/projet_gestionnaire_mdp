import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Static/Aes';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { ExportMdp } from 'src/app/Types/Export/ExportMdp';
import { Mdp } from 'src/app/Types/Mdp';

@Component({
  selector: 'app-listing-mdp',
  templateUrl: './listing-mdp.component.html',
  styleUrls: ['./listing-mdp.component.scss']
})
export class ListingMdpComponent implements OnInit, AfterViewInit 
{
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  listeMdp: MatTableDataSource<Mdp>;

  displayedColumns: string[] = ['Titre', 'Login', 'Mdp', 'action'];

  constructor(
    private mdpServ: MdpService,
    private outilServ: OutilService, 
    private datePipe: DatePipe) { }

  ngOnInit(): void 
  {
    this.listeMdp = new MatTableDataSource();

    this.ListerMesMdp();
  }

  ngAfterViewInit(): void
  {
    this.listeMdp.paginator = this.paginator;
    this.listeMdp.sort = this.sort;
  }

  Ajouter(_form: NgForm): void
  {
    _form.value.DateExpiration = this.datePipe.transform(_form.value.DateExpiration, "yyyy-MM-dd");
    _form.value.IdCompteCreateur = 1//VariableStatic.compte.Id;

    const DATA = this.ChiffrerDonnee(_form.value);

    this.mdpServ.Ajouter(DATA).subscribe({
      next: (retour) =>
      {
        console.log(retour);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  OuvrirUrl(_url: string): void
  {
    window.open(_url, "_blanck");
  }

  DechiffrerMdp(_mdpChiffrer): string
  {
    let aes: Aes = new Aes("$2a$11$5FPfTSv/dy3XWMDx9d7wPuHiBUuyfSsDEnXNnmlh04ChKFHdTZgU.");

    return aes.Dechiffrer(_mdpChiffrer);
  }

  AfficherMdpCaractere(_mdp: string): string
  {
    let caractere = "*";
    return caractere.repeat(_mdp.length);
  }

  applyFilter(event: Event): void
  {
    const filterValue = (event.target as HTMLInputElement).value;
    this.listeMdp.filter = filterValue.trim().toLowerCase();
  }

  private ChiffrerDonnee(_donnee: ExportMdp): ExportMdp
  {
    let aes: Aes = new Aes("$2a$11$5FPfTSv/dy3XWMDx9d7wPuHiBUuyfSsDEnXNnmlh04ChKFHdTZgU.");//VariableStatic.compte.HashCle);

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

  private ListerMesMdp(): void
  {
    this.mdpServ.ListerLesMiens(1).subscribe({
      next: (retour: Mdp[]) =>
      {
        this.listeMdp.data = this.DechiffrerInfoRecu(retour); 
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
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
}
