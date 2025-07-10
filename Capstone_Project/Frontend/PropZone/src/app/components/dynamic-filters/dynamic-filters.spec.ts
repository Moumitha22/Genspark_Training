import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DynamicFiltersComponent } from './dynamic-filters';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';
import { By } from '@angular/platform-browser';

describe('DynamicFiltersComponent', () => {
  let component: DynamicFiltersComponent;
  let fixture: ComponentFixture<DynamicFiltersComponent>;

  const mockFilters: DynamicFeatureModel[] = [
    {
        id: 'f1',
        name: 'Pet Friendly',
        filterMode: 'Boolean',
        dataType: 'boolean',
        options: []
    },
    {
        id: 'f2',
        name: 'Furnishing',
        filterMode: 'Exact',
        dataType: 'string',
        options: [
        { id: 'opt1', value: 'Furnished' },
        { id: 'opt2', value: 'Unfurnished' }
        ]
    },
    {
        id: 'f3',
        name: 'Price Range',
        filterMode: 'Range',
        dataType: 'number',
        options: []
    }
    ];


  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DynamicFiltersComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(DynamicFiltersComponent);
    component = fixture.componentInstance;
    component.dynamicFilters = mockFilters;
    fixture.detectChanges();
  });

  it('should emit exact filter when checkboxes are selected/deselected', () => {
    spyOn(component.filtersChanged, 'emit');

    const allCheckboxes = fixture.debugElement.queryAll(By.css('input[type="checkbox"]'));
    const exactCheckboxes = allCheckboxes.slice(1, 3); // assuming 2 exact options

    const firstCheckbox = exactCheckboxes[0].nativeElement;
    const secondCheckbox = exactCheckboxes[1].nativeElement;

    firstCheckbox.checked = true;
    firstCheckbox.dispatchEvent(new Event('change'));

    secondCheckbox.checked = true;
    secondCheckbox.dispatchEvent(new Event('change'));

    fixture.detectChanges();

    expect(component.filtersChanged.emit).toHaveBeenCalledWith({
        f2: ['Furnished', 'Unfurnished']
    });

    firstCheckbox.checked = false;
    firstCheckbox.dispatchEvent(new Event('change'));

    fixture.detectChanges();

    expect(component.filtersChanged.emit).toHaveBeenCalledWith({
        f2: ['Unfurnished']
    });
    });



  it('should emit range filter when min/max values are entered', () => {
    spyOn(component.filtersChanged, 'emit');

    const minInput = fixture.debugElement.queryAll(By.css('input[placeholder="Min"]'))[0].nativeElement;
    const maxInput = fixture.debugElement.queryAll(By.css('input[placeholder="Max"]'))[0].nativeElement;

    minInput.value = '1000';
    minInput.dispatchEvent(new Event('input'));

    maxInput.value = '5000';
    maxInput.dispatchEvent(new Event('input'));

    fixture.detectChanges();

    expect(component.filtersChanged.emit).toHaveBeenCalledWith({
      f3: ['1000', '5000']
    });
  });
});
