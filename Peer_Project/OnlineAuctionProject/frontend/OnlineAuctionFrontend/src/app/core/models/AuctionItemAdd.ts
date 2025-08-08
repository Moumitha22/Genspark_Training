export interface AuctionItemAddDto {
  name: string;
  description: string;
  startTime: string;
  endTime: string;
  status: string;
  sellerId: string;
  startingPrice: number;
  reservePrice?: number;
  files?: File[];
}
