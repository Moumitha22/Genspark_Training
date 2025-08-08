import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PostNews } from './post-news';

describe('PostNews', () => {
  let component: PostNews;
  let fixture: ComponentFixture<PostNews>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PostNews]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PostNews);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
