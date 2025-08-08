import { Component } from '@angular/core';
import { SidebarComponent } from '../sidebar/sidebar';
import { Header } from '../header/header';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-layout-component',
  imports: [SidebarComponent, Header, RouterOutlet],
  templateUrl: './layout-component.html',
})
export class LayoutComponent {
  role: string | null = '';
  constructor(private authService: AuthService) {}
  ngOnInit() {
    this.role = this.authService.getUserRole();
  }
}
