// game-setup.component.ts
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { GameService, CreateGameDto, GameDto } from '../../services/game.service';
import { NgIf } from '@angular/common'; 

@Component({
  selector: 'app-game-setup',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './game-setup.component.html',
  styleUrl: './game-setup.component.css'
})
export class GameSetupComponent {
  private svc = inject(GameService);
  private router = inject(Router);

  form = signal<CreateGameDto>({ homeName: '', awayName: '', quarterSeconds: 600 });
  loading = signal(false);
  error = signal<string | null>(null);

  create() {
    this.loading.set(true); this.error.set(null);
    this.svc.create(this.form()).subscribe({
      next: (g: GameDto) => this.router.navigate(['/games', g.id]),
      error: (e) => { this.error.set(e?.error?.message ?? 'Error creando juego'); this.loading.set(false); },
      complete: () => this.loading.set(false)
    });
  }
}
