import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainingVideoList } from './training-video-list';

describe('TrainingVideoList', () => {
  let component: TrainingVideoList;
  let fixture: ComponentFixture<TrainingVideoList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrainingVideoList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrainingVideoList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
