import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivatedRoute } from '@angular/router';

import { PropertyList } from './property-list';
import { PropertyService } from '../../core/services/property.service';
import { FeatureService } from '../../core/services/feature.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { PropertyModel } from '../../models/property.model';
import { PaginationInfo } from '../../models/pagination.model';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';

describe('PropertyList', () => {
  let component: PropertyList;
  let fixture: ComponentFixture<PropertyList>;
  let mockPropertyService: jasmine.SpyObj<PropertyService>;
  let mockFeatureService: jasmine.SpyObj<FeatureService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;

  const properties: PropertyModel[] = [{
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

  const pagination: PaginationInfo = {
    currentPage: 1,
    totalPages: 1,
    pageSize: 5,
    totalItems: 1
  };

  const dynamicFilters: DynamicFeatureModel[] = [{
    id: 'f1',
    name: 'Pet Friendly',
    filterMode: 'Boolean',
    dataType: 'boolean',
    options: []
  }];

  beforeEach(async () => {
    mockPropertyService = jasmine.createSpyObj<PropertyService>('PropertyService', ['advancedSearch']);
    mockFeatureService = jasmine.createSpyObj('FeatureService', ['getApplicableFeatures']);
    mockAuthService = jasmine.createSpyObj('AuthService', [], { currentUser: null });
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['warning']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, PropertyList],
      providers: [
        { provide: PropertyService, useValue: mockPropertyService },
        { provide: FeatureService, useValue: mockFeatureService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: NotificationService, useValue: mockNotificationService },
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({
              purpose: 'Buy',
              city: 'Chennai',
              locality: 'Anna Nagar',
              keyword: '2 BHK'
            })
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PropertyList);
    component = fixture.componentInstance;
  });

  it('should load properties and filters on init', fakeAsync(() => {
    mockPropertyService.advancedSearch.and.returnValue(of({ data: { items: properties, pagination } }) as any);
    mockFeatureService.getApplicableFeatures.and.returnValue(of({ data: dynamicFilters }));

    fixture.detectChanges();
    tick();

    expect(component.properties.length).toBe(1);
    expect(component.dynamicFilters.length).toBe(1);
    expect(mockPropertyService.advancedSearch).toHaveBeenCalled();
    expect(mockFeatureService.getApplicableFeatures).toHaveBeenCalled();
  }));

  it('should call advancedSearch on page change', () => {
    mockPropertyService.advancedSearch.and.returnValue(of({ data: { items: [], pagination: component.pagination } }) as any);
    component.onPageChange(2);
    expect(mockPropertyService.advancedSearch).toHaveBeenCalled();
  });

  it('should reload filters when onCoreFiltersChanged is triggered', () => {
    const filters = {
        listingPurpose: 'Sale' as 'Sale',
        propertyTypes: ['Apartment']
    };

    mockPropertyService.advancedSearch.and.returnValue(of({
        data: {
        items: [],
        pagination
        }
    }) as any);

    mockFeatureService.getApplicableFeatures.and.returnValue(of({
        data: dynamicFilters
    }));

    component.onCoreFiltersChanged(filters);

    expect(mockPropertyService.advancedSearch).toHaveBeenCalled();
    expect(mockFeatureService.getApplicableFeatures).toHaveBeenCalled();
    });


  it('should reload properties when onDynamicFiltersChanged is triggered', () => {
    mockPropertyService.advancedSearch.and.returnValue(of({ data: { items: [], pagination: component.pagination } }) as any);

    component.dynamicFilters = [{
      id: 'f1',
      name: 'Pet Friendly',
      filterMode: 'Boolean',
      dataType: 'boolean',
      options: []
    }];

    component.onDynamicFiltersChanged({ f1: ['true'] });
    expect(mockPropertyService.advancedSearch).toHaveBeenCalled();
  });

  it('should set selectedProperty on contactClicked when user is buyer', () => {
    (Object.getOwnPropertyDescriptor(mockAuthService, 'currentUser')?.get as jasmine.Spy)
      .and.returnValue({ id: 'u1', role: 'Buyer' });

    const property = { id: 'p1', title: 'Flat', location: 'City' };
    component.handleContactClicked(property);
    expect(component.selectedPropertyForContact).toEqual(property);
  });

  it('should redirect unauthenticated users on contactClicked', () => {
    (Object.getOwnPropertyDescriptor(mockAuthService, 'currentUser')?.get as jasmine.Spy)
      .and.returnValue(null);

    component.handleContactClicked({ id: 'p1', title: 'Flat', location: 'City' });
    expect(mockNotificationService.warning).toHaveBeenCalledWith('Please log in to contact the lister');
  });

  it('should restrict non-buyer users on contactClicked', () => {
    (Object.getOwnPropertyDescriptor(mockAuthService, 'currentUser')?.get as jasmine.Spy)
      .and.returnValue({ id: 'u2', role: 'Lister' });

    component.handleContactClicked({ id: 'p1', title: 'Flat', location: 'City' });
    expect(mockNotificationService.warning).toHaveBeenCalledWith('Only buyers can contact listers. Please login as buyer to continue.');
  });

});
