import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Cache } from './enum/Cache';
import { VariableStatic } from './Classes/VariableStatic';
import { Router } from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, AfterViewInit
{
  private estFalse = false;

  constructor(private router : Router) { }

  ngOnInit(): void 
  {
    if(sessionStorage.getItem(Cache.INFO_COMPTE))
    {
      VariableStatic.compte = JSON.parse(sessionStorage.getItem(Cache.INFO_COMPTE));
    }
  }

  ngAfterViewInit(): void 
  {
    setTimeout(() => {
      this.estFalse = true;
    }, 0);
  }

  EstConnecter(): boolean
  {
    // en cas d'inscription evite erreur NG1000 de merde !
    if(!this.estFalse)
      return false;

    return VariableStatic.compte != undefined; 
  }
  
  Deconnexion() : void
  {
    sessionStorage.clear();
    VariableStatic.compte = undefined;
    this.router.navigate([""]);
  }
}
