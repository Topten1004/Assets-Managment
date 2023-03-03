import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import axios from 'axios';

import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, getItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent implements OnInit, OnChanges {

  @Input() tank_count: number = 0;

  private hubConnectionBuilder!: HubConnection;

  user_email = getItem('user_email');

  totalAssets: number = 0;
  count: number = 0;
  // assets: number = 0;
  // latitude: number = 0;
  // longitude: number = 0;
  managerAsset: any;

  constructor( ) {
  }

  async ngOnInit() {
    let userEmail = this.user_email
    let totalAmount: number = 0
    let tankCount: number = 0;
    let mng_assets: any;

    const header = authorization();


    await axios.get(`${PRIVATE_URI}TotalAsset/${this.user_email}`, header).then (function(res) {
      totalAmount = res.data.totalAmount;
      tankCount = res.data.count;

      for(let i = 0 ; i < res.data.assets.length ; i++ ) {
        if(userEmail === res.data.assets[i].userEmail) {
          mng_assets = res.data.assets[i]
        }
      }
    }).catch(function(error) {
      console.log(error)
    })

    this.totalAssets = totalAmount;
    this.count = tankCount;
    this.managerAsset = mng_assets;
    this.initWebSocket()
  }

  ngOnChanges(changes:SimpleChanges) {
    this.count = this.tank_count;
  }

  async initWebSocket(): Promise<void> {

    this.hubConnectionBuilder = new HubConnectionBuilder()
      .withUrl('https://10.10.18.211:5001/assets')
      .configureLogging(LogLevel.Information)
      .build();
    this.hubConnectionBuilder
      .start()
      .then(() => console.log('Connection started.......!'))
      .catch(err => console.log('Error while connect with server'));

    this.hubConnectionBuilder.on('SendTotalAsset', (data: any) => {
      this.totalAssets = 0;

      for(let i = 0 ; i < data.assets.length ; i++ ) {
        this.totalAssets += data.assets[i].amount
        if(this.user_email === data.assets[i].userEmail) {
          this.managerAsset = data.assets[i];
        }
      }
    });

  }
}
