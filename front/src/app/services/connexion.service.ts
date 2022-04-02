import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConnexionService 
{
  constructor(private http: HttpClient) { }

  Connexion(_info: any): Observable<boolean>
  {
    return this.http.post<boolean>(`${environment.urlApi}/connexion/connexion`, _info);
  }
}
