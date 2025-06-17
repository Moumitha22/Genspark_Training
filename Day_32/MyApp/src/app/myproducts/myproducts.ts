import { Component, HostListener, OnInit } from '@angular/core';
import { MyProductService } from '../services/myproduct.service';
import { MyProductModel } from '../models/myproduct';
import { MyProduct } from "../myproduct/myproduct";
import { CartItemModel } from '../models/cartItem';
import { FormsModule } from '@angular/forms';
import { debounce, debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';


@Component({
  selector: 'app-myproducts',
  imports: [MyProduct, FormsModule],
  templateUrl: './myproducts.html',
  styleUrl: './myproducts.css'
})

export class MyProducts implements OnInit {
  products:MyProductModel[]=[];
  cartItems:CartItemModel[] =[];
  cartCount:number =0;
  searchString: string = "";
  searchSubject = new Subject<string>();
  loading:boolean = false;
  limit=10;
  skip=0;
  total =0;
  
  constructor(private productService:MyProductService){}
  
  ngOnInit(): void {
    this.searchSubject.pipe(
        debounceTime(5000),
        distinctUntilChanged(),
        tap(()=>this.loading=true),
        switchMap(query=>this.productService.getProductsBySearchResult(query,this.limit,this.skip)),
        tap(()=>this.loading=false)
      ).subscribe({
        next:(data:any)=>{
          this.products = data.products as MyProductModel[];
          this.total = data.total;
        }
          
      });
  }

  handleSearchProduct(){
    this.searchSubject.next(this.searchString);
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

  @HostListener('window:scroll',[])
  onScroll():void
  {

    const scrollPosition = window.innerHeight + window.scrollY;
    const threshold = document.body.offsetHeight-100;
    if(scrollPosition>=threshold && this.products?.length<this.total)
    {
      console.log(scrollPosition);
      console.log(threshold)
      
      this.loadMore();
    }
  }
  loadMore(){
    this.loading = true;
    this.skip += this.limit;
    this.productService.getProductsBySearchResult(this.searchString,this.limit,this.skip)
        .subscribe({
          next:(data:any)=>{
            this.products = [...this.products,...data.products]
            this.loading = false;
          }
        })
  }
}