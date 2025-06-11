import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Myproduct } from './myproduct';

describe('Myproduct', () => {
  let component: Myproduct;
  let fixture: ComponentFixture<Myproduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Myproduct]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Myproduct);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
