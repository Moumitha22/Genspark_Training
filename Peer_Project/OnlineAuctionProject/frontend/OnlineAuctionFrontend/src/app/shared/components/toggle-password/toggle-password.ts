import { Component, Input } from '@angular/core';
import { EyeClosedIcon, EyeIcon, EyeOff, EyeOffIcon, LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-toggle-password',
  imports: [LucideAngularModule],
  templateUrl: './toggle-password.html',
})
export class TogglePassword {
  readonly eyeOff = EyeOffIcon;
  readonly eyeOpen = EyeIcon;
  @Input() targetElement!: HTMLInputElement | null;
  visible: boolean = false;

  toggle(){
    if(this.targetElement){
      this.visible = !this.visible;
      this.targetElement.type = this.visible?'text':'password'
    }
  }
}
