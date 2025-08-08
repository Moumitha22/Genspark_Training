import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { AdminDashboard } from './admin-dashboard';
import { UserService } from '../../../core/services/user.service';
import { AuctionService } from '../../../core/services/auction.service';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

describe('AdminDashboard', () => {
  let component: AdminDashboard;
  let fixture: ComponentFixture<AdminDashboard>;

  let userServiceSpy: jasmine.SpyObj<UserService>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  const mockUsers = {
    $values: [
      { id: 1, role: 'Seller' },
      { id: 2, role: 'Bidder' },
      { id: 3, role: 'Admin' },
      { id: 4, role: 'Seller' },
    ],
  };

  const mockAuctions = {
    data: {
      $values: [
        { id: 1, createdAt: '2025-01-01', status: 'Upcoming' },
        { id: 2, createdAt: '2025-02-01', status: 'Closed' },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    userServiceSpy = jasmine.createSpyObj('UserService', ['getAllUsers']);
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getAllAuctions',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getCurrentUser']);

    TestBed.configureTestingModule({
      imports: [AdminDashboard],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminDashboard);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load and count users correctly', () => {
    userServiceSpy.getAllUsers.and.returnValue(of(mockUsers));

    component.loadUsers();

    expect(userServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.totalUsers).toBe(4);
    expect(component.totalSellers).toBe(2);
    expect(component.totalBidders).toBe(1);
    expect(component.totalAdmins).toBe(1);
  });

  it('should load auctions and set chart options', () => {
    auctionServiceSpy.getAllAuctions.and.returnValue(of(mockAuctions));

    component.loadAuctions();

    expect(auctionServiceSpy.getAllAuctions).toHaveBeenCalled();
    expect(component.auctionData.length).toBe(2);
    expect(component.auctionchartOptions).toBeTruthy();
  });

  it('should handle error in loadAuctions', () => {
    const errorResponse = { status: 500 };
    spyOn(console, 'log');
    auctionServiceSpy.getAllAuctions.and.returnValue(
      throwError(() => errorResponse)
    );

    component.loadAuctions();

    expect(console.log).toHaveBeenCalledWith(errorResponse);
  });

  it('should load user creation stats and set chart options', () => {
    userServiceSpy.getAllUsers.and.returnValue(of(mockUsers));

    component.loadUserCreationStats();

    expect(userServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.userCreationChartOptions).toBeTruthy();
  });

  it('should return correct auction status', () => {
    expect(component.getAuctionStatus({ status: 'Upcoming' })).toBe('Upcoming');
    expect(component.getAuctionStatus({ status: 'Live' })).toBe('Live');
    expect(component.getAuctionStatus({ status: 'Closed' })).toBe('Closed');
    expect(component.getAuctionStatus({ status: 'Invalid' })).toBe('Unknown');
  });

  it('should handle error in loadUsers', () => {
    const errorResponse = { status: 500 };
    spyOn(console, 'log');
    userServiceSpy.getAllUsers.and.returnValue(throwError(() => errorResponse));

    component.loadUsers();

    expect(console.log).toHaveBeenCalledWith(errorResponse);
  });

  it('should initialize and load data on ngOnInit', () => {
    spyOn(component, 'loadUsers');
    spyOn(component, 'loadAuctions');
    spyOn(component, 'loadUserCreationStats');

    component.ngOnInit();

    expect(component.loadUsers).toHaveBeenCalled();
    expect(component.loadAuctions).toHaveBeenCalled();
    expect(component.loadUserCreationStats).toHaveBeenCalled();
  });
});
