import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ExportGroupe } from '../Types/Export/ExportGroupe';
import { Groupe } from '../Types/Groupe';

@Injectable({
  providedIn: 'root'
})
export class GroupeService {

  constructor(private http: HttpClient) { }

  Lister(_idCompte: number): Observable<Groupe[]>
  {
    return this.http.get<Groupe[]>(`${environment.urlApi}/Groupe/lister/${_idCompte}`);
  }

  Ajouter(_info: ExportGroupe): Observable<number>
  {
    return this.http.post<number>(`${environment.urlApi}/Groupe/ajouter`, _info);
  }
}
