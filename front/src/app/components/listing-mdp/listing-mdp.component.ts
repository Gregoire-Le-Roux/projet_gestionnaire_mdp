import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { Aes } from 'src/app/Static/Aes';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { ExportMdp } from 'src/app/Types/Export/ExportMdp';

@Component({
  selector: 'app-listing-mdp',
  templateUrl: './listing-mdp.component.html',
  styleUrls: ['./listing-mdp.component.scss']
})
export class ListingMdpComponent implements OnInit 
{
  constructor(
    private mdpServ: MdpService,
    private outilServ: OutilService, 
    private datePipe: DatePipe) { }

  ngOnInit(): void 
  {
  }

  Ajouter(_form: NgForm): void
  {
    _form.value.DateExpiration = this.datePipe.transform(_form.value.DateExpiration, "yyyy-MM-dd");
    _form.value.IdCompteCreateur = 1//VariableStatic.compte.Id;

    const DATA = this.ChiffrerDonnee(_form.value);

    this.mdpServ.Ajouter(DATA).subscribe({
      next: (retour) =>
      {
        console.log(retour);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  private ChiffrerDonnee(_donnee: ExportMdp): ExportMdp
  {
    let aes: Aes = new Aes("$2a$11$/b0lsRu7I.GuQXzXB/Nie.Q5q7TJ9X28.eD.BIpW23rSc2TxDmdG2");//VariableStatic.compte.HashCle);

    const DATA: ExportMdp =
    {
      Titre: aes.Chiffrer(_donnee.Titre),
      Login: aes.Chiffrer(_donnee.Login),
      Mdp: aes.Chiffrer(_donnee.Mdp),
      Url: aes.Chiffrer(_donnee.Url),
      Description: aes.Chiffrer(_donnee.Description),
      DateExpiration: aes.Chiffrer(_donnee.DateExpiration),
      IdCompteCreateur: _donnee.IdCompteCreateur
    };

    return DATA;
  }

}
