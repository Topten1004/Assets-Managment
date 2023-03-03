import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import axios from 'axios';

import { PRIVATE_URI } from 'src/app/constant/static';
import { setItem } from '../../../utils/helper'

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.scss']
})
export class SigninComponent implements OnInit {

  loginForm: FormGroup;

  private initFormBuilder(): FormGroup {
    return new FormGroup({
      email: new FormControl({value: '', disabled: false}, [Validators.required]),
      pwd: new FormControl({value: '', disabled: false}, [Validators.required]),
    })
  }

  constructor(
    public router: Router,
  ) {
    this.loginForm = this.initFormBuilder()
    this.router.events.subscribe((event) => {
      if(event instanceof NavigationEnd) {
      }
    })
  }

  ngOnInit(): void {
  }

  async onFormSubmit() {
    let userEmail = this.loginForm.value.email;
    let trouter = this.router;

    await axios.post(`${PRIVATE_URI}Login`, {
      userEmail: this.loginForm.value.email,
      password: this.loginForm.value.pwd,
    }) .then(function (response) {
        setItem('access_token', response.data)
        setItem('user_email', userEmail)
        if(userEmail === 'admin')
          trouter.navigate(['/dashboard'])
        else
          trouter.navigate(['/dashboard/manager'])
    }) .catch(function (error) {
      alert(error.response.data)
    })

  }

}
