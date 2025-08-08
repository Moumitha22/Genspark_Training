import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-auction-filter',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './auction-filter.html',
})
export class AuctionFilter implements OnInit {
  @Input() isBidder: boolean = true;
  @Output() applyFilters = new EventEmitter<any>();
  filterForm!: FormGroup;

  private formBuilder = inject(FormBuilder);

  ngOnInit(): void {
    const defaultMin = 0;
    const defaultMax = 10000;

    this.filterForm = this.formBuilder.group({
      startTime: [''],
      endTime: [''],
      status: [''],
      startingPriceMin: [defaultMin],
      startingPriceMax: [defaultMax],
      reservePriceMin: [defaultMin],
      reservePriceMax: [defaultMax],
      hasFileAttachments: [false],
    });
  }

  onApply() {
    const filters: any = {};
    const formValues = this.filterForm.value;

    if(formValues.status){
      filters.status = formValues.status;
    }

    filters.hasFileAttachments = formValues.hasFileAttachments;

    const getRange = (min: number, max: number) => {
      if (min === 0 && max === 10000) return undefined;
      return { min, max };
    };

    const starting = getRange(formValues.startingPriceMin, formValues.startingPriceMax);
    if (starting) {
      filters.startingPriceMin = starting.min;
      filters.startingPriceMax = starting.max;
    }

    const reserve = getRange(formValues.reservePriceMin, formValues.reservePriceMax);
    if (reserve) {
      filters.reservePriceMin = reserve.min;
      filters.reservePriceMax = reserve.max;
    }

    if (formValues.startTime && formValues.endTime) {
      const start = new Date(formValues.startTime);
      const end = new Date(formValues.endTime);
      if (start > end) {
        alert('Start Time cannot be after End Time');
        return;
      }
    }

    if (formValues.startTime) {
      filters.startTime = formValues.startTime;
    }
    if (formValues.endTime) {
      filters.endTime = formValues.endTime;
    }
    console.log(filters);
    this.applyFilters.emit(filters);
  }

  onReset() {
    this.filterForm.reset({
      status: '',
      startingPriceMin: 0,
      startingPriceMax: 10000,
      reservePriceMin: 0,
      reservePriceMax: 10000,
      startTime: '',
      endTime: '',
      hasFileAttachments: false,
    });
    this.applyFilters.emit({});
  }
}
