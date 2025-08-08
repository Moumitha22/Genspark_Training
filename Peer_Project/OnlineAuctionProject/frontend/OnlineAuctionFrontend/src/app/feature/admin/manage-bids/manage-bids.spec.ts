import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageBids } from './manage-bids';

describe('ManageBids', () => {
  let component: ManageBids;
  let fixture: ComponentFixture<ManageBids>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageBids]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageBids);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
