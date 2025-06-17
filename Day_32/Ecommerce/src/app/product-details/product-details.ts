import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';
import { ProductModel } from '../models/product.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-details',
  imports: [CommonModule],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css'
})
export class ProductDetails implements OnInit {
  product:ProductModel = new ProductModel();
  id:number = 0;
  loading: boolean = false;
  error: string | null = null;

  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
    

  ngOnInit(): void {
    this.id = this.route.snapshot.params["id"] as number;   
    console.log(this.id);
    
    if(this.id){
      this.loading = true;
      this.productService.getProduct(this.id).subscribe({
        next:(data:any) => {
          this.product = data as ProductModel;
          this.loading = false;
        },
        error: () => {
          this.error = 'Failed to fetch product details or invalid ID.';
          this.loading = false;
        }
      });
    } else {
      this.error = 'Invalid product ID.';
    }
  }
}
