import { Component, inject, OnInit } from '@angular/core';
import { FeatureService } from '../../core/services/feature.service';
import { FeatureAdminModel } from '../../models/feature-admin.model';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminFeatureFormComponent } from '../../components/admin-feature-form/admin-feature-form';

@Component({
  selector: 'app-admin-features-list',
  imports: [CommonModule, AdminFeaturesListComponent, ReactiveFormsModule, FormsModule, AdminFeatureFormComponent],
  templateUrl: './admin-features-list.html',
})
export class AdminFeaturesListComponent implements OnInit {
  features: FeatureAdminModel[] = [];
  loading = false;
  error = '';

  showFeatureModal = false;
  featureToEdit: FeatureAdminModel | null = null;

  private featureService = inject(FeatureService);
  private notificationService = inject(NotificationService);

  ngOnInit(): void {
    this.loadFeatures();
  }

  loadFeatures(): void {
    this.loading = true;
    this.featureService.getAllFeaturesForAdmin().subscribe({
      next: (res: any) => {
        this.features = res.data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load features.';
        this.loading = false;
      }
    });
  }

  openFeatureModal(): void {
  this.featureToEdit = null;
  this.showFeatureModal = true;
}

onEdit(feature: FeatureAdminModel): void {
  this.featureToEdit = { ...feature }; 
  this.showFeatureModal = true;
}


  onDelete(featureId: string): void {
    if (confirm('Are you sure you want to delete this feature?')) {
      this.featureService.deleteFeature(featureId).subscribe({
        next: () => {
          this.notificationService.success('Feature deleted successfully');
          this.loadFeatures();
        },
        error: () => {
          this.notificationService.error('Failed to delete feature');
        }
      });
    }
  }

  handleFeatureModalClose(updated: boolean): void {
    this.showFeatureModal = false;
    this.featureToEdit = null;
    if (updated) {
      this.loadFeatures();
    }
  }

  getTypes(feature: FeatureAdminModel): string[] {
    const types = new Set<string>();
    feature.applicability.forEach(a => types.add(a.appliesToType));
    return Array.from(types);
  }

  getPurposes(feature: FeatureAdminModel): string[] {
    const purposes = new Set<string>();
    feature.applicability.forEach(a => purposes.add(a.appliesToPurpose));
    return Array.from(purposes);
  }

  getTypePurposeMap(feature: FeatureAdminModel): { type: string, purposes: string[] }[] {
  const map = new Map<string, Set<string>>();
  
  for (const a of feature.applicability) {
    if (!map.has(a.appliesToType)) {
      map.set(a.appliesToType, new Set<string>());
    }
    map.get(a.appliesToType)!.add(a.appliesToPurpose);
  }

  return Array.from(map.entries()).map(([type, purposes]) => ({
    type,
    purposes: Array.from(purposes)
  }));
}

}
