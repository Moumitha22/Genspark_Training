import { FormGroup } from "@angular/forms";

export function setupLowercaseEmail(form: FormGroup, controlName: string) {
  form.get(controlName)?.valueChanges.subscribe((email: string) => {
    if (email) {
      form.get(controlName)?.setValue(email.toLowerCase(), { emitEvent: false });
    }
  });
}