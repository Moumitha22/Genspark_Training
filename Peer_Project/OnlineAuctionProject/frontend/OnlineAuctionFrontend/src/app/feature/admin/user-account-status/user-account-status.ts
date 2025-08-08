import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { UserAccountService } from '../../../core/services/userAccount.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConfirmModal } from '../../../shared/components/confirm-modal/confirm-modal';
import { LucideAngularModule, TriangleAlertIcon } from 'lucide-angular';

@Component({
  selector: 'app-user-account-status',
  templateUrl: './user-account-status.html',
  imports: [CommonModule, FormsModule, ConfirmModal, LucideAngularModule],
})
export class UserAccountStatus implements OnInit {
  @Input() refreshTrigger: number = 0;
  @Output() revoked: EventEmitter<void> = new EventEmitter();
  readonly warning = TriangleAlertIcon;
  deletedUsers: any[] = [];
  searchEmail = '';
  filteredDeletedUsers: any[] = [];
  showRevokeModal = false;
  userToRevoke: any = null;

  constructor(private userAccountService: UserAccountService) {}

  ngOnInit() {
    this.userAccountService.getAllDeletedUsers().subscribe({
      next: (res) => {
        this.deletedUsers = res.data?.$values || [];
        this.filteredDeletedUsers = this.deletedUsers;
      },
      error: () => {
        this.deletedUsers = [];
        this.filteredDeletedUsers = [];
      },
    });
  }
  onSearchEmail() {
    const term = this.searchEmail.trim().toLowerCase();
    this.filteredDeletedUsers = this.deletedUsers.filter((user) =>
      user.email.toLowerCase().includes(term)
    );
  }
  onRevoke(user: any) {
    this.userToRevoke = user;
    this.showRevokeModal = true;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['refreshTrigger'] && !changes['refreshTrigger'].firstChange) {
      this.reload();
    }
  }

  reload() {
    this.userAccountService.getAllDeletedUsers().subscribe({
      next: (res) => {
        this.deletedUsers = res.data?.$values || [];
        this.filteredDeletedUsers = this.deletedUsers;
      },
      error: () => {
        this.deletedUsers = [];
        this.filteredDeletedUsers = [];
      },
    });
  }
  onConfirmRevoke() {
    if (!this.userToRevoke) return;
    this.userAccountService
      .revokeDletedUserAccount(this.userToRevoke.email)
      .subscribe({
        next: (res) => {
          this.deletedUsers = this.deletedUsers.filter(
            (u) => u.id !== this.userToRevoke.id
          );
          this.filteredDeletedUsers = this.filteredDeletedUsers.filter(
            (u) => u.id !== this.userToRevoke.id
          );
          this.revoked.emit();
          this.showRevokeModal = false;
          this.userToRevoke = null;
        },
        error: (err) => {
          console.error('Error revoking delete request:', err);
          this.showRevokeModal = false;
          this.userToRevoke = null;
        },
      });
  }

  onCancelRevoke() {
    this.showRevokeModal = false;
    this.userToRevoke = null;
  }
}
