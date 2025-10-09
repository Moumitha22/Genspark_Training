import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InquiryService } from '../../core/services/inquiry.service';
import { ListerInquiry } from '../../models/lister-inquiry.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-inquiries',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './property-inquiries.html',
  styleUrls: ['../lister-inquiries/lister-inquiries.css'] 
})
export class PropertyInquiriesComponent implements OnInit {
  inquiries: (ListerInquiry & { showFullMessage: boolean })[] = [];
  loading = true;
  errorMessage: string | null = null;

  private inquiryService= inject(InquiryService);
  private route= inject(ActivatedRoute);

  ngOnInit(): void {
    const propertyId = this.route.snapshot.paramMap.get('propertyId');
    if (!propertyId) {
      this.errorMessage = 'No property ID provided.';
      this.loading = false;
      return;
    }

    this.inquiryService.getPropertyInquiries(propertyId).subscribe({
      next: (res) => {
        this.inquiries = res.data.map((inquiry: ListerInquiry) => ({
          ...inquiry,
          showFullMessage: false
        }));
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load inquiries', err);
        this.errorMessage = err.error?.message || 'Failed to load inquiries.';
        this.loading = false;
      }
    });
  }
}
