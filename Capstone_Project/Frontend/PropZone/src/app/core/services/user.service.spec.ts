import { TestBed } from '@angular/core/testing';
import { UserService } from './user.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { User } from '../../models/user.model';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5138/api/v1/User';

  const mockUser: User = {
    id: 'u1',
    name: 'Test User',
    email: 'test@example.com',
    phoneNumber: '1234567890',
    role: 'Buyer',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get current user', () => {
    service.getCurrentUser().subscribe(user => {
      expect(user).toEqual(mockUser);
    });

    const req = httpMock.expectOne(`${baseUrl}/me`);
    expect(req.request.method).toBe('GET');
    req.flush(mockUser);
  });

  it('should load current user and update BehaviorSubject', () => {
    service.user$.subscribe(user => {
      if (user) {
        expect(user.email).toBe('test@example.com');
      }
    });

    service.loadCurrentUser();

    const req = httpMock.expectOne(`${baseUrl}/me`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: mockUser });
  });

  it('should handle error in loadCurrentUser and emit null', () => {
    service.user$.subscribe(user => {
      expect(user).toBeNull();
    });

    service.loadCurrentUser();

    const req = httpMock.expectOne(`${baseUrl}/me`);
    req.flush({}, { status: 500, statusText: 'Server Error' });
  });

  it('should get all users', () => {
    service.getAllUsers().subscribe(res => {
      expect(res.data.length).toBe(1);
      expect(res.data[0].id).toBe('u1');
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockUser] });
  });

  it('should get user by ID', () => {
    service.getUserById('u1').subscribe(user => {
      expect(user.email).toBe('test@example.com');
    });

    const req = httpMock.expectOne(`${baseUrl}/u1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockUser);
  });

//   it('should get user by email', () => {
//     service.getUserByEmail('test@example.com').subscribe(user => {
//       expect(user.id).toBe('u1');
//     });

//     const req = httpMock.expectOne(`${baseUrl}/email/test@example.com`);
//     expect(req.request.method).toBe('GET');
//     req.flush(mockUser);
//   });

  it('should update user', () => {
    const updateDto = { name: 'Updated User' };

    service.updateUser('u1', updateDto).subscribe(res => {
      expect(res.data.name).toBe('Updated User');
    });

    const req = httpMock.expectOne(`${baseUrl}/u1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updateDto);
    req.flush({ data: { ...mockUser, name: 'Updated User' } });
  });

  it('should delete user', () => {
    service.deleteUser('u1').subscribe(res => {
      expect(res).toEqual({ deleted: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/u1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ deleted: true });
  });
});
