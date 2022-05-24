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
import { Jwt } from '../Classes/Jwt';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private outilServ: OutilService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> 
  {
    if(VariableStatic.compte)
    {
      console.log(Jwt.EstExpirer());
      

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

          return throwError(() => null);
        })
    );
  }
}
