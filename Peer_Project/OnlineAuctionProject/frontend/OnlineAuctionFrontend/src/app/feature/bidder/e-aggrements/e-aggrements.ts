import { Component, OnInit } from '@angular/core';
import { EAgreementService } from '../../../core/services/e-agreement.service';
import { AwardIcon, Calendar, CheckIcon, FileCheck2Icon, LucideAngularModule, Package, Tag } from 'lucide-angular';
import { CommonModule } from '@angular/common';
import { paginate } from '../../../shared/utils/pagination-utils';
import { Pagination } from '../../../shared/components/pagination/pagination';

@Component({
  selector: 'app-e-aggrements',
  imports: [LucideAngularModule, CommonModule, Pagination],
  templateUrl: './e-aggrements.html',
})
export class EAggrements implements OnInit {
  readonly fileViewIcon = FileCheck2Icon;
  readonly checkIcon = CheckIcon;
  readonly packageIcon = Package;
  readonly calenderIcon = Calendar;
  readonly tagIcon = Tag;
  readonly awardIcon = AwardIcon

  eArgreementData: any[] = [];
  paginatedEAgreementData: any[] = [];
  constructor(private eAgreementService: EAgreementService) {}

  page = 0;
  pageSize = 5;
  totalPages = 0;

  updatePaginationData() {
    const result = paginate(this.eArgreementData, this.page, this.pageSize);
    this.paginatedEAgreementData = result.data;
    this.totalPages = result.totalPages;
    this.page = result.currentPage;
  }

  goToPage(page: number) {
    this.page = page;
    this.updatePaginationData();
  }

  ngOnInit(): void {
    this.eAgreementService.getMyAgreements().subscribe({
      next: (res) => {
        this.eArgreementData = res?.data?.$values;
        this.updatePaginationData();
      },
    });
  }

  getEAgreement(eId: string) {
    this.eAgreementService.getMyAgreementsFile(eId).subscribe({
      next: (res) => {
        const url = window.URL.createObjectURL(res);
        window.open(url);
      },
    });
  }
}
