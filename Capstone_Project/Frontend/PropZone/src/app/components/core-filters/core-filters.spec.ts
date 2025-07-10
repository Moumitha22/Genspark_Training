import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CoreFiltersComponent } from './core-filters';
import { By } from '@angular/platform-browser';

describe('CoreFiltersComponent', () => {
  let component: CoreFiltersComponent;
  let fixture: ComponentFixture<CoreFiltersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoreFiltersComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(CoreFiltersComponent);
    component = fixture.componentInstance;

    // Set initial model to avoid undefined errors
    component.model = {
      listingPurpose: 'Sale',
      sortBy: 'CreatedAt',
      ascending: false
    };

    fixture.detectChanges();
  });


  it('should emit filtersChanged when location/keyword is entered', () => {
    spyOn(component.filtersChanged, 'emit');
    const input = fixture.debugElement.query(By.css('input[placeholder="Enter location"]')).nativeElement;
    input.value = 'Chennai';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    expect(component.model.city).toBe('Chennai');
    expect(component.filtersChanged.emit).toHaveBeenCalledWith(component.model);
  });

  it('should update sortBy and ascending on sort option change to priceAsc/newest', () => {
    component.selectedSortOption = 'priceAsc';
    spyOn(component.filtersChanged, 'emit');

    component.onSortChange();
    expect(component.model.sortBy).toBe('Price');
    expect(component.model.ascending).toBeTrue();
    expect(component.filtersChanged.emit).toHaveBeenCalledWith(component.model);
  });

  it('should emit filtersChanged when purpose is changed', () => {
    spyOn(component.filtersChanged, 'emit');
    component.model.listingPurpose = 'Rent';
    component.onFilterChange();
    expect(component.filtersChanged.emit).toHaveBeenCalledWith(component.model);
  });

  it('should emit filtersChanged when property type changes', () => {
    spyOn(component.filtersChanged, 'emit');
    component.model.propertyTypes = ['Apartment'];
    component.onFilterChange();
    expect(component.filtersChanged.emit).toHaveBeenCalledWith(component.model);
  });

  it('should emit filtersChanged when lister type changes', () => {
    spyOn(component.filtersChanged, 'emit');
    component.model.listerTypes = ['Owner'];
    component.onFilterChange();
    expect(component.filtersChanged.emit).toHaveBeenCalledWith(component.model);
  });
});
