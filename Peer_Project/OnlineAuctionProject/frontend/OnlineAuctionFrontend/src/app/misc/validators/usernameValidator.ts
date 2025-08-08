import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function UserNameValidator(username: string[]): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const lowercased = control.value.toLowerCase();
    return username.some((word) => lowercased.includes(word))
      ? { bannedWord: true }
      : null;
  };
}
