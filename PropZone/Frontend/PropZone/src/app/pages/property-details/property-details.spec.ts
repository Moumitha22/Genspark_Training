import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { PropertyDetailsComponent } from './property-details';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyService } from '../../core/services/property.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { of, throwError } from 'rxjs';
import { PropertyModel } from '../../models/property.model';
import { RouterTestingModule } from '@angular/router/testing';

describe('PropertyDetailsComponent', () => {
  let component: PropertyDetailsComponent;
  let fixture: ComponentFixture<PropertyDetailsComponent>;
  let router: Router;

  const mockProperty: PropertyModel = {
    id: 'prop1',
    title: 'Ocean View Flat',
    description: 'A nice place',
    imageUrls: ['images/prop1.jpg'],
    location: { locality: 'Beachside', city: 'Goa', state: 'GA' },
    listerId: 'lister1'
  } as PropertyModel;

  const mockPropertyService = {
    getPropertyById: jasmine.createSpy().and.returnValue(of({ data: mockProperty }))
  };

  const mockAuthService = {
    currentUser: { id: 'lister1', role: 'Lister' }
  };

  const mockNotificationService = {
    warning: jasmine.createSpy()
  };

  const mockActivatedRoute = {
    snapshot: {
      paramMap: {
        get: () => 'prop1'
      }
    }
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PropertyDetailsComponent, RouterTestingModule],
      providers: [
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: PropertyService, useValue: mockPropertyService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigate');

    fixture = TestBed.createComponent(PropertyDetailsComponent);
    component = fixture.componentInstance;
  });

  it('should navigate to edit page if user can edit', () => {
    fixture.detectChanges();
    component.property = mockProperty;
    component.goToEdit();
    expect(router.navigate).toHaveBeenCalledWith(['/my-properties', 'prop1', 'edit']);
  });

  it('should not allow non-logged-in users to contact', () => {
    mockAuthService.currentUser = null as any;
    fixture.detectChanges();
    component.property = mockProperty;
    component.propertyData = {
      id: 'prop1',
      title: 'Ocean View Flat',
      location: 'Beachside,Goa,GA'
    };
    component.handleContactClicked();
    expect(mockNotificationService.warning).toHaveBeenCalledWith('Please log in to contact the lister');
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should not allow non-buyer users to contact', () => {
    mockAuthService.currentUser = { id: 'lister2', role: 'Lister' };
    fixture.detectChanges();
    component.property = mockProperty;
    component.propertyData = {
      id: 'prop1',
      title: 'Ocean View Flat',
      location: 'Beachside,Goa,GA'
    };
    component.handleContactClicked();
    expect(mockNotificationService.warning).toHaveBeenCalledWith(
      'Only buyers can contact listers. Please login as buyer to continue.'
    );
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should allow buyers to contact and set selected property', () => {
    mockAuthService.currentUser = { id: 'buyer123', role: 'Buyer' };
    fixture.detectChanges();
    component.property = mockProperty;
    component.propertyData = {
      id: 'prop1',
      title: 'Ocean View Flat',
      location: 'Beachside,Goa,GA'
    };
    component.handleContactClicked();
    expect(component.selectedProperty?.id).toBe('prop1');
  });

  it('should handle image navigation', () => {
    component.property = { ...mockProperty, imageUrls: ['1.jpg', '2.jpg', '3.jpg'] };
    component.currentIndex = 1;
    expect(component.images.length).toBe(3);
    component.nextImage();
    expect(component.currentIndex).toBe(2);
    component.prevImage();
    expect(component.currentIndex).toBe(1);
  });

  it('should not change index when at bounds', () => {
    component.property = { ...mockProperty, imageUrls: ['1.jpg'] };
    component.currentIndex = 0;
    component.prevImage();
    expect(component.currentIndex).toBe(0);
    component.nextImage();
    expect(component.currentIndex).toBe(0);
  });

  it('should return true for canEditProperty for Admin', () => {
    mockAuthService.currentUser = { id: 'admin', role: 'Admin' };
    component.property = mockProperty;
    expect(component.canEditProperty()).toBeTrue();
  });

  it('should return false for canEditProperty if not owner', () => {
    mockAuthService.currentUser = { id: 'other', role: 'Lister' };
    component.property = mockProperty;
    expect(component.canEditProperty()).toBeFalse();
  });

  it('should handle property load error', fakeAsync(() => {
    mockPropertyService.getPropertyById = jasmine.createSpy().and.returnValue(throwError(() => new Error('404')));
    fixture = TestBed.createComponent(PropertyDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    tick();
    expect(component.property).toBeUndefined();
  }));
});
