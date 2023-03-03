import { NgModule } from '@angular/core';
import { AgmCoreModule } from '@agm/core';
import { AngularCountriesFlagsModule } from 'angular-countries-flags'
import { AgmJsMarkerClustererModule } from '@agm/js-marker-clusterer';
import { MapsAllModule } from '@syncfusion/ej2-angular-maps';
import { ReactiveFormsModule } from '@angular/forms';

import { BrowserModule } from '@angular/platform-browser';
import * as $ from 'jquery';

//store
// import { StoreModule } from '@ngrx/store';
// import { addProductReducer } from './reducers/product.reducer';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { OverviewComponent } from './components/dashboard/overview/overview.component';
import { MapviewComponent } from './components/dashboard/mapview/mapview.component';
import { NotificationsComponent } from './components/dashboard/notifications/notifications.component';
import { ActionsComponent } from './components/dashboard/actions/actions.component';
import { AssetsListComponent } from './components/dashboard/assets-list/assets-list.component';
import { MainLayoutComponent } from './pages/main-layout/main-layout.component';
import { HeaderComponent } from './components/main-layout/header/header.component';
import { FooterComponent } from './components/main-layout/footer/footer.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { AuthComponent } from './pages/auth/auth.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { ManagerDashboardComponent } from './pages/manager-dashboard/manager-dashboard.component';
import { ManagementComponent } from './components/dashboard/management/management.component';
import { LogsListComponent } from './components/dashboard/logs-list/logs-list.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    OverviewComponent,
    MapviewComponent,
    NotificationsComponent,
    ActionsComponent,
    AssetsListComponent,
    MainLayoutComponent,
    HeaderComponent,
    FooterComponent,
    SigninComponent,
    AuthComponent,
    SignupComponent,
    ManagerDashboardComponent,
    ManagementComponent,
    LogsListComponent,
  ],
  imports: [
    ReactiveFormsModule,
    MapsAllModule,
    BrowserModule,
    AngularCountriesFlagsModule,
    AppRoutingModule,
    AgmJsMarkerClustererModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyAvKu990uAaoiCk_URypgdgFTa4kdn_MBw',
      libraries: ['places'],
    }),
    BrowserModule,
    // StoreModule.forRoot({product: addProductReducer})
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
