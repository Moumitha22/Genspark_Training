import { TestBed } from '@angular/core/testing';
import { DashboardService } from './dashboard.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AdminDashboardModel } from '../../models/admin-dashboard.model';
import { ListerDashboardModel } from '../../models/lister-dashboard.model';

describe('DashboardService', () => {
  let service: DashboardService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5138/api/v1/Dashboard';

  const mockAdminData: AdminDashboardModel = {
    totalUsers: 100,
    totalProperties: 50,
    totalInquiries: 30,
    totalActiveListers: 10,
    propertyTypeChart: [
      { label: 'Apartment', value: 20 },
      { label: 'Villa', value: 30 }
    ],
    propertyPurposeChart: [
      { label: 'Rent', value: 25 },
      { label: 'Buy', value: 25 }
    ],
    propertyStatusChart: [
      { label: 'Available', value: 40 },
      { label: 'Sold', value: 10 }
    ]
  };

  const mockListerData: ListerDashboardModel = {
    totalPropertiesListed: 15,
    totalForSale: 5,
    totalForRent: 6,
    totalSoldOut: 2,
    totalRented: 1,
    totalAvailable: 6,
    totalInquiriesReceived: 20,
    propertyTypeChart: [
      { label: 'Flat', value: 10 },
      { label: 'House', value: 5 }
    ],
    propertyPurposeChart: [
      { label: 'Buy', value: 8 },
      { label: 'Rent', value: 7 }
    ],
    propertyStatusChart: [
      { label: 'Available', value: 6 },
      { label: 'Rented', value: 1 }
    ]
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DashboardService]
    });

    service = TestBed.inject(DashboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch admin dashboard data', () => {
    service.getAdminDashboard().subscribe(data => {
      expect(data).toEqual(mockAdminData);
      expect(data.totalUsers).toBe(100);
    });

    const req = httpMock.expectOne(`${baseUrl}/admin`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: mockAdminData });
  });

  it('should fetch lister dashboard data', () => {
    service.getListerDashboard().subscribe(data => {
      expect(data).toEqual(mockListerData);
      expect(data.totalPropertiesListed).toBe(15);
    });

    const req = httpMock.expectOne(`${baseUrl}/lister`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: mockListerData });
  });
});
