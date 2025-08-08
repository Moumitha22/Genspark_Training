import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuctionDetailsCard } from './auction-details-card';

describe('AuctionDetailsCard', () => {
  let component: AuctionDetailsCard;
  let fixture: ComponentFixture<AuctionDetailsCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuctionDetailsCard],
    }).compileComponents();

    fixture = TestBed.createComponent(AuctionDetailsCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
