import { Component, inject, signal } from '@angular/core';
import { RecipeService } from '../services/recipe.service';
import { RecipeModel } from '../models/recipe.model';
import { Recipe } from '../recipe/recipe';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrl: './recipes.css',
})
export class Recipes {
  private recipeService = inject(RecipeService);
  recipes = this.recipeService.recipesSignal;
}
