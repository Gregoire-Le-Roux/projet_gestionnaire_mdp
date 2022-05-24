import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConnexionInscriptionComponent } from './components/connexion-inscription/connexion-inscription.component';
import { ListingGroupeComponent } from './components/listing-groupe/listing-groupe.component';
import { ListingMdpComponent } from './components/listing-mdp/listing-mdp.component';
import { MenuMdpComponent } from './components/menu-mdp/menu-mdp.component';
import { ProfilComponent } from './components/profil/profil.component';
import { ConnexionGuard } from './guard/connexion.guard';

const routes: Routes = [
  { path: "", component: ConnexionInscriptionComponent },
  { path: "mdp", canActivate: [ConnexionGuard], component: MenuMdpComponent },
  { path: "groupe", canActivate: [ConnexionGuard], component: ListingGroupeComponent },
  { path: "profil", canActivate: [ConnexionGuard], component: ProfilComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
