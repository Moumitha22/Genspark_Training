import { Component } from '@angular/core';

@Component({
  selector: 'app-customer',
  imports: [],
  templateUrl: './customer.html',
  styleUrl: './customer.css'
})
export class Customer {
  customer = {
    firstName: 'Moumitha',
    lastName: 'Raghu',
    age: 20,
    location: 'Ooty'
  };

  likes = 0;
  dislikes = 0;


  like(){
    this.likes++;
  }

  dislike(){
    this.dislikes++;
  }

}
