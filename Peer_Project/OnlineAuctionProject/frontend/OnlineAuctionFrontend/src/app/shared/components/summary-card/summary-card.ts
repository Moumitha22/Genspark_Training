import { CommonModule, NgOptimizedImage } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-summary-card',
  imports: [CommonModule, NgOptimizedImage],
  templateUrl: './summary-card.html',
})
export class SummaryCard {
  @Input() title!: string;
  @Input() value!: number;
  @Input() imgSrc!: string;
  @Input() bgColor!: string;
  @Input() imgAlt: string = '';
  @Input() imgSize: number = 30;
  @Input() isCurrency: boolean = false;
}
