import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompteService 
{
  constructor(private http: HttpClient) { }

  Existe(_mail: string): Observable<boolean>
  {
    return this.http.get<boolean>(`${environment.urlApi}/Compte/existe/${_mail}`);
  }
}
