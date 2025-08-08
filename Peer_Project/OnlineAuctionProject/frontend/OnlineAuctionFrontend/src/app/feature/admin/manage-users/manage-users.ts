import { Component, OnInit } from '@angular/core';
import { User } from '../../../core/models/User';
import { UserService } from '../../../core/services/user.service';
import { CommonModule } from '@angular/common';
import {
  DeleteIcon,
  EditIcon,
  LucideAngularModule,
  Trash,
  TriangleAlert,
} from 'lucide-angular';
import { ModelView } from '../../../shared/components/model-view/model-view';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { FormsModule } from '@angular/forms';
import { paginate } from '../../../shared/utils/pagination-utils';
import { RouterLink } from '@angular/router';
import { Spinner } from '../../../shared/components/spinner/spinner';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ConfirmModal } from '../../../shared/components/confirm-modal/confirm-modal';
import { UserAccountStatus } from "../user-account-status/user-account-status";

@Component({
  selector: 'app-manage-users',
  imports: [
    CommonModule,
    LucideAngularModule,
    ModelView,
    FormsModule,
    RouterLink,
    Spinner,
    Pagination,
    ConfirmModal,
    UserAccountStatus
],
  templateUrl: './manage-users.html',
})
export class ManageUsers implements OnInit {
  readonly edit = EditIcon;
  readonly delete = Trash;
  readonly warn = TriangleAlert;
  usersData: User[] = [];
  paginatedData: User[] = [];
  showWarningModel = false;
  showEditModal = false;
  showDeletedUsersModel = false;
  userId = '';
  editValues = {
    userName: '',
    email: '',
    role: '',
  };
  reason = '';
  isLoading = false;
  searchTerm: string = '';
  searchSubject: Subject<{ searchTerm: string; sortBy: string }> =
    new Subject();
  selectedSortByTerm: string = '';
  
  refreshDeletedUsers = 0;

  constructor(
    private userService: UserService,
    private snackBar: SnackbarService
  ) {}

  page = 1;
  pageSize = 10;
  totalPages = 0;

  ngOnInit(): void {
    this.loadUsers();
    this.searchSubject
      .pipe(
        debounceTime(400),
        distinctUntilChanged(
          (prev, curr) =>
            prev.searchTerm === curr.searchTerm && prev.sortBy === curr.sortBy
        )
      )
      .subscribe(({ searchTerm, sortBy }) => {
        if (searchTerm.trim() !== '' || sortBy) {
          this.userService
            .getSearchUsers({
              SearchTerm: searchTerm,
              SortBy: sortBy,
            })
            .subscribe({
              next: (res) => {
                this.usersData = res?.data?.$values || [];
                this.updatePaginationData();
              },
              error: (err) => {
                console.log(err);
              },
            });
        } else {
          this.loadUsers();
        }
      });
  }

  updatePaginationData() {
    const result = paginate(this.usersData, this.page, this.pageSize);
    this.paginatedData = result.data;
    this.totalPages = result.totalPages;
  }

  onSearch() {
    this.searchSubject.next({
      searchTerm: this.searchTerm,
      sortBy: this.selectedSortByTerm,
    });
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.usersData = res?.$values || [];

        this.updatePaginationData();
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  goToPage(page: number) {
    this.page = page;
    this.updatePaginationData();
  }

  onOpenModel(user: any, type: string) {
    if (type === 'edit') {
      this.showEditModal = true;
      this.editValues.userName = user.username;
      this.editValues.email = user.email;
      this.editValues.role = user.role;
      this.userId = user.id;
    } else if (type === 'warn') {
      this.userId = user.id;
      this.showWarningModel = true;
    }
  }

  onCloseModel(type: string) {
    if (type === 'edit') {
      this.showEditModal = false;
    } else if (type === 'warn') {
      this.showWarningModel = false;
    }
  }

  onSaveChanges() {
    this.isLoading = true;
    this.userService.updateUser(this.editValues, this.userId).subscribe({
      next: () => {
        setTimeout(() => {
          this.snackBar.showSuccess('User updated successfully');
          this.onCloseModel('edit');
          this.isLoading = false;
          this.loadUsers();
        }, 2000);
      },
      error: (err) => {
        this.snackBar.showError('Failed to update the user');
        this.isLoading = false;
      },
    });
  }
  onDeleteUser(reason: string) {
    const payload: any = {
      userId: this.userId,
      reason: reason,
    };
    this.isLoading = true;
    this.userService.deleteUser(payload).subscribe({
      next: (res) => {
        setTimeout(() => {
          this.snackBar.showSuccess(res.message);
          this.onCloseModel('warn');
          this.isLoading = false;
          this.loadUsers();
          this.refreshDeletedUsers++;
        }, 1000);
      },
      error: (err) => {
        this.snackBar.showError(`${err.message}`);
        this.isLoading = false;
      },
    });
  }
  onUserRevoked() {
    this.loadUsers();
  }
}
