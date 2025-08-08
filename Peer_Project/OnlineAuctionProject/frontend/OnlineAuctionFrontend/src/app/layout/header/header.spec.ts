import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { Header } from './header';
import { Renderer2 } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { of } from 'rxjs';

describe('Header', () => {
  let component: Header;
  let fixture: ComponentFixture<Header>;
  let rendererSpy: jasmine.SpyObj<Renderer2>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(waitForAsync(() => {
    rendererSpy = jasmine.createSpyObj('Renderer2', [
      'addClass',
      'removeClass',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['authme']);

    TestBed.configureTestingModule({
      imports: [Header],
      providers: [
        { provide: Renderer2, useValue: rendererSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    localStorage.clear();
    authServiceSpy.authme.and.returnValue(of({ data: { role: 'Admin' } }));
    fixture = TestBed.createComponent(Header);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should set role from authService', () => {
    component.getUserRole();
    expect(authServiceSpy.authme).toHaveBeenCalled();
    expect(component.role).toBe('Admin');
  });

  it('should call enableDarkTheme from setDarkTheme', () => {
    spyOn(component, 'enableDarkTheme');
    component.setDarkTheme();
    expect(component.enableDarkTheme).toHaveBeenCalled();
  });

  it('should call enableLightTheme from setLightTheme', () => {
    spyOn(component, 'enableLightTheme');
    component.setLightTheme();
    expect(component.enableLightTheme).toHaveBeenCalled();
  });
});
