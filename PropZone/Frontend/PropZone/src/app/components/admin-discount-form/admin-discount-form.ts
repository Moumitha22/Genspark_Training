import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { DiscountCodeService } from '../../core/services/discount-code.service';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-discount-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './admin-discount-form.html',
  styleUrl: './admin-discount-form.css',
})
export class AdminDiscountForm implements OnInit {
  @Input() show = false;
  @Input() discountToEdit: any = null;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<any>();
  editingId: string | null = null;

  discountForm!: FormGroup;
  constructor(
    private fb: FormBuilder,
    private discountService: DiscountCodeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }
  ngOnChanges(changes: SimpleChanges) {
    if (!this.discountForm) return;
    
    if (changes['discountToEdit'] && this.discountToEdit) {
      const formatDate = (dateStr: string) => {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        return d.toISOString().slice(0, 10);
      };

      this.discountForm.patchValue({
        code: this.discountToEdit.code ?? '',
        discountValue: this.discountToEdit.discountValue ?? 0,
        isPercentage: this.discountToEdit.isPercentage ?? false,
        fromDate: formatDate(this.discountToEdit.fromDate),
        toDate: formatDate(this.discountToEdit.toDate),
        isActive: this.discountToEdit.isActive ?? true,
        maxListerLimit: Number(this.discountToEdit.maxListerLimit ?? 0),
      });

      const optionsArray = this.discountForm.get('options') as FormArray;

      if (Array.isArray(this.discountToEdit.options)) {
        while (optionsArray.length > this.discountToEdit.options.length) {
          optionsArray.removeAt(optionsArray.length - 1);
        }
        while (optionsArray.length < this.discountToEdit.options.length) {
          optionsArray.push(
            this.fb.group({
              typeOfProperty: [''],
              purposeOfListing: [''],
              minPrice: [0],
              maxPrice: [0],
            })
          );
        }
        this.discountToEdit.options.forEach((o: any, idx: number) => {
          optionsArray.at(idx).patchValue({
            typeOfProperty: o?.typeOfProperty ?? '',
            purposeOfListing: o?.purposeOfListing ?? '',
            minPrice: o?.minPrice ?? 0,
            maxPrice: o?.maxPrice ?? 0,
          });
        });
      }

      this.editingId = this.discountToEdit.id || null;
    } else {
      this.discountForm.reset();
      (this.discountForm.get('options') as FormArray).clear();
      this.editingId = null;
    }
  }
  initForm(code: any = null) {
    this.discountForm = this.fb.group({
      code: [code?.code || '', Validators.required],
      discountValue: [code?.discountValue || 0, Validators.required],
      isPercentage: [code?.isPercentage || false, Validators.required],
      fromDate: [code?.fromDate || '', Validators.required],
      toDate: [code?.toDate || '', Validators.required],
      isActive: [code?.isActive ?? true],
      maxListerLimit: [code?.maxListerLimit || 0, Validators.required],
      options: this.fb.array(
        code?.options?.map((o: any) =>
          this.fb.group({
            typeOfProperty: [o.typeOfProperty],
            purposeOfListing: [o.purposeOfListing],
            minPrice: [o.minPrice],
            maxPrice: [o.maxPrice],
          })
        ) || []
      ),
    });
  }

  get options(): FormArray {
    return this.discountForm.get('options') as FormArray;
  }

  addOption() {
    this.options.push(
      this.fb.group({
        typeOfProperty: [null],
        purposeOfListing: [null],
        minPrice: [null],
        maxPrice: [null],
      })
    );
  }

  removeOption(index: number) {
    this.options.removeAt(index);
  }

  getEnumValue(enumName: string, value: string): number | null {
    const enums: any = {
      PropertyType: {
        Apartment: 0,
        House: 1,
        Plot: 2,
        CommercialSpace: 3,
      },
      ListingPurpose: {
        Sale: 0,
        Rent: 1,
      },
    };
    return enums[enumName][value] ?? null;
  }
  onSubmit() {
    if (this.discountForm.valid) {
      const formValue = { ...this.discountForm.value };
      formValue.fromDate = new Date(formValue.fromDate).toISOString();
      formValue.toDate = new Date(formValue.toDate).toISOString();
      formValue.isActive = formValue.isActive === true;

      formValue.options = formValue.options.map((opt: any) => ({
        ...opt,
        typeOfProperty:
          typeof opt.typeOfProperty === 'string'
            ? this.getEnumValue('PropertyType', opt.typeOfProperty)
            : opt.typeOfProperty,
        purposeOfListing:
          typeof opt.purposeOfListing === 'string'
            ? this.getEnumValue('ListingPurpose', opt.purposeOfListing)
            : opt.purposeOfListing,
      }));
      if (this.editingId) {
        this.discountService
          .updateDiscount(this.editingId, formValue)
          .subscribe({
            next: (updated) => {
              this.toastr.success('Discount code updated successfully!');
              this.editingId = null;
              this.save.emit(updated);
              this.closeModal();
              this.initForm();
            },
            error: () => this.toastr.error('Error updating discount code'),
          });
      } else {
        this.discountService.createDiscount(formValue).subscribe({
          next: (created) => {
            this.toastr.success('Discount code Created successfully!');
            this.save.emit(created);

            this.closeModal();
            this.initForm();
          },
          error: (err) => {
            console.log(err);
            this.toastr.error('Error creating discount code', err.error.message);
          }
        });
      }
    }
  }

  closeModal() {
    this.show = false;
    this.close.emit();
  }
  onCancelEdit() {
    this.editingId = null;
    this.initForm();
    this.close.emit();
  }
}
