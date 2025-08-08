import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BidFilter } from './bid-filter';

describe('BidFilter', () => {
  let component: BidFilter;
  let fixture: ComponentFixture<BidFilter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BidFilter]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BidFilter);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
