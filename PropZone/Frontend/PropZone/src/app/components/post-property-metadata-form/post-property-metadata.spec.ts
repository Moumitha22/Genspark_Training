import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PostPropertyMetadataComponent } from './post-property-metadata';
import { PropertyFormStateService } from '../../core/services/property-form-state.service';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

const mockStateService = {
  getMetadata: jasmine.createSpy().and.returnValue({
    listerType: '',
    listingPurpose: '',
    propertyType: ''
  }),
  setMetadata: jasmine.createSpy()
};

describe('PostPropertyMetadataComponent', () => {
  let component: PostPropertyMetadataComponent;
  let fixture: ComponentFixture<PostPropertyMetadataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PostPropertyMetadataComponent],
      providers: [
        { provide: PropertyFormStateService, useValue: mockStateService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PostPropertyMetadataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should initialize with values from state service', () => {
    expect(mockStateService.getMetadata).toHaveBeenCalled();
    expect(component.listerType).toBe('');
    expect(component.listingPurpose).toBe('');
    expect(component.propertyType).toBe('');
  });

  it('should not emit next if form is invalid', () => {
    spyOn(component.next, 'emit');
    component.listerType = '';
    component.listingPurpose = '';
    component.propertyType = '';
    fixture.detectChanges();

    component.onNext();
    expect(component.next.emit).not.toHaveBeenCalled();
  });

  it('should emit next with valid data', () => {
    spyOn(component.next, 'emit');

    component.listerType = 'Owner';
    component.listingPurpose = 'Sale';
    component.propertyType = 'Apartment';
    fixture.detectChanges();

    component.onNext();

    expect(mockStateService.setMetadata).toHaveBeenCalledWith('Owner', 'Sale', 'Apartment');
    expect(component.next.emit).toHaveBeenCalledWith({
      listerType: 'Owner',
      listingPurpose: 'Sale',
      propertyType: 'Apartment'
    });
  });

  it('should disable Next button if form is incomplete', () => {
    component.listerType = 'Owner';
    component.listingPurpose = '';
    component.propertyType = '';
    fixture.detectChanges();

    const nextBtn = fixture.debugElement.query(By.css('button.btn-primary')).nativeElement;
    expect(nextBtn.disabled).toBeTrue();
  });

  it('should enable Next button if form is valid', () => {
    component.listerType = 'Owner';
    component.listingPurpose = 'Sale';
    component.propertyType = 'Apartment';
    fixture.detectChanges();

    const nextBtn = fixture.debugElement.query(By.css('button.btn-primary')).nativeElement;
    expect(nextBtn.disabled).toBeFalse();
  });
});
