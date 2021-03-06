import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { VariableStatic } from '../Classes/VariableStatic';
import { ConfirmationComponent } from '../modal/confirmation/confirmation.component';

@Injectable({
  providedIn: 'root'
})
export class OutilService 
{
  reponseConfirmation: Subject<boolean>;

  constructor(private toastrServ: ToastrService, private dialog: MatDialog, private router: Router) { }

  OuvrirModalConfirmation(_titre: string, _message: string): void
  {
    this.reponseConfirmation = new Subject<boolean>();

    const DIALOG_REF = this.dialog.open(ConfirmationComponent, { data: { titre: _titre, msg: _message }});

    DIALOG_REF.afterClosed().subscribe({
      next: (reponse: boolean) =>
      {
        this.reponseConfirmation.next(reponse);
        this.reponseConfirmation.complete();
      }
    });
  }

  SessionExpirer(): void
  {
    this.ToastErreur("Votre session a éxpirée");
    sessionStorage.clear();
    VariableStatic.compte = undefined;

    this.router.navigate([""]);
  }

  ToastErreurHttp(): void
  {
    this.toastrServ.error("Connexion avec le serveur impossible");
  }

  ToastErreur(_text: string): void
  {
    this.toastrServ.error(_text);
  }

  ToastWarning(_text: string): void
  {
    this.toastrServ.warning(_text);
  }

  ToastOK(_text: string): void
  {
    this.toastrServ.success(_text);
  }

  ToastInfo(_text: string): void
  {
    this.toastrServ.info(_text);
  }
}
