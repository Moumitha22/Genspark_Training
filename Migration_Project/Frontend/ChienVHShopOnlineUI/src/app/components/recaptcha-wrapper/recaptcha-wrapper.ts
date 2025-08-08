import { Component, Input, Output, EventEmitter } from '@angular/core';
import { RecaptchaModule } from 'ng-recaptcha';

@Component({
  selector: 'app-recaptcha-wrapper',
  standalone: true,
  imports: [RecaptchaModule],
  template: `
    <re-captcha
      [siteKey]="siteKey"
      (resolved)="handleResolved($event)"
    ></re-captcha>
  `
})
export class RecaptchaWrapperComponent {
  @Input() siteKey!: string;
  @Output() token = new EventEmitter<string>();

  handleResolved(token: string | null): void {
    if (token) {
      this.token.emit(token);
    }
  }

}
