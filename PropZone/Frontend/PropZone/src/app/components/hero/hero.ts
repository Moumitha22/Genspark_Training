import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-hero',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './hero.html',
  styleUrl: './hero.css'
})
export class HeroComponent {
  searchModel = {
    purpose: 'Sale',
    location: '',
    keyword: ''
  };

  private router = inject(Router);

  setPurpose(purpose: 'Sale' | 'Rent') {
    this.searchModel.purpose = purpose;
  }

  search() {
    const queryParams: any = {
      purpose: this.searchModel.purpose || 'Rent',
      city: this.searchModel.location || '', 
      keyword: this.searchModel.keyword || '',
    };

    this.router.navigate(['/properties'], { queryParams });
  }
}