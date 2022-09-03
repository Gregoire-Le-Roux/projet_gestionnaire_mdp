import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { VariableStatic } from '../Classes/VariableStatic';
import { Compte } from '../Types/Compte';
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

  Lister(): Observable<Groupe[]>
  {
    return this.http.get<Groupe[]>(`${environment.urlApi}/Groupe/lister`);
  }

  ListerCompte(_idGroupe: number): Observable<CompteImport[]>
  {
    return this.http.get<CompteImport[]>(`${environment.urlApi}/Groupe/listerCompte/${_idGroupe}`);
  }

  ExisteCompte(_compteMail: string, _idGroupe: number): Observable<boolean>
  {
    return this.http.get<boolean>(`${environment.urlApi}/Groupe/existeCompte/${_idGroupe}/${_compteMail}`);
  }

  ExisteMdp(_idMdp: number, _idGroupe: number): Observable<Compte>
  {
    return this.http.get<Compte>(`${environment.urlApi}/Groupe/existeMdp/${_idGroupe}/${_idMdp}`);
  }

  Ajouter(_info: ExportGroupe): Observable<number>
  {
    return this.http.post<number>(`${environment.urlApi}/Groupe/ajouter`, _info);
  }

  AjouterCompte(_groupeMail: any): Observable<boolean | Compte>
  {
    return this.http.post<boolean | Compte>(`${environment.urlApi}/Groupe/ajouterCompte`, _groupeMail);
  }

  AjouterMdp(_groupeMdp: any): Observable<boolean>
  {
    return this.http.post<boolean>(`${environment.urlApi}/Groupe/ajouterMdp`, _groupeMdp);
  }

  Supprimer(_idGroupe: number): Observable<boolean>
  {
    return this.http.delete<boolean>(`${environment.urlApi}/Groupe/supprimer/${_idGroupe}`);
  }

  SupprimerMdp(_info: ExportMdpGroupe): Observable<boolean>
  {
    return this.http.post<boolean>(`${environment.urlApi}/Groupe/supprimerMdp`, _info);
  }

  SupprimerCompte(_info: ExportCompteGroupe): Observable<boolean>
  {
    return this.http.post<boolean>(`${environment.urlApi}/Groupe/supprimerCompte`, _info);
  }
}
