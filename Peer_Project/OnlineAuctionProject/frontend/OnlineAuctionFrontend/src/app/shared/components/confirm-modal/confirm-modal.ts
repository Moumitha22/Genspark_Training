import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ModelView } from '../model-view/model-view';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-confirm-modal',
  imports: [ModelView, FormsModule, LucideAngularModule],
  templateUrl: './confirm-modal.html',
  styleUrl: './confirm-modal.css',
})
export class ConfirmModal {
  @Input() show = false;
  @Input() icon: any = '';
  @Input() message: string = '';
  @Input() showInput = false;
  @Input() inputPlaceholder: string = '';
  @Output() close = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<string | void>();
  inputValue: string = '';

  onConfirm() {
    this.confirm.emit(this.showInput ? this.inputValue : undefined);
  }
}
