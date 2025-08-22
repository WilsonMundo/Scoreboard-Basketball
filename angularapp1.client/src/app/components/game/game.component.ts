import { Component, inject, signal, DestroyRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgIf } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { JsonPipe } from '@angular/common';   
import { GameService, GameDto, TeamDto } from '../../services/game.service';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [NgIf, JsonPipe],   // 👈 agréga JsonPipe aquí
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent {
  private route = inject(ActivatedRoute);
  private svc = inject(GameService);
  private destroyRef = inject(DestroyRef);

  id = signal<number>(Number(this.route.snapshot.paramMap.get('id')));
  game = signal<GameDto | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  saving = signal<boolean>(false);

  constructor() {
    // Carga inicial
    this.load();

    // Si el usuario navega a otro /games/:id sin destruir el componente:
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(pm => { this.id.set(Number(pm.get('id'))); this.load(); });
  }

  load() {
    this.loading.set(true);
    this.error.set(null);
    this.svc.getById(this.id())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: g => { this.game.set(g); this.loading.set(false); },
        error: _ => { this.error.set('No se pudo cargar el juego'); this.loading.set(false); }
      });
  }

  changeScore(team: TeamDto | undefined | null, delta: number) {
    if (!team) return;
    this.saving.set(true);
    this.svc.updateScore(this.id(), { teamId: team.id, deltaPoints: delta })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: g => { this.game.set(g); this.saving.set(false); },
        error: e => {
          this.error.set(e?.error?.message ?? 'No se pudo actualizar el marcador');
          this.saving.set(false);
        }
      });
  }
}
