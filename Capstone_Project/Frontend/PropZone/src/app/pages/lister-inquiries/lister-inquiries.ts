import { Component, inject, Input, OnInit } from '@angular/core';
import { ListerInquiry } from '../../models/lister-inquiry.model';
import { InquiryService } from '../../core/services/inquiry.service';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-lister-inquiries',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lister-inquiries.html',
  styleUrl: './lister-inquiries.css'
})
export class ListerInquiriesComponent implements OnInit {
  @Input() maxCount?: number;

  inquiries: (ListerInquiry & { showFullMessage: boolean })[] = [];
  filteredInquiries: (ListerInquiry & { showFullMessage: boolean })[] = [];
  loading = true;
  searchTerm: string = '';

  private inquiryService= inject(InquiryService);
  private route= inject(ActivatedRoute);
  private authService = inject(AuthService);

  ngOnInit(): void {
    const listerId = this.authService.currentUser?.id;
    if (!listerId) return;

    this.inquiryService.getListerInquiries(listerId).subscribe({
      next: (res) => {
        let loaded = res.data.map((inquiry: ListerInquiry) => ({
          ...inquiry,
          showFullMessage: false
        }));

        if (this.maxCount) {
          loaded = loaded.slice(0, this.maxCount);
        }

        this.inquiries = loaded;
        this.filteredInquiries = loaded;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load inquiries', err);
        this.loading = false;
      }
    });
  }


  onSearch(): void {
    const term = this.searchTerm.toLowerCase().trim();
    this.filteredInquiries = this.inquiries.filter(inquiry =>
      inquiry.propertyTitle.toLowerCase().includes(term) ||
      inquiry.location.toLowerCase().includes(term)
    );
  }
}
