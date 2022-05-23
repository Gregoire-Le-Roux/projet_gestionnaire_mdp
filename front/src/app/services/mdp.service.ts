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

  ListerLesMiens(): Observable<Mdp[]>
  {
    return this.http.get<Mdp[]>(`${environment.urlApi}/Mdp/listerMesMdp`);
  }

  ListerPartagerAvecMoi(): Observable<Mdp[]>
  {
    return this.http.get<Mdp[]>(`${environment.urlApi}/Mdp/listerPartagerAvecMoi`);
  }

  Ajouter(_info: ExportMdp): Observable<number>
  {
    return this.http.post<number>(`${environment.urlApi}/Mdp/ajouter`, _info);
  }

  Modifier(_info: ExportMdp): Observable<boolean>
  {
    return this.http.put<boolean>(`${environment.urlApi}/Mdp/modifier`, _info)
  }

  Supprimer(_idMdp: number): Observable<boolean>
  {
    return this.http.delete<boolean>(`${environment.urlApi}/Mdp/supprimer/${_idMdp}`);
  }
}
