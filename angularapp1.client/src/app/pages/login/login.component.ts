import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  form: FormGroup;
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      login: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  login() {
    if (this.form.invalid) return;

    this.loading = true;
    this.error = null;

    this.authService.login(this.form.value).subscribe({
      next: res => {
        console.log('Respuesta del backend:', res);

        // Si el backend devuelve vacío, evitamos que quede colgado
        if (!res) {
          this.error = 'El servidor respondió sin datos';
          this.loading = false;
          return;
        }

        // Ya se guarda el token en el AuthService
        this.router.navigate(['/']); // redirige al home
        this.loading = false; // 🔥 aseguramos que se resetee el loading
      },
      error: err => {
        this.error = 'Credenciales inválidas o error en el servidor';
        this.loading = false;
      }
    });
  }
}
