import { TestBed } from '@angular/core/testing';
import { InquiryService } from './inquiry.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ContactListerRequest } from '../../models/contact-lister-request.model';

describe('InquiryService', () => {
  let service: InquiryService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5138/api/v1/Contact'; // Adjust if env differs

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [InquiryService]
    });

    service = TestBed.inject(InquiryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should contact lister', () => {
    const request: ContactListerRequest = {
      propertyId: 'p1',
      buyerPhoneNumber: '9999999999',
      buyerEmail: 'buyer@example.com',
      message: 'I am interested'
    };

    const mockResponse = { success: true };

    service.contactLister(request).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/lister`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush(mockResponse);
  });

  it('should fetch all contact logs', () => {
    const mockLogs = [{ id: '1' }, { id: '2' }];
    
    service.getAllContactLogs().subscribe(res => {
      expect(res.length).toBe(2);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockLogs);
  });

  it('should get property inquiries', () => {
    const propertyId = 'property123';
    const mockLogs = [{ message: 'Hello' }];

    service.getPropertyInquiries(propertyId).subscribe(res => {
      expect(res).toEqual(mockLogs);
    });

    const req = httpMock.expectOne(`${baseUrl}/logs/property/${propertyId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockLogs);
  });

  it('should get lister inquiries', () => {
    const listerId = 'lister456';
    const mockLogs = [{ message: 'Interested' }];

    service.getListerInquiries(listerId).subscribe(res => {
      expect(res).toEqual(mockLogs);
    });

    const req = httpMock.expectOne(`${baseUrl}/logs/lister/${listerId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockLogs);
  });

  it('should get buyer inquiries', () => {
    const buyerId = 'buyer789';
    const mockLogs = [{ message: 'Please contact me' }];

    service.getBuyerInquiries(buyerId).subscribe(res => {
      expect(res).toEqual(mockLogs);
    });

    const req = httpMock.expectOne(`${baseUrl}/logs/buyer/${buyerId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockLogs);
  });
});
