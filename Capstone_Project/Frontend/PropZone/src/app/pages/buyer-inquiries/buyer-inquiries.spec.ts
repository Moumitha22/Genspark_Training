import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { BuyerInquiriesComponent } from './buyer-inquiries';
import { InquiryService } from '../../core/services/inquiry.service';
import { AuthService } from '../../core/services/auth.service';
import { BuyerInquiry } from '../../models/buyer-inquiry-model';
import { of, throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';

describe('BuyerInquiriesComponent', () => {
  let component: BuyerInquiriesComponent;
  let fixture: ComponentFixture<BuyerInquiriesComponent>;

  const mockInquiries: BuyerInquiry[] = [
    {
      propertyId: '1',
      propertyTitle: 'Luxury Apartment',
      location: 'CityA',
      message: 'Looking forward to a visit this weekend.',
      listerName: 'Alice',
      listerEmail: 'alice@example.com',
      listerPhoneNumber: '1234567890',
      createdAt: new Date().toISOString()
    },
    {
      propertyId: '2',
      propertyTitle: 'Cozy Villa',
      location: 'CityB',
      message: 'Interested in buying.',
      listerName: 'Bob',
      listerEmail: 'bob@example.com',
      listerPhoneNumber: '9876543210',
      createdAt: new Date().toISOString()
    }
  ];

  const mockInquiryService = {
    getBuyerInquiries: jasmine.createSpy().and.returnValue(of({ data: mockInquiries }))
  };

  const mockAuthService = {
    currentUser: { id: 'buyer123', role: 'Buyer' }
    };

    beforeEach(fakeAsync(() => {
    mockInquiryService.getBuyerInquiries = jasmine.createSpy().and.returnValue(
        of({
        data: mockInquiries.map(i => ({
            ...i,
            showFullMessage: false,
            isLongMessage: i.message.length > 150
        }))
        })
    );

    TestBed.configureTestingModule({
        imports: [BuyerInquiriesComponent, RouterTestingModule],
        providers: [
        { provide: InquiryService, useValue: mockInquiryService },
        { provide: AuthService, useValue: mockAuthService }
        ]
    }).compileComponents();

    fixture = TestBed.createComponent(BuyerInquiriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();
    }));


  it('should create the component and load inquiries', fakeAsync(() => {
    tick();
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(mockInquiryService.getBuyerInquiries).toHaveBeenCalledWith('buyer123');
    expect(component.inquiries.length).toBe(2);
    expect(component.filteredInquiries.length).toBe(2);
    expect(component.loading).toBeFalse();
  }));

  it('should filter inquiries based on search input', fakeAsync(() => {
    tick();
    fixture.detectChanges();

    component.searchTitle = 'Villa';
    component.onSearch();

    expect(component.filteredInquiries.length).toBe(1);
    expect(component.filteredInquiries[0].propertyTitle).toBe('Cozy Villa');
  }));

  it('should toggle full message on click', fakeAsync(() => {
    tick();
    fixture.detectChanges();

    const firstInquiry = component.filteredInquiries[0];
    expect(firstInquiry.showFullMessage).toBeFalse();

    firstInquiry.showFullMessage = true;
    fixture.detectChanges();

    expect(firstInquiry.showFullMessage).toBeTrue();
  }));

  it('should handle error during inquiry loading', fakeAsync(() => {
    mockInquiryService.getBuyerInquiries = jasmine
      .createSpy()
      .and.returnValue(throwError(() => new Error('API failure')));
    
    fixture = TestBed.createComponent(BuyerInquiriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();

    expect(component.loading).toBeFalse();
    expect(component.inquiries.length).toBe(0);
  }));
});
