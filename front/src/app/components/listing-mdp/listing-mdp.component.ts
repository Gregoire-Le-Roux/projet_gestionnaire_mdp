import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PartagerMdpComponent } from 'src/app/modal/partager-mdp/partager-mdp.component';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { VariableStatic } from 'src/app/Static/VariableStatic';
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

  displayedColumns: string[] = ['Titre', 'Login', 'Mdp', 'Url', 'action'];

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
