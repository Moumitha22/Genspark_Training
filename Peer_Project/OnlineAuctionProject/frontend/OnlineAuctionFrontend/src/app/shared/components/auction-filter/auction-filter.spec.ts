import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { AuctionFilter } from './auction-filter';
import { ReactiveFormsModule } from '@angular/forms';

describe('AuctionFilter', () => {
  let component: AuctionFilter;
  let fixture: ComponentFixture<AuctionFilter>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AuctionFilter, ReactiveFormsModule],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuctionFilter);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should emit correct filters on apply', () => {
    spyOn(component.applyFilters, 'emit');
    component.filterForm.setValue({
      startTime: '2025-01-01',
      endTime: '2025-01-10',
      sortBy: 'createdAt',
      sortDirection: 'asc',
      status: 'Live',
    });
    component.onApply();
    expect(component.applyFilters.emit).toHaveBeenCalledWith(
      jasmine.objectContaining({
        startTime: new Date('2025-01-01').toISOString(),
        endTime: new Date('2025-01-10').toISOString(),
        sortBy: 'createdAt',
        sortDirection: 'asc',
        status: 'Live',
      })
    );
  });

  it('should emit empty object on reset', () => {
    spyOn(component.applyFilters, 'emit');
    component.filterForm.setValue({
      startTime: '2025-01-01',
      endTime: '2025-01-10',
      sortBy: 'createdAt',
      sortDirection: 'asc',
      status: 'Live',
    });
    component.onReset();
    expect(component.applyFilters.emit).toHaveBeenCalledWith({});
    expect(component.filterForm.value).toEqual({
      startTime: null,
      endTime: null,
      sortBy: '',
      sortDirection: '',
      status: '',
    });
  });
});
