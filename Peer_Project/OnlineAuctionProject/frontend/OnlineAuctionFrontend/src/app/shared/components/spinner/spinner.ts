import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  imports: [],
  templateUrl: './spinner.html',
})
export class Spinner {
  @Input() loading:boolean=false;
} 
