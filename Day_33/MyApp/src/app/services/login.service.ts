import { Injectable } from '@angular/core';
import { LoginModel } from '../models/login';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  login(user: LoginModel): boolean {
    if (user.username === 'admin' && user.password === 'password') {
      return true;
    }
    return false;
  }
}
