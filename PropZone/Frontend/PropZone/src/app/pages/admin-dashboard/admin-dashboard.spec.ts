import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AdminDashboardComponent } from './admin-dashboard';
import { of, throwError } from 'rxjs';
import { DashboardService } from '../../core/services/dashboard.service';
import { AdminDashboardModel } from '../../models/admin-dashboard.model';
import { ChartItemModel } from '../../models/chart-item.model';
import { PropertyService } from '../../core/services/property.service';
import { NotificationService } from '../../core/services/notification.service';
import { RouterTestingModule } from '@angular/router/testing';

describe('AdminDashboardComponent', () => {
  let component: AdminDashboardComponent;
  let fixture: ComponentFixture<AdminDashboardComponent>;
  let mockDashboardService: jasmine.SpyObj<DashboardService>;
  let mockPropertyService: jasmine.SpyObj<PropertyService>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;

  const mockChartItems: ChartItemModel[] = [
    { label: 'Item A', value: 10 },
    { label: 'Item B', value: 20 }
  ];

  const mockDashboardData: AdminDashboardModel = {
    totalUsers: 100,
    totalProperties: 50,
    totalInquiries: 25,
    totalActiveListers: 25,
    propertyTypeChart: mockChartItems,
    propertyPurposeChart: mockChartItems,
    propertyStatusChart: mockChartItems
  };

  

  beforeEach(async () => {
    mockDashboardService = jasmine.createSpyObj('DashboardService', ['getAdminDashboard']);
    mockPropertyService = jasmine.createSpyObj('PropertyService', ['getAll', 'getById']); 
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['showSuccess', 'showError']); // or whatever is used


    await TestBed.configureTestingModule({
      imports: [AdminDashboardComponent, RouterTestingModule.withRoutes([])],
      providers: [
        { provide: DashboardService, useValue: mockDashboardService },
        { provide: PropertyService, useValue: mockPropertyService } ,
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
  });

  it('should load dashboard data and setup charts on init', fakeAsync(() => {
    mockDashboardService.getAdminDashboard.and.returnValue(of(mockDashboardData));

    component.ngOnInit();
    tick();

    expect(component.loading).toBeFalse();
    expect(component.data).toEqual(mockDashboardData);
    expect(component.typeChartData.labels).toEqual(['Item A', 'Item B']);
    expect(component.purposeChartData.labels).toEqual(['Item A', 'Item B']);
    expect(component.statusChartData.labels).toEqual(['Item A', 'Item B']);
  }));

  it('should handle error during dashboard load', fakeAsync(() => {
    mockDashboardService.getAdminDashboard.and.returnValue(throwError(() => new Error('Load error')));

    component.ngOnInit();
    tick();

    expect(component.loading).toBeFalse();
    expect(component.data).toBeNull();
  }));
});
