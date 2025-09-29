import { inject } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn,
  HttpErrorResponse
} from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

/**
 * Interceptor de autenticación para Angular.
 *
 * Funcionalidad:
 * - Adjunta el token JWT en el header `Authorization` de cada petición HTTP.
 * - Intercepta errores de respuesta:
 *   - Si la API devuelve `401 Unauthorized`, limpia la sesión (logout)
 *     y redirige al login.
 *
 * @param req - Petición HTTP original.
 * @param next - Función que reenvía la petición al siguiente interceptor o al backend.
 * @returns Observable con la respuesta HTTP, o un error propagado.
 */
export const AuthInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken();

  // Adjuntar token si existe
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Token ya no es válido
        authService.logout(); // limpia localStorage y estado
        router.navigate(['/login']); // redirige al login
      }
      return throwError(() => error);
    })
  );
};
