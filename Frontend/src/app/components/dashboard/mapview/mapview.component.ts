import { Component, OnInit } from '@angular/core';


import data from 'src/json/data.json' ;
import { getName, getCode } from 'country-list';
import { MapTypeStyle } from '@agm/core';

@Component({
  selector: 'app-mapview',
  templateUrl: './mapview.component.html',
  styleUrls: ['./mapview.component.scss']
})

export class MapviewComponent implements OnInit {


  points : Array<any> = [] ;

  isShowDataInfo :boolean = false ;
  zoom: number = 1;
  current_opened_index : number = -1 ;

  // isShowDataInfo : boolean = false ;
  current_info : any ;

  // initial center position for the map
  ct_lat: number = 0;
  ct_lng: number = 0;

  labelOptions = {
      color: 'white',
      fontFamily: 'bold',
      fontSize: '18px',
      fontWeight: 'bold',
      text: "87"
  }

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

  isOpened(index : number) {
    if(index === this.current_opened_index) return true;
    return false;
  }

  showEndpointInfo(data: any) {
    this.current_info = data;
    // this.isShowDataInfo = true ;

    console.log(data);
    return ;
  }

  clickedMarker(label: string, index: number) {
    this.current_opened_index = index;
  }

  getCountryName(code : string) {
    return getName(code) ;
  }

  constructor() {
    this.points = data;
    this.JSON_MAP_STYLES.forEach((style:any) => {
      this.Styles.push(style);
    })
    // if(Array.isArray(endpointsMap)) {
    //   endpointsMap.map((item) => {
    //     this.endpoints_arr.push({
    //       ...item,
    //       country_name : this.getCountryName(item.country_code),
    //       flag_path : `http://purecatamphetamine.github.io/country-flag-icons/3x2/${item.country_code}.svg`
    //     })
    //   })
    //   console.log('dfdf');
    // }
  }

  ngOnInit(): void {
  }

}
