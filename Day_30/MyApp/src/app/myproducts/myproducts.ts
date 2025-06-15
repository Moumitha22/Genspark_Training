import { Component, OnInit } from '@angular/core';
import { MyProductService } from '../services/myproduct.service';
import { MyProductModel } from '../models/myproduct';
import { MyProduct } from "../myproduct/myproduct";
import { CartItemModel } from '../models/cartItem';


@Component({
  selector: 'app-myproducts',
  imports: [MyProduct],
  templateUrl: './myproducts.html',
  styleUrl: './myproducts.css'
})
export class MyProducts implements OnInit {
  products:MyProductModel[]|undefined=undefined;
  cartItems:CartItemModel[] =[];
  cartCount:number =0;
  
  constructor(private productService:MyProductService){}
  
  ngOnInit(): void {
    this.productService.getAllProducts().subscribe(
      {
        next:(data:any)=>{
         this.products = data.products as MyProductModel[];
        },
        error:(err)=>{},
        complete:()=>{}
      }
    )
  }

  handleAddToCart(event:number)
  {
    console.log("Handling add to cart - "+event)
    let flag = false;
    for(let i=0;i<this.cartItems.length;i++)
    {
      if(this.cartItems[i].id==event)
      {
        flag=true;
        this.cartItems[i].count++;
        break;
      }
    }
    if(!flag)
      this.cartItems.push(new CartItemModel(event,1));
    this.cartCount++;
  }
}