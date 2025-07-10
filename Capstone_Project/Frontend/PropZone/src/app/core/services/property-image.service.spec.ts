import { TestBed } from '@angular/core/testing';
import { PropertyImageService } from './property-image.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PropertyImageModel } from '../../models/property-image.model';
import { BulkPropertyImageUploadRequest } from '../../models/property-images-bulk-upload.model';

describe('PropertyImageService', () => {
  let service: PropertyImageService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5138/api/v1/PropertyImage';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PropertyImageService]
    });
    service = TestBed.inject(PropertyImageService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get images by property ID', () => {
    const mockImages: PropertyImageModel[] = [
      {
        id: 'img1',
        propertyId: 'prop123',
        imageUrl: 'http://example.com/img1.jpg'
      }
    ];

    service.getImagesByPropertyId('prop123').subscribe(images => {
      expect(images.length).toBe(1);
      expect(images[0].id).toBe('img1');
    });

    const req = httpMock.expectOne(`${baseUrl}/by-property/prop123`);
    expect(req.request.method).toBe('GET');
    req.flush(mockImages);
  });

  it('should upload images', () => {
    const fakeFile = new File(['dummy content'], 'test.jpg', { type: 'image/jpeg' });
    const dto: BulkPropertyImageUploadRequest = {
      propertyId: 'prop123',
      files: [fakeFile]
    };

    service.uploadImages(dto).subscribe(response => {
      expect(response).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/upload-multiple`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body instanceof FormData).toBeTrue();

    const formData = req.request.body as FormData;
    expect(formData.get('PropertyId')).toBe('prop123');
    expect(formData.getAll('Files').length).toBe(1);

    req.flush({ success: true });
  });

  it('should delete image by ID', () => {
    service.deleteImage('img1').subscribe(response => {
      expect(response).toEqual({ deleted: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/image/img1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ deleted: true });
  });
});
