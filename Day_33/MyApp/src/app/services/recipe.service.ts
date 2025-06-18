import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toSignal } from '@angular/core/rxjs-interop';
import { RecipeModel } from '../models/recipe.model';
import { map } from 'rxjs';

@Injectable()
export class RecipeService {
  private http = inject(HttpClient);
  private apiUrl = 'https://dummyjson.com/recipes';
  recipes = signal<RecipeModel[]>([]);

  constructor() {
    this.loadProducts();
  }

  loadProducts(): void {
    this.http.get<any>(this.apiUrl).subscribe({
      next: (data) => this.recipes.set(data.recipes as RecipeModel[]),
      error: (err) => console.error('Recipe load failed:', err),
      complete:() => {},
    });
  }

  // recipesSignal = toSignal(
  //   this.http.get<{ recipes: RecipeModel[] }>(this.apiUrl).pipe(
  //     map((res) => res.recipes)
  //   ),
  //   { initialValue: [] } 
  // );
}
