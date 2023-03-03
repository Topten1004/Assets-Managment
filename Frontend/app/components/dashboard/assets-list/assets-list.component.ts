import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import axios from 'axios';
import { Asset } from 'src/app/constant/asset.model';
import { PRIVATE_URI } from 'src/app/constant/static';
import { authorization, formatDBDate, getItem } from 'src/app/utils/helper';

@Component({
  selector: 'app-assets-list',
  templateUrl: './assets-list.component.html',
  styleUrls: ['./assets-list.component.scss']
})
export class AssetsListComponent implements OnInit, OnChanges {
  @Input() listData: any = [];
  @Output() newSelectedEvent: EventEmitter<number> = new EventEmitter();

  index: number = -1;
  logList: Array<any> = [];
  viewDetail: boolean = false;

  constructor() { }

  search_text: string = '';

  assetList : Array<number> = [1,2,3,4,5,6,7,8,9,10] ;

  perPageCount : number = 10;
  pageIndex : number = 0;

  pageCount : number = this.listData.length % this.perPageCount ? Number(Math.floor(this.listData.length / this.perPageCount)) + 1 : Number(Math.floor(this.listData.length / this.perPageCount));
  perPageList : Array<Asset> = [];

  ngOnInit(): void {
  }

  ngOnChanges(changes:SimpleChanges) {
    this.changePerPageList();
  }

  changePerPageList() {
    if(this.perPageCount * (this.pageIndex + 1) >= this.listData.length) {
      this.perPageList = this.listData.slice(this.pageIndex * this.perPageCount, this.listData.length);
    } else {
      this.perPageList = this.listData.slice(this.pageIndex * this.perPageCount, this.perPageCount * (this.pageIndex + 1));
    }

  }

  goToNext() {
    if(this.pageIndex === this.pageCount) return ;
    this.pageIndex++;
    this.changePerPageList() ;
  }

  goToPrev() {
    if(this.pageIndex === 0) return ;
    this.pageIndex--;
    this.changePerPageList();
  }

  onChangeIndex(i: number) {
    this.viewDetail = false;
    this.index = i
    this.newSelectedEvent.emit(this.index);
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
}
