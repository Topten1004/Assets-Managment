import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapviewComponent } from './mapview.component';

describe('MapviewComponent', () => {
  let component: MapviewComponent;
  let fixture: ComponentFixture<MapviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MapviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MapviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
