import { Component } from '@angular/core';
import { VariableStatic } from './Static/VariableStatic';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent 
{
  EstConnecter(): boolean
  {
    return VariableStatic.compte?.Id ? true : false;  
  }
}
