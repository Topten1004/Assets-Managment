import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';


import { getName, getCode } from 'country-list';
import { MapTypeStyle } from '@agm/core';
import axios from 'axios';
import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, formatDBDate } from 'src/app/utils/helper';

@Component({
  selector: 'app-mapview',
  templateUrl: './mapview.component.html',
  styleUrls: ['./mapview.component.scss']
})

export class MapviewComponent implements OnInit, OnChanges {
  // tank assets list
  @Input() listData: Array<any> = [];

  //selected item on asset-list
  @Input() centerIndex: number = -1;

  //customize listData
  points : Array<any> = [] ;
  isRepair: Array<boolean> = [];

  //view logs
  logList: Array<any> = [];
  viewDetail: boolean = false;

  //check command(fill or repair)
  select_fill_oil_cmd : boolean = false;
  select_repair_tank_cmd : boolean = false;

  //selected item on map
  index: number = 0;

  zoom: number = 1.8;
  current_opened_index : number = -1 ;

  // initial center position for the map
  ct_lat: number = 53.65914;
  ct_lng: number = 0.072050;

  labelOptions = [{
      color: 'white',
      fontFamily: 'bold',
      fontSize: '18px',
      fontWeight: 'bold',
      text: "87"
  },{
    color: 'white',
    fontFamily: 'bold',
    fontSize: '18px',
    fontWeight: 'bold',
    text: "87"
},{
  color: 'white',
  fontFamily: 'bold',
  fontSize: '18px',
  fontWeight: 'bold',
  text: "87"
},{
  color: 'white',
  fontFamily: 'bold',
  fontSize: '18px',
  fontWeight: 'bold',
  text: "87"
},{
  color: 'white',
  fontFamily: 'bold',
  fontSize: '18px',
  fontWeight: 'bold',
  text: "87"
  }]

  endpoints_arr : Array<any> = [];

  Styles : MapTypeStyle[] = [] ;

