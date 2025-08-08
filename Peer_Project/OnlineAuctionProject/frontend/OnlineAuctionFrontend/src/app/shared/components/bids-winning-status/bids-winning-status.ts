import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-bids-winning-status',
  imports: [],
  templateUrl: './bids-winning-status.html',
})
export class BidsWinningStatus {
  @Input() bid: string = '';
  @Input() auctionItems: any[] = [];

  getBidStatus(bid: any): 'Win' | 'Lose' | 'Pending' {
    const auction = this.auctionItems.find((a) => a.id === bid.auctionItemId);
    if (!auction) return 'Pending';

    const status = (auction.status || '').toLowerCase();
    if (['closed', 'completed', 'cancelled'].includes(status)) {
      if (auction.winnerId === bid.id) return 'Win';
      return 'Lose';
    }
    return 'Pending';
  }
}
