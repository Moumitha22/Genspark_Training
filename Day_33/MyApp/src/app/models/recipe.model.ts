export class RecipeModel {
  constructor(
    public id: number = 0,
    public name: string = "",
    public cuisine: string = "",
    public difficulty: string = "",
    public ingredients: string[] = [],
    public image: string = "",
    public prepTimeMinutes: number = 0,
    public cookTimeMinutes: number = 0,
    public servings: number = 0,
    public rating: number = 0,
    public mealType: string[] = []
  ) {}
}
