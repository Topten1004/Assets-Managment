import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import axios from 'axios';
import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, getItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-manager-dashboard',
  templateUrl: './manager-dashboard.component.html',
  styleUrls: ['./manager-dashboard.component.scss']
})
export class ManagerDashboardComponent implements OnInit {
  buyAmount: string = '';
  sellAmount: string = ''

  userEmail: string = String(getItem('user_email'));

  private hubConnectionBuilder!: HubConnection;
  notify_list: Array<any> = [];

  ngOnInit(): void {
    this.initWebSocket()
  }

  async initWebSocket(): Promise<void> {

    this.hubConnectionBuilder = new HubConnectionBuilder()
      .withUrl('https://10.10.18.211:5001/commands')
      .configureLogging(LogLevel.Information)
      .build();
    this.hubConnectionBuilder
      .start()
      .then(() => console.log('Connection started.......!'))
      .catch(err => console.log('Error while connect with server'));

    this.hubConnectionBuilder.on('SendCommands', (data: any) => {
      this.notify_list = data;
    });

    // this.hubConnectionBuilder.send("SendOffersToUser", () => {

    // });

  }

  buyEventHandler(amount: string) {
    this.buyAmount = amount;
  }

  sellEventHandler(amount: string) {
    this.sellAmount = amount;
  }

  confirmNotify(item: any) {
    if(item.command === 'Fill')
      (<any>$('#buyOil')).modal('show')
    else
      (<any>$('#repairModal')).modal('show')
  }
}
