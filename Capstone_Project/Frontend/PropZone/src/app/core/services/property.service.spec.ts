import { TestBed } from '@angular/core/testing';
import { PropertyService } from './property.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PropertyModel } from '../../models/property.model';
import { PropertyAddRequest } from '../../models/property-add-request.model';
import { environment } from '../../environments/environment';

describe('PropertyService', () => {
  let service: PropertyService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBaseUrl}/api/v1/Property`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PropertyService]
    });
    service = TestBed.inject(PropertyService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get property by ID', () => {
    const mockProperty: PropertyModel = {
      id: '1',
      listerId: 'L1',
      title: 'Test Property',
      price: 500000,
      location: { city: 'CityX', state: 'StateX', locality: 'LocX' },
      propertyType: 'Apartment',
      listingPurpose: 'Sale',
      listerType: 'Agent',
      areaSqFt: 1200,
      createdAt: new Date().toISOString(),
      status: 'Active',
      imageUrls: [],
      featureSummary: []
    };

    service.getPropertyById('1').subscribe(res => {
      expect(res.data.title).toBe('Test Property');
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: mockProperty });
  });

  it('should perform basic search', () => {
    const searchModel = { city: 'CityX', propertyTypes: ['Apartment'] };
    const sort = { sortBy: 'Price', ascending: true };
    const pagination = { page: 1, pageSize: 10 };

    const mockData: PropertyModel[] = [
    {
        id: '1',
        listerId: 'lister123',
        title: 'Luxury Apartment',
        price: 100000,
        areaSqFt: 1200,
        propertyType: 'Apartment',
        listingPurpose: 'Sale',
        listerType: 'Agent',
        location: {
        city: 'CityX',
        state: 'StateY',
        locality: 'Downtown',
        },
        createdAt: new Date().toISOString(),
        status: 'Active',
        imageUrls: ['image1.jpg'],
        featureSummary: []
    }
    ];


    service.basicSearch(searchModel, sort, pagination).subscribe(data => {
      expect(data.length).toBe(1);
    });

    const req = httpMock.expectOne(req => req.url === `${baseUrl}/search`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('City')).toBe('CityX');
    expect(req.request.params.getAll('PropertyTypes')).toContain('Apartment');
    req.flush(mockData);
  });

  it('should perform advanced search', () => {
    const advancedModel = { filters: ['Bedrooms'] };
    const sort = { sortBy: 'CreatedAt', ascending: false };
    const pagination = { page: 1, pageSize: 5 };

    service.advancedSearch(advancedModel, sort, pagination).subscribe(data => {
      expect(data.length).toBe(0);
    });

    const req = httpMock.expectOne(
      r => r.method === 'POST' && r.url === 'http://localhost:5138/api/v1/Property/search'
    );
    expect(req.request.params.get('SortBy')).toBe('CreatedAt');
    req.flush([]);
  });

  it('should create property', () => {
    const newProp: PropertyAddRequest = {
      title: 'New Prop',
      price: 100000,
      listerType: 'Owner',
      propertyType: 'Plot',
      listingPurpose: 'Sale',
      areaSqFt: 800,
      location: { city: 'C', state: 'S', locality: 'L' },
      features: []
    };

    service.createProperty(newProp).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body.title).toBe('New Prop');
    req.flush({ success: true });
  });

  it('should update property', () => {
    const updatedProp: PropertyAddRequest = {
    title: 'Updated Prop',
    price: 200000,
    listerType: 'Agent',
    propertyType: 'Apartment',
    listingPurpose: 'Rent',
    areaSqFt: 900,
    location: {
        city: 'UpdateCity',
        state: 'UpdateState',
        locality: 'UpdateLocality',
    },
    features: []
    };

    service.updateProperty('1', updatedProp).subscribe(res => {
        expect(res).toEqual({ updated: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body.title).toBe('Updated Prop');
    req.flush({ updated: true });
    });


  it('should delete property', () => {
    service.deleteProperty('1').subscribe(res => {
      expect(res).toEqual({ deleted: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ deleted: true });
  });

  it('should fetch properties by lister', () => {
    const listerId = 'L1';
    const pagination = { page: 1, pageSize: 5 };

    const mockResponse = {
      data: {
        items: [{
          id: 'P1',
          listerId: 'L1',
          title: 'Test',
          description: 'Test Desc',
          price: 1000,
          propertyType: 'Apartment',
          listingPurpose: 'Sale',
          listerType: 'Owner',
          areaSqFt: 800,
          createdAt: new Date().toISOString(),
          status: 'Active',
          imageUrls: [],
          location: {
            city: 'CityX',
            state: 'StateX',
            locality: 'LocX',
          },
          featureSummary: []
        }],
        pagination: {
          currentPage: 1,
          pageSize: 5,
          totalItems: 1,
          totalPages: 1
        }
      }
    };

    service.getPropertiesByLister(listerId, pagination).subscribe(res => {
      expect(res.data.items.length).toBe(1);
      expect(res.data.items[0].title).toBe('Test');
    });

    const req = httpMock.expectOne(req =>
      req.method === 'GET' &&
      req.url === `${baseUrl}/by-lister/${listerId}` &&
      req.params.get('Page') === '1' &&
      req.params.get('PageSize') === '5'
    );

    req.flush(mockResponse);
  });


  it('should get all properties', () => {
    const mockData = {
      data: [{ id: '1', title: 'AllProp' } as PropertyModel]
    };

    service.getAllProperties().subscribe(res => {
      expect(res.data[0].title).toBe('AllProp');
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockData);
  });
});
