import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { UploadPropertyImagesComponent } from './upload-images';
import { of, throwError } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyImageService } from '../../core/services/property-image.service';
import { NotificationService } from '../../core/services/notification.service';
import { FormBuilder } from '@angular/forms';

describe('UploadPropertyImagesComponent', () => {
  let component: UploadPropertyImagesComponent;
  let fixture: ComponentFixture<UploadPropertyImagesComponent>;
  let router: jasmine.SpyObj<Router>;

  const mockRoute = {
    snapshot: {
      paramMap: {
        get: (key: string) => 'prop123'
      }
    }
  };

  const mockImageService = {
    getImagesByPropertyId: jasmine.createSpy(),
    uploadImages: jasmine.createSpy(),
    deleteImage: jasmine.createSpy()
  };

  const mockNotificationService = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy()
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UploadPropertyImagesComponent],
      providers: [
        FormBuilder,
        { provide: ActivatedRoute, useValue: mockRoute },
        { provide: PropertyImageService, useValue: mockImageService },
        { provide: NotificationService, useValue: mockNotificationService },
        {
          provide: Router,
          useValue: jasmine.createSpyObj('Router', ['navigate'])
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UploadPropertyImagesComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should initialize and load existing images', fakeAsync(() => {
    const mockImages = [{
      id: 'img1',
      propertyId: 'prop123',
      imageUrl: 'http://example.com/image1.jpg'
    }];
    mockImageService.getImagesByPropertyId.and.returnValue(of({ data: mockImages }));

    component.ngOnInit();
    tick();

    expect(component.propertyId).toBe('prop123');
    expect(component.existingImages).toEqual(mockImages);
  }));

  it('should show error when image fetch fails', fakeAsync(() => {
    mockImageService.getImagesByPropertyId.and.returnValue(throwError(() => new Error()));
    component.ngOnInit();
    tick();
    expect(component.errorMsg).toContain('Failed to load existing images');
  }));

  it('should remove selected file and show notification', () => {
    const file = new File(['data'], 'test.jpg', { type: 'image/jpeg' });
    component.selectedFiles = [file];
    component.removeSelectedFile(0);
    expect(component.selectedFiles.length).toBe(0);
    expect(mockNotificationService.success).toHaveBeenCalledWith('Image deleted successfully');
  });

  it('should delete existing image and update list', () => {
    component.existingImages = [{
      id: 'img1',
      propertyId: 'prop123',
      imageUrl: 'http://example.com/img1.jpg'
    }];
    mockImageService.deleteImage.and.returnValue(of({}));

    component.deleteImage('img1');

    expect(mockImageService.deleteImage).toHaveBeenCalledWith('img1');
    expect(component.existingImages.length).toBe(0);
    expect(mockNotificationService.success).toHaveBeenCalledWith('Image deleted successfully');
  });

 
  it('should upload images and navigate on success', fakeAsync(() => {
    component.propertyId = 'prop123';
    const file = new File(['data'], 'test.jpg', { type: 'image/jpeg' });
    component.selectedFiles = [file];

    mockImageService.uploadImages.and.returnValue(of({}));

    component.upload();
    tick();

    expect(mockImageService.uploadImages).toHaveBeenCalled();
    expect(component.uploadSuccess).toBeTrue();
    expect(router.navigate).toHaveBeenCalledWith(['/property', 'prop123']);
    expect(mockNotificationService.success).toHaveBeenCalledWith('Images updated successfully');
  }));


});
