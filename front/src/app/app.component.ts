import { Component, OnInit } from '@angular/core';
import { Cache } from './enum/Cache';
import { VariableStatic } from './Static/VariableStatic';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit
{
  ngOnInit(): void 
  {
    if(sessionStorage.getItem(Cache.INFO_COMPTE))
    {
      VariableStatic.compte = JSON.parse(sessionStorage.getItem(Cache.INFO_COMPTE));
    }
  }

  EstConnecter(): boolean
  {
    return VariableStatic.compte?.Id ? true : false;  
  }
}
