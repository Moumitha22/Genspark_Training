import { Component, inject, Input } from '@angular/core';
import { RecipeService } from '../services/recipe.service';
import { RecipeModel } from '../models/recipe.model';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-recipe',
  imports: [CurrencyPipe],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css'
})
export class Recipe {
   @Input() recipe:RecipeModel|null = new RecipeModel();

   private recipeService = inject(RecipeService);

   constructor(){}
}
