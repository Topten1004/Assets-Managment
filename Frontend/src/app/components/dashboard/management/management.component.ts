import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import axios from 'axios';
import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, getItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-management',
  templateUrl: './management.component.html',
  styleUrls: ['./management.component.scss']
})
export class ManagementComponent implements OnInit, OnChanges {

  @Input() command_list: Array<any> = [];
  @Output() newBuyEvent: EventEmitter<any> = new EventEmitter();
  @Output() newSellEvent: EventEmitter<any> = new EventEmitter();

  userEmail: string = String(getItem('user_email'));
  own_command: Array<any> = [];

  buyAmount: string = '';
  sellAmount: string = '';
  buyer: string = '';
  seller: string = '';
  isRepair: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.command_list.length !== 0) {
      for(let i = 0 ; i < this.command_list.length ; i++) {
        if(this.command_list[i].userEmail === this.userEmail && this.command_list[i].flag === false) {
          this.own_command.push(this.command_list[i])
        }
      }
    }
  }

  onBuyAmount(event: Event) {
    this.buyAmount = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
    this.newBuyEvent.emit(this.buyAmount);
  }

  onChangeBuyer(event: Event) {
    this.buyer = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  onSellAmount(event: Event) {
    this.sellAmount = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
    this.newSellEvent.emit(this.sellAmount);
  }

  onChangeSeller(event: Event) {
    this.seller = (event.target as HTMLInputElement).value && (event.target as HTMLInputElement).value;
  }

  async onBuyOil() {
    let fillCount = 0;
    const header = authorization();
    for(let i = 0 ; i < this.own_command.length ; i++) {
      if(this.own_command[i].command === 'Fill') {
        let command = this.own_command[i];
        if(fillCount === 0)
            await axios.post(`${PRIVATE_URI}BuyAsset`, {
              userEmail: this.userEmail,
              amount: Number(this.buyAmount),
              from: this.buyer
            }, header) .then(function () {
              axios.get(`${PRIVATE_URI}Log`, header);
              alert("Successfully purchased")
              fillCount++;
            }) .catch(function (error) {
              alert(error.response.data.title)
            })

        if(fillCount !== 0)
            await axios.get(`${PRIVATE_URI}Command/${command.id}`, header)
              .then(async function(res) {
                await axios.get(`${PRIVATE_URI}TotalAsset/${command.userEmail}`, header)
                .then(function() {console.log('asdfsfd')})
                .catch(function(error) { console.log(error.response.data.title)})
              })

      }
    }

    if(fillCount === 0) {
      const user_email = this.userEmail;
      axios.post(`${PRIVATE_URI}BuyAsset`, {
        userEmail: this.userEmail,
        amount: Number(this.buyAmount),
        from: this.buyer
      }, header) .then(async function () {
        alert("Successfully purchased")
        await axios.get(`${PRIVATE_URI}Log`, header);
        await axios.get(`${PRIVATE_URI}TotalAsset/${user_email}`, header)
      }) .catch(function(error) {
        alert(error.response.data.title)
      })
    }
  }

  async onSellOil() {
    const user_email = this.userEmail;
    const header = authorization();

    await axios.post(`${PRIVATE_URI}SellAsset`, {
      userEmail: this.userEmail,
      amount: Number(this.sellAmount),
      from: this.seller
    }, header) .then(function(res) {
      alert('Successfully sold')
      axios.get(`${PRIVATE_URI}TotalAsset/${user_email}`, header)
      axios.get(`${PRIVATE_URI}Log`, header);
    }) .catch(function(error) {
      console.log(error)
      alert(error.response.data.title)
    })

  }

  async onRepair() {
    const header = authorization();
    let repairCount = 0;

    for(let i = 0 ; i < this.own_command.length ; i++) {
      if(this.own_command[i].command === 'Repair') {
        if(repairCount === 0) {
            try{
              let res = await axios.get(`${PRIVATE_URI}RepairAsset/${this.userEmail}`, header)
              if(res.status === 200) {
                alert("Successfully repaired");
                repairCount++;
                axios.get(`${PRIVATE_URI}Log`, header);
              }
            } catch(error) {
              alert(error)
            }
        }

        if(repairCount !== 0) {
          await axios.get(`${PRIVATE_URI}Command/${this.own_command[i].id}`, header);
        }

      }
    }

    if(repairCount === 0) {
      try{
          let res = await axios.get(`${PRIVATE_URI}RepairAsset/${this.userEmail}`, header)
          if(res.status === 200) {
            alert("Successfully repaired");
            axios.get(`${PRIVATE_URI}Log`, header);
          }
      } catch(error) {
        alert(error)
      }
    }
  }
}
