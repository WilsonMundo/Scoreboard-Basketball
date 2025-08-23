import { Component, inject, signal, DestroyRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgIf } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { JsonPipe } from '@angular/common';
import { GameService, GameDto, TeamDto, API_BASE, } from '../../services/game.service';
import { interval, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common'; 

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [NgIf, JsonPipe, CommonModule],  
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
  apiBase = API_BASE;
  tickSub?: Subscription;
  constructor() {
    // Carga inicial
    this.load();
    

    // Si el usuario navega a otro /games/:id sin destruir el componente:
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(pm => { this.id.set(Number(pm.get('id'))); this.load(); });
  }

  startLocalTick() {
    this.stopLocalTick();
    this.tickSub = interval(1000).subscribe(() => {
      const g = this.game();
      if (!g || !g.isTimerRunning) return;
      if (g.remainingSeconds > 0) {
        this.game.set({ ...g, remainingSeconds: g.remainingSeconds - 1 });
      }
    });
  }
  stopLocalTick() {
    this.tickSub?.unsubscribe();
    this.tickSub = undefined;
  }

  onStart() {
    this.saving.set(true);
    this.svc.startTimer(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.startLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo iniciar el timer'); this.saving.set(false); }
    });
  }
  onPause() {
    this.saving.set(true);
    this.svc.pauseTimer(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.stopLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo pausar el timer'); this.saving.set(false); }
    });
  }
  onReset() {
    this.saving.set(true);
    this.svc.resetTimer(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.stopLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo reiniciar el timer'); this.saving.set(false); }
    });
  }
  formatClock(total: number | undefined | null): string {
    const t = Math.max(0, total ?? 0);
    const m = Math.floor(t / 60).toString().padStart(2, '0');
    const s = (t % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
  }

  load() {
    this.loading.set(true);
    this.error.set(null);

    this.svc.getById(this.id())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: g => {
          this.game.set(g);
          this.loading.set(false);

          // sincroniza el tick local con el estado del server
          if (g.isTimerRunning) {
            this.startLocalTick();
          } else {
            this.stopLocalTick();
          }
        },
        error: _ => {
          this.error.set('No se pudo cargar el juego');
          this.loading.set(false);
          this.stopLocalTick();
        }
      });
  }
  onFinish() {
    this.saving.set(true);
    this.svc.finish(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.stopLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo finalizar el partido'); this.saving.set(false); }
    });
  }

  isTie(g: GameDto): boolean {
    return (g.home?.score ?? 0) === (g.away?.score ?? 0);
  }

  isFinished(g: GameDto): boolean {
    return g.status === 'Finished';
  }

  shouldShowFinish(g: GameDto): boolean {
    return g.quarter >= 4 && !this.isTie(g) && !this.isFinished(g);
  }


  canGoNext(g: GameDto): boolean {
    if (this.isFinished(g)) return false;
    if (g.quarter < 4) return true;
    return this.isTie(g);
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

  onNextQuarter() {
    this.saving.set(true);
    this.svc.nextQuarter(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.stopLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo avanzar de cuarto'); this.saving.set(false); }
    });
  }
  onPrevQuarter() {
    this.saving.set(true);
    this.svc.prevQuarter(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.stopLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo retroceder de cuarto'); this.saving.set(false); }
    });
  }
}
