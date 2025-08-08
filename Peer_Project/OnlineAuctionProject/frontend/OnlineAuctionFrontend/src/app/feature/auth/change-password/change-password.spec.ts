import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ChangePassword } from './change-password';
import { UserService } from '../../../core/services/user.service';
import { AuctionService } from '../../../core/services/auction.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { AuthService } from '../../../core/services/auth.service';
import { of } from 'rxjs';

describe('ChangePassword', () => {
  let component: ChangePassword;
  let fixture: ComponentFixture<ChangePassword>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;

  beforeEach(waitForAsync(() => {
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'changeUserPassword',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['authme']);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
    ]);

    userServiceSpy.changeUserPassword.and.returnValue(of({}));
    authServiceSpy.authme.and.returnValue(
      of({ data: { id: '123', role: 'User' } })
    );

    TestBed.configureTestingModule({
      imports: [ChangePassword],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangePassword);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form on ngOnInit', () => {
    component.ngOnInit();
    expect(component.changePasssword).toBeDefined();
    expect(component.changePasssword.valid).toBeFalse();
  });
  it('should call changeUserPassword on form submit', () => {
    component.changePasssword.setValue({
      currentPassword: 'currentPass',
      newPassword: 'newPass',
      confirmPassword: 'newPass',
    });
    component.userId = '123';
    component.onSubmit();
    expect(userServiceSpy.changeUserPassword).toHaveBeenCalledWith({
      userId: '123',
      currentPassword: 'currentPass',
      newPassword: 'newPass',
    });
    expect(snackBarSpy.showSuccess).toHaveBeenCalledWith(
      'Password changed successfully'
    );
  });
});
