import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyProduct } from './myproduct';

describe('MyProduct', () => {
  let component: MyProduct;
  let fixture: ComponentFixture<MyProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyProduct]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyProduct);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
