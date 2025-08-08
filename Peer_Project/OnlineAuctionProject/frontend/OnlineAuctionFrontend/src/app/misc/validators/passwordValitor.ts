import { AbstractControl, ValidationErrors } from '@angular/forms';

export function passwordValidator(
  control: AbstractControl
): ValidationErrors | null {
  const value: string = control.value || '';
  if (value.length < 6) return { weakPassword: true };

  let hasNumber = false;
  let hasSymbol = false;

  for (let char of value) {
    if (!isNaN(Number(char)) && char !== ' ') {
      hasNumber = true;
    } else if (
      !(char >= 'a' && char <= 'z') &&
      !(char >= 'A' && char <= 'Z') &&
      char !== ' ' &&
      isNaN(Number(char))
    ) {
      hasSymbol = true;
    }
    if (hasNumber && hasSymbol) break;
  }

  if (!hasNumber || !hasSymbol) {
    return { weakPassword: true };
  }
  return null;
}

export function matchPassValidator(
  group: AbstractControl
): ValidationErrors | null {
  const password = (group.get('password') || group.get('newPassword'))?.value;
  const confirmPassword = group.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passWordDifferent: true };
}
