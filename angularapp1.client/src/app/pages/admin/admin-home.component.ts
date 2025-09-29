import { Component } from '@angular/core';

@Component({
  selector: 'app-admin-home',
  standalone: true,
  template: `
    <div class="p-6">
      <h2 class="text-2xl font-bold">Bienvenido al Panel de Administración</h2>
      <p class="mt-2 text-gray-600">
        Selecciona una opción del menú lateral para comenzar.
      </p>
    </div>
  `
})
export class AdminHomeComponent {}
