import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Aes } from 'src/app/Classes/Aes';
import { Jwt } from 'src/app/Classes/Jwt';
import { VariableStatic } from 'src/app/Classes/VariableStatic';
import { Cache } from 'src/app/enum/Cache';
import { CompteService } from 'src/app/services/compte.service';
import { OutilService } from 'src/app/services/outil.service';
import { Compte } from 'src/app/Types/Compte';
import { ExportCompte } from 'src/app/Types/Export/ExportCompte';

@Component({
  selector: 'app-compte-valider',
  templateUrl: './compte-valider.component.html',
  styleUrls: ['./compte-valider.component.scss']
})
export class CompteValiderComponent implements OnInit 
{
  private readonly CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

  estInscrit: boolean = false;
  retour: string;

  constructor(
    private activateRoute: ActivatedRoute, 
    private compteServ: CompteService,
    private outilServ: OutilService,
    private router: Router) { }

  ngOnInit(): void 
  {
    let token = this.activateRoute.snapshot.paramMap.get("token");

    let jwt = new Jwt(token);

    if(!jwt.EstValide())
    {
      this.outilServ.ToastInfo("Cette confirmation à expiré");
      return;
    }

    let infoJwt = jwt.InfoToken();

    const DATA: ExportCompte =
    {
      Nom: infoJwt.Nom,
      Prenom: infoJwt.Prenom,
      Mail: infoJwt.Mail,
      Mdp: infoJwt.Mdp
    }

    this.compteServ.Inscription(DATA).subscribe({
      next: (retour: Compte) =>
      {
        let aes = new Aes(this.CLE_SECRETE);

        VariableStatic.compte = 
        {
          Id:  retour.Id,
          Nom: aes.Dechiffrer(DATA.Nom),
          Prenom: aes.Dechiffrer(DATA.Prenom),
          Mail: aes.Dechiffrer(DATA.Mail),
          HashCle: retour.HashCle,
          Jwt: retour.Jwt
        }

        this.estInscrit = true;
        sessionStorage.setItem(Cache.INFO_COMPTE, JSON.stringify(VariableStatic.compte));

        this.outilServ.ToastOK(`Incription réussi, bienvenue ${VariableStatic.compte.Prenom} !`);

        this.router.navigate(["/mdp"])
      }
    });
  }
}
