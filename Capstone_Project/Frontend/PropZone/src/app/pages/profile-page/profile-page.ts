import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { ListerProfileService } from '../../core/services/lister-profile.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile-page.html',
  styleUrls: ['./profile-page.css']
})
export class ProfilePageComponent implements OnInit {
  userForm: FormGroup;
  profileForm: FormGroup;
  isUserEdit = false;
  isProfileEdit = false;
  profileExists = false;

  profileId = '';
  role: string | null = null;
  profilePictureUrl: string | null = null;
  defaultAvatarUrl = 'https://cdn-icons-png.flaticon.com/512/149/149071.png';

  private authService = inject(AuthService);
  private userService = inject(UserService);
  private profileService = inject(ListerProfileService);
  private fb = inject(FormBuilder);
  private notificationService = inject(NotificationService);

  constructor() {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      email: [{ value: '', disabled: true }],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]]
    });

    this.profileForm = this.fb.group({
      agencyName: [''],
      licenseNumber: [''],
      businessPhoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]]
    });
  }

  ngOnInit(): void {
    this.role = this.authService.currentUserRole;

    this.userService.user$.subscribe(user => {
      if (user) {
        this.userForm.patchValue(user);
        this.userForm.disable();
      }
    });

    if (this.role === 'Lister') {
      const listerId = this.authService.currentUser?.id;
      if (!listerId) return;

      this.profileService.getByListerId(listerId).subscribe({
        next: res => {
          this.profileExists = true;
          this.profileId = res.data.id;
          this.profileForm.patchValue(res.data);
          this.profileForm.disable();
        },
        error: err => {
          if (err.status === 404) {
            this.profileExists = false;
            this.isProfileEdit = true;
            this.profileForm.enable();
          }
        }
      });
    }
  }

  // User section
  onUserEdit(): void {
    this.isUserEdit = true;
    this.userForm.enable();
    this.userForm.get('email')?.disable(); // keep email read-only
  }

  onUserCancel(): void {
    this.userForm.disable();
    this.isUserEdit = false;
  }

  onUserSubmit(): void {
    if (this.userForm.invalid) return;

    const dto = this.userForm.getRawValue();
    const userId = this.authService.currentUser?.id;
    if (!userId) return;

    this.userService.updateUser(userId, dto).subscribe({
      next: () => {
        this.notificationService.success('User info updated');
        this.isUserEdit = false;
        this.userForm.disable();
        this.userForm.get('email')?.disable();
        this.userService.loadCurrentUser();
      },
      error: () => this.notificationService.error('Failed to update user info')
    });
  }

  // Profile section
  onProfileEdit(): void {
    this.isProfileEdit = true;
    this.profileForm.enable();
  }

  onProfileCancel(): void {
    this.profileForm.disable();
    this.isProfileEdit = false;
  }

  onProfileSubmit(): void {
    if (this.profileForm.invalid) return;
    const dto = this.profileForm.value;

    const request = this.profileExists
      ? this.profileService.updateProfile(this.profileId, dto)
      : this.profileService.createProfile(dto);

    request.subscribe({
      next: () => {
        this.notificationService.success(this.profileExists ? 'Profile updated' : 'Profile created');
        this.isProfileEdit = false;
        this.profileForm.disable();
        this.profileExists = true;
      },
      error: () => this.notificationService.error('Failed to update profile')
    });
  }
}
