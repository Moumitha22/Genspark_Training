import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PropertyCard } from './property-card';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';
import { PropertyModel } from '../../models/property.model';
import { DebugElement } from '@angular/core';

describe('PropertyCard', () => {
  let component: PropertyCard;
  let fixture: ComponentFixture<PropertyCard>;

  const mockProperty: PropertyModel = {
    id: '123',
    title: 'Spacious Apartment',
    price: 5000000,
    areaSqFt: 1200,
    propertyType: 'Apartment',
    listingPurpose: 'Sale',
    listerType: 'Owner',
    listerId: 'lister-456', 
    createdAt: new Date().toISOString(), 
    status: 'Available',

    imageUrls: [],
    location: {
        city: 'Chennai',
        state: 'Tamil Nadu',
        locality: 'Adyar'
    },
    featureSummary: [
        { featureId: 'f1', featureName: 'BHK', values: ['2'] },
        { featureId: 'f2', featureName: 'Bathrooms', values: ['2'] },
        { featureId: 'f3', featureName: 'Furnishing', values: ['Fully Furnished'] },
        { featureId: 'f4', featureName: 'Power Backup', values: ['Yes'] },
        { featureId: 'f5', featureName: 'Gym', values: ['Yes'] },
        { featureId: 'f6', featureName: 'Lift', values: ['Yes'] },
        { featureId: 'f7', featureName: 'Amenities', values: ['Club House', 'Kids Play Area'] }
    ]
    };


  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PropertyCard, RouterTestingModule]
    }).compileComponents();

    fixture = TestBed.createComponent(PropertyCard);
    component = fixture.componentInstance;
    component.property = mockProperty;
    component.context = 'public'; 
    fixture.detectChanges();
  });

  it('should display the property title and location', () => {
    const title = fixture.debugElement.query(By.css('h5')).nativeElement;
    const location = fixture.debugElement.query(By.css('p.text-muted')).nativeElement;

    expect(title.textContent).toContain('Spacious Apartment');
    expect(location.textContent).toContain('Adyar, Chennai, Tamil Nadu');
  });

  it('should display top 6 features excluding amenities', () => {
    const features = fixture.debugElement.queryAll(By.css('.feature-box'));
    expect(features.length).toBeLessThanOrEqual(6);
    features.forEach(f => {
      expect(f.nativeElement.textContent).not.toContain('Amenities');
    });
  });

  it('should display amenities if present', () => {
    const amenitiesEl = fixture.debugElement.query(By.css('.row-3'));
    expect(amenitiesEl.nativeElement.textContent).toContain('Amenities');
    expect(amenitiesEl.nativeElement.textContent).toContain('Club House, Kids Play Area');
  });

  it('should emit contactClicked when Contact button is clicked', () => {
    spyOn(component.contactClicked, 'emit');

    const button: DebugElement = fixture.debugElement.query(By.css('.btn.btn-primary'));
    button.triggerEventHandler('click', null);

    expect(component.contactClicked.emit).toHaveBeenCalledWith({
      id: mockProperty.id,
      title: mockProperty.title,
      location: 'Adyar, Chennai, Tamil Nadu'
    });
  });

  it('should show lister-specific buttons when context is lister', () => {
    component.context = 'lister';
    fixture.detectChanges();

    const buttons = fixture.debugElement.queryAll(By.css('.responsive-btn'));
    const texts = buttons.map(btn => btn.nativeElement.textContent.trim());

    expect(texts).toContain('View Inquiries');
    expect(texts).toContain('View/Edit Details');
    expect(texts).toContain('Add Images');
  });

  it('should show public-specific buttons when context is public', () => {
    component.context = 'public';
    fixture.detectChanges();

    const buttons = fixture.debugElement.queryAll(By.css('.responsive-btn'));
    const texts = buttons.map(btn => btn.nativeElement.textContent.trim());

    expect(texts).toContain('View Details');
    expect(texts).toContain('Contact Owner');
  });
});
