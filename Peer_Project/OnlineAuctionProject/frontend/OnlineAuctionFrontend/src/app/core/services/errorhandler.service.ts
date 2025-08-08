import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorHandlerService {
  handleError(error: HttpErrorResponse) {
    return throwError(() => error);
  }
}
