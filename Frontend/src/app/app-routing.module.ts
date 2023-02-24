import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthComponent } from './pages/auth/auth.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { MainLayoutComponent } from './pages/main-layout/main-layout.component';
import { SignupComponent } from './components/auth/signup/signup.component';

const routes: Routes = [
  {
    path: 'auth',
    component: AuthComponent,
    children : [
      {
        path : 'signin',
        component : SigninComponent
      },
      {
        path : 'signup',
        component : SignupComponent
      }
    ]
  },
  {
    path: '**',
    component: MainLayoutComponent,
    children : [
      {
        path : '**',
        component : DashboardComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
