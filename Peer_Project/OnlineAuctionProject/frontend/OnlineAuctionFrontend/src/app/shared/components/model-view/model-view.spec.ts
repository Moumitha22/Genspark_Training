import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModelView } from './model-view';

describe('ModelView', () => {
  let component: ModelView;
  let fixture: ComponentFixture<ModelView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModelView],
    }).compileComponents();

    fixture = TestBed.createComponent(ModelView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
