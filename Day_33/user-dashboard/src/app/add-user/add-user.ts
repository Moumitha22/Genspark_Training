import { Component,inject } from '@angular/core';
import { UserAddModel } from '../models/useradd.model';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router} from '@angular/router';

@Component({
  selector: 'app-add-user',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './add-user.html',
  styleUrl: './add-user.css'
})
export class AddUser {
  addUserForm: FormGroup;
  newUser: UserAddModel = new UserAddModel();

  private userService = inject(UserService);
  private router = inject(Router);

  constructor() {
    this.addUserForm = new FormGroup({
      username: new FormControl(null, Validators.required),
      email: new FormControl(null, [Validators.required, Validators.email]),
      firstName: new FormControl(null),
      lastName: new FormControl(null),
      gender: new FormControl(null, Validators.required),
      password: new FormControl(null, [Validators.required, Validators.minLength(5)]),
      company: new FormGroup({
        title: new FormControl(null, Validators.required)
      }),
      address: new FormGroup({
        state: new FormControl(null, Validators.required)
      }),
      image: new FormControl(null) 
    });
  }

  public get username() { return this.addUserForm.get('username'); }
  public get email() { return this.addUserForm.get('email'); }
  public get password() { return this.addUserForm.get('password'); }
  public get gender() { return this.addUserForm.get('gender'); }
  public get role() { return this.addUserForm.get('company.title'); }
  public get state() { return this.addUserForm.get('address.state'); }
  public get companyGroup(): FormGroup {
    return this.addUserForm.get('company') as FormGroup;
  }
  public get addressGroup(): FormGroup {
    return this.addUserForm.get('address') as FormGroup;
  }



  handleAddUser() {
    if (this.addUserForm.invalid) 
      return;

    this.newUser = this.addUserForm.value;

    this.userService.addUser(this.newUser);
    
    this.newUser = new UserAddModel(); 
    this.addUserForm.reset();
  }

  handleNavigate(){
    this.router.navigateByUrl('/dashboard');
  }

}
