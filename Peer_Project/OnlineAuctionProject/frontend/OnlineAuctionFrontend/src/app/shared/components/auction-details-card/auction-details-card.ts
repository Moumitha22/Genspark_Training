import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-auction-details-card',
  imports: [CommonModule],
  templateUrl: './auction-details-card.html',
})
export class AuctionDetailsCard {
  @Input() selectedAuction: any;
  @Output() close = new EventEmitter<void>();

  closeAuctionModal() {
    this.close.emit();
  }
}
