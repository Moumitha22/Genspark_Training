import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PaymentFormComponent } from './components/payment-form/payment-form';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, PaymentFormComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'razorpay';
}
