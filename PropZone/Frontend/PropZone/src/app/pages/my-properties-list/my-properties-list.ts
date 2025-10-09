import { Component, inject, OnInit } from '@angular/core';
import { BasicPropertySearchModel } from '../../models/basic-property-search.model';
import { PropertyModel } from '../../models/property.model';
import { PropertyService } from '../../core/services/property.service';
import { AuthService } from '../../core/services/auth.service';
import { PropertyCard } from '../../components/property-card/property-card';
import { CoreFiltersComponent } from '../../components/core-filters/core-filters';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PaginationModel } from '../../models/pagination.model';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../../components/pagination/pagination';

@Component({
  selector: 'app-my-properties-list',
  standalone: true,
  imports: [
    CommonModule,
    PropertyCard,
    CoreFiltersComponent,
    RouterLink,
    FormsModule,
    PaginationComponent,
  ],
  templateUrl: './my-properties-list.html',
  styleUrls: ['./my-properties-list.css'],
})
export class MyPropertiesListComponent implements OnInit {
  private propertyService = inject(PropertyService);
  private authService = inject(AuthService);

  allProperties: PropertyModel[] = [];
  filteredProperties: PropertyModel[] = [];

  filterModel: BasicPropertySearchModel = {
    propertyTypes: [],
    sortBy: 'CreatedAt',
    ascending: false,
  };

  currentPage = 1;
  pageSize = 5;

  loading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadAllProperties();
  }

  loadAllProperties(): void {
    const user = this.authService.currentUser;
    if (!user) return;

    const pagination: PaginationModel = { page: 1, pageSize: 100 }; // large pageSize to get all

    this.loading = true;

    this.propertyService.getPropertiesByLister(user.id, pagination).subscribe({
      next: (res) => {
        this.allProperties = res.data.items;
        this.applyFiltersAndSorting();
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to load properties.';
        this.loading = false;
      },
    });
  }

  onFiltersChanged(filters: BasicPropertySearchModel): void {
    this.filterModel = filters;
    this.currentPage = 1;
    this.applyFiltersAndSorting();
  }

  applyFiltersAndSorting(): void {
    const filters = this.filterModel;

    let result = this.allProperties.filter((p) => {
      const matchPurpose =
        !filters.listingPurpose || filters.listingPurpose == p.listingPurpose;
      const matchType =
        !filters.propertyTypes ||
        filters.propertyTypes.length === 0 ||
        filters.propertyTypes.includes(p.propertyType);
      const matchLister =
        !filters.listerTypes ||
        filters.listerTypes.length === 0 ||
        filters.listerTypes.includes(p.listerType);
      const matchStatus = !filters.status || p.status === filters.status;
      const matchCity =
        !filters.city ||
        p.location.city?.toLowerCase().includes(filters.city.toLowerCase());
      const matchKeyword =
        !filters.keyword ||
        p.title?.toLowerCase().includes(filters.keyword.toLowerCase()) ||
        p.description?.toLowerCase().includes(filters.keyword.toLowerCase());
      const hasDiscount =
        filters.isDiscountAvailable === undefined ||
        filters.isDiscountAvailable === null
          ? true
          : filters.isDiscountAvailable
          ? p.discountCodes && p.discountCodes.length > 0
          : !p.discountCodes || p.discountCodes.length === 0;
      return (
        matchPurpose &&
        matchType &&
        matchLister &&
        matchStatus &&
        matchCity &&
        matchKeyword &&
        hasDiscount
      );
    });

    switch (filters.sortBy) {
      case 'Price':
        result.sort((a, b) =>
          filters.ascending ? a.price - b.price : b.price - a.price
        );
        break;
      case 'CreatedAt':
      default:
        result.sort((a, b) =>
          filters.ascending
            ? new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
            : new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        break;
    }

    this.filteredProperties = result;
  }

  clearAllFilters(): void {
    this.filterModel = {
      propertyTypes: [],
      sortBy: 'CreatedAt',
      ascending: false,
      // Optional: include other default filter values like listingPurpose, listerTypes, etc.
    };
    this.currentPage = 1;
    this.applyFiltersAndSorting();
  }

  get paginatedProperties(): PropertyModel[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredProperties.slice(start, start + this.pageSize);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
  }

  get totalPages(): number {
    return Math.ceil(this.filteredProperties.length / this.pageSize);
  }
}
