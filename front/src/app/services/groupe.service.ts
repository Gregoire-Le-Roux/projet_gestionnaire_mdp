import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { VariableStatic } from '../Static/VariableStatic';
import { ExportGroupe } from '../Types/Export/ExportGroupe';
import { Groupe } from '../Types/Groupe';
import { CompteImport } from '../Types/Import/CompteImport';

type ExportCompteGroupe =
{
  listeIdCompte: number[],
  idGroupe: number
}

type ExportMdpGroupe =
{
  listeIdMdp: number[],
  idGroupe: number
}

@Injectable({
  providedIn: 'root'
})
export class GroupeService 
{
  constructor(private http: HttpClient) { }

  Lister(_idCompte: number): Observable<Groupe[]>
  {
    return this.http.get<Groupe[]>(`${environment.urlApi}/Groupe/lister/${_idCompte}`);
  }

  ListerCompte(_idGroupe: number): Observable<CompteImport[]>
  {
    return this.http.get<CompteImport[]>(`${environment.urlApi}/Groupe/listerCompte/${_idGroupe}/${VariableStatic.compte.Id}`);
  }

  Ajouter(_info: ExportGroupe): Observable<number>
  {
    return this.http.post<number>(`${environment.urlApi}/Groupe/ajouter`, _info);
  }

  Supprimer(_idGroupe: number): Observable<boolean>
  {
    return this.http.delete<boolean>(`${environment.urlApi}/Groupe/supprimer/${_idGroupe}`);
  }

  SupprimerMdp(_info: ExportMdpGroupe): Observable<boolean>
  {
    return this.http.post<boolean>(`${environment.urlApi}/Groupe/supprimerMdp`, _info);
  }

  SupprimerCompte(_info: ExportMdpGroupe): Observable<boolean>
  {
    return this.http.post<boolean>(`${environment.urlApi}/Groupe/supprimerCompte`, _info);
  }
}
