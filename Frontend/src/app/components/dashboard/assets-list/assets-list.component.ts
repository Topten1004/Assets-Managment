import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-assets-list',
  templateUrl: './assets-list.component.html',
  styleUrls: ['./assets-list.component.scss']
})
export class AssetsListComponent implements OnInit {

  constructor() { }

  select_fill_oil_cmd : boolean = false;
  select_repair_tank_cmd : boolean = false;

  assetList : Array<number> = [1,2,3,4,5,6,7,8,9,10] ;

  perPageCount : number = 8;
  pageIndex : number = 0;

  pageCount : number = this.assetList.length % this.perPageCount ? Number(Math.floor(this.assetList.length / this.perPageCount)) + 1 : Number(Math.floor(this.assetList.length / this.perPageCount));
  perPageList : Array<number> = [];

  ngOnInit(): void {
    this.changePerPageList();
  }

  changePerPageList() {
    if(this.perPageCount * (this.pageIndex + 1) >= this.assetList.length) {
      this.perPageList = this.assetList.slice(this.pageIndex * this.perPageCount, this.assetList.length);
    } else {
      this.perPageList = this.assetList.slice(this.pageIndex * this.perPageCount, this.perPageCount * (this.pageIndex + 1));
    }

    console.log(this.pageCount);
  }

  goToNext() {
    if(this.pageIndex === this.pageCount-1) return ;
    this.pageIndex++;
    this.changePerPageList() ;
  }

  goToPrev() {
    if(this.pageIndex === 0) return ;
    this.pageIndex--;
    this.changePerPageList();
  }

  submitCommand() {
    this.select_fill_oil_cmd = false;
    this.select_repair_tank_cmd = false;
  }
}
