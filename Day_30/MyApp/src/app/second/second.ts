import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-second',
  imports: [],
  templateUrl: './second.html',
  styleUrl: './second.css'
})
export class Second {
  @Input() message: string = "";
  @Output() notify = new EventEmitter<string>();
  @Output() like = new EventEmitter<void>();

  send(){
    this.notify.emit('Child clicked the button');
  }

  onClick(){
    this.like.emit();
  }

}
