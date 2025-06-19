import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserModel } from '../models/user.model';
import { UserAddModel } from '../models/useradd.model';

@Injectable()
export class UserService {
  private usersSubject = new BehaviorSubject<UserModel[]>([]);
  users$: Observable<UserModel[]> = this.usersSubject.asObservable();
  
  private hasFetched = false;

  

  constructor(private http: HttpClient) {
    this.fetchUsers();
  }

  // Fetch all users from API
  fetchUsers(): void {
    if (this.hasFetched) 
      return;
    
    this.hasFetched = true;

    const roleList = [
      "Web Developer",
      "Sales Manager",
      "Business Analyst",
      "Human Resources Manager",
      "Database Administrator"
    ];

    this.http.get<any>('https://dummyjson.com/users').subscribe({
      next: (res) => {
        const usersData = res.users.map((user: any, index: number) => ({
        ...user,
        role: roleList[index % roleList.length],  
        state: user.address?.state || 'Unknown'
      })) as UserModel[];

      this.usersSubject.next(usersData);
      },
      error: (err) => console.error("Error fetching users:", err)
    });
  }

  // fetchUsers(): void {
  //   if (this.hasFetched) 
  //     return;
    
  //   this.hasFetched = true;

  //   this.http.get<any>('https://dummyjson.com/users').subscribe({
  //     next: (res) => {
  //       const usersData = res.users.map((user: any) => ({
  //         ...user,
  //         role: user.company?.title || 'User',          
  //         state: user.address?.state || 'Unknown'       
  //       })) as UserModel[];

  //       this.usersSubject.next(usersData);
  //     },
  //     error: (err) => console.error("Error fetching users:", err)
  //   });
  // }

  // Add a new user
  addUser(user: UserAddModel): void {
    this.http.post<any>('https://dummyjson.com/users/add', user).subscribe({
        next: (res) => {
        const updatedUser: UserModel = {
            ...res,
            image: res.image ?? '',
            role: res.company?.title ?? 'User',
            state: res.address?.state ?? 'Active'
        };

        const currentUsers = this.usersSubject.value;
        this.usersSubject.next([...currentUsers, updatedUser]);
        },
        error: (err) => console.error('Error adding user:', err)
    });
  }
}
