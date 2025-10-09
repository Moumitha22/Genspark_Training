import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AdminPropertyListComponent } from './admin-properties-list';
import { of, throwError } from 'rxjs';
import { PropertyService } from '../../core/services/property.service';
import { NotificationService } from '../../core/services/notification.service';
import { PropertyModel } from '../../models/property.model';
import { RouterTestingModule } from '@angular/router/testing';

describe('AdminPropertyListComponent', () => {
  let component: AdminPropertyListComponent;
  let fixture: ComponentFixture<AdminPropertyListComponent>;
  let mockPropertyService: jasmine.SpyObj<PropertyService>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;

  const mockProperties: PropertyModel[] = [{
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
  }];

  beforeEach(async () => {
    mockPropertyService = jasmine.createSpyObj('PropertyService', ['getAllProperties', 'deleteProperty', 'advancedSearch']);
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['success', 'error']);

    await TestBed.configureTestingModule({
      imports: [AdminPropertyListComponent, RouterTestingModule.withRoutes([])],
      providers: [
        { provide: PropertyService, useValue: mockPropertyService },
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminPropertyListComponent);
    component = fixture.componentInstance;
  });

  it('should load all properties on init', fakeAsync(() => {
    mockPropertyService.advancedSearch.and.returnValue(of({
      data: { items: mockProperties }
    } as any)); 

    component.ngOnInit();
    tick();

    expect(component.loading).toBeFalse();
    expect(component.properties.length).toBe(1);
    expect(component.properties[0].title).toBe('Spacious Apartment');
  }));


    
  it('should delete property and show success', fakeAsync(() => {
    spyOn(window, 'confirm').and.returnValue(true);
    component.properties = [...mockProperties];
    mockPropertyService.deleteProperty.and.returnValue(of({}));

    component.deleteProperty('123');
    tick();

    expect(mockPropertyService.deleteProperty).toHaveBeenCalledWith('123');
    expect(component.properties.length).toBe(0);
    expect(mockNotificationService.success).toHaveBeenCalledWith('Property deleted successfully');
  }));

  it('should not delete property if confirmation is cancelled', () => {
    spyOn(window, 'confirm').and.returnValue(false);
    component.properties = [...mockProperties];

    component.deleteProperty('123');

    expect(mockPropertyService.deleteProperty).not.toHaveBeenCalled();
    expect(component.properties.length).toBe(1); 
  });

  it('should show error if property deletion fails', fakeAsync(() => {
    spyOn(window, 'confirm').and.returnValue(true);
    component.properties = [...mockProperties];
    mockPropertyService.deleteProperty.and.returnValue(throwError(() => new Error('Delete error')));

    component.deleteProperty('123');
    tick();

    expect(mockNotificationService.error).toHaveBeenCalledWith('Failed to delete property');
  }));
});
