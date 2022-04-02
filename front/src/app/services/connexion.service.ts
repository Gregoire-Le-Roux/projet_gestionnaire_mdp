import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Compte } from '../Types/Compte';

@Injectable({
  providedIn: 'root'
})
export class ConnexionService 
{
  constructor(private http: HttpClient) { }

  Connexion(_info: any): Observable<string | Compte>
  {
    return this.http.post<string | Compte>(`${environment.urlApi}/connexion/connexion`, _info);
  }
}
