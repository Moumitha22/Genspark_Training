import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDiscountList } from './admin-discount-list';

describe('AdminDiscountList', () => {
  let component: AdminDiscountList;
  let fixture: ComponentFixture<AdminDiscountList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDiscountList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminDiscountList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
