import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ExportMdp } from '../Types/Export/ExportMdp';
import { Mdp } from '../Types/Mdp';

@Injectable({
  providedIn: 'root'
})
export class MdpService
{
  constructor(private http: HttpClient) { }

  Lister(_idCompte: number): Observable<Mdp[]>
  {
    return this.http.get<Mdp[]>(`${environment.urlApi}/Mdp/lister`);
  }

  Ajouter(_info: ExportMdp): Observable<any>
  {
    return this.http.post(`${environment.urlApi}/Mdp/ajouter`, _info);
  }
}