import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

/**
 * Guarda de autenticación que protege rutas en Angular.
 *
 * Verifica si el usuario esta autenticado:
 * - verdadero, permite acceder.
 * - falso, redirige al login .
 *
 * @param route - Información de la ruta que se intenta activar.
 * @param state - Estado del router al momento de la navegación.
 * @returns `true` si el usuario está autenticado, en caso contrario `false`.
 */

export const AuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true; // token válido → deja pasar
  } else {
    router.navigate(['/login'], {
      queryParams: { returnUrl: state.url } // 
    });
    return false;
  }
};

