import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UtilizationComponent } from './utilization.component';

describe('UtilizationComponent', () => {
  let component: UtilizationComponent;
  let fixture: ComponentFixture<UtilizationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UtilizationComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UtilizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
