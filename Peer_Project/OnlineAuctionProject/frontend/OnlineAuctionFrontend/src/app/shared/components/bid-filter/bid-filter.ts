import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-bid-filter',
  imports: [CommonModule, FormsModule],
  templateUrl: './bid-filter.html',
})
export class BidFilter {
  @Output() applyFilters = new EventEmitter<any>();

  filters = {
    amountMin: null,
    amountMax: null,
    dateMin: '',
    dateMax: ''
  };

  apply() {
    this.applyFilters.emit(this.filters);
  }

  reset() {
    this.filters = {
      amountMin: null,
      amountMax: null,
      dateMin: '',
      dateMax: '',
    };
    this.apply();
  }
}
