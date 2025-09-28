import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt';
import { ReactiveFormsModule } from '@angular/forms';

import { routes } from './app.routes';
import { AuthInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    // Rutas
    provideRouter(routes, withComponentInputBinding()),

    // HttpClient + Interceptor para adjuntar token
    provideHttpClient(withInterceptors([AuthInterceptor])),

    // Formularios reactivos disponibles globalmente
    importProvidersFrom(ReactiveFormsModule),

    // Configuración para JwtHelperService
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    JwtHelperService
  ]
};
