import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';

import { PRIVATE_URI } from 'src/app/constant/static';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {

  registerForm: FormGroup;
  isSignUp: boolean = false;

  private initFormBuilder(): FormGroup {
    return new FormGroup({
      email: new FormControl({value: '', disabled: false}, [Validators.required]),
      pwd: new FormControl({value: '', disabled: false}, [Validators.required]),
      confirm_pwd: new FormControl({value: '', disabled: false}, [Validators.required]),
    })
  }

  constructor() {
    this.registerForm = this.initFormBuilder()
  }

  async onFormSubmit() {
    if(this.registerForm.value.pwd === this.registerForm.value.confirm_pwd)
    {
      await axios.post(`${PRIVATE_URI}Register`, {
        userEmail: this.registerForm.value.email,
        password: this.registerForm.value.pwd,
        role: 1
      }) .then(function (response) {

        if(response.data.statusCode === 200) {
          console.log("Success")
        }
      }) .catch(function (error) {
        console.log(error)
      })
    }
  }

  ngOnInit(): void {
  }

}
