import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-menu',
  imports: [CommonModule, RouterLink],
  templateUrl: './menu.html',
  styleUrl: './menu.css'
})
export class Menu {
  username$:any;
  myusername:string|null = "";

  constructor(private userService:UserService)
  {
    // this.username$ = this.userService.username$;
    this.userService.username$.subscribe(
      {
       next:(value) =>{
        console.log("Username is", value);
          this.myusername = value ;
        },
        error:(err)=>{
          alert(err);
        }
      }
    )
  }
}
