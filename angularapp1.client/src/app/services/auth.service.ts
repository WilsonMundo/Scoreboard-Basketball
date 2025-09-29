import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment.prod';
import { LoginRequest } from '../models/dto/login-request.dto';
import { LoginResponse } from '../models/dto/login-response.dto';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

/**
 * Servicio central de autenticación.
 *
 * Funcionalidad principal:
 * - Maneja el inicio y cierre de sesión del usuario.
 * - Persiste tokens (`authToken` y `refreshToken`) en `localStorage`.
 * - Valida la autenticación y expiración del token JWT.
 * - Expone un `BehaviorSubject` para que otros componentes puedan suscribirse
 *   a los cambios de sesión.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiBase}/Autenticacion`;
  private currentUserToken = new BehaviorSubject<LoginResponse | null>(null);

  constructor(
    private http: HttpClient,
    private router: Router,
    private jwtHelper: JwtHelperService
  ) {
    const savedToken = JSON.parse(localStorage.getItem('authToken') || 'null');
    if (savedToken) {
      this.currentUserToken.next(savedToken);
    }
  }

  login(user: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, user).pipe(
      tap((token) => {
        localStorage.setItem('authToken', JSON.stringify(token));
        this.currentUserToken.next(token);
      })
    );
  }

  logout() {
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
      next: () => this.clearSession(),
      error: () => this.clearSession()
    });
  }

  private clearSession() {
    localStorage.removeItem('authToken');
    this.currentUserToken.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    const token =
      this.currentUserToken.value ||
      JSON.parse(localStorage.getItem('authToken') || 'null');
    return token ? token.token : null;
  }
  isLoggedIn(): boolean {
    const token = this.getToken();
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  isAuthenticated(): boolean {
    const token =
      this.currentUserToken.value ||
      JSON.parse(localStorage.getItem('authToken') || 'null');

    if (!token) return false;

    if (this.jwtHelper.isTokenExpired(token.token)) {
      this.clearSession();
      return false;
    }

    return true;
  }
  private getDecodedToken(): any {
    const token = this.getToken();
    if (!token) return null;
    try {
      return this.jwtHelper.decodeToken(token);
    } catch {
      return null;
    }
  }

  /** Saber si es administrador */
  isAdmin(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const decoded = this.jwtHelper.decodeToken(token);

    // claim de .NET
    return decoded && decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'Admin';
  }



}
