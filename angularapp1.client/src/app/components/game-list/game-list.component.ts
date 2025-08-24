import { Component, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule, DatePipe } from '@angular/common';
import { GameService, GameListItemDto } from '../../services/game.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './game-list.component.html',
  styleUrl: './game-list.component.css'
})
export class GameListComponent {
  private svc = inject(GameService);
  private destroyRef = inject(DestroyRef);

  loading = signal(true);
  error = signal<string | null>(null);
  items = signal<GameListItemDto[]>([]);

  constructor() { this.load(); }

  load() {
    this.loading.set(true);
    this.error.set(null);
    this.svc.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: data => { this.items.set(data); this.loading.set(false); },
        error: _ => { this.error.set('No se pudo cargar la lista'); this.loading.set(false); }
      });
  }

  statusBadge(s: string) {
    switch (s) {
      case 'Finished': return 'bg-green-100 text-green-800';
      case 'Active': return 'bg-blue-100 text-blue-800';
      default: return 'bg-slate-100 text-slate-800';
    }
  }

}
