import { Injectable } from '@angular/core';
import { PropertyAddRequest } from '../../models/property-add-request.model';

@Injectable({ providedIn: 'root' })
export class PropertyFormStateService {
  private state: Partial<PropertyAddRequest> = {};

  setMetadata(listerType: string, listingPurpose: string, propertyType: string) {
    this.state.listerType = listerType;
    this.state.listingPurpose = listingPurpose;
    this.state.propertyType = propertyType;
  }

  getMetadata() {
    const { listerType, listingPurpose, propertyType } = this.state;
    return { listerType, listingPurpose, propertyType };
  }

  setFormData(data: PropertyAddRequest) {
    this.state = { ...data };
  }

  getFormData(): Partial<PropertyAddRequest> {
    return this.state;
  }

  reset() {
    this.state = {};
  }
}
