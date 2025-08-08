import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BidsWinningStatus } from './bids-winning-status';

describe('BidsWinningStatus', () => {
  let component: BidsWinningStatus;
  let fixture: ComponentFixture<BidsWinningStatus>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BidsWinningStatus],
    }).compileComponents();

    fixture = TestBed.createComponent(BidsWinningStatus);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
