import { CommonModule } from '@angular/common';
import { Component, inject, NgZone, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-payment-form',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './payment-form.html',
  styleUrls: ['./payment-form.css']
})
export class PaymentFormComponent implements OnInit {
  paymentForm!: FormGroup;
  isLoading = false;
  result: any = null;
  showForm = false;


  private fb = inject(FormBuilder);
  private zone =  inject(NgZone);

  ngOnInit(): void {
    if (!(window as any).Razorpay) {
      console.error('Razorpay script not loaded!');
    }
    this.paymentForm = this.fb.group({
      amount: [null, [Validators.required, Validators.min(1)]],
      customerName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      contactNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    });
  }

  showPaymentForm() {
    this.showForm = true;
  }


  submit(): void {
    if (this.paymentForm.invalid) 
      return;
    this.isLoading = true;
    this.launchRazorpay();
  }

  launchRazorpay(): void {
    const formData = this.paymentForm.value;

    const options = {
      key: 'rzp_test_aPJQCEj0mTKknw', 
      amount: formData.amount * 100,
      currency: 'INR',
      name: formData.customerName,
      prefill: {
        email: formData.email,
        contact: formData.contactNumber
      },
      method: {
        upi: true,
        card: false,
        netbanking: false,
        wallet: false,
      },
      handler: (response: any) => {
        this.zone.run(() => {
        this.isLoading = false;
        this.result = {
          success: true,
          paymentId: response.razorpay_payment_id
        };
        this.paymentForm.reset();
        this.showForm = false;  
      });

      },
      modal: {
        ondismiss: () => {
        this.zone.run(() => {
          this.isLoading = false;
          this.result = {
            success: false,
            message: 'Payment was cancelled.'
          };
          this.paymentForm.reset();
          this.showForm = false; 
        });
      }
    }
    };

    const razorpay = new (window as any).Razorpay(options);
    razorpay.open();
  }

  resetPayment(): void {
    this.result = null;
    this.paymentForm.reset();
    this.showForm = true;
  }

}
