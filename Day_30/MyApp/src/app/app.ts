import { Component } from '@angular/core';
import { First} from './first/first'; 
import { Customer } from './customer/customer';
import { Products } from './products/products';
import { MyProducts } from './myproducts/myproducts';
import { Recipes } from './recipes/recipes';
import { Login } from './login/login';
import { Storage } from './storage/storage';
import { Menu } from './menu/menu';

@Component({
  selector: 'app-root',
  imports: [First, Customer, Products, MyProducts, Recipes, Login, Menu, Storage],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App {
  protected title = 'MyApp';
}
