import { Component, Inject, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MdpService } from 'src/app/services/mdp.service';
import { OutilService } from 'src/app/services/outil.service';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { Mdp } from 'src/app/Types/Mdp';

@Component({
  selector: 'app-modifier-mdp',
  templateUrl: './modifier-mdp.component.html',
  styleUrls: ['./modifier-mdp.component.scss']
})
export class ModifierMdpComponent implements OnInit 
{
  voirMdp: boolean = false;
  mdp: Mdp;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private mdpServ: MdpService, 
    private outilServ: OutilService
    ) { }

  ngOnInit(): void
  {
    this.mdp = this.data.mdp;
  }

  Modifier(_form: NgForm): void
  {
    if(_form.invalid)
    {
      this.outilServ.ToastInfo("Veuillez compléter tous les champs");
      return;
    }

    _form.value.IdCompteCreateur = VariableStatic.compte.Id;
    _form.value.Id = this.mdp.Id;

    this.mdpServ.Modifier(_form.value).subscribe({
      next: (retour: boolean) =>
      { 
        console.log(retour);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  AfficherMdp(): void
  {
    this.voirMdp = !this.voirMdp;
  }

}
