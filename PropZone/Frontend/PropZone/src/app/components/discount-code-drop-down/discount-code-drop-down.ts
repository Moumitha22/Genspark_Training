import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgSelectModule } from "@ng-select/ng-select";
import { DiscountCode } from '../../models/discount-code.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-discount-code-drop-down',
  imports: [NgSelectModule, ReactiveFormsModule, CommonModule],
  templateUrl: './discount-code-drop-down.html',
  styleUrl: './discount-code-drop-down.css'
})
export class DiscountCodeDropDown {
 
  @Input() discountCodes: DiscountCode[] = [];
  @Input() control!: FormControl;
  @Input() maxSelectedItems: number = 3;
  @Output() selectionChange = new EventEmitter<string[]>();

  onChange(selectedCodes: string[]) {
    this.selectionChange.emit(selectedCodes);
  }
  compareDiscountCodes(a: any, b: any) {
    if(a && b && typeof a === 'object' && typeof b === 'object') {
      return a.id === b.id;
    }
    if(a && b && typeof a === 'object' && typeof b === 'string') {
      return a.id === b;
    }
    if(a && b && typeof a === 'string' && typeof b === 'object') {
      return a === b.id;
    }
    return a === b;
  }

}
