import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PropertyService } from '../../core/services/property.service';
import { PropertyFormStateService } from '../../core/services/property-form-state.service';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';
import { FeatureService } from '../../core/services/feature.service';
import { NotificationService } from '../../core/services/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-post-property-form',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
  templateUrl: './post-property-form.html',
  styleUrls: ['./post-property-form.css']
})
export class PostPropertyFormComponent implements OnInit {
  @Output() back = new EventEmitter<void>();
  
  private propertyService = inject(PropertyService);
  private featureService = inject(FeatureService);
  private notificationService = inject(NotificationService);
  private stateService = inject(PropertyFormStateService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  
  listerType!: string;
  listingPurpose!: string;
  propertyType!: string;

  propertyForm!: FormGroup;
  dynamicFeatures: DynamicFeatureModel[] = [];


  ngOnInit(): void {
    const meta = this.stateService.getMetadata();
    this.listerType = meta.listerType || '';
    this.listingPurpose = meta.listingPurpose || '';
    this.propertyType = meta.propertyType || '';

    this.initForm();
    this.loadDynamicFeatures();
  }

  initForm(): void {
    this.propertyForm = this.fb.group({
      listerType: [this.listerType],
      listingPurpose: [this.listingPurpose],
      propertyType: [this.propertyType],
      title: ['', [Validators.required, Validators.maxLength(150)]],
      description: [''],
      price: [null, [Validators.required, Validators.min(1)]],
      areaSqFt: [null, [Validators.required, Validators.min(1)]],
      location: this.fb.group({
        locality: ['', Validators.required],
        city: ['', Validators.required],
        state: ['', Validators.required]
      }),
      features: this.fb.array([])
    });
  }

  loadDynamicFeatures(): void {
    this.featureService.getApplicableFeatures(this.listingPurpose, this.propertyType).subscribe((res) => {
      this.dynamicFeatures = res.data.map(f => ({
        ...f,
        dataType: f.dataType?.toLowerCase()
      }));

      const featuresArray = this.fb.array<FormGroup>([]);

      for (const feature of this.dynamicFeatures) {
        featuresArray.push(this.fb.group({
          featureId: [feature.id],
          dataType: [feature.dataType],
          value: [feature.dataType === 'boolean' ? false : ''],
          optionId: [''],
          selectedOptionIds: [[]]
        }));
      }

      this.propertyForm.setControl('features', featuresArray);
    });
  }

  get featuresFormArray(): FormArray {
    return this.propertyForm.get('features') as FormArray;
  }

  onMultiSelectCheckboxChange(event: Event, index: number, optionId: string): void {
    const input = event.target as HTMLInputElement;
    const control = this.featuresFormArray.at(index).get('selectedOptionIds');
    const current = control?.value || [];

    if (input.checked) {
      if (!current.includes(optionId)) current.push(optionId);
    } else {
      const idx = current.indexOf(optionId);
      if (idx > -1) current.splice(idx, 1);
    }

    control?.setValue([...current]);
    control?.markAsDirty();
  }

  onSubmit(): void {
    if (this.propertyForm.invalid) {
      this.propertyForm.markAllAsTouched();
      return;
    }

    const raw = this.propertyForm.value;

    const features = raw.features.flatMap((f: any) => {
      switch (f.dataType) {
        case 'multiselect':
          return f.selectedOptionIds.map((id: string) => ({
            featureId: f.featureId,
            dataType: 'multiselect',
            optionId: id,
            value: ''
          }));
        case 'dropdown':
          return f.optionId ? [{
            featureId: f.featureId,
            dataType: 'dropdown',
            optionId: f.optionId,
            value: ''
          }] : [];
        case 'boolean':
          console.log('✅ Boolean Feature:', f.featureId, '→', f.value);

          return [{
            featureId: f.featureId,
            dataType: 'boolean',
            value: String(f.value),
            optionId: null
          }];
        case 'text':
        case 'number':
          return f.value ? [{
            featureId: f.featureId,
            dataType: f.dataType,
            value: String(f.value),
            optionId: null
          }] : [];
        default:
          return [];
      }
    });

    const payload = { ...raw, features };

    this.propertyService.createProperty(payload).subscribe({
      next: () => {
        this.notificationService.success('✅ Property posted successfully');
        this.propertyForm.reset();
        this.router.navigate(['/my-properties'])
      },
      error: (err) => {
        this.notificationService.error('❌ Error: ' + (err.error?.message || err.message));
      }
    });
  }
}
