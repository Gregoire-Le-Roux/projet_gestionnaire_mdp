import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { VariableStatic } from '../Classes/VariableStatic';
import { OutilService } from '../services/outil.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor 
{
  constructor(private outilServ: OutilService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> 
  {
    if(VariableStatic.compte != undefined)
    {
      request = request.clone({
        headers: request.headers.set("Authorization", `Bearer  ${VariableStatic.compte.Jwt}`)
      });
    }

    return next.handle(request).pipe(
      catchError(
        (erreur) => 
        { 
          if(erreur.status == 401)
            this.outilServ.SessionExpirer();
          else if(erreur.status == 0)
            this.outilServ.ToastErreurHttp();
          else
            this.outilServ.ToastErreur("Une erreur c'est produite demande à Gérard de fix le soucis");

          return throwError(() => null);
        })
    );
  }
}
