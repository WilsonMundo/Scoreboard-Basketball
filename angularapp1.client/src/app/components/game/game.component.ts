import { Component, inject, signal, DestroyRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgIf } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { JsonPipe } from '@angular/common';
import { GameService, GameDto, TeamDto, API_BASE, } from '../../services/game.service';
import { interval, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RealtimeService } from '../../services/realtime.service';
import { HubConnectionState } from '@microsoft/signalr';
import { NotificationComponent } from '../../components/notification/notification.component';
import { BeepService } from '../../services/beep.service';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [NgIf, JsonPipe, CommonModule, NotificationComponent],  
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent {
  private route = inject(ActivatedRoute);
  private svc = inject(GameService);
  private destroyRef = inject(DestroyRef);

  realtime = inject(RealtimeService);
  
  public HubConnectionState = HubConnectionState;
  id = signal<number>(Number(this.route.snapshot.paramMap.get('id')));
  game = signal<GameDto | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  saving = signal<boolean>(false);
  starting = signal<boolean>(false);
  apiBase = API_BASE;
  tickSub?: Subscription;
  notification = signal<string | null>(null);
  private handlingPeriodEnd = false;
  private beep = inject(BeepService);
  
  constructor() {
    // Carga inicial
    this.load();
    this.initRealtime();

    // Si el usuario navega a otro /games/:id sin destruir el componente:
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(pm => { this.id.set(Number(pm.get('id'))); this.load(); });
  }

  // Fusión que preserva el reloj si está corriendo localmente
  private mergePreservingClock(server: GameDto): GameDto {
    const local = this.game();
    if (!local) return server;
    if (local.isTimerRunning) {
      // mantené el tiempo local (nunca avances hacia atrás)
      const remaining = Math.min(local.remainingSeconds, server.remainingSeconds ?? local.remainingSeconds);
      return { ...server, remainingSeconds: remaining, isTimerRunning: true };
    }
    return server;
  }
  stateLabel(): string {
    const s = this.realtime.state();
    switch (s) {
      case this.HubConnectionState.Connected: return 'Conectado';
      case this.HubConnectionState.Connecting: return 'Conectando';
      case this.HubConnectionState.Reconnecting: return 'Reintentando';
      case this.HubConnectionState.Disconnected:
      default: return 'Desconectado';
    }
  }
  async initRealtime() {
    try {
      await this.realtime.connect(''); // vacío si usas proxy; o API_BASE si no
      await this.realtime.joinGame(this.id());
      this.realtime.onGameUpdated((dto: GameDto) => {
        const prev = this.game();
        const wasRunning = !!prev?.isTimerRunning;

        // fusiona sin pisar el reloj local
        const next = this.mergePreservingClock(dto);
        this.game.set(next);

        // SOLO se transiciona el tick si cambió el estado
        if (!wasRunning && next.isTimerRunning) this.startLocalTick();
        if (wasRunning && !next.isTimerRunning) this.stopLocalTick();
      });
    } catch (e) {
      console.error('SignalR connection error', e);
    }
  }
  changeFouls(team: TeamDto | null | undefined, delta: number) {
    if (!team) return;
    if (this.saving()) {
      return;
    }
    if (!this.starting()) {
      this.notification.set('Debes iniciar el juego primero');
      setTimeout(() => this.notification.set(null), 3000);
      return;
    }
    this.saving.set(true);
    this.svc.updateFouls(this.id(), team.id, delta)
      .subscribe({
        next: g => {
          // No tocar el tick; fusionar por si el server trae otra info
          this.game.set(this.mergePreservingClock(g));
          this.saving.set(false);
        },
        error: e => {
          this.notification.set(e?.error?.message ?? 'No se pudo actualizar faltas');          
          this.saving.set(false);

        }
      });
  }
  async cleanupRealtime() {
    try {
      this.stopLocalTick();
      await this.realtime.leaveGame(this.id());
      await this.realtime.disconnect(); // este componente es el único que usa el hub ??
    } catch { }
  }


  startLocalTick() {
    if (this.tickSub) return; //  corriendo: no se reinicia
    this.tickSub = interval(1000).subscribe(() => {
      const g = this.game();
      if (!g || !g.isTimerRunning) return;
      if (g.remainingSeconds > 0) {
        this.game.set({ ...g, remainingSeconds: g.remainingSeconds - 1 });
        // cuando llegue a 0, ejecutamos flujo de cierre/avance
        if (g.remainingSeconds - 1 === 0) {
          this.beep.beepSequence(5);
          this.notification.set(`Finaliza Cuarto: ` + g.quarter);
          this.handlePeriodEnd(); // 
        }
      }

    });
  }
  private handlePeriodEnd() {
    if (this.handlingPeriodEnd) return; // evita doble disparo
    this.handlingPeriodEnd = true;

    // 1) Pausar en servidor para materializar timer y, si aplica, marcar Finished
    this.svc.pauseTimer(this.id()).subscribe({
      next: paused => {
        this.game.set(paused); // el server ya trae remainingSeconds=0
        const g = paused;

        const tie = (g.home?.score ?? 0) === (g.away?.score ?? 0);
        const finished = g.status === 'Finished';

        // 2) Decidir avance
        const canAutoNext =
          (!finished) &&
          (
            (g.quarter < 4) ||               // Q1..Q3: siempre
            (g.quarter >= 4 && tie)          // Q4 u OT: solo si empate
          );

        if (canAutoNext) {
          this.svc.nextQuarter(this.id()).subscribe({
            next: ng => {
              this.game.set(ng);
              this.stopLocalTick(); // reiniciaremos solo si corre
              if (ng.isTimerRunning) this.startLocalTick();
              this.handlingPeriodEnd = false;
            },
            error: _ => { this.handlingPeriodEnd = false; }
          });
        } else {
          // Si no avanza, detener tick (Finished) o queda en 0 esperando acción
          if (finished) this.stopLocalTick();
          this.handlingPeriodEnd = false;
        }
      },
      error: _ => { this.handlingPeriodEnd = false; }
    });
  }
  stopLocalTick() {    
    this.tickSub?.unsubscribe();
    this.tickSub = undefined;
  }

  onStart() {
    this.saving.set(true);
    this.starting.set(true);
    this.svc.startTimer(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.startLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo iniciar el timer'); this.saving.set(false); }
    });
  }
  onPause() {
    this.saving.set(true);
    this.starting.set(false);
    this.svc.pauseTimer(this.id()).subscribe({
      next: g => { this.game.set(g); this.saving.set(false); this.stopLocalTick(); },
      error: e => { this.error.set(e?.error?.message ?? 'No se pudo pausar el timer'); this.saving.set(false); }
    });
  }
  onReset() {
    this.saving.set(true);
    this.starting.set(false);
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
    this.starting.set(false);
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
    if (this.saving()) {
      return;
    }
    if (!this.starting()) {
      this.notification.set('Debes iniciar el juego primero');
      setTimeout(() => this.notification.set(null), 3000);
      return;
    }

    this.saving.set(true);
    

    this.svc.updateScore(this.id(), { teamId: team.id, deltaPoints: delta })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: g => { this.game.set(this.mergePreservingClock(g)); this.saving.set(false); },
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
