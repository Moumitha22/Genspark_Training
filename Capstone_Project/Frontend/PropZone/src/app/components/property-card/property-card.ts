import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PropertyModel } from '../../models/property.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-property-card',
  imports: [CommonModule, RouterLink],
  templateUrl: './property-card.html',
  styleUrl: './property-card.css'
})
export class PropertyCard {
  @Input() context: 'public' | 'lister' = 'public';
  @Input() property!: PropertyModel;
  @Input() selected: boolean = false;
  @Output() compareClicked = new EventEmitter<PropertyModel>();

  apiBaseUrl = 'http://localhost:5138';

  @Output() contactClicked = new EventEmitter<{ id: string; title: string, location: string }>();


  iconMap: Record<string, string> = {
    'Amenities': 'fa-star',
    'BHK': 'fa-bed',
    'Bathrooms': 'fa-toilet',
    'Furnishing': 'fa-couch',
    'Price': 'fa-money-bill',
    'Facing': 'fa-compass',
    'Road Facing': 'fa-compass',
    'Gated Community': 'fa-door-closed',
    'Parking Availability': 'fa-square-parking',
    'Pet Friendly': 'fa-dog',
    'Water Supply Type': 'fa-tint',
    'Tenants Preferred': 'fa-users',
    'Property Age (Years)': 'fa-calendar',
    'Location': 'fa-location-dot',
    'Ownership': 'fa-user-tie',
    'Is Negotiable': 'fa-handshake',
    'Deposit': 'fa-wallet',
    'EMI Available': 'fa-wallet',
    'Square Feet': 'fa-ruler-combined',
    'Property Type': 'fa-building',
    'Floor Number': 'fa-building',
    'Available': 'fa-circle-check',
    'Sold': 'fa-circle-xmark',
    'Rented': 'fa-circle-xmark',
  };

  getFeatureIcon(featureName: string): string {
    const icons: { [key: string]: string } = {
      'Lift': 'bi bi-arrow-up',
      'Park': 'bi bi-tree',
      'Gym': 'bi bi-heart-pulse',
      'Power Backup': 'bi bi-lightning-charge',
      'Swimming Pool': 'bi bi-water',
      'Wi-Fi': 'bi bi-wifi',
      'AC': 'bi bi-snow',
      'Piped Gas': 'bi bi-fire',
      'Vastu Compliance': 'bi bi-compass',
      'Parking Availability': 'bi bi-car-front',
      'Security': 'bi bi-shield-lock',
      'Furnishing': 'bi bi-house-gear',
      'Facing': 'bi bi-compass-fill',
      'Amenities': 'bi bi-stars',
      'isEMIAvailable': 'bi bi-cash-coin',
      'Gated Community': 'bi bi-house-lock',
      'Tenants Preferred': 'bi bi-people',
      'DTCP Approved': 'bi bi-patch-check',
      'Water Supply Type': 'bi bi-droplet',
      'Road Facing': 'bi bi-signpost',
      'Bathrooms': 'bi bi-droplet-half',
      'BHK': 'bi bi-door-closed',
      'Floor Number': 'bi bi-building',
      'Property Age (Years)': 'bi bi-building-lock',
      'Pet Friendly': 'bi bi-paw',
    };

    return icons[featureName] || 'bi bi-check-circle';
  }

  formatFeatureValues(values: any[]): string {
    if (!values || values.length === 0) return '—';

    if (values.length === 1) {
      const v = values[0];
      return v === 'true' ? 'Yes' : v === 'false' ? 'No' : v;
    }

    return values
      .map(v => v === 'true' ? 'Yes' : v === 'false' ? 'No' : v)
      .join(', ');
  }


  get topFeaturesExcludingAmenities() {
    return this.property.featureSummary
      ?.filter(f => f.featureName !== 'Amenities')
      .slice(0, 6) ?? [];
  }

  get amenitiesFeature() {
    return this.property.featureSummary
      ?.find(f => f.featureName === 'Amenities');
  }

  onContactClick() {
    this.contactClicked.emit({ id: this.property.id, title: this.property.title , location: `${this.property.location.locality}, ${this.property.location.city}, ${this.property.location.state}`});
  }

}
