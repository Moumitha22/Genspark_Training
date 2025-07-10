import { TestBed } from '@angular/core/testing';
import { ListerProfileService } from './lister-profile.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ListerProfileRequest } from '../../models/lister-profile.model';

describe('ListerProfileService', () => {
  let service: ListerProfileService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5138/api/v1/ListerProfile'; // Update if your env is different

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ListerProfileService]
    });

    service = TestBed.inject(ListerProfileService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should check profile completion', () => {
    const mockResponse = { isComplete: true };

    service.checkProfileCompletion().subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/is-complete`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should create profile', () => {
    const dto: ListerProfileRequest = {
      agencyName: 'ABC Realty',
      licenseNumber: 'LIC1234',
      businessPhoneNumber: '9876543210'
    };

    const mockResponse = { success: true };

    service.createProfile(dto).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResponse);
  });

  it('should update profile', () => {
    const profileId = 'profile123';
    const dto: ListerProfileRequest = {
      agencyName: 'Updated Agency',
      licenseNumber: 'NEW456',
      businessPhoneNumber: '9999999999'
    };

    const mockResponse = { updated: true };

    service.updateProfile(profileId, dto).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/${profileId}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResponse);
  });

  it('should get profile by lister ID', () => {
    const listerId = 'lister456';
    const mockProfile = {
      id: 'profile789',
      listerId,
      agencyName: 'XYZ Agency',
      businessPhoneNumber: '8888888888'
    };

    service.getByListerId(listerId).subscribe(res => {
      expect(res).toEqual(mockProfile);
    });

    const req = httpMock.expectOne(`${baseUrl}/by-lister/${listerId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockProfile);
  });
});
