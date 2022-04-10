import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConnexionComponent } from './components/connexion/connexion.component';
import { ListingMdpComponent } from './components/listing-mdp/listing-mdp.component';
import { MenuMdpComponent } from './components/menu-mdp/menu-mdp.component';

const routes: Routes = [
  { path: "", component: ConnexionComponent },
  { path: "mdp", component: MenuMdpComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
