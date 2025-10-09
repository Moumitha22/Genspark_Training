import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../core/services/user.service';
import { User } from '../../models/user.model';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../core/services/notification.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './users-list.html'
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  private notificationService = inject(NotificationService);

  users: User[] = [];
  filteredUsers: User[] = [];

  loading = true;
  errorMessage = '';
  searchTerm: string = '';
  selectedRole: string = 'All Roles';
  selectedStatus: string = 'All Users';

  availableRoles: string[] = ['All Roles', 'Admin', 'Lister', 'Buyer'];

  ngOnInit(): void {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.users = res.data;
        this.filteredUsers = res.data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load users', err);
        this.loading = false;
        this.errorMessage = err.error?.message || 'Failed to load users.';
      }
    });
  }

  onSearchOrFilter(): void {
    const term = this.searchTerm.trim().toLowerCase();
    this.filteredUsers = this.users.filter(user => {
      const matchesName = user.name.toLowerCase().includes(term);
      const matchesRole = this.selectedRole === 'All Roles' || user.role === this.selectedRole;
      const matchesStatus =
        this.selectedStatus === 'All Users' ||
        (this.selectedStatus === 'Active' && !user.isDeleted) ||
        (this.selectedStatus === 'Disabled' && user.isDeleted);
      return matchesName && matchesRole && matchesStatus;
    });
  }


  toggleDisableUser(user: User) {
    const action = user.isDeleted ? 'enable' : 'disable';
    const confirmed = confirm(`Are you sure you want to ${action} this user?`);
    if (!confirmed) return;

    this.userService.updateUserStatus(user.id, !user.isDeleted).subscribe({
      next: () => {
        user.isDeleted = !user.isDeleted;
        this.notificationService.success(`User ${action}d successfully`);
      },
      error: () => {
        this.notificationService.error(`Failed to ${action} user`);
      }
    });
  }
}
