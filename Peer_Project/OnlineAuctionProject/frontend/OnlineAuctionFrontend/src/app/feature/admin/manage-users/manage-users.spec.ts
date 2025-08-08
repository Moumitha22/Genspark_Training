import {
  ComponentFixture,
  TestBed,
  waitForAsync,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { Component } from '@angular/core';
import { ManageUsers } from './manage-users';
import { UserService } from '../../../core/services/user.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
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
  imports: [ManageUsers],
  template: `<app-manage-users></app-manage-users>`,
})
class HostComponent {}

describe('ManageUsers', () => {
  let hostFixture: ComponentFixture<HostComponent>;
  let hostComponent: HostComponent;
  let component: ManageUsers;
  let fixture: ComponentFixture<ManageUsers>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let snackbarSpy: jasmine.SpyObj<SnackbarService>;
  let userAccountServiceSpy: jasmine.SpyObj<UserAccountService>;

  beforeEach(waitForAsync(() => {
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'getAllUsers',
      'updateUser',
      'deleteUser',
    ]);
    snackbarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
      'showInfo',
    ]);

    userServiceSpy.getAllUsers.and.returnValue(of({ $values: [] }));
    userServiceSpy.updateUser.and.returnValue(of({}));
    userServiceSpy.deleteUser.and.returnValue(
      of({ message: 'Deleted successfully' })
    );
    userAccountServiceSpy = jasmine.createSpyObj('UserAccountService', [
      'getAllDeletedUsers',
    ]);

    userAccountServiceSpy.getAllDeletedUsers.and.returnValue(of([]));

    TestBed.configureTestingModule({
      imports: [HostComponent],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: SnackbarService, useValue: snackbarSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: UserAccountService, useValue: userAccountServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    hostFixture = TestBed.createComponent(HostComponent);
    hostComponent = hostFixture.componentInstance;
    hostFixture.detectChanges();

    fixture = TestBed.createComponent(ManageUsers);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create HostComponent', () => {
    expect(hostComponent).toBeTruthy();
  });

  it('should create ManageUsers component', () => {
    expect(component).toBeTruthy();
  });

  it('should load users on init', () => {
    const users = {
      $values: [{ id: 1, username: 'test', email: 'a@b.com', role: 'Bidder' }],
    };
    userServiceSpy.getAllUsers.and.returnValue(of(users));
    component.loadUsers();
    expect(component.usersData.length).toBe(1);
  });

  it('should handle error on loadUsers', () => {
    userServiceSpy.getAllUsers.and.returnValue(
      throwError(() => new Error('fail'))
    );
    spyOn(console, 'log');
    component.loadUsers();
    expect(console.log).toHaveBeenCalledWith(jasmine.any(Error));
  });

  it('should call updateUser and show success', fakeAsync(() => {
    component.editValues = {
      userName: 'test',
      email: 'a@b.com',
      role: 'Bidder',
    };
    component.userId = '1';
    userServiceSpy.updateUser.and.returnValue(of({}));
    component.onSaveChanges();
    tick(2100);
    expect(snackbarSpy.showSuccess).toHaveBeenCalledWith(
      'User updated successfully'
    );
    expect(component.isLoading).toBeFalse();
  }));

  it('should handle error on updateUser', () => {
    userServiceSpy.updateUser.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.onSaveChanges();
    expect(snackbarSpy.showError).toHaveBeenCalledWith(
      'Failed to update the user'
    );
    expect(component.isLoading).toBeFalse();
  });

  it('should call deleteUser and show success', fakeAsync(() => {
    component.userId = '1';
    component.reason = 'test reason';
    userServiceSpy.deleteUser.and.returnValue(
      of({ message: 'Deleted successfully' })
    );
    component.onDeleteUser(component.reason);
    tick(1100);
    expect(snackbarSpy.showSuccess).toHaveBeenCalledWith(
      'Deleted successfully'
    );
    expect(component.isLoading).toBeFalse();
  }));

  it('should handle error on deleteUser', () => {
    userServiceSpy.deleteUser.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.onDeleteUser(component.reason);
    expect(snackbarSpy.showError).toHaveBeenCalledWith('fail');
    expect(component.isLoading).toBeFalse();
  });
});
