import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { DiscountCodeService } from '../../core/services/discount-code.service';
import { AdminDiscountForm } from '../../components/admin-discount-form/admin-discount-form';
import { DiscountCode } from '../../models/discount-code.model';
import { AdminDiscountFilter } from '../../components/admin-discount-filter/admin-discount-filter';
import { SortModel } from '../../models/sort.model';
import { PaginationInfo, PaginationModel } from '../../models/pagination.model';
import { PaginationComponent } from '../../components/pagination/pagination';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-discount-list',
  imports: [
    CommonModule,
    AdminDiscountForm,
    AdminDiscountFilter,
    PaginationComponent,
    FormsModule
  ],
  templateUrl: './admin-discount-list.html'
})
export class AdminDiscountList {
  discountCodes: any[] = [];
  loading = false;
  error: string | null = null;
  showDiscountForm = false;
  showFilters = false;
  discountToEdit: any = null;
  objectKeys = Object.keys;
  filters: BasicDiscountFilter = {
    code: '',
    minDiscountValue: null,
    maxDiscountValue: null,
    isPercentage: null,
    isDeleted: null,
    isActive: null,
    sortBy: 'CreatedAt',
    ascending: true,
    fromDate: null,
    toDate: null,
    typeOfProperty: null,
    purpose: null,
  };
  sortSelection: string = 'createdAt_desc';
  sort: SortModel = { sortBy: 'CreatedAt', ascending: false };

  pagination: PaginationInfo = {
    currentPage: 1,
    pageSize: 5,
    totalItems: 0,
    totalPages: 0,
  };

  constructor(private discountService: DiscountCodeService) {}

  ngOnInit() {
    this.loadDiscountCodes();
  }

  loadDiscountCodes(page: number = this.pagination.currentPage) {
    this.loading = true;
    const pagination: PaginationModel = {
      page,
      pageSize: this.pagination.pageSize,
    };
    this.discountService
      .searchDiscounts(this.filters, this.sort, pagination)
      .subscribe({
        next: (result) => {
          this.discountCodes = result.items;
          this.pagination.totalItems = result.totalItems;
          this.pagination.totalPages = Math.ceil(
            result.totalItems / this.pagination.pageSize
          );
          this.pagination.currentPage = page;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load discount codes';
          console.error(err);
          this.loading = false;
        },
      });
  }

  onPageChange(page: number) {
    this.loadDiscountCodes(page);
  }

  openDiscountModal(discount: DiscountCode | null = null) {
    this.discountToEdit = discount;
    this.showDiscountForm = true;
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }


  onEditDiscount(discount: DiscountCode) {
    this.discountToEdit = discount;
    this.showDiscountForm = true;
  }

  onDeleteDiscount(discount: DiscountCode) {
    var msg = discount.isDeleted ? 'enable' : 'disable';
    if (confirm(`Are you sure you want to ${msg} this discount code?`)) {
      this.discountService
        .updateDeletionStatus(discount.id, !discount.isDeleted)
        .subscribe({
          next: () => {
            this.loadDiscountCodes();
          },
          error: (err) => {
            this.error = 'Failed to delete discount code';
            console.error(err);
          },
        });
    }
  }

  onCodeChange() {
    this.pagination.currentPage = 1;
    this.loadDiscountCodes();
  }

  onSortChange(value: string) {
    const [field, direction] = value.split('_');
    switch (field) {
      case 'createdAt':
        this.sort.sortBy = 'CreatedAt';
        break;
      case 'code':
        this.sort.sortBy = 'Code';
        break;
      case 'discountValue':
        this.sort.sortBy = 'DiscountValue';
        break;
    }
    this.sort.ascending = direction === 'asc';
    this.pagination.currentPage = 1;
    this.loadDiscountCodes(1);
  }

  onFilterChange(filters: BasicDiscountFilter) {
    this.filters = filters;
    this.sort.sortBy = filters.sortBy || 'CreatedAt';
    this.sort.ascending = filters.ascending ?? false;
    this.pagination.currentPage = 1;
    this.loadDiscountCodes(1);
  }

  onSaveDiscount() {
    this.loadDiscountCodes();
    this.closeDiscountModal();
  }
  closeDiscountModal() {
    this.showDiscountForm = false;
    this.discountToEdit = null;
  }
}
