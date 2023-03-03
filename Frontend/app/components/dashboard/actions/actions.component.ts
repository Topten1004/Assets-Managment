import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import axios from 'axios';

import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, getItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-actions',
  templateUrl: './actions.component.html',
  styleUrls: ['./actions.component.scss']
})
export class ActionsComponent implements OnInit {

  @Output() newActionElement: EventEmitter<any> = new EventEmitter();

  name: String = '';
  email: String = '';
  latitude: String = '';
  longitude: String = '';
  password: String = '';
  maxAmount: String = '';

  limitAmount: string = '';
  periodDay: string = '';

  constructor() { }

  async ngOnInit() {
    const header = authorization();

    let res = await axios.get(`${PRIVATE_URI}Asset`, header);

    if(res.status === 200) {
      this.limitAmount = res.data[0].minAmount;
      this.periodDay = res.data[0].period;
    }
  }

  onNameChange(event: Event): void {
    this.name = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  onEmailChange(event: Event): void {
    this.email = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  onLatitudeChange(event: Event): void {
    this.latitude = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  onLongitudeChange(event: Event): void {
    this.longitude = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  onPasswordChange(event: Event): void {
    this.password = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  onAmountChange(event: Event): void {
    this.maxAmount = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  async onRegister() {

    const tankName = this.name;
    const header = authorization();

    await axios.post(`${PRIVATE_URI}Asset`, {
      tankName: this.name,
      userEmail: this.email,
      password: this.password,
      latitude: Number(this.latitude),
      longitude: Number(this.longitude),
      maxAmount: Number(this.maxAmount),
    }, header)
      .then(async function (response) {
          alert("Successfully registered")
      }) .catch(function (error) {
        console.log('This manager already exist')
      })

    let resp = await axios.get(`${PRIVATE_URI}TotalAsset/${getItem('user_email')}`, header);

    if(resp.status === 200) {
      this.newActionElement.emit(resp.data)
    }
  }

  onSetLimitAmount(event: Event): void {
    this.limitAmount = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }
  onSetPeriodDay(event: Event): void {
    this.periodDay = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  async onSendAlert() {
    const header = authorization();

    let res = await axios.post(`${PRIVATE_URI}SetLimitAmount`, {
      minAmount: this.limitAmount,
      period: this.periodDay
    }, header)
    if(res.status === 200) {
      alert('Success')
    }

    let resp = await axios.get(`${PRIVATE_URI}TotalAsset/${getItem('user_email')}`, header);

    if(resp.status === 200) {
      this.newActionElement.emit(resp.data)
    }
  }
}
