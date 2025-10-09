import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { PropertyModel } from '../../models/property.model';
import { BasicPropertySearchModel } from '../../models/basic-property-search.model';
import { SortModel } from '../../models/sort.model';
import { PaginationModel, PaginationInfo } from '../../models/pagination.model';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';

import { PropertyService } from '../../core/services/property.service';
import { FeatureService } from '../../core/services/feature.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';

import { PropertyCard } from '../../components/property-card/property-card';
import { CoreFiltersComponent } from '../../components/core-filters/core-filters';
import { DynamicFiltersComponent } from '../../components/dynamic-filters/dynamic-filters';
import { ContactListerFormComponent } from '../../components/contact-lister-form/contact-lister-form';
import { PaginationComponent } from '../../components/pagination/pagination';
import { CompareModal } from '../../components/compare-modal/compare-modal';
import { DiscountCodeService } from '../../core/services/discount-code.service';
import { DiscountSimulationRequest } from '../../models/simulate-discount-request.model';

@Component({
  selector: 'app-property-list',
  standalone: true,
  imports: [
    CommonModule,
    PropertyCard,
    ContactListerFormComponent,
    CoreFiltersComponent,
    DynamicFiltersComponent,
    FormsModule,
    PaginationComponent,
    CompareModal,
  ],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css',
})
export class PropertyList implements OnInit {
  properties: PropertyModel[] = [];
  dynamicFilters: DynamicFeatureModel[] = [];
  discountedPrices: { [propertyId: string]: number | null } = {};
  coreFilters: BasicPropertySearchModel = { listingPurpose: 'Sale' };
  loading = true;
  errorMessage = '';
  showMobileFilters = false;
  showCompareModal = false;
  currentFeatureFilterMap: { [featureId: string]: string[] } = {};
  selectedPropertyForContact: {
    id: string;
    title: string;
    location: string;
  } | null = null;
  selectedPropertiesForComparison: PropertyModel[] = [];
  selectedDiscountIdsMap: { [propertyId: string]: string[] } = {};

  pagination: PaginationInfo = {
    currentPage: 1,
    pageSize: 5,
    totalItems: 0,
    totalPages: 0,
  };

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private propertyService = inject(PropertyService);
  private authService = inject(AuthService);
  private featureService = inject(FeatureService);
  private notificationService = inject(NotificationService);
  private discountService = inject(DiscountCodeService);

  private lastPurpose: string | undefined;
  private lastPropertyType: string | undefined;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const rawPurpose = params['purpose'];
      const mappedPurpose = rawPurpose === 'Rent' ? 'Rent' : 'Sale';

      const propertyType = params['propertyType'];
      const propertyTypes = propertyType ? [propertyType] : [];

      this.coreFilters = {
        listingPurpose: mappedPurpose,
        propertyTypes: propertyTypes,
        city: params['city'],
        locality: params['locality'],
        keyword: params['keyword'],
        sortBy: 'CreatedAt',
        ascending: false,
      };

