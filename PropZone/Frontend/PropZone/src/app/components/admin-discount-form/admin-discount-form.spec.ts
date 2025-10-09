import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDiscountForm } from './admin-discount-form';

describe('AdminDiscountForm', () => {
  let component: AdminDiscountForm;
  let fixture: ComponentFixture<AdminDiscountForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDiscountForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminDiscountForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
