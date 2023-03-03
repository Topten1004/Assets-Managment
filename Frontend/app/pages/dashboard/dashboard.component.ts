import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import axios from 'axios';

import { Asset } from 'src/app/constant/asset.model';
import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, getItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  private hubConnectionBuilder!: HubConnection;

  tank_list: Array<Asset> = [];
  tankCount: number = 0;
  selected_index: number = -1;

  constructor() {
  }

  async ngOnInit() {

    const header = authorization();

    let res = await axios.get(`${PRIVATE_URI}TotalAsset/${getItem('user_email')}`, header);

    if(res.status === 200) {
      this.tank_list = res.data.assets
    }

    this.initWebSocket()
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
      this.tank_list = data.assets;
    });

  }

  selectedEventHandler(index: number) {
    this.selected_index = index;
  }

  actionEventHandler(data: any) {
    this.tank_list = data.assets;
    this.tankCount = data.assets.length;
  }
}
