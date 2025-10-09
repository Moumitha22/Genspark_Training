import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDiscountFilter } from './admin-discount-filter';

describe('AdminDiscountFilter', () => {
  let component: AdminDiscountFilter;
  let fixture: ComponentFixture<AdminDiscountFilter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDiscountFilter]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminDiscountFilter);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
