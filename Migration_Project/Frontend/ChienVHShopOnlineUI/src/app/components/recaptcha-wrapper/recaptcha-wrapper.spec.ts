import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecaptchaWrapper } from './recaptcha-wrapper';

describe('RecaptchaWrapper', () => {
  let component: RecaptchaWrapper;
  let fixture: ComponentFixture<RecaptchaWrapper>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecaptchaWrapper]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecaptchaWrapper);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
