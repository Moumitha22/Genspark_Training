import {
  ComponentFixture,
  TestBed,
  waitForAsync,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { SidebarComponent } from './sidebar';
import { AuthService } from '../../core/services/auth.service';
import { SnackbarService } from '../../core/services/snackbar.service';
import { of, throwError } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

const mockActivatedRoute = {
  snapshot: {
    params: { id: '1' },
    paramMap: {
      get: (key: string) => (key === 'id' ? '1' : null),
    },
  },
};

describe('SidebarComponent', () => {
  let component: SidebarComponent;
  let fixture: ComponentFixture<SidebarComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;

  beforeEach(waitForAsync(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['authme', 'logout']);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', [
      'showInfo',
      'showError',
    ]);

    TestBed.configureTestingModule({
      imports: [SidebarComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    authServiceSpy.authme.and.returnValue(
      of({
        data: {
          username: 'testuser',
          email: 'test@mail.com',
          createdAt: '2025-01-01',
        },
      })
    );
    authServiceSpy.logout.and.returnValue(of({}));
    fixture = TestBed.createComponent(SidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch user info on init', () => {
    expect(authServiceSpy.authme).toHaveBeenCalled();
    expect(component.username).toBe('testuser');
    expect(component.email).toBe('test@mail.com');
    expect(component.firstLetter).toBe('T');
    expect(component.createdAt).toBe('2025-01-01');
  });

  it('should show and close profile modal', () => {
    component.showModel('profile');
    expect(component.showProfileModal).toBeTrue();
    component.closeModel('profile');
    expect(component.showProfileModal).toBeFalse();
  });

  it('should show and close logout modal', () => {
    component.showModel('logout');
    expect(component.showLogoutModal).toBeTrue();
    component.closeModel('logout');
    expect(component.showLogoutModal).toBeFalse();
  });

  it('should logout and show info', fakeAsync(() => {
    spyOn(component, 'authme');
    component.onLogout();
    tick(2000);
    expect(authServiceSpy.logout).toHaveBeenCalled();
    expect(snackBarSpy.showInfo).toHaveBeenCalledWith(
      'Logged out!, thank you for visiting'
    );
    expect(component.isLoading).toBeFalse();
  }));

  it('should handle logout error', fakeAsync(() => {
    authServiceSpy.logout.and.returnValue(throwError(() => new Error('fail')));
    component.onLogout();
    tick(2000);
    expect(snackBarSpy.showError).toHaveBeenCalledWith(
      "Can't able to logout, contact admin"
    );
    expect(component.isLoading).toBeFalse();
  }));
});
