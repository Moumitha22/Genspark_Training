import { CommonModule } from "@angular/common";
import { PropertyFormStateService } from "../../core/services/property-form-state.service";
import { PostPropertyFormComponent } from "../post-property-form/post-property-form";
import { Component, EventEmitter, inject, OnInit, Output } from "@angular/core";


@Component({
  selector: 'app-post-property-metadata',
  imports: [CommonModule, PostPropertyFormComponent],
  templateUrl: './post-property-metadata.html',
  styleUrls: ['./post-property-metadata.css']
})
export class PostPropertyMetadataComponent implements OnInit {
  @Output() next = new EventEmitter<{ listerType: string; listingPurpose: string; propertyType: string }>();
  
  private stateService = inject(PropertyFormStateService);
  
  listerType: 'Owner' | 'Agent' | '' = '';
  listingPurpose: 'Sale' | 'Rent' | '' = '';
  propertyType: 'Apartment' | 'House' | 'CommercialSpace' | 'Plot' | '' = '';

  propertyTypes: Array<'' | 'Apartment' | 'House' | 'CommercialSpace' | 'Plot'> = [
    'Apartment', 'House', 'CommercialSpace', 'Plot'
  ];
  

  ngOnInit(): void {
    
    const saved = this.stateService.getMetadata();
    this.listerType = saved.listerType as 'Owner' | 'Agent' | '';
    this.listingPurpose = saved.listingPurpose as 'Sale' | 'Rent' | '';
    this.propertyType = saved.propertyType as 'Apartment' | 'House' | 'CommercialSpace' | 'Plot' | '';
  }



  isValid(): boolean {
    return !!(this.listerType && this.listingPurpose && this.propertyType);
  }

  onNext(): void {
    if (!this.isValid()) return;

    this.stateService.setMetadata(this.listerType, this.listingPurpose, this.propertyType);

    this.next.emit({
      listerType: this.listerType,
      listingPurpose: this.listingPurpose,
      propertyType: this.propertyType
    });
  }
}