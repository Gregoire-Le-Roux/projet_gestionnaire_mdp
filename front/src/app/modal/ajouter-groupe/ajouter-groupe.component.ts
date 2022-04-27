import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CompteService } from 'src/app/services/compte.service';
import { GroupeService } from 'src/app/services/groupe.service';
import { OutilService } from 'src/app/services/outil.service';
import { VariableStatic } from 'src/app/Static/VariableStatic';
import { ExportGroupe } from 'src/app/Types/Export/ExportGroupe';

@Component({
  selector: 'app-ajouter-groupe',
  templateUrl: './ajouter-groupe.component.html',
  styleUrls: ['./ajouter-groupe.component.scss']
})
export class AjouterGroupeComponent implements OnInit 
{
  @ViewChild("inputMail") inputMail: ElementRef;

  listeMail: string[] = [];

  constructor(
    private groupeServ: GroupeService, 
    private outilServ: OutilService,
    private compteServ: CompteService
    ) { }

  ngOnInit(): void {
  }

  AjouterListeMail(_mail: string): void
  { 
    if(_mail.replace(/ /g, "") == "")
      return;

    _mail = _mail.toLowerCase();
    
    if(_mail.match(/([a-z0-9-._]+)@([a-z]+).([a-z]+)/) == null)
    {
      this.outilServ.ToastErreur("Veuillez indiquer une adresse mail");
      return;
    }

    this.compteServ.Existe(_mail).subscribe({
      next: (retour: boolean) =>
      {
        if(retour == true)
        {
          this.listeMail.push(_mail);
          this.inputMail.nativeElement.value = "";
        }
        else
          this.outilServ.ToastInfo(`L'adresse: ${_mail} n'existe pas`);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    });
  }

  Supprimer(_index: number): void
  {
    this.listeMail.splice(_index, 1);
  }

  Ajouter(_form: NgForm): void
  {
    if(_form.invalid)
      return;

    const DATA: ExportGroupe =
    {
      IdCreateur: VariableStatic.compte.Id,
      Titre: _form.value.Titre,
      listeMail: this.listeMail
    };

    this.groupeServ.Ajouter(DATA).subscribe({
      next: (retour) =>
      {
        console.log(retour);
      },
      error: () =>
      {
        this.outilServ.ToastErreurHttp();
      }
    })
  }
}
