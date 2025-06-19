import { AfterViewInit, Component, ElementRef, ViewChild, inject, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { BehaviorSubject, combineLatest, debounceTime, distinctUntilChanged, fromEvent, map } from 'rxjs';
import { UserModel } from '../models/user.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserListComponent implements OnInit, AfterViewInit {
  public userService = inject(UserService);

  @ViewChild('searchInput') searchInput!: ElementRef;

  roleSubject = new BehaviorSubject<string>('All Roles');
  searchSubject = new BehaviorSubject<string>('');
  filteredUsers: UserModel[] = [];

  roles: string[] = [
    'All Roles',
    'Web Developer',
    'Sales Manager',
    'Business Analyst',
    'Human Resources Manager',
    'Database Administrator'
  ];

  ngOnInit(): void {
    combineLatest([
      this.userService.users$,
      this.searchSubject,
      this.roleSubject
    ])
      .pipe(
        map(([users, searchTerm, selectedRole]) => {
          return users.filter(user => {
            const matchesSearch =
              user.username.toLowerCase().includes(searchTerm.toLowerCase()) ||
              user.role.toLowerCase().includes(searchTerm.toLowerCase());

            const matchesRole = selectedRole === 'All Roles' || user.role === selectedRole;

            return matchesSearch && matchesRole;
          });
        })
      )
      .subscribe(filtered => {
        this.filteredUsers = filtered;
      });
  }

  ngAfterViewInit(): void {
    fromEvent(this.searchInput.nativeElement, 'input')
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        map((event: any) => event.target.value)
      )
      .subscribe(searchTerm => this.searchSubject.next(searchTerm));
  }

  onRoleFilterChange(role: string): void {
    this.roleSubject.next(role);
  }

  onSelectChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.onRoleFilterChange(target.value);
  }
}