      this.loadProperties(this.coreFilters, 1);
      this.fetchDynamicFilters(mappedPurpose, propertyTypes[0]);
    });
  }

  loadProperties(filters: BasicPropertySearchModel, page: number): void {
    this.loading = true;

    const sort: SortModel = {
      sortBy: filters.sortBy || 'CreatedAt',
      ascending: filters.ascending || false,
    };

    const pagination: PaginationModel = {
      page,
      pageSize: this.pagination.pageSize,
    };

    const searchModel = {
      ...filters,
      priceRange: {
        min: filters.minPrice,
        max: filters.maxPrice,
      },
      statuses: ['Available'],
      featureFilters: Object.entries(this.currentFeatureFilterMap)
        .filter(([_, values]) => values.length > 0)
        .map(([featureId, values]) => {
          const mode =
            this.dynamicFilters.find((f) => f.id === featureId)?.filterMode ??
            'Exact';
          return { featureId, values, filterMode: mode };
        }),
    };

    this.propertyService
      .advancedSearch(searchModel, sort, pagination)
      .subscribe({
        next: (res: any) => {
          this.properties = res.data.items;
          this.pagination = res.data.pagination;
          this.loading = false;
        },
        error: (err) => {
          console.error('Search error:', err);
          this.errorMessage =
            err.error?.message || 'Failed to load properties.';
          this.properties = [];
          this.loading = false;
        },
      });
  }

  get hasActiveFilters(): boolean {
    return !!(
      this.coreFilters.keyword ||
      this.coreFilters.city ||
      (this.coreFilters.propertyTypes &&
        this.coreFilters.propertyTypes.length > 0) ||
      (this.coreFilters.listerTypes &&
        this.coreFilters.listerTypes.length > 0) ||
      this.coreFilters.minPrice ||
      this.coreFilters.maxPrice ||
      this.coreFilters.isDiscountAvailable ||
      Object.keys(this.currentFeatureFilterMap).some(
        (key) => this.currentFeatureFilterMap[key].length > 0
      )
    );
  }

  onCoreFiltersChanged(filters: BasicPropertySearchModel): void {
    this.coreFilters = { ...filters };
    this.currentFeatureFilterMap = {};
    this.loadProperties(filters, 1);

    const newPurpose = filters.listingPurpose!;
    const newPropertyType = filters.propertyTypes?.[0];

    const shouldFetchDynamicFilters =
      newPurpose !== this.lastPurpose ||
      newPropertyType !== this.lastPropertyType;

    if (shouldFetchDynamicFilters) {
      this.fetchDynamicFilters(newPurpose, newPropertyType);
      this.lastPurpose = newPurpose;
      this.lastPropertyType = newPropertyType;
    }
  }

  onDynamicFiltersChanged(featureFilterMap: { [featureId: string]: string[] }) {
    this.currentFeatureFilterMap = featureFilterMap;
    this.loadProperties(this.coreFilters, 1);
  }

  onPageChange(page: number) {
    this.loadProperties(this.coreFilters, page);
  }

  private fetchDynamicFilters(
    purpose: 'Sale' | 'Rent',
    propertyType?: string
  ): void {
    this.featureService.getApplicableFeatures(purpose, propertyType).subscribe({
      next: (res) => {
        this.dynamicFilters = res.data;
      },
      error: (err) => {
        console.error('Failed to load dynamic filters:', err);
        this.dynamicFilters = [];
      },
    });
  }

  toggleMobileFilters(): void {
    this.showMobileFilters = !this.showMobileFilters;
  }

  handleContactClicked(data: { id: string; title: string; location: string }) {
    const user = this.authService.currentUser;

    if (!user) {
      this.notificationService.warning('Please log in to contact the lister');
      this.router.navigate(['/login']);
      return;
    }

    if (user.role !== 'Buyer') {
      this.notificationService.warning(
        'Only buyers can contact listers. Please login as buyer to continue.'
      );
      this.router.navigate(['/login']);
      return;
    }

    this.selectedPropertyForContact = data;
  }

  toggleCompare(property: PropertyModel): void {
    const index = this.selectedPropertiesForComparison.findIndex(
      (p) => p.id === property.id
    );

    if (index !== -1) {
      this.selectedPropertiesForComparison.splice(index, 1);
    } else {
      if (this.selectedPropertiesForComparison.length >= 3) {
        this.notificationService.warning(
          'You can only compare up to 3 properties.'
        );
        return;
      }

      this.selectedPropertiesForComparison.push(property);
    }
  }

  isPropertySelected(id: string): boolean {
    return this.selectedPropertiesForComparison.some((p) => p.id === id);
  }

  get canCompare(): boolean {
    return this.selectedPropertiesForComparison.length >= 1;
  }

  getPropertyField(prop: PropertyModel, field: string): any {
    switch (field) {
      case 'price':
        return prop.price;
      case 'propertyType':
        return prop.propertyType;
      case 'areaSqFt':
        return prop.areaSqFt;
      case 'listerType':
        return prop.listerType;
      case 'listingPurpose':
        return prop.listingPurpose;
      default:
        return '—';
    }
  }

  onDiscountSelection(event: DiscountSimulationRequest, propertyId: string) {
    this.discountService.simulateDiscounts(event).subscribe({
      next: (result) => {
        const finalPrice = result.discountedPrice;
        this.selectedDiscountIdsMap[propertyId] = event.discountCodeIds;
        const originalPrice = result.originalPrice;
        if (finalPrice == originalPrice) {
          this.notificationService.info(
            'No discount applied. Final price is the same as original price.'
          );
          this.discountedPrices[propertyId] = null;
          return;
        }
        this.discountedPrices[propertyId] = finalPrice;
        this.notificationService.success(
          `Final price has been updated: ₹${finalPrice}`
        );
      },
      error: (err) => {
        console.error('Discount simulation error:', err);
        this.notificationService.error('Failed to simulate discount');
      },
    });
  }

  openCompareModal() {
    if (this.selectedPropertiesForComparison.length < 2) {
      this.notificationService.warning(
        'Select at least 2 properties to compare.'
      );
      return;
    }
    this.showCompareModal = true;
  }

  clearComparison() {
    this.selectedPropertiesForComparison = [];
    this.showCompareModal = false;
  }

  clearAllFilters(): void {
    this.coreFilters = {
      listingPurpose: 'Sale',
      propertyTypes: [],
      sortBy: 'CreatedAt',
      ascending: false,
    };

    this.currentFeatureFilterMap = {};
    this.lastPurpose = undefined;
    this.lastPropertyType = undefined;
    this.fetchDynamicFilters('Sale');
    this.loadProperties(this.coreFilters, 1);
  }
}
