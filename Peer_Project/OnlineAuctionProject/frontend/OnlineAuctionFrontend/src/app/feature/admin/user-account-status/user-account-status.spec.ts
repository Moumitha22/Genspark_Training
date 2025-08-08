import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAccountStatus } from './user-account-status';
import { UserAccountService } from '../../../core/services/userAccount.service';
import { of } from 'rxjs';

describe('UserAccountStatus', () => {
  let component: UserAccountStatus;
  let fixture: ComponentFixture<UserAccountStatus>;

  let userAccountServiceSpy: jasmine.SpyObj<UserAccountService>;
  beforeEach(() => {
    userAccountServiceSpy = jasmine.createSpyObj('UserAccountService', [
      'getAllDeletedUsers',
    ]);
    userAccountServiceSpy.getAllDeletedUsers.and.returnValue(of([]));

    TestBed.configureTestingModule({
      imports: [UserAccountStatus],
      providers: [
        { provide: UserAccountService, useValue: userAccountServiceSpy },
      ],
    }).compileComponents();
  });

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserAccountStatus],
    }).compileComponents();

    fixture = TestBed.createComponent(UserAccountStatus);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
