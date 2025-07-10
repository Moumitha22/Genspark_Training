import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyService } from '../../core/services/property.service';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { FeatureService } from '../../core/services/feature.service';

@Component({
  selector: 'app-property-edit',
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './property-edit.html',
  styleUrls: ['./property-edit.css'],
})
export class PropertyEditComponent implements OnInit {

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private propertyService = inject(PropertyService);
  private featureService = inject(FeatureService);
  private notificationService = inject(NotificationService);

  propertyForm!: FormGroup;
  dynamicFeatures: DynamicFeatureModel[] = [];
  mode: 'view' | 'edit' = 'edit';
  propertyId!: string;

  ngOnInit(): void {
    this.propertyId = this.route.snapshot.paramMap.get('id')!;
    this.loadProperty();
  }

  loadProperty(): void {
    this.propertyService.getPropertyById(this.propertyId).subscribe
      ({
        next: (res) => {
          this.initForm(res.data);
          this.loadDynamicFeatures(res.data.listingPurpose, res.data.propertyType, res.data.featureSummary);
        },
        error: (err) => console.error('Failed to fetch property:', err)
      });
  }

  initForm(data: any): void {
    this.propertyForm = this.fb.group({
      listerType: [{ value: data.listerType, disabled: this.mode === 'view' }, Validators.required],
      listingPurpose: [{ value: data.listingPurpose, disabled: this.mode === 'view' }, Validators.required],
      propertyType: [{ value: data.propertyType, disabled: this.mode === 'view' }, Validators.required],
      status: [{ value: data.status, disabled: this.mode === 'view' }, Validators.required],
      title: [{ value: data.title, disabled: this.mode === 'view' }, [Validators.required, Validators.maxLength(150)]],
      description: [{ value: data.description, disabled: this.mode === 'view' }],
      price: [{ value: data.price, disabled: this.mode === 'view' }, [Validators.required, Validators.min(1)]],
      areaSqFt: [{ value: data.areaSqFt, disabled: this.mode === 'view' }, [Validators.required, Validators.min(1)]],
      location: this.fb.group({
        locality: [{ value: data.location.locality, disabled: this.mode === 'view' }, Validators.required],
        city: [{ value: data.location.city, disabled: this.mode === 'view' }, Validators.required],
        state: [{ value: data.location.state, disabled: this.mode === 'view' }],
      }),
      features: this.fb.array([]),
    });
  }

  loadDynamicFeatures(purpose: string, type: string, existingFeatures: any[]): void {
  this.featureService.getApplicableFeatures(purpose, type).subscribe((res) => {
    this.dynamicFeatures = res.data.map((f: any) => ({
      ...f,
      dataType: f.dataType?.toLowerCase(),
    }));

    const featuresArray = this.fb.array<FormGroup>([]);

for (let feature of this.dynamicFeatures) {
  const existing = existingFeatures.find((ef: any) => ef.featureId === feature.id);

  let value: string | boolean = '';
  let optionId: string = '';
  let selectedOptionIds: string[] = [];

  switch (feature.dataType) {
    case 'boolean':
      value = existing?.values?.[0] === 'true';
      break;

    case 'text':
    case 'number':
      value = existing?.values?.[0] || '';
      break;

    case 'dropdown':
      const dropdownMap: Record<string, string> = {};
      for (let opt of feature.options || []) {
        dropdownMap[opt.value] = opt.id;
      }
      optionId = dropdownMap[existing?.values?.[0] || ''] || '';
      break;


    case 'multiselect':
      const optionMap: Record<string, string> = {};
      for (let opt of feature.options || []) {
        optionMap[opt.value] = opt.id;
      }

      selectedOptionIds = (existing?.values || [])
        .map((v: string) => optionMap[v])
        .filter((id: string | undefined) => !!id);
      break;
  }

  const group = this.fb.group({
    featureId: [feature.id],
    dataType: [feature.dataType],
    value: [{ value, disabled: this.mode === 'view' }],
    optionId: [{ value: optionId, disabled: this.mode === 'view' }],
    selectedOptionIds: [{ value: selectedOptionIds, disabled: this.mode === 'view' }]
  });

  featuresArray.push(group);
}


    this.propertyForm.setControl('features', featuresArray);
  });
}


  get featuresFormArray(): FormArray {
    return this.propertyForm.get('features') as FormArray;
  }

  onMultiSelectCheckboxChange(event: Event, index: number, optionId: string): void {
    const input = event.target as HTMLInputElement;
    const isChecked = input.checked;
    const control = this.featuresFormArray.at(index).get('selectedOptionIds')!;
    let currentValues: string[] = control.value || [];

    if (isChecked) {
      currentValues.push(optionId);
    } else {
      currentValues = currentValues.filter(id => id !== optionId);
    }

    control.setValue(currentValues);
    control.markAsDirty();
  }

  onSubmit(): void {
    if (this.propertyForm.invalid) {
      this.propertyForm.markAllAsTouched();
      return;
    }

    const raw = this.propertyForm.getRawValue();

    const features = raw.features.flatMap((f: any) => {
      switch (f.dataType) {
        case 'multiselect':
          return f.selectedOptionIds.map((optionId: string) => ({
            featureId: f.featureId,
            dataType: 'multiselect',
            optionId,
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
          return [{
            featureId: f.featureId,
            dataType: 'boolean',
            optionId: null,
            value: String(f.value)
          }];
        case 'text':
        case 'number':
          return [{
            featureId: f.featureId,
            dataType: f.dataType,
            optionId: null,
            value: String(f.value)
          }];
        default:
          return [];
      }
    });

    const payload = {
      ...raw,
      id: this.propertyId,
      features
    };

    this.propertyService.updateProperty(this.propertyId,payload).subscribe({
      next: () => {
        this.notificationService.success('Property updated successfully');
        this.router.navigate(['/property', this.propertyId]);
      },
      error: (err) => this.notificationService.error('Update failed: ' + (err.error?.message || err.message))
    });
  }
}
