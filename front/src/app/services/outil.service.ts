import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class OutilService {

  constructor(private toastrServ: ToastrService) { }

  MsgErreurHttp(): void
  {
    this.toastrServ.error("Connexion avec le serveur impossible");
  }
}
