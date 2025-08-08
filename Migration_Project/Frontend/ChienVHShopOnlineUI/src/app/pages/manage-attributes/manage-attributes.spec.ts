import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageAttributes } from './manage-attributes';

describe('ManageAttributes', () => {
  let component: ManageAttributes;
  let fixture: ComponentFixture<ManageAttributes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageAttributes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageAttributes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
