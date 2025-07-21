import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TrainingVideo } from '../models/training-video';
import { TrainingVideoUpload } from '../models/training-video-upload';

@Injectable({
  providedIn: 'root',
})
export class TrainingVideoService {
  private readonly baseUrl = 'http://localhost:5031/api/TrainingVideos'; 

  private http = inject(HttpClient);

  getAllVideos(): Observable<TrainingVideo[]> {
    return this.http.get<TrainingVideo[]>(this.baseUrl);
  }

  getVideoById(id: string): Observable<TrainingVideo> {
    return this.http.get<TrainingVideo>(`${this.baseUrl}/${id}`);
  }

  uploadVideo(data: TrainingVideoUpload): Observable<TrainingVideo> {
    const formData = new FormData();
    formData.append('title', data.title);
    formData.append('description', data.description);
    formData.append('file', data.file);

    return this.http.post<TrainingVideo>(`${this.baseUrl}/upload`, formData);
 }


}
