import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LoginResponse } from '../../models/login-response.model';
import { LoginRequest } from '../../models/login-request.model';
import { RegisterRequest } from '../../models/register-request.model';
import { UserService } from './user.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let mockUserService: jasmine.SpyObj<UserService>;

  const dummyToken = [
    btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' })), // Header
    btoa(JSON.stringify({ nameid: '123', email: 'test@example.com', role: 'Buyer' })), // Payload
    'signature' // Signature
  ].join('.');

  const baseUrl = 'http://localhost:5138/api/v1/Authentication';

  beforeEach(() => {
    mockUserService = jasmine.createSpyObj('UserService', ['loadCurrentUser']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: UserService, useValue: mockUserService }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should register a user', () => {
    const registerData: RegisterRequest = {
      name: 'Alice',
      email: 'alice@example.com',
      password: 'pass123',
      role: 'Buyer'
    };

    service.register(registerData).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/register`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(registerData);
    req.flush({ success: true });
  });


    it('should login and set token and call loadCurrentUser', () => {
    const loginData: LoginRequest = {
        email: 'test@example.com',
        password: 'pass123',
        role: 'Buyer'
    };

    const loginResponse: LoginResponse = {
        username: 'Test',
        accessToken: dummyToken
    };

        service.login(loginData).subscribe(res => {
        const actual = res as unknown as { data: LoginResponse };

        expect(actual.data.username).toBe('Test');
        expect(actual.data.accessToken).toBe(dummyToken);
        expect(service.currentUser?.email).toBe('test@example.com');
        expect(mockUserService.loadCurrentUser).toHaveBeenCalled();
        });


    const req = httpMock.expectOne(`${baseUrl}/login`);
    expect(req.request.method).toBe('POST');
    req.flush({ data: loginResponse });
    });

    it('should refresh token and update user', () => {
    const loginResponse: LoginResponse = {
        username: 'Test',
        accessToken: dummyToken
    };

    service.refreshToken().subscribe(res => {
        const actual = res as unknown as { data: LoginResponse };
        expect(actual.data.username).toBe('Test');
        expect(actual.data.accessToken).toBe(dummyToken);
        expect(service.currentUser?.email).toBe('test@example.com');
    });

    const req = httpMock.expectOne(`${baseUrl}/refresh-token`);
    expect(req.request.method).toBe('POST');
    req.flush({ data: loginResponse });
    });


  it('should clear local state and reset user', () => {
    service['setAccessToken'](dummyToken);
    expect(localStorage.getItem('accessToken')).toBeTruthy();

    service.clearLocalState();

    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(service.currentUser).toBeNull();
  });

  it('should return access token from local storage', () => {
    service['setAccessToken'](dummyToken);
    expect(service.getAccessToken()).toBe(dummyToken);
  });

  it('should expose userRole$ and isLoggedIn$ as observables', (done) => {
    service['setAccessToken'](dummyToken);

    let roleChecked = false;
    let loginChecked = false;

    service.userRole$.subscribe(role => {
      expect(role).toBe('Buyer');
      roleChecked = true;
      if (roleChecked && loginChecked) done();
    });

    service.isLoggedIn$.subscribe(isLogged => {
      expect(isLogged).toBeTrue();
      loginChecked = true;
      if (roleChecked && loginChecked) done();
    });
  });
});
