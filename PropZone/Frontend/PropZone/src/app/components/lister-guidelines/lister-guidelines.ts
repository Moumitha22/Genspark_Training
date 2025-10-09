import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-lister-guidelines',
  imports: [CommonModule],
  templateUrl: './lister-guidelines.html',
  styleUrl: './lister-guidelines.css'
})
export class ListerGuidelinesComponent {

  guidelines = [
  {
    icon: 'fas fa-clipboard-list',
    title: 'Post your Property Ad',
    text: 'Enter all details like title, description, price, location, etc.,. accurately.'
  },
  {
    icon: 'fas fa-map-marker-alt',
    title: 'Choose Correct Location',
    text: 'Fill in accurate location details to ensure you receive genuine and relevant buyer inquiries.'
  },
  {
    icon: 'fas fa-list-alt',
    title: 'Add Additional Details',
    text: 'Mention features suitable to you preperty to increase visibility in search results.'
  },
  {
    icon: 'fas fa-check-circle',
    title: 'Be Transparent and Honest',
    text: 'Ensure all the information provided is truthful. Transparency builds trust and helps avoid legal or buyer issues later.'
  },
  {
    icon: 'fas fa-camera',
    title: 'Add Quality Photos',
    text: 'Add high-quality photos to help your property stand out and gain attention from serious buyers.'
  }
];


}
