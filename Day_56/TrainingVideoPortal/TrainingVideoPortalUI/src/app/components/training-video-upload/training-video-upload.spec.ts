import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainingVideoUpload } from './training-video-upload';

describe('TrainingVideoUpload', () => {
  let component: TrainingVideoUpload;
  let fixture: ComponentFixture<TrainingVideoUpload>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrainingVideoUpload]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrainingVideoUpload);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
