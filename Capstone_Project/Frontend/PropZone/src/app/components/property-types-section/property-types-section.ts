import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-types-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './property-types-section.html',
  styleUrls: ['./property-types-section.css'],
})
export class PropertyTypesSectionComponent {
  private router = inject(Router);

  propertyTypes = [
    { name: 'House/Villa', type: 'House', imageUrl: 'https://i.pinimg.com/736x/b2/d8/4e/b2d84ea78340de300df9f235509ddb9e.jpg' },
    { name: 'Apartments', type: 'Apartment', imageUrl: 'https://i.pinimg.com/736x/4b/a5/2f/4ba52fa1b1e1d8ac298a1829e0d5e7ef.jpg' },
    { name: 'Plots', type: 'Plot', imageUrl: 'https://i.pinimg.com/736x/4a/f1/ca/4af1cafccad7215044e1d7d6453b6595.jpg' },
    { name: 'Commercial Space', type: 'CommercialSpace', imageUrl: 'https://i.pinimg.com/736x/87/1b/50/871b50b416c397d06266bb38edfb4b59.jpg' }
  ];

  navigateToProperties(type: string) {
    this.router.navigate(['/properties'], {
      queryParams: { propertyType: type }
    });
  }
}
