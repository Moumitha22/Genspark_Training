import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormArray,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  FormControl,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyService } from '../../core/services/property.service';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { FeatureService } from '../../core/services/feature.service';
import L from 'leaflet';
import { NominatimService } from '../../core/services/nominatim.service';
import { Subject, debounceTime, takeUntil } from 'rxjs';
import { DiscountCode } from '../../models/discount-code.model';
import { DiscountCodeService } from '../../core/services/discount-code.service';
import { ActiveDiscountCodeFilter } from '../../models/active-discount-code-filter';
import { NgSelectModule } from '@ng-select/ng-select';
import { DiscountCodeDropDown } from '../../components/discount-code-drop-down/discount-code-drop-down';

@Component({
  selector: 'app-property-edit',
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    NgSelectModule,
    DiscountCodeDropDown,
  ],
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
  private nominatim = inject(NominatimService);
  private discountService = inject(DiscountCodeService);

  propertyForm!: FormGroup;
  dynamicFeatures: DynamicFeatureModel[] = [];
  mode: 'view' | 'edit' = 'edit';
  propertyId!: string;
  leafletMap!: L.Map;
  private mapMarker: L.Marker | null = null;
  private reverseGeocodeSubject = new Subject<{ lat: number; lng: number }>();
  private suppressForwardGeocode = false;
  private destroy$ = new Subject<void>();
  discountCodes: DiscountCode[] = [];

  ngOnInit(): void {
    this.propertyId = this.route.snapshot.paramMap.get('id')!;
    this.loadProperty();
    this.subscribeToReverseGeocode();
  }

  loadProperty(): void {
    this.propertyService.getPropertyById(this.propertyId).subscribe({
      next: (res) => {
        console.log('Fetched property data:', res.data);
        this.initForm(res.data);
        this.loadAllDiscountCodes();
        this.subscribeToLocationChanges();
        this.loadDynamicFeatures(
          res.data.listingPurpose,
          res.data.propertyType,
          res.data.featureSummary
        );
        this.setupFeatureReloadWatchers();
      },
      error: (err) => console.error('Failed to fetch property:', err),
    });
  }

  loadAllDiscountCodes() {
    const filter: ActiveDiscountCodeFilter = {
      typeOfProperty: this.propertyForm.get('propertyType')?.value,
      purposeOfListing: this.propertyForm.get('listingPurpose')?.value,
      price: this.propertyForm.get('price')?.value,
    };

    this.discountService.getActiveDiscount(filter).subscribe({
      next: (res) => {
        this.discountCodes = res;
      },
      error: (err) => {
        this.notificationService.error(
          'Error loading discount codes: ' + (err.error?.message || err.message)
        );
      },
    });
  }

  onPriceChange(): void {
    this.loadAllDiscountCodes();
  }

  initForm(data: any): void {
    const discountCodeIds = (data.discountCodes || []).map((dc: any) => dc.id);

    this.propertyForm = this.fb.group({
      listerType: [
        { value: data.listerType, disabled: this.mode === 'view' },
        Validators.required,
      ],
      listingPurpose: [
        { value: data.listingPurpose, disabled: this.mode === 'view' },
        Validators.required,
      ],
      propertyType: [
        { value: data.propertyType, disabled: this.mode === 'view' },
        Validators.required,
      ],
      status: [
        { value: data.status, disabled: this.mode === 'view' },
        Validators.required,
      ],
      title: [
        { value: data.title, disabled: this.mode === 'view' },
        [Validators.required, Validators.maxLength(150)],
      ],
      description: [
        { value: data.description, disabled: this.mode === 'view' },
      ],
      price: [
        { value: data.price, disabled: this.mode === 'view' },
        [Validators.required, Validators.min(1)],
      ],
      areaSqFt: [
        { value: data.areaSqFt, disabled: this.mode === 'view' },
        [Validators.required, Validators.min(1)],
      ],
      discountCodeIds: [discountCodeIds],
      location: this.fb.group({
        locality: [
          { value: data.location.locality, disabled: this.mode === 'view' },
          Validators.required,
        ],
        city: [
          { value: data.location.city, disabled: this.mode === 'view' },
          Validators.required,
        ],
        state: [{ value: data.location.state, disabled: this.mode === 'view' }],
        latitude: [
          { value: data.location.latitude, disabled: this.mode === 'view' },
        ],
        longitude: [
          { value: data.location.longitude, disabled: this.mode === 'view' },
        ],
      }),
      features: this.fb.array([]),
    });
  }

  get discountCodeIdsControl() {
    return this.propertyForm.get('discountCodeIds') as FormControl;
  }

  onDiscountCodesChanged(selectedCodes: string[]) {
    this.discountCodeIdsControl.setValue(selectedCodes);
  }

  loadDynamicFeatures(
    purpose: string,
    type: string,
    existingFeatures: any[]
  ): void {
    this.featureService
      .getApplicableFeatures(purpose, type)
      .subscribe((res) => {
        this.dynamicFeatures = res.data.map((f: any) => ({
          ...f,
          dataType: f.dataType?.toLowerCase(),
        }));

        const featuresArray = this.fb.array<FormGroup>([]);

        for (let feature of this.dynamicFeatures) {
          const existing = existingFeatures.find(
            (ef: any) => ef.featureId === feature.id
          );

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
            selectedOptionIds: [
              { value: selectedOptionIds, disabled: this.mode === 'view' },
            ],
          });

          featuresArray.push(group);
        }

        this.propertyForm.setControl('features', featuresArray);
      });
    setTimeout(() => this.initLeafletMap(), 300);
  }

  private setupFeatureReloadWatchers(): void {
    this.propertyForm
      .get('listingPurpose')
      ?.valueChanges.pipe(takeUntil(this.destroy$))
      .subscribe((purpose) => {
        const type = this.propertyForm.get('propertyType')?.value;
        if (
          confirm(
            'Changing listing purpose will reset selected features. Continue?'
          )
        ) {
          this.loadDynamicFeatures(purpose, type, []);
        }
      });

    this.propertyForm
      .get('propertyType')
      ?.valueChanges.pipe(takeUntil(this.destroy$))
      .subscribe((type) => {
        const purpose = this.propertyForm.get('listingPurpose')?.value;
        if (
          confirm(
            'Changing property type will reset selected features. Continue?'
          )
        ) {
          this.loadDynamicFeatures(purpose, type, []);
        }
      });
  }

  get featuresFormArray(): FormArray {
    return this.propertyForm.get('features') as FormArray;
  }

  subscribeToLocationChanges(): void {
    const locGroup = this.propertyForm.get('location');
    if (!locGroup) return;

    ['locality', 'city', 'state'].forEach((field) => {
      const control = locGroup.get(field);
      if (control) {
        control.valueChanges
          .pipe(debounceTime(600), takeUntil(this.destroy$))
          .subscribe(() => {
            if (this.suppressForwardGeocode) return;
            this.updateLatLngFromAddress();
          });
      }
    });
  }

  subscribeToReverseGeocode(): void {
    this.reverseGeocodeSubject
      .pipe(debounceTime(600), takeUntil(this.destroy$))
      .subscribe(({ lat, lng }) => {
        this.nominatim.reverseGeocode(lat, lng).subscribe({
          next: (res) => {
            const address = res.address || {};
            const locality =
              address.suburb ||
              address.village ||
              address.hamlet ||
              address.neighbourhood ||
              '';
            const city =
              address.city ||
              address.town ||
              address.municipality ||
              address.county ||
              '';
            const state = address.state || '';

            this.suppressForwardGeocode = true;
            this.propertyForm
              .get('location')
              ?.patchValue({ locality, city, state });
            setTimeout(() => (this.suppressForwardGeocode = false), 1000);
          },
          error: () => console.error('⚠️ Reverse geocoding failed'),
        });
      });
  }

  updateLatLngFromAddress(): void {
    const loc = this.propertyForm.get('location')?.value;
    if (!loc.locality || !loc.city || !loc.state) return;

    this.nominatim.geocode(loc.locality, loc.city, loc.state).subscribe({
      next: (results) => {
        if (results.length > 0) {
          const lat = parseFloat(results[0].lat);
          const lon = parseFloat(results[0].lon);

          this.propertyForm
            .get('location')
            ?.patchValue({ latitude: lat, longitude: lon });

          if (this.leafletMap) {
            this.leafletMap.setView([lat, lon], 15);

            const faIcon = L.divIcon({
              html: '<i class="fa fa-location-dot fa-2x text-primary"></i>',
              className: '',
              iconAnchor: [10, 20],
            });

            if (this.mapMarker) {
              this.mapMarker.setLatLng([lat, lon]);
            } else {
              this.mapMarker = L.marker([lat, lon], { icon: faIcon }).addTo(
                this.leafletMap
              );
            }
          }
        }
      },
      error: () => console.error('Nominatim request failed'),
    });
  }

  initLeafletMap(): void {
    const loc = this.propertyForm.get('location')?.value;
    const lat = loc?.latitude || 11.0168;
    const lng = loc?.longitude || 76.9558;

    this.leafletMap = L.map('locationPickerMap').setView([lat, lng], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
    }).addTo(this.leafletMap);

    const faIcon = L.divIcon({
      html: '<i class="fa fa-location-dot fa-2x text-primary"></i>',
      className: '',
      iconAnchor: [10, 20],
    });

    this.mapMarker = L.marker([lat, lng], { icon: faIcon }).addTo(
      this.leafletMap
    );

    this.leafletMap.on('click', (e: L.LeafletMouseEvent) => {
      const { lat, lng } = e.latlng;
      this.propertyForm.get('location.latitude')?.setValue(lat);
      this.propertyForm.get('location.longitude')?.setValue(lng);

      if (this.mapMarker) {
        this.mapMarker.setLatLng(e.latlng);
      } else {
        this.mapMarker = L.marker(e.latlng, { icon: faIcon }).addTo(
          this.leafletMap
        );
      }

      this.reverseGeocodeSubject.next({ lat, lng });
    });

    setTimeout(() => this.leafletMap?.invalidateSize(), 200);
  }

  onMultiSelectCheckboxChange(
    event: Event,
    index: number,
    optionId: string
  ): void {
    const input = event.target as HTMLInputElement;
    const isChecked = input.checked;
    const control = this.featuresFormArray.at(index).get('selectedOptionIds')!;
    let currentValues: string[] = control.value || [];

    if (isChecked) {
      currentValues.push(optionId);
    } else {
      currentValues = currentValues.filter((id) => id !== optionId);
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
    const discountCodeIds = (raw.discountCodeIds || []).map((id: any) =>
      typeof id === 'object' && id.id ? id.id : id
    );
    console.log('Submitting property with discount codes:', discountCodeIds);
    const features = raw.features.flatMap((f: any) => {
      switch (f.dataType) {
        case 'multiselect':
          return f.selectedOptionIds.map((optionId: string) => ({
            featureId: f.featureId,
            dataType: 'multiselect',
            optionId,
            value: '',
          }));
        case 'dropdown':
          return f.optionId
            ? [
                {
                  featureId: f.featureId,
                  dataType: 'dropdown',
                  optionId: f.optionId,
                  value: '',
                },
              ]
            : [];
        case 'boolean':
          return [
            {
              featureId: f.featureId,
              dataType: 'boolean',
              optionId: null,
              value: String(f.value),
            },
          ];
        case 'text':
          return f.value?.trim?.()
            ? [
                {
                  featureId: f.featureId,
                  dataType: 'text',
                  value: f.value.trim(),
                  optionId: null,
                },
              ]
            : [];

        case 'number':
          return f.value !== null && f.value !== '' && !isNaN(Number(f.value))
            ? [
                {
                  featureId: f.featureId,
                  dataType: 'number',
                  value: String(f.value),
                  optionId: null,
                },
              ]
            : [];

        default:
          return [];
      }
    });

    const payload = {
      ...raw,
      id: this.propertyId,
      features,
      discountCodeIds,
    };

    this.propertyService.updateProperty(this.propertyId, payload).subscribe({
      next: () => {
        this.notificationService.success('Property updated successfully');
        this.router.navigate(['/property', this.propertyId]);
      },
      error: (err) => {
        this.notificationService.error(
          'Update failed: ' + (err.error?.message || err.message)
        ),
          console.error('Update error:', err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/property', this.propertyId]);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
