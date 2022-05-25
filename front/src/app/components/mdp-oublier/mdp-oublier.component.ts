import { Component } from '@angular/core';
import { CompteService } from 'src/app/services/compte.service';
import { OutilService } from 'src/app/services/outil.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-mdp-oublier',
  templateUrl: './mdp-oublier.component.html',
  styleUrls: ['./mdp-oublier.component.scss']
})
export class MdpOublierComponent 
{
  btnClicker: boolean = false;
  patternMail: RegExp = environment.patternMail;

  constructor(private compteServ: CompteService, private outilServ: OutilService) { }

  EnvoyerMailMdpOublier(_mail: string): void
  {
    if(this.btnClicker)
      return;

    this.btnClicker = true;

    this.compteServ.EnvoyerMailMotDePasseOublie(_mail).subscribe({
      next: (retour: string) =>
      {
        this.btnClicker = false;
        this.outilServ.ToastInfo(retour);
      },
      error: () =>
      {
        this.btnClicker = false;
      }
    });
  }
}
