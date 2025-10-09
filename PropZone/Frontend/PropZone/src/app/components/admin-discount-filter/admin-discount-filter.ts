import {
  Component,
  EventEmitter,
  Output,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-admin-discount-filter',
  imports: [FormsModule],
  templateUrl: './admin-discount-filter.html',
  styleUrl: './admin-discount-filter.css',
})
export class AdminDiscountFilter {
  @Output() filterChange = new EventEmitter<any>();
  @Output() close = new EventEmitter<void>();

  filterform: BasicDiscountFilter = {
    code: '',
    minDiscountValue: null,
    maxDiscountValue: null,
    isPercentage: null,
    fromDate: null,
    toDate: null,
    isDeleted: null,
    isActive: null,
    sortBy: '',
    ascending: false,
    typeOfProperty: null,
    purpose: null,
  };

  private filterSubject = new Subject<void>();

  constructor() {
    this.filterSubject.pipe(debounceTime(300)).subscribe(() => {
      this.applyFilter();
    });
  }

  onFieldChange() {
    this.filterSubject.next();
  }

  applyFilter() {
    const filterToSend: { [key: string]: any } = {};
    Object.entries(this.filterform).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        filterToSend[key] = value;
      }
    });

    this.filterChange.emit(filterToSend);
  }

  closeFilter() {
    this.close.emit();
  }
}
