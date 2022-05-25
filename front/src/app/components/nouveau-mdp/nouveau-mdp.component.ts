import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Jwt } from 'src/app/Classes/Jwt';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { CompteService } from 'src/app/services/compte.service';
import { OutilService } from 'src/app/services/outil.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-nouveau-mdp',
  templateUrl: './nouveau-mdp.component.html',
  styleUrls: ['./nouveau-mdp.component.scss']
})
export class NouveauMdpComponent implements OnInit 
{
  mdpVisible: boolean = false;
  mdpConfirmeVisible: boolean = false;
  btnClicker: boolean = false;

  private jwt: Jwt;
  private token: string;
  private jwtEstValide: boolean = true;
  private patternVide: RegExp = environment.patternVide;

  constructor(
    private activateRoute: ActivatedRoute, 
    private router: Router,
    private outilServ: OutilService,
    private compteServ: CompteService
    ) { }

  ngOnInit(): void 
  {
    this.token = this.activateRoute.snapshot.paramMap.get("token");

    this.jwt = new Jwt(this.token);

    if(!this.jwt.EstValide())
    {
      this.jwtEstValide = false;
      this.outilServ.ToastErreur("Cette demande est expirée");
      this.router.navigate([""]);
    }
  }

  AfficherMdp(): void
  {
    this.mdpVisible = !this.mdpVisible;
  }

  AfficherMdpConfirmer(): void
  {
    this.mdpConfirmeVisible = !this.mdpConfirmeVisible;
  }

  ValiderNouveauMdp(_form: NgForm): void
  {
    if(!this.jwtEstValide || this.btnClicker)
      return;

    let mdp = _form.value.mdp.replace(this.patternVide, "");
    let mdpConfirmer = _form.value.mdpConfirmer.replace(this.patternVide, "");

    if(mdp != mdpConfirmer || mdp == "" || mdpConfirmer == "")
    {
      this.outilServ.ToastWarning("Les deux mots de passes ne sont pas identiques");
      return;
    }

    this.btnClicker = true;

    // fack compte pour passer l'interceptor
    VariableStatic.compte = 
    {
      Id: 0,
      Nom: "",
      Prenom: "",
      Mail: "",
      HashCle: "",
      Jwt: this.token
    };

    this.compteServ.ModifierMdp(mdp).subscribe({
      next: (retour: boolean) =>
      {
        VariableStatic.compte = undefined;
        this.outilServ.ToastOK("Votre mot de passe a été modifié");
        this.router.navigate([""]);
      },
      error: () =>
      {
        this.btnClicker = false;
      }
    });
  }
}
