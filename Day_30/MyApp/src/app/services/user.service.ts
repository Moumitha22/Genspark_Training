import { BehaviorSubject, Observable } from "rxjs";
import { UserLoginModel } from "../models/userLogin";

export class UserService
{
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
            this.usernameSubject.next(user.username);
            
            // Store in Local Storage
            localStorage.setItem('user', JSON.stringify(user));

            // Store in Session Storage
            sessionStorage.setItem('user', JSON.stringify(user));

            alert('Data Stored in Local & Session Storage!');
        }
    }

    logout(){
        this.usernameSubject.next(null);
    }

}