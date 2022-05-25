import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompteValiderComponent } from './components/compte-valider/compte-valider.component';
import { ConnexionInscriptionComponent } from './components/connexion-inscription/connexion-inscription.component';
import { ListingGroupeComponent } from './components/listing-groupe/listing-groupe.component';
import { ListingMdpComponent } from './components/listing-mdp/listing-mdp.component';
import { MdpOublierComponent } from './components/mdp-oublier/mdp-oublier.component';
import { MenuMdpComponent } from './components/menu-mdp/menu-mdp.component';
import { NouveauMdpComponent } from './components/nouveau-mdp/nouveau-mdp.component';
import { ProfilComponent } from './components/profil/profil.component';
import { ConnexionGuard } from './guard/connexion.guard';

const routes: Routes = [
  { path: "", component: ConnexionInscriptionComponent },
  { path: "mdp", canActivate: [ConnexionGuard], component: MenuMdpComponent },
  { path: "groupe", canActivate: [ConnexionGuard], component: ListingGroupeComponent },
  { path: "profil", canActivate: [ConnexionGuard], component: ProfilComponent },

  { path: "compteValider/:token", component: CompteValiderComponent },
  { path: "mot-de-passe-oublie", component: MdpOublierComponent },
  { path: "nouveau-mot-de-passe/:token", component: NouveauMdpComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