  JSON_MAP_STYLES = [
    {
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#f5f5f5"
        }
      ]
    },
    {
      "elementType": "labels.icon",
      "stylers": [
        {
          "visibility": "off"
        }
      ]
    },
    {
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#616161"
        }
      ]
    },
    {
      "elementType": "labels.text.stroke",
      "stylers": [
        {
          "color": "#f5f5f5"
        }
      ]
    },
    {
      "featureType": "administrative.land_parcel",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#bdbdbd"
        }
      ]
    },
    {
      "featureType": "poi",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#eeeeee"
        }
      ]
    },
    {
      "featureType": "poi",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#757575"
        }
      ]
    },
    {
      "featureType": "poi.park",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#e5e5e5"
        }
      ]
    },
    {
      "featureType": "poi.park",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#9e9e9e"
        }
      ]
    },
    {
      "featureType": "road",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#ffffff"
        }
      ]
    },
    {
      "featureType": "road.arterial",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#757575"
        }
      ]
    },
    {
      "featureType": "road.highway",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#dadada"
        }
      ]
    },
    {
      "featureType": "road.highway",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#616161"
        }
      ]
    },
    {
      "featureType": "road.local",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#9e9e9e"
        }
      ]
    },
    {
      "featureType": "transit.line",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#e5e5e5"
        }
      ]
    },
    {
      "featureType": "transit.station",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#eeeeee"
        }
      ]
    },
    {
      "featureType": "water",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#c9c9c9"
        }
      ]
    },
    {
      "featureType": "water",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#9e9e9e"
        }
      ]
    }
  ]

  async ngOnInit() {
    // this.getCity(this.ct_lat, this.ct_lng)
    const header = authorization();

    const res =  await axios.get(`${PRIVATE_URI}Command`, header);
  }

  async ngOnChanges(changes:SimpleChanges) {
    let options = [];

    for(let i = 0 ; i < this.listData.length ; i++) {
      options.push({
        color: 'white',
        fontFamily: 'bold',
        fontSize: '18px',
        fontWeight: 'bold',
        text: String(this.listData[i].amount)
      })
    }

    let tank_data = [];
    for (let i = 0 ; i < this.listData.length ; i++) {
      tank_data.push({
          "country_code": "DE",
          "is_block": 1,
          "latitude": this.listData[i].latitude,
          "longitude": this.listData[i].longitude,
          "minAmount": this.listData[i].minAmount,
          "userName": this.listData[i].tankName,
          'userEmail': this.listData[i].userEmail,
          "period": this.listData[i].period
      })
    }
    this.labelOptions = options;
    this.points = tank_data;

    await this.checkedRepair();
  }

  isOpened(index : number) {
    if(index === this.current_opened_index) return true;
    return false;
  }

  clickedMarker(label: string, index: number) {
    this.current_opened_index = index;
  }

  handleOpenModal(i: number) {
    this.index = i;
    this.viewDetail = false;
    (<any>$("#tankSituation")).modal('show')
  }

  getCountryName(code : string) {
    return getName(code) ;
  }

  stringToNumber(value: string) {
    return Number(value)
  }

  constructor() {
    this.JSON_MAP_STYLES.forEach((style:any) => {
      this.Styles.push(style);
    })
  }

  async submitCommand() {
    const header = authorization();


    if(this.select_fill_oil_cmd ) {
      try {

        const res =  await axios.post(`${PRIVATE_URI}Command`, {
          tankName: this.listData[this.index].tankName,
          command: 'Fill'
        }, header)

        if(res.status === 200) {
          this.select_fill_oil_cmd = false;
          alert("Successfully notified Fill")
        }
      } catch(error){
        this.select_fill_oil_cmd = false;
        alert("You already notified Fill")
      }
    }

    if(this.select_repair_tank_cmd ) {
      try{
        const res =  await axios.post(`${PRIVATE_URI}Command`, {
          tankName: this.listData[this.index].tankName,
          command: 'Repair'
        }, header)

        if(res.status === 200) {
          this.select_repair_tank_cmd = false;
          alert("Successfully notified Repair")
        }
      } catch(error){
        this.select_repair_tank_cmd = false;
        alert("You already notified Repair")
      }
    }

  }

  async checkedRepair() {

    const header = authorization();

    for(let i = 0 ; i < this.points.length ; i++) {
      let res = await axios.post(`${PRIVATE_URI}IsRepair`, {
        userEmail: this.points[i].userEmail,
        period: this.points[i].period,
      }, header);

      if(res.status === 200) {
        console.log(res.data)
        this.isRepair[i] = res.data;
      }
    }

  }

  async onViewDetail() {
    this.viewDetail = !this.viewDetail;
    this.logList = [];
    const header = authorization();

    let res = await axios.get(`${PRIVATE_URI}Log`, header);

    if(res.status === 200) {
      for( let i = res.data.length-1 ; i >= 0 ; i--) {
        if(res.data[i].userEmail === this.listData[this.index].userEmail)
          this.logList.push(res.data[i])
      }
    }
  }
  changeDateType(db_date: any) {
    return formatDBDate(db_date)
  }

  // async getCity(lat: number, lng: number) {
  //   xhr.open('GET', "https://us1.locationiq.com/v1/reverse.php?key=AIzaSyAvKu990uAaoiCk_URypgdgFTa4kdn_MBw&lat=" + lat + "&lon=" + lng + "&format=json", true);
  //   xhr.send();
  //   xhr.onreadystatechange = processRequest;
  //   xhr.addEventListener("readystatechange", processRequest, false);

  //   function processRequest(e: any) {
  //       if (xhr.readyState == 4 && xhr.status == 200) {
  //           var response = JSON.parse(xhr.responseText);
  //           var city = response.address.city;
  //           console.log(city);
  //           return;
  //       }
  //   }
  // }
}
