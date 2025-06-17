import { Component, inject, Input } from '@angular/core';
import { ProductModel } from '../models/product.model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product',
  imports: [CommonModule],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product {
  @Input() product:ProductModel|null = new ProductModel();
  @Input() searchTerm: string = ''; 

  private router = inject(Router);

  handleView(id:number){
   this.router.navigate(['/products', id]);
  }

  highlightParts(title: string | undefined): { text: string; isMatch: boolean }[] {
    if (!title) return [];

    const term = this.searchTerm.trim();
    if (!term) return [{ text: title, isMatch: false }];

    const regex = new RegExp(`(${term})`, 'gi');
    const parts: { text: string; isMatch: boolean }[] = [];

    let lastIndex = 0;
    let match;

    while ((match = regex.exec(title)) !== null) {
      const start = match.index;
      const end = regex.lastIndex;

      if (start > lastIndex) {
        parts.push({ text: title.substring(lastIndex, start), isMatch: false });
      }

      parts.push({ text: title.substring(start, end), isMatch: true });
      lastIndex = end;
    }

    if (lastIndex < title.length) {
      parts.push({ text: title.substring(lastIndex), isMatch: false });
    }

    return parts;
  }

}