import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function bannedUsernameValidator(bannedWords: string[]): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value?.toLowerCase();
    if (!value) return null;

    const hasBanned = bannedWords.some(word => value.includes(word));
    return hasBanned ? { bannedWord: true } : null;
  };
}
