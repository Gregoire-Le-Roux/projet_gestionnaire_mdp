import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Compte } from '../Types/Compte';
import { ExportCompte } from '../Types/Export/ExportCompte';

@Injectable({
  providedIn: 'root'
})
export class CompteService 
{
  constructor(private http: HttpClient) { }

  EnvoyerMailMotDePasseOublie(_mail: string): Observable<string>
  {
    return this.http.get<string>(`${environment.urlApi}/Compte/mdpOublier/${_mail}`);
  }

  ModifierMdp(_mdp: string): Observable<boolean>
  {
    const DATA = { Mdp: _mdp };

    return this.http.put<boolean>(`${environment.urlApi}/Compte/modifierMdp`, DATA);
  }

  DemanderInscription(_compte: ExportCompte): Observable<string>
  {
    return this.http.post<string>(`${environment.urlApi}/Compte/demanderInscription`, _compte);
  }

  Inscription(_compte: ExportCompte): Observable<Compte>
  {
    return this.http.post<Compte>(`${environment.urlApi}/Compte/inscription`, _compte);
  }

  Existe(_mail: string): Observable<boolean>
  {
    return this.http.get<boolean>(`${environment.urlApi}/Compte/existe/${_mail}`);
  }

  Supprimer(): Observable<boolean>
  {
    return this.http.delete<boolean>(`${environment.urlApi}/Compte/supprimer`);
  }
}
