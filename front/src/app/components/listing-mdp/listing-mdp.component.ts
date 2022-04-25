import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ModifierMdpComponent } from 'src/app/modal/modifier-mdp/modifier-mdp.component';
import { PartagerMdpComponent } from 'src/app/modal/partager-mdp/partager-mdp.component';
import { OutilService } from 'src/app/services/outil.service';
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

  // permet d'etre accessible depuis l'HTML comme attribut HTML
  @Input() mesMdp: boolean;

  dataSource: MatTableDataSource<Mdp>;

  displayedColumns: string[] = ['Titre', 'Login', 'Mdp', 'Url', 'DateExpiration', 'action'];

  constructor(private outilServ: OutilService, private dialog: MatDialog) { }

  ngOnInit(): void
  {
    this.dataSource = new MatTableDataSource();
  }

  ngAfterViewInit(): void
  {
    this.dataSource.paginator = this.paginator;
    this.dataSource.paginator._intl.itemsPerPageLabel = "Mots de passe par ligne";
    this.dataSource.sort = this.sort;
  }

  InitListe(_liste: Mdp[]): void
  {
    this.dataSource.data = _liste;
  }

  OuvrirUrl(_url: string): void
  {
    window.open(_url, "_blanck");
  }

  OuvrirModalPartagerMdp(_mdp: Mdp): void
  {
    this.dialog.open(PartagerMdpComponent, { data: { mdp: _mdp }});
  }

  OuvrirModalModifierMdp(_mdp: Mdp): void
  {
    const DIALOG_REF = this.dialog.open(ModifierMdpComponent, { data: { mdp: _mdp }});

    DIALOG_REF.afterClosed().subscribe({
      next: (retour: Mdp) =>
      {
        _mdp.DateExpiration = retour.DateExpiration;
        _mdp.Description = retour.Description;
        _mdp.Login = retour.Login;
        _mdp.Mdp = retour.Mdp;
        _mdp.Url = retour.Url;
      }
    });
  }

  AfficherMdpCaractere(_mdp: string): string
  {
    let caractere = "*";
    return caractere.repeat(_mdp.length);
  }

  ToastrCopierMdp(): void
  {
    this.outilServ.ToastOK("Le mot de passe a été copié");
  }

  applyFilter(event: Event): void
  {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}
