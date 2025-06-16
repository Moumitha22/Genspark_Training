import { Component } from '@angular/core';
import { LoginModel } from '../models/login';

@Component({
  selector: 'app-storage',
  templateUrl: './storage.html',
  styleUrl: './storage.css'
})
export class Storage {
  localUser: LoginModel | null = null;
  sessionUser: LoginModel | null = null;

  retrieveFromLocal() {
    const data = localStorage.getItem('user');
    this.localUser = data ? JSON.parse(data) : null;
  }

  retrieveFromSession() {
    const data = sessionStorage.getItem('user');
    this.sessionUser = data ? JSON.parse(data) : null;
  }

}
