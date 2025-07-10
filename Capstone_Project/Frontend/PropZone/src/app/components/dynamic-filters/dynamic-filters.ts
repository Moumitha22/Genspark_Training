import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dynamic-filters',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dynamic-filters.html',
  styleUrls: ['./dynamic-filters.css']
})
export class DynamicFiltersComponent {
  @Input() dynamicFilters: DynamicFeatureModel[] = [];
  @Output() filtersChanged = new EventEmitter<any>();

  currentFilters: any = {};

  handleToggleChange(event: Event, featureId: string) {
  const input = event.target as HTMLInputElement;

  if (input.checked) {
    this.currentFilters[featureId] = ['true'];
  } else {
    delete this.currentFilters[featureId];
  }

  this.emitChanges();
}

  handleCheckboxChange(event: Event, featureId: string, value: string) {
    const input = event.target as HTMLInputElement;
    if (!this.currentFilters[featureId]) {
      this.currentFilters[featureId] = [];
    }

    if (input.checked) {
      this.currentFilters[featureId].push(value);
    } else {
      this.currentFilters[featureId] = this.currentFilters[featureId].filter((v: string) => v !== value);
    }

    this.emitChanges();
  }

  handleRangeMinChange(event: Event, featureId: string) {
    const input = event.target as HTMLInputElement;
    const min = input.value;
    const max = this.currentFilters[featureId]?.[1] || '';
    this.currentFilters[featureId] = [min, max];
    this.emitChanges();
  }

  handleRangeMaxChange(event: Event, featureId: string) {
    const input = event.target as HTMLInputElement;
    const min = this.currentFilters[featureId]?.[0] || '';
    const max = input.value;
    this.currentFilters[featureId] = [min, max];
    this.emitChanges();
  }

  emitChanges() {
    this.filtersChanged.emit(this.currentFilters);
  }
}
