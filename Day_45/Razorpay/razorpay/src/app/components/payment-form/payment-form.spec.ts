import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentFormComponent } from './payment-form';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser'; 

describe('PaymentFormComponent', () => {
  let component: PaymentFormComponent;
  let fixture: ComponentFixture<PaymentFormComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [PaymentFormComponent, ReactiveFormsModule] 
    });

    fixture = TestBed.createComponent(PaymentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  
  it('should show validation message when touched and invalid', () => {
    component.showPaymentForm();
    fixture.detectChanges();

    component.paymentForm.get('customerName')?.setValue('');
    
    component.paymentForm.get('customerName')?.markAsTouched();
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('.error-text'));
    expect(error).toBeTruthy();
  });


  it('should mark form as invalid when email format is incorrect', () => {
    component.paymentForm.setValue({
      amount: 100,
      customerName: 'minu',
      email: 'invalid-email',  
      contactNumber: '9876543210'
    });

    expect(component.paymentForm.invalid).toBeTrue();
    expect(component.paymentForm.get('email')?.errors?.['email']).toBeTrue();
  });

  it('should mark form as invalid if fields are empty', () => {
    component.paymentForm.setValue({
      amount: null,
      customerName: '',
      email: '',
      contactNumber: ''
    });
  
    expect(component.paymentForm.invalid).toBeTrue();
  });
  
  it('should mark form as valid for correct input', () => {
    component.paymentForm.setValue({
      amount: 100,
      customerName: 'Minu',
      email: 'minu@example.com',
      contactNumber: '9876543210'
    });
  
    expect(component.paymentForm.valid).toBeTrue();
  });

  it('should call Razorpay.open() on valid form submit', () => {
    component.showPaymentForm();
    fixture.detectChanges();

    component.paymentForm.setValue({
      amount: 100,
      customerName: 'Minu',
      email: 'minu@example.com',
      contactNumber: '9876543210'
    });

    const openSpy = jasmine.createSpy('open');
    const RazorpayMock = jasmine.createSpy('Razorpay').and.returnValue({
      open: openSpy
    });

    (window as any).Razorpay = RazorpayMock;

    component.submit();

    expect(RazorpayMock).toHaveBeenCalled();
    expect(openSpy).toHaveBeenCalled();
  });

  it('should handle Razorpay success callback correctly', () => {
    const fakeResponse = { razorpay_payment_id: 'pay_12345' };
    
    (window as any).Razorpay = function (options: any) {
      options.handler(fakeResponse);
      return { open: () => {} };
    };

    component.paymentForm.setValue({
      amount: 500,
      customerName: 'Minu',
      email: 'minu@example.com',
      contactNumber: '9876543210'
    });

    component.submit();

    expect(component.result).toEqual({
      success: true,
      paymentId: 'pay_12345'
    });
    expect(component.isLoading).toBeFalse();
  });

  it('should handle Razorpay cancellation correctly', () => {
    (window as any).Razorpay = function (options: any) {
      options.modal.ondismiss(); 
      return { open: () => {} };
    };

    component.paymentForm.setValue({
      amount: 200,
      customerName: 'Minu',
      email: 'minu@example.com',
      contactNumber: '9876543210'
    });

    component.submit();

    expect(component.result).toEqual({
      success: false,
      message: 'Payment was cancelled.'
    });
    expect(component.isLoading).toBeFalse();
  });

});
