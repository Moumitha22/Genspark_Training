import { Component, EventEmitter, Input, Output } from '@angular/core';


@Component({
  selector: 'app-product-item',
  imports: [],
  templateUrl: './product-item.html',
  styleUrl: './product-item.css'
})
export class ProductItem {
  @Input() product: any;
  @Output() addToCart = new EventEmitter<void>();

  // onClickAdd() {
  //   this.addToCart.emit();
  // }

}
