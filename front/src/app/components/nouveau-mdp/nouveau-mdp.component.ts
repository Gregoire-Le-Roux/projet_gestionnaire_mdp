import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Jwt } from 'src/app/Classes/Jwt';
import { OutilService } from 'src/app/services/outil.service';

@Component({
  selector: 'app-nouveau-mdp',
  templateUrl: './nouveau-mdp.component.html',
  styleUrls: ['./nouveau-mdp.component.scss']
})
export class NouveauMdpComponent implements OnInit 
{
  private jwt: Jwt;

  constructor(private activateRoute: ActivatedRoute, private outilServ: OutilService) { }

  ngOnInit(): void 
  {
    const TOKEN = this.activateRoute.snapshot.paramMap.get("token");

    this.jwt = new Jwt(TOKEN);

    if(!this.jwt.EstValide())
    {
      this.outilServ.ToastErreur("Cette demande est expirée");
      return;
    }
  }

  ValiderNouveauMdp(_form: NgForm): void
  {
    if(_form.value.mdp == _form.value.mdpConfirmer)
    {
      this.outilServ.ToastWarning("Les deux mots de passes ne sont pas identique");
      return;
    }
  }

  private FormValide(_donnee): boolean
  {
    let formValide: boolean = true;

    

    return formValide;
  }
}
