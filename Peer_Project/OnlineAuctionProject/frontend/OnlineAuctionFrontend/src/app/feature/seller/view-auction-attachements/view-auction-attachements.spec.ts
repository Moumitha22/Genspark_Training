import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ViewAuctionAttachements } from './view-auction-attachements';
import { AuctionService } from '../../../core/services/auction.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

describe('ViewAuctionAttachements', () => {
  let component: ViewAuctionAttachements;
  let fixture: ComponentFixture<ViewAuctionAttachements>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let activatedRouteStub: any;

  const mockFiles = [
    { name: 'file1.pdf', contentType: 'application/pdf' },
    { name: 'img1.png', contentType: 'image/png' },
  ];
  const mockAuctionData = { data: { files: { $values: mockFiles } } };
  beforeEach(waitForAsync(() => {
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getAuctionByAuctionId',
      'getfile',
    ]);
    auctionServiceSpy.getAuctionByAuctionId.and.returnValue(
      of(mockAuctionData)
    );
    authServiceSpy = jasmine.createSpyObj('AuthService', ['authme']);
    authServiceSpy.authme.and.returnValue(of({ data: { role: 'seller' } }));
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    activatedRouteStub = {
      snapshot: {
        params: { auctionId: '123' },
        queryParams: { page: 1 },
      },
    };

    TestBed.configureTestingModule({
      imports: [ViewAuctionAttachements],
      providers: [
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewAuctionAttachements);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  beforeEach(() => {
    auctionServiceSpy.getAuctionByAuctionId.and.returnValue(
      of(mockAuctionData)
    );
    fixture = TestBed.createComponent(ViewAuctionAttachements);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch auction files on init', () => {
    expect(auctionServiceSpy.getAuctionByAuctionId).toHaveBeenCalledWith('123');
    expect(component.auctionData.length).toBe(2);
    expect(component.auctionData[0].name).toBe('file1.pdf');
  });

  it('should handle error when fetching auction files', () => {
    auctionServiceSpy.getAuctionByAuctionId.and.returnValue(
      throwError(() => new Error('fail'))
    );
    spyOn(console, 'log');
    fixture = TestBed.createComponent(ViewAuctionAttachements);
    component = fixture.componentInstance;
    fixture.detectChanges();
    expect(console.log).toHaveBeenCalled();
  });

  it('should download file attachment', () => {
    const blob = new Blob(['test'], { type: 'application/pdf' });
    auctionServiceSpy.getfile.and.returnValue(of(blob));
    spyOn(window.URL, 'createObjectURL').and.returnValue('blob:url');
    spyOn(document, 'createElement').and.callThrough();
    const file = { name: 'file1.pdf' };
    component.auctionId = '123';
    component.getfileAttachment(file);
    expect(auctionServiceSpy.getfile).toHaveBeenCalledWith('123', 'file1.pdf');
  });

  it('should view file in new tab', () => {
    const blob = new Blob(['test'], { type: 'application/pdf' });
    auctionServiceSpy.getfile.and.returnValue(of(blob));
    spyOn(window.URL, 'createObjectURL').and.returnValue('blob:url');
    spyOn(window, 'open');
    component.auctionId = '123';
    component.viewFile('file1.pdf');
    expect(auctionServiceSpy.getfile).toHaveBeenCalledWith('123', 'file1.pdf');
    expect(window.open).toHaveBeenCalledWith('blob:url');
  });

  it('should return false for undefined or null type in isImage', () => {
    expect(component.isImage(undefined)).toBeFalse();
    expect(component.isImage(null)).toBeFalse();
  });

  it('should navigate back to auction list', () => {
    component.goBackToAuctionList();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/seller/view-auctions'], {
      queryParams: { page: 1 },
    });
  });
});
