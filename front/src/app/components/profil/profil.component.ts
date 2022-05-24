import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { ConfirmationComponent } from 'src/app/modal/confirmation/confirmation.component';
import { CompteService } from 'src/app/services/compte.service';
import { OutilService } from 'src/app/services/outil.service';
import { Compte } from 'src/app/Types/Compte';

@Component({
  selector: 'app-profil',
  templateUrl: './profil.component.html',
  styleUrls: ['./profil.component.scss']
})
export class ProfilComponent implements OnInit {

  compte = VariableStatic.compte;
  
  constructor(
    private dialog: MatDialog,
    private compteServ: CompteService, 
    private outilServ: OutilService,
    private router: Router) { }

  ngOnInit(): void {
  }

  SupprimerCompte(): void
  {
    this.compteServ.Supprimer().subscribe({
      next: (retour: boolean) =>
      {        
        if(retour) {
          sessionStorage.clear();
          VariableStatic.compte = undefined;
          this.outilServ.ToastOK("Votre compte et toutes vos données ont bien été supprimés");
          this.router.navigate([""]);
        }
        else {
          this.outilServ.ToastErreur("Votre compte n'a pas pu être supprimé.");
        }
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  OuvrirModalSupprimerCompte(): void
  {
    const DIALOG_REF = this.dialog.open(ConfirmationComponent, { data: { titre: "Supprimer mon compte", msg: "" }});

    DIALOG_REF.afterClosed().subscribe({
      next: (retour: boolean) =>
      {
        if(retour)
        {
          this.SupprimerCompte();
        }
      }
    });
  }
}
