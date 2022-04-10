import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class OutilService {

  constructor(private toastrServ: ToastrService) { }

  ToastErreurHttp(): void
  {
    this.toastrServ.error("Connexion avec le serveur impossible");
  }

  ToastErreur(_text: string): void
  {
    this.toastrServ.error(_text);
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
