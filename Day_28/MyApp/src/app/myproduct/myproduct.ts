import { Component, inject, Input } from '@angular/core';
import { MyProductService } from '../services/myproduct.service';
import { MyProductModel } from '../models/myproduct';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-product',
  imports: [CurrencyPipe],
  templateUrl: './myproduct.html',
  styleUrl: './myproduct.css'
})
export class MyProduct {
  @Input() product:MyProductModel|null = new MyProductModel();
// product:MyProductModel|null = new MyProductModel();

private productService = inject(MyProductService);

constructor(){
    // this.productService.getProduct(1).subscribe(
    //   {
    //     next:(data)=>{
     
    //       this.product = data as MyProductModel;
    //       console.log(this.product)
    //     },
    //     error:(err)=>{
    //       console.log(err)
    //     },
    //     complete:()=>{
    //       console.log("All done");
    //     }
    //   })
}

}