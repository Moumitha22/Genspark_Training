import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscountCodeDropDown } from './discount-code-drop-down';

describe('DiscountCodeDropDown', () => {
  let component: DiscountCodeDropDown;
  let fixture: ComponentFixture<DiscountCodeDropDown>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DiscountCodeDropDown]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DiscountCodeDropDown);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
