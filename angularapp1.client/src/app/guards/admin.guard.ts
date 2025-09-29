import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(): boolean {
    // Primero, revisamos si hay un token válido
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }

    // Luego, revisamos si el usuario es Admin
    if (this.auth.isAdmin()) {
      return true;
    }

    // Si no es Admin, lo mandamos a inicio o login
    this.router.navigate(['/']);
    return false;
  }
}
