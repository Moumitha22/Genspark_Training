import {
  Component,
  EventEmitter,
  Input,
  Output,
  inject,
  OnChanges,
  SimpleChanges
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FeatureService } from '../../core/services/feature.service';
import { NotificationService } from '../../core/services/notification.service';
import { FeatureAdminModel } from '../../models/feature-admin.model';

@Component({
  selector: 'app-admin-feature-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-feature-form.html',
  styleUrl: './admin-feature-form.css'
})
export class AdminFeatureFormComponent implements OnChanges {
  @Input() show = false;
  @Input() featureToEdit: FeatureAdminModel | null = null;
  @Output() close = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private featureService = inject(FeatureService);
  private notification = inject(NotificationService);

  minApplicabilityValidator(): Validators {
    return (formArray: FormArray): { [key: string]: any } | null => {
      return formArray.length > 0 ? null : { minApplicability: true };
    };
  }


  form: FormGroup = this.createEmptyForm();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['featureToEdit']) {
      if (this.featureToEdit) {
        this.setFormWithFeature(this.featureToEdit);
      } else {
        this.form = this.createEmptyForm(); 
      }
    }
  }

  createEmptyForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      dataType: ['', Validators.required],
      filterMode: ['', Validators.required],
      options: this.fb.array<FormControl<string | null>>([]),
      applicabilities: this.fb.array<FormGroup>([], this.minApplicabilityValidator())
    });
  }


  setFormWithFeature(feature: FeatureAdminModel): void {
    this.form = this.createEmptyForm();
    this.form.patchValue({
      name: feature.name,
      dataType: feature.dataType,
      filterMode: feature.filterMode
    });

    feature.options?.forEach(opt => {
      this.options.push(this.fb.control(opt.value, Validators.required));
    });

    feature.applicability?.forEach(app => {
      this.applicabilities.push(
        this.fb.group({
          appliesToPurpose: [app.appliesToPurpose, Validators.required],
          appliesToType: [app.appliesToType, Validators.required]
        })
      );
    });
  }

  get options(): FormArray<FormControl<string | null>> {
    return this.form.get('options') as FormArray<FormControl<string | null>>;
  }


  get applicabilities(): FormArray<FormGroup> {
    return this.form.get('applicabilities') as FormArray<FormGroup>;
  }

  get showOptionsSection(): boolean {
    const type = this.form.get('dataType')?.value;
    return type === 'Dropdown' || type === 'MultiSelect';
  }

  addOption() {
    this.options.push(this.fb.control('', Validators.required));
  }

  removeOption(index: number) {
    this.options.removeAt(index);
  }

  addApplicability() {
    this.applicabilities.push(
      this.fb.group({
        appliesToPurpose: ['', Validators.required],
        appliesToType: ['', Validators.required]
      })
    );
  }

  removeApplicability(index: number) {
    this.applicabilities.removeAt(index);
  }

  submit() {
    if (this.form.invalid) {
      this.notification.error('Please fill all required fields');
      this.form.markAllAsTouched();
      return;
    }

    const dto = {
      ...this.form.value,
      id: this.featureToEdit?.id ?? undefined
    };

    console.log(dto);

    const request$ = this.featureToEdit
      ? this.featureService.updateFeature(this.featureToEdit.id, dto)
      : this.featureService.createFeature(dto);

    request$.subscribe({
      next: () => {
        this.notification.success(this.featureToEdit ? 'Feature updated' : 'Feature created');
        this.form = this.createEmptyForm();
        this.close.emit(true);
      },
      error: () => {
        this.notification.error('Something went wrong');
      }
    });
  }

  closeModal() {
    this.form = this.createEmptyForm();
    this.close.emit(false);
  }
}
