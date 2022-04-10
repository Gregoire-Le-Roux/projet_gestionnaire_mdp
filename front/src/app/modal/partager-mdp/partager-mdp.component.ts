import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Mdp } from 'src/app/Types/Mdp';

@Component({
  selector: 'app-partager-mdp',
  templateUrl: './partager-mdp.component.html',
  styleUrls: ['./partager-mdp.component.scss']
})
export class PartagerMdpComponent implements OnInit 
{
  mdp: Mdp;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void 
  {
    this.mdp = this.data.mdp;
  }

  Partager(): void
  {
    
  }
}
