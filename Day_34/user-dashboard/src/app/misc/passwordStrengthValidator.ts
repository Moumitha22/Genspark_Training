import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function strongPasswordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value: string = control.value;
    if (!value) return null;

    const hasNumber = /\d/.test(value);
    const hasSymbol = /[!@#$%^&*(),.?":{}|<>]/.test(value);
    const isValidLength = value.length >= 6;

    return hasNumber && hasSymbol && isValidLength
      ? null
      : { weakPassword: true };
  };
}
