import { Injectable, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toSignal } from '@angular/core/rxjs-interop';
import { RecipeModel } from '../models/recipe.model';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  private http = inject(HttpClient);
  private apiUrl = 'https://dummyjson.com/recipes';

  recipesSignal = toSignal(
    this.http.get<{ recipes: RecipeModel[] }>(this.apiUrl).pipe(
      map((res) => res.recipes)
    ),
    { initialValue: [] } 
  );
}
