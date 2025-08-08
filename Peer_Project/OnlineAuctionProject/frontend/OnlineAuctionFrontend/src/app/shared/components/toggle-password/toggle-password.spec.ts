import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TogglePassword } from './toggle-password';

describe('TogglePassword', () => {
  let component: TogglePassword;
  let fixture: ComponentFixture<TogglePassword>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TogglePassword]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TogglePassword);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
