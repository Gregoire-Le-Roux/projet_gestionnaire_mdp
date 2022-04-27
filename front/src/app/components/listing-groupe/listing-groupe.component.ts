import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AjouterGroupeComponent } from 'src/app/modal/ajouter-groupe/ajouter-groupe.component';
import { GroupeService } from 'src/app/services/groupe.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Static/Aes';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { Groupe } from 'src/app/Types/Groupe';
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

  displayedColumns: string[] = ['Titre'];

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

  OuvrirModalAjouterGroupe(): void
  {
    const DIALOG_REF = this.dialog.open(AjouterGroupeComponent);

    DIALOG_REF.afterClosed().subscribe({
      next: (retour: Groupe) =>
      {
        console.log(retour);
        
        if(retour)
        {
          this.dataSource.data.push(retour);
          this.dataSource.data = this.dataSource.data;
        }
      }
    });
  }
  
  applyFilter(event: Event): void
  {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  private ListerGroupe(): void
  {
    this.groupeServ.Lister(VariableStatic.compte.Id).subscribe({
      next: (liste: Groupe[]) =>
      {
        console.log(liste);
        
        this.dataSource.data = liste;
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    })
  }
}
