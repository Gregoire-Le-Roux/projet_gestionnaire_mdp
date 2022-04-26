import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConnexionComponent } from './components/connexion/connexion.component';
import { ListingGroupeComponent } from './components/listing-groupe/listing-groupe.component';
import { ListingMdpComponent } from './components/listing-mdp/listing-mdp.component';
import { MenuMdpComponent } from './components/menu-mdp/menu-mdp.component';
import { ConnexionGuard } from './guard/connexion.guard';

const routes: Routes = [
  { path: "", component: ConnexionComponent },
  { path: "mdp", canActivate: [ConnexionGuard], component: MenuMdpComponent },
  { path: "groupe", canActivate: [ConnexionGuard], component: ListingGroupeComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
