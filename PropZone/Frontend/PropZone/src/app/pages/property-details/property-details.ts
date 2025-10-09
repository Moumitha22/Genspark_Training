import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PropertyService } from '../../core/services/property.service';
import { PropertyModel } from '../../models/property.model';
import { CommonModule } from '@angular/common';
import { ContactListerFormComponent } from '../../components/contact-lister-form/contact-lister-form';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { environment } from '../../environments/environment';
import L from 'leaflet';


@Component({
  selector: 'app-property-details',
  standalone: true,
  imports: [CommonModule, ContactListerFormComponent, RouterLink],
  templateUrl: './property-details.html',
  styleUrls: ['./property-details.css']
})
export class PropertyDetailsComponent implements OnInit {
  property!: PropertyModel;
  currentIndex = 0;
  propertyData: { id: string; title: string; location: string } | null = null;
  selectedProperty: { id: string; title: string; location: string } | null = null;
  apiBaseUrl = environment.apiBaseUrl;

  private leafletMap: L.Map | null = null;

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

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private propertyService = inject(PropertyService);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.propertyService.getPropertyById(id).subscribe({
        next: (res) => {
          this.property = res.data;

          this.propertyData = {
            id: res.data.id,
            title: res.data.title,
            location: `${res.data.location.locality}, ${res.data.location.city}, ${res.data.location.state}`
          };

          setTimeout(() => {
            const mapContainer = document.getElementById('leaflet-map');
            if (mapContainer) {
              this.initMap();
            } else {
              console.warn('Map container not found');
            }
          }, 100); 
        },
        error: (err) => console.error('Failed to fetch property:', err)
      });
    }
  }

  initMap(): void {
    const lat = this.property?.location?.latitude;
    const lng = this.property?.location?.longitude;

    if (!lat || !lng) return;

    if (this.leafletMap) {
      this.leafletMap.setView([lat, lng], 15);
      return;
    }

    this.leafletMap = L.map('leaflet-map', {
      center: [lat, lng],
      zoom: 15
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.leafletMap);

    
    const customIcon = L.divIcon({
      html: '<i class="fa fa-location-dot fa-2x text-primary"></i>',
      className: '', 
      iconAnchor: [10, 20],
      popupAnchor: [0, -20]  
    });

    L.marker([lat, lng], { icon: customIcon })
      .addTo(this.leafletMap)
      .bindPopup(`
          <strong>${this.property.title}</strong>
      `)
      .openPopup();


    setTimeout(() => {
        this.leafletMap?.invalidateSize();
      }, 0);
    }


  get images(): string[] {
    if (!this.property?.imageUrls?.length) return [];

    return this.property.imageUrls.map(url =>
      url.startsWith('http') ? url : `${this.apiBaseUrl}/${url}`
    );
  }

  nextImage() {
    if (this.currentIndex < this.images.length - 1) {
      this.currentIndex++;
    }
  }

  prevImage() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
    }
  }

  @ViewChild('locationSection') locationSection!: ElementRef;

  scrollToLocation() {
    if (this.locationSection) {
      this.locationSection.nativeElement.scrollIntoView({ behavior: 'smooth' });
    }
  }

  canEditProperty(): boolean {
    const user = this.authService.currentUser;
    if (!user) return false;
    return user.role === 'Admin' || (user.role === 'Lister' && user.id === this.property.listerId);
  }

  goToEdit(): void {
    this.router.navigate(['/my-properties', this.property.id, 'edit']);
  }

  handleEnquiriesClicked(): void {
    this.router.navigate(['/property', this.property.id, 'inquiries']);
  }

  handleContactClicked(): void {
    const user = this.authService.currentUser;

    if (!user) {
      this.notificationService.warning('Please log in to contact the lister');
      this.router.navigate(['/login']);
      return;
    }

    if (user.role !== 'Buyer') {
      this.notificationService.warning('Only buyers can contact listers. Please login as a buyer to continue.');
      this.router.navigate(['/login']);
      return;
    }

    this.selectedProperty = this.propertyData;
  }
}
