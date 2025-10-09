import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-compare-modal',
  imports: [CommonModule],
  templateUrl: './compare-modal.html',
  styleUrl: './compare-modal.css'
})
export class CompareModal {
  @Input() selectedProperties: any[] = [];
  @Output() closed = new EventEmitter<void>();

  fieldsToCompare = ['price', 'propertyType', 'areaSqFt', 'listerType', 'listingPurpose'];

  getField(property: any, field: string): string {
    return property[field] ?? '-';
  }

  close() {
    this.closed.emit();
  }
}