
import { Component } from '@angular/core';
import { UserLoginModel } from '../models/userLogin';
import { UserService } from '../services/user.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  user:UserLoginModel = new UserLoginModel();
  constructor(private userService:UserService){}

  handleLogin(){
    this.userService.validateUserLogin(this.user);
  }
}