import {
  Component,
  EventEmitter,
  inject,
  OnInit,
  Output,
  OnDestroy,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormArray,
  Validators,
  ReactiveFormsModule,
  FormsModule,
  FormControl,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PropertyService } from '../../core/services/property.service';
import { PropertyFormStateService } from '../../core/services/property-form-state.service';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';
import { FeatureService } from '../../core/services/feature.service';
import { NotificationService } from '../../core/services/notification.service';
import { Router } from '@angular/router';
import L from 'leaflet';
import { NominatimService } from '../../core/services/nominatim.service';
import { Subject, debounceTime, takeUntil } from 'rxjs';
import { DiscountCode } from '../../models/discount-code.model';
import { DiscountCodeService } from '../../core/services/discount-code.service';
import { ActiveDiscountCodeFilter } from '../../models/active-discount-code-filter';
import { NgSelectModule } from '@ng-select/ng-select';
import { DiscountCodeDropDown } from '../discount-code-drop-down/discount-code-drop-down';

@Component({
  selector: 'app-post-property-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
    NgSelectModule,
    DiscountCodeDropDown,
  ],
  templateUrl: './post-property-form.html',
  styleUrls: ['./post-property-form.css'],
})
export class PostPropertyFormComponent implements OnInit, OnDestroy {
  @Output() back = new EventEmitter<void>();

  private propertyService = inject(PropertyService);
  private featureService = inject(FeatureService);
  private notificationService = inject(NotificationService);
  private stateService = inject(PropertyFormStateService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private nominatim = inject(NominatimService);
  private discountService = inject(DiscountCodeService);

  listerType!: string;
  listingPurpose!: string;
  propertyType!: string;
  leafletMap!: L.Map;
  private mapMarker: L.Marker | null = null;

  propertyForm!: FormGroup;
  dynamicFeatures: DynamicFeatureModel[] = [];
  activeDiscountCodes: DiscountCode[] = [];

  private suppressForwardGeocode = false;
  private reverseGeocodeSubject = new Subject<{ lat: number; lng: number }>();
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    const meta = this.stateService.getMetadata();
    this.listerType = meta.listerType || '';
    this.listingPurpose = meta.listingPurpose || '';
    this.propertyType = meta.propertyType || '';

    this.initForm();
    this.subscribeToLocationChanges();
    this.subscribeToReverseGeocode();
    this.loadDynamicFeatures();
    this.loadActiveDiscountCodes();

    setTimeout(() => this.initLeafletMap(), 300);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadActiveDiscountCodes() {
    const filter: ActiveDiscountCodeFilter = {
      typeOfProperty: this.propertyForm.get('propertyType')?.value,
      purposeOfListing: this.propertyForm.get('listingPurpose')?.value,
      price: this.propertyForm.get('price')?.value,
    };
    this.discountService.getActiveDiscount(filter).subscribe({
      next: (res) => {
        this.activeDiscountCodes = res;
      },
      error: (err) => {
        this.notificationService.error(
          'Error loading discount codes: ' + (err.error?.message || err.message)
        );
      },
    });
  }

  initForm(): void {
    this.propertyForm = this.fb.group({
      listerType: [this.listerType],
      listingPurpose: [this.listingPurpose],
      propertyType: [this.propertyType],
      title: ['', [Validators.required, Validators.maxLength(50)]],
      description: [''],
      price: [null, [Validators.required, Validators.min(1)]],
      areaSqFt: [null, [Validators.required, Validators.min(1)]],
      discountCodeIds: [],
      location: this.fb.group({
        locality: ['', Validators.required],
        city: ['', Validators.required],
        state: ['', Validators.required],
        latitude: [null],
        longitude: [null],
      }),
      features: this.fb.array([]),
    });
  }

  get discountCodeIdsControl() {
    return this.propertyForm.get('discountCodeIds') as FormControl;
  }

  onDiscountCodesChanged(event: string[]) {
    this.discountCodeIdsControl.setValue(event);
  }

  onPriceChange(): void {
    this.loadActiveDiscountCodes();
  }
  initLeafletMap(): void {
    this.leafletMap = L.map('locationPickerMap').setView(
      [11.0168, 76.9558],
      13
    ); // Default Coimbatore

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
    }).addTo(this.leafletMap);

    const faIcon = L.divIcon({
      html: '<i class="fa fa-location-dot fa-2x text-primary"></i>',
      className: '',
    });

