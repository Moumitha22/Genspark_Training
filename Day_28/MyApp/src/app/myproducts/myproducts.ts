import { Component, OnInit } from '@angular/core';
import { MyProductService } from '../services/myproduct.service';
import { MyProductModel } from '../models/myproduct';
import { MyProduct } from "../myproduct/myproduct";


@Component({
  selector: 'app-myproducts',
  imports: [MyProduct],
  templateUrl: './myproducts.html',
  styleUrl: './myproducts.css'
})
export class MyProducts implements OnInit {
  products:MyProductModel[]|undefined=undefined;
  constructor(private productService:MyProductService){

  }
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

}