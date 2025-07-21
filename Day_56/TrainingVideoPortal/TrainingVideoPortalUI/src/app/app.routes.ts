import { Routes } from '@angular/router';
import { TrainingVideoListComponent } from './components/training-video-list/training-video-list';
import { TrainingVideoUploadComponent } from './components/training-video-upload/training-video-upload';

export const routes: Routes = [
  { path: '', component: TrainingVideoListComponent, pathMatch: 'full' },  
  { path: 'videos', component: TrainingVideoListComponent },
  { path: 'upload', component: TrainingVideoUploadComponent }
];
