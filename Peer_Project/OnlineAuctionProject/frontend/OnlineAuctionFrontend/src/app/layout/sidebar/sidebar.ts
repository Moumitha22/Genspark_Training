import { Component, Input, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import {
  LucideAngularModule,
  HomeIcon,
  CircleFadingPlus,
  Hammer,
  Crown,
  LogOut,
  LogOutIcon,
  UserIcon,
  Pickaxe,
  TicketSlash,
  SearchIcon,
  EllipsisVertical,
  WalletIcon,
  FileCheck,
  FileCheckIcon,
} from 'lucide-angular';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { CommonModule } from '@angular/common';
import { ModelView } from '../../shared/components/model-view/model-view';
import { SnackbarService } from '../../core/services/snackbar.service';
import { Spinner } from '../../shared/components/spinner/spinner';
import { ConfirmModal } from "../../shared/components/confirm-modal/confirm-modal";

@Component({
  selector: 'app-sidebar',
  imports: [
    LucideAngularModule,
    RouterLink,
    CommonModule,
    ModelView,
    Spinner,
    RouterLinkActive,
    ConfirmModal
],
  templateUrl: './sidebar.html',
})
export class SidebarComponent implements OnInit {
  username: string = '';
  firstLetter: string = '';
  email: string = '';
  createdAt: string = '';
  showLogoutModal = false;
  showProfileModal = false;
  isLoading = false;

  readonly homeIcon = HomeIcon;
  readonly fadingPlus = CircleFadingPlus;
  readonly hammer = Hammer;
  readonly crown = Crown;
  readonly logout = LogOutIcon;
  readonly userIcon = UserIcon;
  readonly auctionIcon = Pickaxe;
  readonly bidsIcon = TicketSlash;
  readonly searchicon = SearchIcon;
  readonly moreVertical = EllipsisVertical;
  readonly virtualWallet = WalletIcon;
  readonly invoice = FileCheckIcon;

  constructor(
    private authService: AuthService,
    private snackBar: SnackbarService
  ) {}

  @Input() role: string | null = '';
  ngOnInit() {
    this.authme();
  }

  authme() {
    this.authService.authme().subscribe({
      next: (res) => {
        const user = res.data;
        if (user) {
          this.username = user.username;
          this.email = user.email;
          this.firstLetter = this.username.charAt(0).toUpperCase();
          this.createdAt = user.createdAt;
        }
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  showModel(type: string) {
    if (type == 'profile') {
      this.showProfileModal = true;
    } else if (type == 'logout') {
      this.showLogoutModal = true;
    }
  }

  closeModel(type: string) {
    if (type == 'profile') {
      this.showProfileModal = false;
    } else if (type == 'logout') {
      this.showLogoutModal = false;
    }
  }

  onLogout(): void {
    this.isLoading = true;
    setTimeout(() => {
      this.isLoading = false;
      this.authService.logout().subscribe({
        next: () => {
          this.snackBar.showInfo('Logged out!, thank you for visiting');
        },
        error: (err) => {
          this.isLoading = false;
          this.snackBar.showError("Can't able to logout, contact admin");
          console.error(
            'Error during logout subscription (this should ideally be handled by service catchError):',
            err
          );
        },
        complete: () => {
          console.log('Logout observable complete.');
        },
      });
    }, 2000);
  }
}
