import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CircleX, CircleXIcon, LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-model-view',
  imports: [LucideAngularModule],
  templateUrl: './model-view.html',
})
export class ModelView {
  readonly closeSymbol = CircleXIcon;
  @Input() title: string = '';
  @Input() show: boolean = false;
  @Input() contentTemplate: any;
  @Output() close = new EventEmitter<void>();

  onClose() {
    this.close.emit();
  }
}
