import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WelcomeCard } from './welcome-card';
import { AuthService } from '../../../core/services/auth.service';
import { UserService } from '../../../core/services/user.service';
import { of } from 'rxjs';

describe('WelcomeCard', () => {
  let component: WelcomeCard;
  let fixture: ComponentFixture<WelcomeCard>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  beforeEach(waitForAsync(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'getUserRole',
      'getEmailFromToken',
    ]);
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'getCurrentUser',
      'getByEmail',
    ]);
    authServiceSpy.getUserRole.and.returnValue('Bidder');
    authServiceSpy.getEmailFromToken.and.returnValue('test@mail.com');

    userServiceSpy.getByEmail.and.returnValue(
      of({ data: { email: 'test@mail.com' } })
    );

    TestBed.configureTestingModule({
      imports: [WelcomeCard],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(async () => {
    authServiceSpy.getUserRole.and.returnValue('Bidder');
    authServiceSpy.getEmailFromToken.and.returnValue('test@gmail.com');
    await TestBed.configureTestingModule({
      imports: [WelcomeCard],
    }).compileComponents();

    fixture = TestBed.createComponent(WelcomeCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
