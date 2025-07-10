import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ListerInquiriesComponent } from './lister-inquiries';
import { InquiryService } from '../../core/services/inquiry.service';
import { AuthService } from '../../core/services/auth.service';
import { of, throwError } from 'rxjs';
import { ListerInquiry } from '../../models/lister-inquiry.model';
import { RouterTestingModule } from '@angular/router/testing';

describe('ListerInquiriesComponent', () => {
  let component: ListerInquiriesComponent;
  let fixture: ComponentFixture<ListerInquiriesComponent>;

  const mockInquiries: ListerInquiry[] = [
    {
      propertyId: 'p1',
      propertyTitle: 'Beach House',
      location: 'Goa',
      message: 'Is this still available?',
      buyerEmail: 'ravi@example.com',
      buyerPhoneNumber: '1111111111',
      createdAt: new Date().toISOString()
    },
    {
      propertyId: 'p2',
      propertyTitle: 'Hilltop Villa',
      location: 'Ooty',
      message: 'Interested in a visit next week.',
      buyerEmail: 'sneha@example.com',
      buyerPhoneNumber: '2222222222',
      createdAt: new Date().toISOString()
    }
  ];

  const mockInquiryService = {
    getListerInquiries: jasmine.createSpy().and.returnValue(of({ data: mockInquiries }))
  };

  const mockAuthService = {
    currentUser: { id: 'lister123', role: 'Lister' }
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListerInquiriesComponent, RouterTestingModule],
      providers: [
        { provide: InquiryService, useValue: mockInquiryService },
        { provide: AuthService, useValue: mockAuthService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ListerInquiriesComponent);
    component = fixture.componentInstance;
  });

  it('should create and load inquiries', fakeAsync(() => {
    fixture.detectChanges(); 
    tick();
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(mockInquiryService.getListerInquiries).toHaveBeenCalledWith('lister123');
    expect(component.inquiries.length).toBe(2);
    expect(component.filteredInquiries.length).toBe(2);
    expect(component.loading).toBeFalse();
  }));

  it('should apply maxCount if provided', fakeAsync(() => {
    component.maxCount = 1;
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    expect(component.inquiries.length).toBe(1);
    expect(component.filteredInquiries.length).toBe(1);
  }));

  it('should filter inquiries by search term', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    component.searchTerm = 'goa';
    component.onSearch();

    expect(component.filteredInquiries.length).toBe(1);
    expect(component.filteredInquiries[0].location).toBe('Goa');
  }));
});
