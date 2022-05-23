import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatDialogRef } from '@angular/material/dialog';
import { GenerateurMDP } from 'src/app/Static/GenerateurMDP';

@Component({
  selector: 'app-generateur-mdp',
  templateUrl: './generateur-mdp.component.html',
  styleUrls: ['./generateur-mdp.component.scss']
})
export class GenerateurMDPComponent implements OnInit, AfterViewInit {

  @ViewChild ("inputMdp") inputMdp : ElementRef;
  voirMdp: boolean = false;
  longueur: number = 18;
  contientMinuscule: boolean = true;
  contientMajuscule: boolean = true;
  contientNombre: boolean = true;
  contientSpeciaux: boolean = true;

  constructor(
    private dialogRef: MatDialogRef<GenerateurMDPComponent>) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.Generer();      
    }, 0);
  }

  AfficherMdp()
  {
    this.voirMdp = !this.voirMdp;
  }

  Ajouter() : void
  {
    this.dialogRef.close(this.inputMdp.nativeElement.value);
  }

  ChangerLongueur(number : number) {
    this.longueur = number;
    this.Generer();
  }

  ContientMdp(checkbox : MatCheckboxChange) {
    switch(checkbox.source.name) {
      case "minuscule":
        this.contientMinuscule = checkbox.checked;
        break;
      case "majuscule":
        this.contientMajuscule = checkbox.checked;
        break;
      case "nombre":
        this.contientNombre = checkbox.checked;
        break;
      case "speciaux":
        this.contientSpeciaux = checkbox.checked;
        break;
      default:
        break;
    }
    this.Generer();
  }

  Generer() : void
  {
    let generateur = new GenerateurMDP(this.longueur, this.contientMinuscule, this.contientMajuscule, this.contientNombre, this.contientSpeciaux);
    let mdp : string = generateur.Generer();
    this.inputMdp.nativeElement.value = mdp;
  }
}
