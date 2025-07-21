import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TrainingVideoUploadComponent } from './components/training-video-upload/training-video-upload';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TrainingVideoUploadComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'TrainingVideoPortalUI';
}
