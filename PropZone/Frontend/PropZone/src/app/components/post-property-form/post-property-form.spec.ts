import { ComponentFixture, TestBed, fakeAsync, tick  } from '@angular/core/testing';
import { PostPropertyFormComponent } from './post-property-form';
import { PropertyService } from '../../core/services/property.service';
import { FeatureService } from '../../core/services/feature.service';
import { PropertyFormStateService } from '../../core/services/property-form-state.service';
import { NotificationService } from '../../core/services/notification.service';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms'
import { flush } from '@angular/core/testing';

describe('PostPropertyFormComponent', () => {
  let component: PostPropertyFormComponent;
  let fixture: ComponentFixture<PostPropertyFormComponent>;

  const mockPropertyService = {
    createProperty: jasmine.createSpy('createProperty').and.returnValue(of({}))
  };

  const mockFeatureService = {
    getApplicableFeatures: jasmine.createSpy('getApplicableFeatures').and.returnValue(
      of({
        data: [
          {
            id: 'f1',
            name: 'Pet Friendly',
            dataType: 'boolean',
            options: []
          },
          {
            id: 'f2',
            name: 'Furnishing',
            dataType: 'dropdown',
            options: [{ id: 'opt1', value: 'Furnished' }]
          }
        ]
      })
    )
  };

  const mockStateService = {
    getMetadata: () => ({
      listerType: 'Owner',
      listingPurpose: 'Sale',
      propertyType: 'Apartment'
    }),
    setMetadata: jasmine.createSpy()
  };

  const mockNotificationService = {
    success: jasmine.createSpy('success'),
    error: jasmine.createSpy('error')
  };

  const mockRouter = {
    navigate: jasmine.createSpy('navigate')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PostPropertyFormComponent, ReactiveFormsModule],
      providers: [
        { provide: PropertyService, useValue: mockPropertyService },
        { provide: FeatureService, useValue: mockFeatureService },
        { provide: PropertyFormStateService, useValue: mockStateService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PostPropertyFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should show error notification on submission failure', fakeAsync(() => {
    mockPropertyService.createProperty.and.returnValue(throwError(() => ({
      error: { message: 'Error occurred' }
    })));

    component.propertyForm.patchValue({
      title: 'Error Test',
      price: 500000,
      areaSqFt: 800,
      location: { city: 'City', state: 'State', locality: 'Locality' }
    });

    component.featuresFormArray.at(0).patchValue({ value: false });

    component.onSubmit();
    tick();

    expect(mockNotificationService.error).toHaveBeenCalledWith('❌ Error: Error occurred');
  }));

});
