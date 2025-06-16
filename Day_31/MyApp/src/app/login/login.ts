// import { Component, Input } from '@angular/core';
// import { LoginModel } from '../models/login';
// import { FormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-login',
//   imports: [FormsModule],
//   templateUrl: './login.html',
//   styleUrl: './login.css'
// })
// export class Login {
//   user:LoginModel = new LoginModel();
//   handleLogin() {
//     // Store in Local Storage
//     localStorage.setItem('user', JSON.stringify(this.user));

//     // Store in Session Storage
//     sessionStorage.setItem('user', JSON.stringify(this.user));

//     alert('Data Stored in Local & Session Storage!');
//   }
// }



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