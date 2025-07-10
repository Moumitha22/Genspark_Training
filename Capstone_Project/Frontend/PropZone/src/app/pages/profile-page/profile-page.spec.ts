import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ProfilePageComponent } from './profile-page';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { ListerProfileService } from '../../core/services/lister-profile.service';
import { NotificationService } from '../../core/services/notification.service';
import { FormBuilder } from '@angular/forms';

describe('ProfilePageComponent', () => {
  let component: ProfilePageComponent;
  let fixture: ComponentFixture<ProfilePageComponent>;

  const mockAuthService = {
    currentUser: { id: 'user1', role: 'Lister' },
    currentUserRole: 'Lister'
  };

  const mockUserService = {
    user$: of({ name: 'Alice', email: 'alice@example.com', phoneNumber: '9876543210' }),
    updateUser: jasmine.createSpy().and.returnValue(of({})),
    loadCurrentUser: jasmine.createSpy()
  };

  const mockProfileService = {
    getByListerId: jasmine.createSpy(),
    updateProfile: jasmine.createSpy().and.returnValue(of({})),
    createProfile: jasmine.createSpy().and.returnValue(of({}))
  };

  const mockNotificationService = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy()
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfilePageComponent],
      providers: [
        FormBuilder,
        { provide: AuthService, useValue: mockAuthService },
        { provide: UserService, useValue: mockUserService },
        { provide: ListerProfileService, useValue: mockProfileService },
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfilePageComponent);
    component = fixture.componentInstance;

    // Reset spies
    mockUserService.updateUser.calls.reset();
    mockUserService.loadCurrentUser.calls.reset();
    mockProfileService.getByListerId.calls.reset();
    mockProfileService.updateProfile.calls.reset();
    mockProfileService.createProfile.calls.reset();
    mockNotificationService.success.calls.reset();
    mockNotificationService.error.calls.reset();
  });

  it('should load user info on init', fakeAsync(() => {
    mockProfileService.getByListerId.and.returnValue(of({ data: {} }));
    component.ngOnInit();
    tick();

    expect(component.userForm.getRawValue().name).toBe('Alice');
    expect(component.userForm.disabled).toBeTrue();
  }));

  it('should load lister profile if it exists', fakeAsync(() => {
    const mockProfile = {
      id: 'profile1',
      agencyName: 'My Agency',
      licenseNumber: 'LIC123',
      businessPhoneNumber: '9988776655'
    };
    mockProfileService.getByListerId.and.returnValue(of({ data: mockProfile }));

    component.ngOnInit();
    tick();

    expect(component.profileExists).toBeTrue();
    expect(component.profileForm.disabled).toBeTrue();
    expect(component.profileForm.value.agencyName).toBe('My Agency');
  }));

  
  it('should handle creating new profile when none exists', fakeAsync(() => {
    mockProfileService.getByListerId.and.returnValue(throwError(() => ({ status: 404 })));
    component.ngOnInit();
    tick();

    expect(component.profileExists).toBeFalse();
    expect(component.isProfileEdit).toBeTrue();

    component.profileForm.setValue({
      agencyName: 'New Agency',
      licenseNumber: 'NEW123',
      businessPhoneNumber: '9999999999'
    });

    component.onProfileSubmit();

    expect(mockProfileService.createProfile).toHaveBeenCalledWith(jasmine.any(Object));
    expect(mockNotificationService.success).toHaveBeenCalledWith('Profile created');
  }));


  it('should show error on profile update failure', () => {
    component.profileExists = true;
    component.profileId = 'profile1';
    mockProfileService.updateProfile.and.returnValue(throwError(() => new Error()));

    component.onProfileEdit();
    component.profileForm.patchValue({
      agencyName: 'Error Agency',
      licenseNumber: 'ERR001',
      businessPhoneNumber: '9876543210'
    });

    component.onProfileSubmit();

    expect(mockNotificationService.error).toHaveBeenCalledWith('Failed to update profile');
  });
});
