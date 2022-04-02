import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import * as CryptoJS from 'crypto-js';
import { ConnexionService } from 'src/app/services/connexion.service';
import { OutilService } from 'src/app/services/outil.service';

@Component({
  selector: 'app-connexion',
  templateUrl: './connexion.component.html',
  styleUrls: ['./connexion.component.scss']
})
export class ConnexionComponent implements OnInit 
{
  private readonly CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

  constructor(private connexionServ: ConnexionService, private outilServ: OutilService) { }

  ngOnInit(): void 
  {
  }
  
  SeConnecter(_form: NgForm): void
  {
    let cleSecrete = CryptoJS.enc.Utf8.parse(this.CLE_SECRETE);
    let iv = CryptoJS.enc.Utf8.parse(this.CLE_SECRETE.substring(0, 16));

    let mdpChiffrer = CryptoJS.AES.encrypt(_form.value.mdp, cleSecrete, { iv: iv }).toString();
    let loginChiffrer = CryptoJS.AES.encrypt(_form.value.login, cleSecrete, { iv: iv }).toString();

    this.connexionServ.Connexion({ login: loginChiffrer, mdp: mdpChiffrer }).subscribe({
      next: (retour) =>
      {
        console.log(retour);
        
      },
      error: () =>
      {
        this.outilServ.MsgErreurHttp();
      }
    });

    let mdpDecrypt = CryptoJS.AES.decrypt(mdpChiffrer, cleSecrete, { iv: iv }).toString(CryptoJS.enc.Utf8);

    console.log(mdpDecrypt);
  }
}
