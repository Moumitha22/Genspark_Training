import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PostProduct } from './post-product';

describe('PostProduct', () => {
  let component: PostProduct;
  let fixture: ComponentFixture<PostProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PostProduct]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PostProduct);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
