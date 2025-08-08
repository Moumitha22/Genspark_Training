import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ForgetPassword } from './forget-password';
import { UserService } from '../../../core/services/user.service';
import { of, throwError } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

describe('ForgetPassword', () => {
  let component: ForgetPassword;
  let fixture: ComponentFixture<ForgetPassword>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(waitForAsync(() => {
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'getByEmail',
      'forgetPassword',
    ]);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [ForgetPassword, ReactiveFormsModule],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ForgetPassword);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should verify email and set emailVerified to true if user exists', () => {
    userServiceSpy.getByEmail.and.returnValue(
      of({ id: 1, email: 'test@example.com' })
    );
    component.forgetPassword.get('email')?.setValue('test@example.com');
    component.checkEmail();
    expect(userServiceSpy.getByEmail).toHaveBeenCalledWith('test@example.com');
    expect(component.emailVerified).toBeTrue();
  });

  it('should alert if email not found', () => {
    spyOn(window, 'alert');
    userServiceSpy.getByEmail.and.returnValue(
      throwError(() => new Error('Not found'))
    );
    component.forgetPassword.get('email')?.setValue('notfound@example.com');
    component.checkEmail();
    expect(window.alert).toHaveBeenCalledWith('Email not found!');
    expect(component.emailVerified).toBeFalse();
  });
});
