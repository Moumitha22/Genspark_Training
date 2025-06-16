import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Second } from '../second/second';
import { CustomerModel } from '../models/customer';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-first',
  imports: [FormsModule, CommonModule, Second],
  templateUrl: './first.html',
  styleUrl: './first.css'
})
export class First {
  name:string;
  className:string = "bi bi-balloon-heart";
  like:boolean = false;
  childMsg: string = "";
  count:number = 0;
  show = false;
  color = 'green';

  myclassName:string = "bi bi-balloon-heart";

  customer:CustomerModel = new CustomerModel(1,"Dia",30);

  customers:CustomerModel[] = [
    new CustomerModel(1,"Dia",30,0,0),
    new CustomerModel(2,'Ria',45,0,0),
    new CustomerModel(3,'Mia',45,0,0),
  ];
  fillHeart = false;

  constructor(){
    this.name = "Ram"
  }

  onButtonClick(uname:string) {
    alert("Button clicked by " + uname)
  }

  toggleLike(){
    this.like = !this.like;
    if(this.like)
      this.className ="bi bi-balloon-heart-fill";
    else
        this.className ="bi bi-balloon-heart";
  }

  onNotify(msg:string){
    this.childMsg = msg;
  }

  handleLikeClick(){
    this.count++;
  }

  handleCustomerLike(){
    this.customer.like++;
  }
  handleCustomerDislike(){
    this.customer.dislike++;
  }

  handleCustomerLikes(custom:CustomerModel){
    custom.like++;
  }
  handleCustomerDislikes(custom:CustomerModel){
    custom.dislike++;
  }

  handleToggle(){
    this.fillHeart = !this.fillHeart;
    if(this.fillHeart){
      this.myclassName = "bi bi-balloon-heart-fill";
    }else{
      this.myclassName = "bi bi-balloon-heart";
    }
  }
}
