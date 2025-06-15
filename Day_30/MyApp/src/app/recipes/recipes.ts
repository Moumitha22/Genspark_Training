import { Component, inject} from '@angular/core';
import { RecipeService } from '../services/recipe.service';
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
  recipes = this.recipeService.recipes;
}
