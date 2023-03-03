import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogsListComponent } from './logs-list.component';

describe('LogsListComponent', () => {
  let component: LogsListComponent;
  let fixture: ComponentFixture<LogsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LogsListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LogsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
