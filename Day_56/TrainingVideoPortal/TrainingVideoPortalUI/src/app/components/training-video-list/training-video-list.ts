import { Component, inject, OnInit } from '@angular/core';
import { TrainingVideo } from '../../models/training-video';
import { TrainingVideoService } from '../../services/training-video.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-training-video-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './training-video-list.html',
  styleUrl: './training-video-list.css'
})
export class TrainingVideoListComponent implements OnInit {
  videos: TrainingVideo[] = [];

  private videoService = inject(TrainingVideoService);

  ngOnInit(): void {
    this.videoService.getAllVideos().subscribe({
      next: (res) => {
        this.videos = res;
      },
      error: (err) => console.error('Failed to load videos:', err)
    });
  }
}