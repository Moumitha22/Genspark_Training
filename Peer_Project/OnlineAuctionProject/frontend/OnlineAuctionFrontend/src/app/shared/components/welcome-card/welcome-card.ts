import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { UserService } from '../../../core/services/user.service';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, SmileIcon } from 'lucide-angular';

@Component({
  selector: 'app-welcome-card',
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './welcome-card.html',
})
export class WelcomeCard implements OnInit {
  readonly welcome = SmileIcon;
  constructor(
    private authService: AuthService,
    private userService: UserService
  ) {}
  username!: string;
  email: string = '';
  currentDateTime: Date = new Date();

  ngOnInit(): void {
    const email = this.authService.getEmailFromToken() ?? '';
    this.email = email;
    this.userService.getByEmail(email).subscribe((res) => {
      const user = res.data;
      if (user) {
        this.username = user.username;
      }
    });
  }
}
