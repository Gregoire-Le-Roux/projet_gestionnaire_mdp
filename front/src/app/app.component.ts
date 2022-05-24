import { Component, OnInit } from '@angular/core';
import { Cache } from './enum/Cache';
import { VariableStatic } from './Classes/VariableStatic';
import { Router } from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit
{
  constructor(private router : Router) { }

  ngOnInit(): void 
  {
    if(sessionStorage.getItem(Cache.INFO_COMPTE))
    {
      VariableStatic.compte = JSON.parse(sessionStorage.getItem(Cache.INFO_COMPTE));
    }
  }

  EstConnecter(): boolean
  {
    return VariableStatic.compte != undefined;  
  }
  
  Deconnexion() : void
  {
    sessionStorage.clear();
    VariableStatic.compte = undefined;
    this.router.navigate([""]);
  }
}
