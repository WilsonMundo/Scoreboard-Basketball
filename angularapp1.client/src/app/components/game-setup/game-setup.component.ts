// game-setup.component.ts
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { GameService, CreateGameDto, GameDto } from '../../services/game.service';
import { NgIf } from '@angular/common';
import { forkJoin, of, switchMap } from 'rxjs';


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

    
  form = signal<CreateGameDto>({ homeName: '', awayName: '', quarterSeconds: 600});
  loading = signal(false);
  error = signal<string | null>(null);

  // files seleccionados y previews
  homeFile = signal<File | null>(null);
  awayFile = signal<File | null>(null);
  homePreview = signal<string | null>(null);
  awayPreview = signal<string | null>(null);

  onPickHome(ev: Event) {
    const file = (ev.target as HTMLInputElement).files?.[0] ?? null;
    this.homeFile.set(file);
    this.homePreview.set(file ? URL.createObjectURL(file) : null);
  }
  onPickAway(ev: Event) {
    const file = (ev.target as HTMLInputElement).files?.[0] ?? null;
    this.awayFile.set(file);
    this.awayPreview.set(file ? URL.createObjectURL(file) : null);
  }
  formatClock(total: number | undefined | null) {
    const t = Math.max(0, total ?? 0);
    const m = Math.floor(t / 60).toString().padStart(2, '0');
    const s = (t % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
  }



  create() {
    this.loading.set(true); this.error.set(null);

    this.svc.create(this.form()).pipe(
      // luego de crear, si hay logos, súbelos
      switchMap((g: GameDto) => {
        const uploads = [];
        if (this.homeFile() && g.home?.id) uploads.push(this.svc.uploadTeamLogo(g.id, g.home.id, this.homeFile()!));
        if (this.awayFile() && g.away?.id) uploads.push(this.svc.uploadTeamLogo(g.id, g.away.id, this.awayFile()!));
        if (uploads.length === 0) return of(g);
        return forkJoin(uploads).pipe( // devuelve el último estado del juego (cualquiera de las respuestas)
          switchMap(last => of(Array.isArray(last) ? last[last.length - 1] as GameDto : g))
        );
      })
    ).subscribe({
      next: (g: GameDto) => this.router.navigate(['/games', g.id]),
      error: (e) => { this.error.set(e?.error?.message ?? 'Error creando juego'); this.loading.set(false); },
      complete: () => this.loading.set(false)
    });
  
  }
  
  
}