    this.leafletMap.on('click', (e: L.LeafletMouseEvent) => {
      const { lat, lng } = e.latlng;

      if (this.mapMarker) {
        this.mapMarker.setLatLng(e.latlng);
      } else {
        this.mapMarker = L.marker(e.latlng, { icon: faIcon }).addTo(
          this.leafletMap
        );
      }

      this.propertyForm.get('location.latitude')?.setValue(lat);
      this.propertyForm.get('location.longitude')?.setValue(lng);

      this.reverseGeocodeSubject.next({ lat, lng });

      console.log('📍 Selected location:', lat, lng);
    });

    setTimeout(() => this.leafletMap?.invalidateSize(), 200);
  }

  subscribeToLocationChanges(): void {
    const locGroup = this.propertyForm.get('location');
    if (!locGroup) return;

    ['locality', 'city', 'state'].forEach((field) => {
      const control = locGroup.get(field);
      if (control) {
        control.valueChanges
          .pipe(debounceTime(600), takeUntil(this.destroy$))
          .subscribe((value) => {
            if (this.suppressForwardGeocode) {
              console.log(
                `[🛑 Skipped forward geocode due to reverse patch] ${field} = ${value}`
              );
              return;
            }
            console.log(`📥 ${field} changed:`, value);
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
            console.log('📍 Reverse geocoded address:', res);
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
            setTimeout(() => (this.suppressForwardGeocode = false), 1000); // Reset after patch
          },
          error: () => console.error('⚠️ Reverse geocoding failed'),
        });
      });
  }

  updateLatLngFromAddress(): void {
    console.log('📍 updateLatLngFromAddress() triggered');

    const loc = this.propertyForm.get('location')?.value;
    if (!loc.locality || !loc.city || !loc.state) {
      console.warn('⚠️ Incomplete address, skipping geocode', loc);
      return;
    }

    this.nominatim.geocode(loc.locality, loc.city, loc.state).subscribe({
      next: (results) => {
        if (results.length > 0) {
          const lat = parseFloat(results[0].lat);
          const lon = parseFloat(results[0].lon);
          console.log('📍 Geocoded:', lat, lon);

          this.propertyForm.get('location')?.patchValue({
            latitude: lat,
            longitude: lon,
          });

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
        } else {
          console.warn('No geocoding result');
        }
      },
      error: () => console.error('Nominatim request failed'),
    });
  }

  loadDynamicFeatures(): void {
    this.featureService
      .getApplicableFeatures(this.listingPurpose, this.propertyType)
      .subscribe((res) => {
        this.dynamicFeatures = res.data.map((f) => ({
          ...f,
          dataType: f.dataType?.toLowerCase(),
        }));

        const featuresArray = this.fb.array<FormGroup>([]);

        for (const feature of this.dynamicFeatures) {
          featuresArray.push(
            this.fb.group({
              featureId: [feature.id],
              dataType: [feature.dataType],
              value: [feature.dataType === 'boolean' ? false : ''],
              optionId: [''],
              selectedOptionIds: [[]],
            })
          );
        }

        this.propertyForm.setControl('features', featuresArray);
      });
  }

  get featuresFormArray(): FormArray {
    return this.propertyForm.get('features') as FormArray;
  }

  onMultiSelectCheckboxChange(
    event: Event,
    index: number,
    optionId: string
  ): void {
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

    const discountCodeIds = (raw.discountCodeIds || []).map((id: any) =>
      typeof id === 'object' && id.id ? id.id : id
    );

    const features = raw.features.flatMap((f: any) => {
      switch (f.dataType) {
        case 'multiselect':
          return f.selectedOptionIds.map((id: string) => ({
            featureId: f.featureId,
            dataType: 'multiselect',
            optionId: id,
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
              value: String(f.value),
              optionId: null,
            },
          ];
        case 'text':
        case 'number':
          return f.value
            ? [
                {
                  featureId: f.featureId,
                  dataType: f.dataType,
                  value: String(f.value),
                  optionId: null,
                },
              ]
            : [];
        default:
          return [];
      }
    });

    const payload = { ...raw, features, discountCodeIds };

    this.propertyService.createProperty(payload).subscribe({
      next: () => {
        this.notificationService.success('✅ Property posted successfully');
        this.propertyForm.reset();
        this.router.navigate(['/my-properties']);
      },
      error: (err) => {
        this.notificationService.error(
          '❌ Error: ' + (err.error?.message || err.message)
        );
        console.error('Error posting property:', err);
      },
    });
  }
}
