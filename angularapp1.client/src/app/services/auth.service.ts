import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment.prod';
import { LoginRequestDto } from '../models/dto/login-request.dto';
import { LoginResponseDto } from '../models/dto/login-response.dto';
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
  /** Endpoint base para autenticación en la API */
  private apiUrl = `${environment.apiBase}/Autenticacion`;

  /** Estado actual del token del usuario */
  private currentUserToken = new BehaviorSubject<LoginResponseDto | null>(null);

  constructor(
    private http: HttpClient,
    private router: Router,
    private jwtHelper: JwtHelperService 
  ) {}

  /**
   * Realiza el inicio de sesión contra la API.
   *
   * @param user Credenciales del usuario (`username`, `password`).
   * @returns Observable con la respuesta que contiene el token JWT.
   *
   * Efectos secundarios:
   * - Guarda el token en `localStorage`.
   * - Actualiza el `BehaviorSubject` con el nuevo token.
   */
  login(user: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, user).pipe(
      tap((token) => {
        localStorage.setItem('authToken', JSON.stringify(token));
        this.currentUserToken.next(token);
      })
    );
  }

  /**
   * Cierra la sesión del usuario.
   *
   * - Llama al endpoint `/logout` en la API.
   * - Si responde con éxito o error, limpia el `localStorage` y
   *   reinicia el estado de sesión.
   * - Redirige al usuario a la pantalla de login.
   */
  logout() {
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
      next: () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
        this.currentUserToken.next(null);
        this.router.navigate(['/login']);
      },
      error: () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
        this.currentUserToken.next(null);
        this.router.navigate(['/login']);
      }
    });
  }

  /**
   * Obtiene el token JWT actual.
   *
   * @returns El token como `string`, o `null` si no existe.
   *
   * Busca primero en el `BehaviorSubject` y, si no hay valor,
   * lo recupera desde `localStorage`.
   */
  getToken(): string | null {
    const token =
      this.currentUserToken.value ||
      JSON.parse(localStorage.getItem('authToken') || 'null');
    return token ? token.token : null;
  }

  /**
   * Verifica si el usuario está autenticado.
   *
   * @returns `true` si existe un token válido y no está expirado.
   */
  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    return !this.jwtHelper.isTokenExpired(token); // valida expiración
  }
}