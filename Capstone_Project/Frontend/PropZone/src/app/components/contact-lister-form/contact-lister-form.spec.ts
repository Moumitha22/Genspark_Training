import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ContactListerFormComponent } from './contact-lister-form';
import { ReactiveFormsModule } from '@angular/forms';
import { InquiryService } from '../../core/services/inquiry.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';

describe('ContactListerFormComponent', () => {
  let component: ContactListerFormComponent;
  let fixture: ComponentFixture<ContactListerFormComponent>;
  let mockInquiryService: jasmine.SpyObj<InquiryService>;

  const mockProperty = {
    id: 'prop123',
    title: 'Sea View Apartment',
    location: 'Marine Drive, Mumbai'
  };

  const mockListerResponse = {
    listerName: 'John Doe',
    listerEmail: 'john@example.com',
    listerPhoneNumber: '9876543210'
  };

  beforeEach(async () => {
    mockInquiryService = jasmine.createSpyObj('InquiryService', ['contactLister']);

    await TestBed.configureTestingModule({
      imports: [ContactListerFormComponent, ReactiveFormsModule],
      providers: [
        { provide: InquiryService, useValue: mockInquiryService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ContactListerFormComponent);
    component = fixture.componentInstance;
    component.property = mockProperty;
    fixture.detectChanges();
  });

  it('should show validation errors for empty form', () => {
    component.onSubmit();
    fixture.detectChanges();

    const errors = fixture.debugElement.queryAll(By.css('.text-danger'));
    expect(errors.length).toBeGreaterThan(0);
  });

  it('should call contactLister and show lister info on success', fakeAsync(() => {
    mockInquiryService.contactLister.and.returnValue(of(mockListerResponse));

    component.form.setValue({
        buyerPhoneNumber: '9876543210',
        buyerEmail: 'test@example.com',
        message: 'I am interested in this property.'
    });

    component.onSubmit();
    tick();

    expect(component.submitting).toBeFalse();
    expect(component.fetchingLister).toBeTrue();

    tick(1500); 

    expect(component.fetchingLister).toBeFalse();
    expect(component.listerInfo).toEqual(mockListerResponse);
    }));



  it('should handle service error', fakeAsync(() => {
    spyOn(console, 'error');
    mockInquiryService.contactLister.and.returnValue(throwError(() => new Error('Failed')));

    component.form.setValue({
      buyerPhoneNumber: '9876543210',
      buyerEmail: 'test@example.com',
      message: 'Hello'
    });

    component.onSubmit();
    tick();
    fixture.detectChanges();

    expect(component.submitting).toBeFalse();
    expect(console.error).toHaveBeenCalledWith('❌ Error: Failed');
  }));

  it('should emit close event when close is clicked', () => {
    spyOn(component.closed, 'emit');
    component.close();
    expect(component.closed.emit).toHaveBeenCalled();
  });
});
