import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
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
  // product:MyProductModel|null = new MyProductModel();
  @Input() product:MyProductModel|null = new MyProductModel();
  @Output() addToCart:EventEmitter<number> = new EventEmitter<number>();

  private productService = inject(MyProductService);

  handleBuyClick(pid:number|undefined){
    if(pid)
    {
        this.addToCart.emit(pid);
    }
  }

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