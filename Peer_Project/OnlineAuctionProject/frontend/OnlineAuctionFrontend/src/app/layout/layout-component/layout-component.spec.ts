import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LayoutComponent } from './layout-component';
import { AuthService } from '../../core/services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

const mockActivatedRoute = {
  snapshot: {
    params: { id: '1' },
    paramMap: {
      get: (key: string) => (key === 'id' ? '1' : null),
    },
  },
};
describe('LayoutComponent', () => {
  let component: LayoutComponent;
  let fixture: ComponentFixture<LayoutComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(waitForAsync(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'getUserRole',
      'getEmailFromToken',
      'authme',
    ]);
    authServiceSpy.getUserRole.and.returnValue('Bidder');
    authServiceSpy.authme.and.returnValue(
      of({
        data: { role: 'Bidder', email: 'test@mail.com', username: 'TestUser' },
      })
    );
    TestBed.configureTestingModule({
      imports: [LayoutComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
      ],
    }).compileComponents();
  }));
  beforeEach(async () => {
    fixture = TestBed.createComponent(LayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
