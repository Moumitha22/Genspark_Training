import { Component, inject, OnInit } from '@angular/core';
import { NewsService } from '../../services/news.service';
import { NewsModel } from '../../models/news';
import { environment } from '../../environments/environment';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-news',
  imports: [CommonModule, RouterLink],
  templateUrl: './news.html',
  styleUrl: './news.css'
})
export class NewsListComponent implements OnInit {
  newsList: NewsModel[] = [];
  imageBaseUrl = environment.apiBaseUrl + '/';
  isAdmin = false;

  private newsService = inject(NewsService);
  private authService = inject(AuthService);

  ngOnInit(): void {
    this.isAdmin = this.authService.currentUserRole === 'Admin';
    this.loadNews();
  }

  loadNews(): void {
    this.newsService.getAll().subscribe({
      next: (data) => {
        this.newsList = data;
      },
      error: (err) => {
        console.error('Error fetching news', err);
      },
    });
  }

  editNews(newsId: number): void {
  }

  deleteNews(newsId: number): void {
    if (confirm('Are you sure you want to delete this news item?')) {
      this.newsService.delete(newsId).subscribe({
        next: () => {
          this.loadNews();
        },
        error: (err) => {
          console.error('Delete failed', err);
        }
      });
    }
  }


  downloadCsv() {
    this.newsService.downloadCsv().subscribe({
      next: (blob) => {
        const link = document.createElement('a');
        const url = window.URL.createObjectURL(blob);
        link.href = url;
        link.download = 'NewsList.csv';
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('CSV download failed', err);
      }
    });
  }

  downloadExcel() {
    this.newsService.downloadExcel().subscribe(blob => {
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = 'news.xlsx';
      link.click();
      URL.revokeObjectURL(link.href);
    });
  }
}
