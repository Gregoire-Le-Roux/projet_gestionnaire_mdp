import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';

// angular mat
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatTableModule} from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatMenuModule } from '@angular/material/menu';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';

import { AppRoutingModule } from './app-routing.module';
import { ToastrModule } from 'ngx-toastr';

import { AppComponent } from './app.component';
import { ConnexionComponent } from './components/connexion/connexion.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ListingMdpComponent } from './components/listing-mdp/listing-mdp.component';
import { AjouterMdpComponent } from './modal/ajouter-mdp/ajouter-mdp.component';
import { MenuMdpComponent } from './components/menu-mdp/menu-mdp.component';
import { PartagerMdpComponent } from './modal/partager-mdp/partager-mdp.component';
import { ModifierMdpComponent } from './modal/modifier-mdp/modifier-mdp.component';
import { ListingGroupeComponent } from './components/listing-groupe/listing-groupe.component';

// permet de donner la possibilité de refrech la page en mode prod en ajoutant un # sur URL
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { AjouterGroupeComponent } from './modal/ajouter-groupe/ajouter-groupe.component';
import { ConfirmationComponent } from './modal/confirmation/confirmation.component';
import { InfoGroupeComponent } from './modal/info-groupe/info-groupe.component';
import { InscriptionComponent } from './components/inscription/inscription.component';
import { ConnexionInscriptionComponent } from './components/connexion-inscription/connexion-inscription.component';
import { GenerateurMDPComponent } from './modal/generateur-mdp/generateur-mdp.component';

import { JwtInterceptor } from './interceptor/jwt.interceptor';
import { ProfilComponent } from './components/profil/profil.component';
import { CompteValiderComponent } from './components/compte-valider/compte-valider.component';
import { MdpOublierComponent } from './components/mdp-oublier/mdp-oublier.component';
import { NouveauMdpComponent } from './components/nouveau-mdp/nouveau-mdp.component';
import { AjoutMdpGroupeComponent } from './modal/ajout-mdp-groupe/ajout-mdp-groupe.component';

@NgModule({
  declarations: [
    AppComponent,
    ConnexionComponent,
    ListingMdpComponent,
    AjouterMdpComponent,
    MenuMdpComponent,
    PartagerMdpComponent,
    ModifierMdpComponent,
    ListingGroupeComponent,
    AjouterGroupeComponent,
    ConfirmationComponent,
    InfoGroupeComponent,
    InscriptionComponent,
    ConnexionInscriptionComponent,
    GenerateurMDPComponent,
    ProfilComponent,
    CompteValiderComponent,
    MdpOublierComponent,
    NouveauMdpComponent,
    AjoutMdpGroupeComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ToastrModule.forRoot({
      timeOut: 3000,
      progressBar: true,
      progressAnimation: 'increasing'
    }),
    BrowserAnimationsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatTooltipModule,
    ClipboardModule,
    MatCheckboxModule,
    MatDialogModule,
    MatTabsModule,
    MatMenuModule,
    MatCardModule,
    MatToolbarModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatExpansionModule
  ],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy }, 
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: MAT_DATE_LOCALE, useValue: 'fr-FR' }, 

    DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
