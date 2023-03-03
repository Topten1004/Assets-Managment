import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { getItem, isAuthenticated } from './utils/helper';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent {

  constructor(private router: Router) {

    if(isAuthenticated()) {
      let userEmail = getItem('user_email')
      if(userEmail === 'admin')
        this.router.navigate(['/dashboard'])
      else
        this.router.navigate(['/dashboard/manager'])
    } else {
      this.router.navigate(['/auth/signin'])
    }
  }

  ngInit() {
  }
}
