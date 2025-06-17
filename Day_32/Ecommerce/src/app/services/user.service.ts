import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { UserLoginModel } from "../models/userLogin.model";
import { Router } from "@angular/router";

@Injectable()
export class UserService{

    private http = inject(HttpClient);
    private router = inject(Router);
    private usernameSubject = new BehaviorSubject<string|null>(null);
    username$:Observable<string|null> = this.usernameSubject.asObservable();

    validateUserLogin(user:UserLoginModel)
    {
        if(user.username.length<3)
        {
            this.usernameSubject.next(null);
            this.usernameSubject.error("Too short for username");
        }
        else{
            // this.usernameSubject.next(user.username);
            this.callLoginApi(user).subscribe({
                next:(data:any) => {
                    this.usernameSubject.next(user.username);
                    localStorage.setItem("token",data.accessToken);
                    this.router.navigateByUrl("/products");
                },
            });
                
            
        }
    }

    callLoginApi(user:UserLoginModel){
        return this.http.post('https://dummyjson.com/auth/login',user);
    }


}