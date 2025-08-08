import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  DownloadIcon,
  FileTextIcon,
  ImageIcon,
  LucideAngularModule,
} from 'lucide-angular';
import { AuctionService } from '../../../core/services/auction.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-view-auction-attachements',
  imports: [LucideAngularModule],
  templateUrl: './view-auction-attachements.html',
})
export class ViewAuctionAttachements implements OnInit {
  readonly image = ImageIcon;
  readonly fileText = FileTextIcon;
  readonly download = DownloadIcon;
  constructor(
    private route: ActivatedRoute,
    private auctionService: AuctionService,
    private router: Router,
    private authService: AuthService
  ) {}
  auctionId: string = '';
  auctionData: any[] = [];
  role: string = '';

  ngOnInit(): void {
    this.auctionId = this.route.snapshot.params['auctionId'];
    this.auctionService.getAuctionByAuctionId(this.auctionId).subscribe({
      next: (res) => {
        this.auctionData = res.data?.files?.$values;
      },
      error: (err) => {
        console.log(err);
      },
    });

    this.authService.authme().subscribe({
      next: (res) => {
        this.role = res.data.role.toLowerCase();
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  getfileAttachment(file: any) {
    this.auctionService.getfile(this.auctionId, file.name).subscribe((res) => {
      const url = window.URL.createObjectURL(res);
      const a = document.createElement('a');
      a.href = url;
      a.download = file.name;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  isImage(type: string | undefined | null): boolean {
    return typeof type === 'string' && type.startsWith('image/');
  }

  viewFile(fileName: string) {
    this.auctionService.getfile(this.auctionId, fileName).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      window.open(url);
    });
  }

  goBackToAuctionList() {
    const queryParams = this.route.snapshot.queryParams;

    if (this.role === 'seller') {
      this.router.navigate(['/seller/view-auctions'], { queryParams });
    } else if (this.role === 'admin') {
      this.router.navigate(['/admin/manage-auctions'], { queryParams });
    }
  }
}
