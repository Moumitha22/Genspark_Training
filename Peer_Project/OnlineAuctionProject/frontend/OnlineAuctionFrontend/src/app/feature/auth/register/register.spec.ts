import {
  ComponentFixture,
  TestBed,
  waitForAsync,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { Component } from '@angular/core';
import { Register } from './register';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { of, throwError } from 'rxjs';
import { AddUser } from '../../../core/models/AddUser';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { Login } from '../login/login';
import { UserAccountService } from '../../../core/services/userAccount.service';

const mockActivatedRoute = {
  snapshot: {
    params: { id: '1' },
    paramMap: {
      get: (key: string) => (key === 'id' ? '1' : null),
    },
  },
};

@Component({
  standalone: true,
  imports: [Register],
  template: `<app-register></app-register>`,
})
class HostComponent {}

describe('Register (HostComponent model)', () => {
  let hostFixture: ComponentFixture<HostComponent>;
  let hostComponent: HostComponent;
  let component: Register;
  let fixture: ComponentFixture<Register>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let snackbarSpy: jasmine.SpyObj<SnackbarService>;
  let userAccountServiceSpy: jasmine.SpyObj<UserAccountService>;

  beforeEach(waitForAsync(() => {
    userServiceSpy = jasmine.createSpyObj('UserService', ['registerNewuser']);
    snackbarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
    ]);
    userAccountServiceSpy = jasmine.createSpyObj('UserAccountService', [
      'getAllDeletedUsers',
      'getDeleteReasonByEmail'
    ]);

    userServiceSpy.registerNewuser.and.returnValue(of({}));
    userAccountServiceSpy.getAllDeletedUsers.and.returnValue(of([]));
    userAccountServiceSpy.getDeleteReasonByEmail.and.returnValue(of(''));

    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, HostComponent],
      providers: [
        FormBuilder,
        { provide: UserService, useValue: userServiceSpy },
        { provide: SnackbarService, useValue: snackbarSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: UserAccountService, useValue: userAccountServiceSpy },
        provideRouter([
          { path: 'login', component: Login },
        ]),

      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    hostFixture = TestBed.createComponent(HostComponent);
    hostComponent = hostFixture.componentInstance;
    hostFixture.detectChanges();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create HostComponent', () => {
    expect(hostComponent).toBeTruthy();
  });

  it('should create Register component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize userForm on ngOnInit', () => {
    component.ngOnInit();
    expect(component.userForm).toBeDefined();
    expect(component.userForm.valid).toBeFalse();
  });

  it('should call registerNewuser and show success on valid form submit', fakeAsync(() => {
    component.userForm.setValue({
      username: 'testuser',
      email: 'test@gmail.com',
      password: 'password',
      confirmPassword: 'password',
      role: 'seller',
    });
    expect(component.userForm.valid).toBeTrue();
    userServiceSpy.registerNewuser.and.returnValue(of({}));
    component.onFormSubmit();
    tick(2000);
    const payload: AddUser = {
      username: 'testuser',
      email: 'test@gmail.com',
      password: 'password',
      role: 'seller',
    };
    expect(userServiceSpy.registerNewuser).toHaveBeenCalledWith(payload);
    expect(snackbarSpy.showSuccess).toHaveBeenCalledWith('New User Created');
    expect(component.isLoading).toBeFalse();
  }));

  it('should show error on registerNewuser failure', fakeAsync(() => {
    component.userForm.setValue({
      username: 'testuser',
      email: 'test@example.com',
      password: 'password',
      confirmPassword: 'password',
      role: 'seller',
    });
    userServiceSpy.registerNewuser.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.onFormSubmit();
    tick();
    expect(snackbarSpy.showError).toHaveBeenCalledWith(
      'Failed to create - fail'
    );
    expect(component.isLoading).toBeFalse();
  }));

  it('should mark all as touched if form is invalid', () => {
    spyOn(component.userForm, 'markAllAsTouched');
    component.userForm.patchValue({
      username: '',
      email: '',
      password: '',
      confirmPassword: '',
      role: '',
    });
    component.onFormSubmit();
    expect(component.userForm.markAllAsTouched).toHaveBeenCalled();
  });
});
