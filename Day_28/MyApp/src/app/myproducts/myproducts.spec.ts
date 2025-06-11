import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Myproducts } from './myproducts';

describe('Myproducts', () => {
  let component: Myproducts;
  let fixture: ComponentFixture<Myproducts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Myproducts]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Myproducts);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
