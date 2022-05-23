import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { VariableStatic } from '../Static/VariableStatic';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> 
  {
    if(VariableStatic.compte)
    {
      request = request.clone({
        headers: request.headers.set("Authorization", `Bearer  ${VariableStatic.compte.Jwt}`)
      });
    }

    return next.handle(request);
  }
}
