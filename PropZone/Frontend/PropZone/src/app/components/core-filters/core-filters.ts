import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BasicPropertySearchModel } from '../../models/basic-property-search.model';

@Component({
  selector: 'app-core-filters',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './core-filters.html',
  styleUrl: './core-filters.css'
})
export class CoreFiltersComponent implements OnInit {
  @Input() model: BasicPropertySearchModel = {
    listingPurpose: 'Sale',
    propertyTypes: [],
    sortBy: 'CreatedAt',
    ascending: false
  };

  @Output() filtersChanged = new EventEmitter<BasicPropertySearchModel>();

  selectedPropertyType: string | undefined;
  selectedSortOption: string = 'newest';

  ngOnInit(): void {
    if (this.model.propertyTypes && this.model.propertyTypes.length > 0) {
      this.selectedPropertyType = this.model.propertyTypes[0];
    }

    if (this.model.sortBy === 'Price') {
      this.selectedSortOption = this.model.ascending ? 'priceAsc' : 'priceDesc';
    } else if (this.model.sortBy === 'CreatedAt') {
      this.selectedSortOption = this.model.ascending ? 'oldest' : 'newest';
    }
  }

  onPropertyTypeChange(value: string | undefined): void {
    this.selectedPropertyType = value;
    this.model.propertyTypes = value ? [value] : [];
    this.onFilterChange();
  }

  onSortChange(): void {
    switch (this.selectedSortOption) {
      case 'priceAsc':
        this.model.sortBy = 'Price';
        this.model.ascending = true;
        break;
      case 'priceDesc':
        this.model.sortBy = 'Price';
        this.model.ascending = false;
        break;
      case 'oldest':
        this.model.sortBy = 'CreatedAt';
        this.model.ascending = true;
        break;
      case 'newest':
      default:
        this.model.sortBy = 'CreatedAt';
        this.model.ascending = false;
        break;
    }
    this.onFilterChange();
  }

  onFilterChange(): void {
    this.filtersChanged.emit(this.model);
  }

  ngOnChanges() {
    if (this.model.sortBy === 'Price') {
      this.selectedSortOption = this.model.ascending ? 'priceAsc' : 'priceDesc';
    } else {
      this.selectedSortOption = this.model.ascending ? 'oldest' : 'newest';
    }

     if (this.model.propertyTypes && this.model.propertyTypes.length > 0) {
      this.selectedPropertyType = this.model.propertyTypes[0];
    } else {
      this.selectedPropertyType = undefined;
    }
  }

}
