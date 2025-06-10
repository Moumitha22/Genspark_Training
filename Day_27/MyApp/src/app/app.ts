import { Component } from '@angular/core';
import { First} from './first/first'; 
import { Customer } from './customer/customer';
import { Products } from './products/products';

@Component({
  selector: 'app-root',
  imports: [First, Customer, Products],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'MyApp';
}
