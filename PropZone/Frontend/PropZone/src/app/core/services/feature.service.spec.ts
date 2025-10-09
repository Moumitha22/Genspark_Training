import { TestBed } from '@angular/core/testing';
import { FeatureService } from './feature.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';

describe('FeatureService', () => {
  let service: FeatureService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5138/api/v1/FeatureMaster/applicable';

  const mockFeatures: DynamicFeatureModel[] = [
    {
        id: 'f1',
        name: 'Has Lift',
        filterMode: 'Boolean',
        dataType: 'boolean',
        options: [] 
    },
    {
        id: 'f2',
        name: 'Price',
        filterMode: 'Range',
        dataType: 'number',
        options: [] 
    }
    ];


  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [FeatureService]
    });

    service = TestBed.inject(FeatureService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });
  
  it('should fetch applicable features with only purpose', () => {
    const purpose = 'Rent';

    service.getApplicableFeatures(purpose).subscribe(res => {
      expect(res.data).toEqual(mockFeatures);
      expect(res.data.length).toBe(2);
    });

    const req = httpMock.expectOne(
      r => r.url === baseUrl && r.params.has('listingPurpose') && !r.params.has('propertyType')
    );
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('listingPurpose')).toBe('Rent');
    req.flush({ data: mockFeatures });
  });

  it('should fetch applicable features with purpose and propertyType', () => {
    const purpose = 'Buy';
    const propertyType = 'Apartment';

    service.getApplicableFeatures(purpose, propertyType).subscribe(res => {
      expect(res.data).toEqual(mockFeatures);
    });

    const req = httpMock.expectOne(
      r =>
        r.url === baseUrl &&
        r.params.get('listingPurpose') === 'Buy' &&
        r.params.get('propertyType') === 'Apartment'
    );

    expect(req.request.method).toBe('GET');
    req.flush({ data: mockFeatures });
  });
});
