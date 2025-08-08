import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { EAggrements } from './e-aggrements';
import { EAgreementService } from '../../../core/services/e-agreement.service';
import { of } from 'rxjs';

describe('EAggrements', () => {
  let component: EAggrements;
  let fixture: ComponentFixture<EAggrements>;
  let eAgreementServiceSpy: jasmine.SpyObj<EAgreementService>;

  const mockAgreements = {
    data: {
      $values: [
        { id: 'a1', name: 'Agreement 1' },
        { id: 'a2', name: 'Agreement 2' },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    eAgreementServiceSpy = jasmine.createSpyObj('EAgreementService', [
      'getMyAgreements',
      'getMyAgreementsFile',
    ]);

    TestBed.configureTestingModule({
      imports: [EAggrements],
      providers: [
        { provide: EAgreementService, useValue: eAgreementServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    eAgreementServiceSpy.getMyAgreements.and.returnValue(of(mockAgreements));
    fixture = TestBed.createComponent(EAggrements);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch agreements on init and paginate', () => {
    expect(eAgreementServiceSpy.getMyAgreements).toHaveBeenCalled();
    expect(component.eArgreementData.length).toBe(2);
    expect(component.paginatedEAgreementData.length).toBe(2);
    expect(component.totalPages).toBe(1);
  });

  it('should update pagination data when goToPage is called', () => {
    component.eArgreementData = Array.from({ length: 25 }, (_, i) => ({
      id: `a${i + 1}`,
    }));
    component.pageSize = 10;
    component.goToPage(2);
    expect(component.paginatedEAgreementData.length).toBe(10);
    component.goToPage(3);
    expect(component.paginatedEAgreementData.length).toBe(5);
  });

  it('should call getMyAgreementsFile and open file', () => {
    const blob = new Blob(['test'], { type: 'application/pdf' });
    spyOn(window, 'open');
    spyOn(window.URL, 'createObjectURL').and.returnValue('blob:url');
    eAgreementServiceSpy.getMyAgreementsFile.and.returnValue(of(blob));
    component.getEAgreement('a1');
    expect(eAgreementServiceSpy.getMyAgreementsFile).toHaveBeenCalledWith('a1');
    expect(window.URL.createObjectURL).toHaveBeenCalledWith(blob);
    expect(window.open).toHaveBeenCalledWith('blob:url');
  });
});
