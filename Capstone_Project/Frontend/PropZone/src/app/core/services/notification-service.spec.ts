import { TestBed } from '@angular/core/testing';
import { NotificationService } from './notification.service';
import { ToastrService } from 'ngx-toastr';

describe('NotificationService', () => {
  let service: NotificationService;
  let mockToastr: jasmine.SpyObj<ToastrService>;

  beforeEach(() => {
    mockToastr = jasmine.createSpyObj('ToastrService', ['success', 'error', 'warning', 'info', 'clear']);

    TestBed.configureTestingModule({
      providers: [
        NotificationService,
        { provide: ToastrService, useValue: mockToastr }
      ]
    });

    service = TestBed.inject(NotificationService);
  });

  it('should call toastr.success', () => {
    service.success('Message', 'Title');
    expect(mockToastr.success).toHaveBeenCalledWith('Message', 'Title');
  });

  it('should call toastr.error', () => {
    service.error('Error message', 'Error Title');
    expect(mockToastr.error).toHaveBeenCalledWith('Error message', 'Error Title');
  });

  it('should call toastr.warning', () => {
    service.warning('Warning message', 'Warning Title');
    expect(mockToastr.warning).toHaveBeenCalledWith('Warning message', 'Warning Title');
  });

  it('should call toastr.info', () => {
    service.info('Info message', 'Info Title');
    expect(mockToastr.info).toHaveBeenCalledWith('Info message', 'Info Title');
  });

  it('should call toastr.clear', () => {
    service.clearAll();
    expect(mockToastr.clear).toHaveBeenCalled();
  });
});
