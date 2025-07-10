import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { BulkPropertyImageUploadRequest } from '../../models/property-images-bulk-upload.model';
import { PropertyImageModel } from '../../models/property-image.model';

@Injectable()
export class PropertyImageService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/PropertyImage`;

  constructor(private http: HttpClient) {}

    getImagesByPropertyId(propertyId: string): Observable<PropertyImageModel[]> {
        return this.http.get<PropertyImageModel[]>(`${this.baseUrl}/by-property/${propertyId}`);
    }

    uploadImages(dto: BulkPropertyImageUploadRequest): Observable<any> {
        const formData = new FormData();
        formData.append('PropertyId', dto.propertyId);
        dto.files.forEach(file => formData.append('Files', file)); 

        return this.http.post(`${this.baseUrl}/upload-multiple`, formData);
    }

    deleteImage(imageId: string): Observable<any> {
        return this.http.delete(`${this.baseUrl}/image/${imageId}`);
    }

}
