import { Component, inject, OnInit } from '@angular/core';
import { PostPropertyMetadataComponent } from '../../components/post-property-metadata-form/post-property-metadata';
import { PostPropertyFormComponent } from '../../components/post-property-form/post-property-form';
import { CommonModule } from '@angular/common';
import { ListerGuidelinesComponent } from '../../components/lister-guidelines/lister-guidelines';
import { PropertyFormStateService } from '../../core/services/property-form-state.service';

@Component({
  selector: 'app-post-property-stepper',
  imports: [PostPropertyMetadataComponent, PostPropertyFormComponent, ListerGuidelinesComponent, CommonModule],
  templateUrl: './post-property-stepper.html',
  styleUrl: './post-property-stepper.css'
})
export class PostPropertyStepperComponent implements OnInit {
  private formState = inject(PropertyFormStateService);

  step = 1;
  
  listerType = '';
  listingPurpose = '';
  propertyType = '';

  ngOnInit(): void {
    this.formState.reset(); 
  }

  handleStep1Complete(data: { listerType: string; listingPurpose: string; propertyType: string }) {
    this.listerType = data.listerType;
    this.listingPurpose = data.listingPurpose;
    this.propertyType = data.propertyType;
    this.step = 2;
  }

  backToStep1() {
    this.step = 1;
  }
}