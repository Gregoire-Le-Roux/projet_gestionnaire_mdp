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

  Inscription(_compte: ExportCompte): Observable<Compte>
  {
    return this.http.post<Compte>(`${environment.urlApi}/Compte/Ajouter`, _compte);
  }

  Existe(_mail: string): Observable<boolean>
  {
    return this.http.get<boolean>(`${environment.urlApi}/Compte/existe/${_mail}`);
  }
}
