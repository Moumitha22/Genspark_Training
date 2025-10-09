import { Component, OnInit } from '@angular/core';
import { HeroComponent } from '../../components/hero/hero';
import { HttpClient } from '@angular/common/http';
import { PropertyTypesSectionComponent } from '../../components/property-types-section/property-types-section';
import { Footer } from '../../components/footer/footer';

@Component({
  selector: 'app-landing',
  imports: [HeroComponent, PropertyTypesSectionComponent, Footer],
  templateUrl: './landing.html',
  styleUrl: './landing.css'
})
export class Landing {



}
