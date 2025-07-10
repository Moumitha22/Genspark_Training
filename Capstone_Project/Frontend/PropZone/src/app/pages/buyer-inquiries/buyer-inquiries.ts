import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { InquiryService } from '../../core/services/inquiry.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BuyerInquiry } from '../../models/buyer-inquiry-model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-buyer-inquiries',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './buyer-inquiries.html',
  styleUrl: './buyer-inquiries.css'
})
export class BuyerInquiriesComponent implements OnInit {
  inquiries: (BuyerInquiry & { showFullMessage: boolean } & { isLongMessage: boolean})[] = [];
  filteredInquiries: (BuyerInquiry & { showFullMessage: boolean; isLongMessage: boolean })[] = [];
  loading = true;
  searchTitle = '';
  searchLocation = '';
  searchListerName = '';

  onSearch(): void {
    const title = this.searchTitle.toLowerCase().trim();
    const location = this.searchLocation.toLowerCase().trim();
    const lister = this.searchListerName.toLowerCase().trim();

    this.filteredInquiries = this.inquiries.filter(inquiry =>
      inquiry.propertyTitle.toLowerCase().includes(title) &&
      inquiry.location.toLowerCase().includes(location) &&
      inquiry.listerName.toLowerCase().includes(lister)
    );
  }


  constructor(
    private inquiryService: InquiryService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const buyerId = this.authService.currentUser?.id;
    if (!buyerId) return;

    this.inquiryService.getBuyerInquiries(buyerId).subscribe({
      next: (res) => {
        this.inquiries = res.data.map((inquiry : BuyerInquiry) => ({
          ...inquiry,
          showFullMessage: false,
          isLongMessage: inquiry.message.length > 150 
        }));
        this.filteredInquiries = this.inquiries;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load inquiries', err);
        this.loading = false;
      }
    });
  }
}
