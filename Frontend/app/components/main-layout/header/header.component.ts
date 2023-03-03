import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import axios from 'axios';
import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, getItem, removeItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  isMatch: boolean = false;

  oldPwd: string = '';
  newPwd: string = '';
  confirmPwd: string = '';

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  onSignOut(): void {
    removeItem('access_token');
    removeItem('user_email');
    this.router.navigate(['/auth/signin']);
  }

  onOldPassword(event: any) {
    this.oldPwd = event.target.value;
  }

  onNewPassword(event: any) {
    this.newPwd = event.target.value;
  }

  onConfirmPassword(event: any) {
    this.confirmPwd = event.target.value;
  }

  async onChangePassword() {

    const header = authorization();
    console.log(this.newPwd, this.confirmPwd)
    if(this.newPwd !== this.confirmPwd) {
      alert(`Password is do not matched`);
    }
    else {
        let res = await axios.post(`${PRIVATE_URI}ChangePassword`, {
          userEmail: getItem('user_email'),
          oldPassword: this.oldPwd,
          newPassword: this.newPwd
        }, header) .then(function() {
          alert("Successfully changed")

        }) .catch(function(error) {
        alert(error.response.data)
      })
    }
  }

}
