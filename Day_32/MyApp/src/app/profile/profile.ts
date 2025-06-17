import { Component, inject } from '@angular/core';
import { UserModel } from '../models/UserModel';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {
  userService = inject(UserService);
     profileData:UserModel = new UserModel();

     constructor(){
        this.userService.callGetProfile().subscribe({
          next:(data:any)=>{
            this.profileData = UserModel.fromForm(data);
          }
        })
     }

}
