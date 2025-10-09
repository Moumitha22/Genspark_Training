import { ComponentFixture, TestBed, fakeAsync, tick, flush } from '@angular/core/testing';
import { MyPropertiesListComponent } from './my-properties-list';
import { PropertyService } from '../../core/services/property.service';
import { AuthService } from '../../core/services/auth.service';
import { of, throwError } from 'rxjs';
import { PropertyModel } from '../../models/property.model';
import { RouterTestingModule } from '@angular/router/testing';

describe('MyPropertiesListComponent', () => {
  let component: MyPropertiesListComponent;
  let fixture: ComponentFixture<MyPropertiesListComponent>;

  const mockProperties: PropertyModel[] = [
    {
      id: '1',
      title: 'Lovely Apartment',
      description: 'A great place',
      listingPurpose: 'Sale',
      propertyType: 'Apartment',
      listerType: 'Owner',
      location: { city: 'CityA', state: 'State', locality: 'Locality' }
    } as PropertyModel,
    {
      id: '2',
      title: 'Modern Villa',
      description: 'Spacious',
      listingPurpose: 'Sale',
      propertyType: 'Villa',
      listerType: 'Builder',
      location: { city: 'CityB', state: 'State', locality: 'Locality' }
    } as PropertyModel
  ];

  let mockPropertyService: jasmine.SpyObj<PropertyService>;

  const mockAuthService = {
    currentUser: { id: 'user123', role: 'Lister' }
  };

  beforeEach(async () => {
    mockPropertyService = jasmine.createSpyObj('PropertyService', ['getPropertiesByLister']);
    mockPropertyService.getPropertiesByLister.and.returnValue(
      of({
        data: {
          items: mockProperties,
          pagination: { currentPage: 1, totalItems: 2, totalPages: 1, pageSize: 5 }
        }
      })
    );

    await TestBed.configureTestingModule({
      imports: [MyPropertiesListComponent, RouterTestingModule],
      providers: [
        { provide: PropertyService, useValue: mockPropertyService },
        { provide: AuthService, useValue: mockAuthService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MyPropertiesListComponent);
    component = fixture.componentInstance;
  });

  it('should create the component and load properties on init', fakeAsync(() => {
    fixture.detectChanges(); // triggers ngOnInit
    tick();                  // flush observable
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(mockPropertyService.getPropertiesByLister).toHaveBeenCalledWith('user123', {
      page: 1,
      pageSize: 100
    });
    expect(component.filteredProperties.length).toBe(2);
    flush();
  }));

  it('should handle property loading error', fakeAsync(() => {
    mockPropertyService.getPropertiesByLister.and.returnValue(
      throwError(() => ({ error: { message: 'Server error' } }))
    );

    component.loadAllProperties();
    tick();
    fixture.detectChanges();

    expect(component.errorMessage).toBe('Server error');
    flush();
  }));


  it('should filter properties using onFiltersChanged', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    component.allProperties = mockProperties;
    const filters = {
      listingPurpose: 'Sale' as const,
      propertyTypes: ['Villa'],
      listerTypes: ['Builder'],
      city: 'CityB',
      keyword: 'modern'
    };
    component.onFiltersChanged(filters);

    expect(component.filteredProperties.length).toBe(1);
    expect(component.filteredProperties[0].title).toBe('Modern Villa');
    flush();
  }));
});
