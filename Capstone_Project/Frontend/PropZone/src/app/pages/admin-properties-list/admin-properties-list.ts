import { Component, inject, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { PropertyService } from '../../core/services/property.service';
import { NotificationService } from '../../core/services/notification.service';

import { PropertyModel } from '../../models/property.model';
import { BasicPropertySearchModel } from '../../models/basic-property-search.model';
import { SortModel } from '../../models/sort.model';
import { PaginationModel } from '../../models/pagination.model';

import { CoreFiltersComponent } from '../../components/core-filters/core-filters';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-properties-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, CoreFiltersComponent],
  templateUrl: './admin-properties-list.html',
  styleUrl: '../lister-inquiries/lister-inquiries.css'
})
export class AdminPropertyListComponent implements OnInit {
  @Input() maxCount?: number;
  properties: PropertyModel[] = [];
  loading = true;
  listerName?: string;

  coreFilters: BasicPropertySearchModel = {
    propertyTypes: [],
    sortBy: 'CreatedAt',
    ascending: false
  };

  private propertyService = inject(PropertyService);
  private notificationService = inject(NotificationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
    if (params['listerId']) {
      this.coreFilters.listerId = params['listerId'];
      this.listerName = params['listerName'];
    }
    this.loadProperties();
    });
  }

  onFiltersChanged(filters: BasicPropertySearchModel): void {
    this.coreFilters = { ...filters };
    this.loadProperties();
  }

  loadProperties(): void {
    this.loading = true;

    const sortModel: SortModel = {
      sortBy: this.coreFilters.sortBy ?? 'CreatedAt',
      ascending: this.coreFilters.ascending ?? false
    };

    const paginationModel: PaginationModel = {
      page: 1,
      pageSize: 1000
    };

    this.propertyService.advancedSearch(
      {
        ...this.coreFilters,
        featureFilters: [] 
      },
      sortModel,
      paginationModel
    ).subscribe({
      next: (res: any) => {
        let loaded = res.data.items.map((property: PropertyModel) => ({
          ...property,
          showFullMessage: false
        }));

        if (this.maxCount) {
          loaded = loaded.slice(0, this.maxCount);
        }

        this.properties = loaded;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load properties', err);
        this.loading = false;
      }
    });
  }

  deleteProperty(id: string): void {
    if (!confirm('Are you sure you want to delete this property?')) return;

    this.propertyService.deleteProperty(id).subscribe({
      next: () => {
        this.notificationService.success('Property deleted successfully');
        this.properties = this.properties.filter(p => p.id !== id);
      },
      error: (err) => {
        console.error('Delete failed:', err);
        this.notificationService.error('Failed to delete property');
      }
    });
  }

  clearListerFilter() {
    this.coreFilters.listerId = undefined;
    this.listerName = undefined;
    this.router.navigate([], {
      queryParams: { listerId: null, listerName: null },
      queryParamsHandling: 'merge'
    });
    this.loadProperties();
  }


}
