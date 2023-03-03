import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import axios from 'axios';

import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, formatDBDate, getItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-logs-list',
  templateUrl: './logs-list.component.html',
  styleUrls: ['./logs-list.component.scss']
})

export class LogsListComponent implements OnInit {

  private hubConnectionBuilder!: HubConnection;

  log_list: Array<any> = [];

  constructor() { }

  async ngOnInit() {
    this.log_list = [];
    const header = authorization();

    let res = await axios.get(`${PRIVATE_URI}Log`, header);

    if(res.status === 200) {
      for( let i = res.data.length-1 ; i >= 0 ; i--) {
        if(res.data[i].userEmail === getItem('user_email'))
          this.log_list.push(res.data[i])
      }
    }

    this.initWebSocket()

  }

  async initWebSocket(): Promise<void> {

    this.hubConnectionBuilder = new HubConnectionBuilder()
      .withUrl('https://10.10.18.211:5001/logs')
      .configureLogging(LogLevel.Information)
      .build();
    this.hubConnectionBuilder
      .start()
      .then(() => console.log('Connection started.......!'))
      .catch(err => console.log('Error while connect with server'));

    this.hubConnectionBuilder.on('SendLogs', (data: any) => {
      this.log_list = [];
      for( let i = data.length-1 ; i >= 0 ; i--) {
        if(data[i].userEmail === getItem('user_email'))
          this.log_list.push(data[i])
      }
      console.log(this.log_list)
    });

  }

  changeDateType(db_date: any) {
    return formatDBDate(db_date)
  }

}
