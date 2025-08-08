import { Component, Input } from '@angular/core';
import { ConfirmModal } from './confirm-modal';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { ModelView } from '../model-view/model-view';

@Component({
  selector: 'lucide-angular',
  template: '',
  standalone: true,
})
class MockLucideIconComponent {
  @Input() img: any;
}

describe('ConfirmModal', () => {
  let component: ConfirmModal;
  let fixture: ComponentFixture<ConfirmModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmModal, FormsModule, MockLucideIconComponent],
    })
    .overrideComponent(ConfirmModal, {
      set: {
        imports: [FormsModule, MockLucideIconComponent, ModelView], 
      }
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfirmModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});