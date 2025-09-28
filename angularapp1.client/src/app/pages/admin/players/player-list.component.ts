import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PlayerService, PlayerDto, PlayerListRequest, PagedResult } from '../../../services/player.service';
import { TeamService, TeamDto } from '../../../services/team.service';
import { NotificationComponent } from '../../../components/notification/notification.component';

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, NotificationComponent],
  templateUrl: './player-list.component.html'
})
export class PlayerListComponent implements OnInit {
  players: PlayerDto[] = [];

  // signals
  loading = signal(false);
  notification = signal<string | null>(null);

  // búsqueda y paginación
  search = signal<string>('');       // texto libre
  filterTeam = signal<number | null>(null); // filtro de equipo
  page = signal<number>(1);
  size = 10;
  total = signal<number>(0);
  Math = Math;

  // equipos disponibles para el filtro
  allTeams: TeamDto[] = [];

  constructor(
    private playerService: PlayerService,
    private teamService: TeamService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPlayers();
    this.loadAllTeams();
  }

  loadPlayers() {
    this.loading.set(true);

    const req: PlayerListRequest = {
      q: this.search(),
      teamId: this.filterTeam() || undefined,
      page: this.page(),
      size: this.size,
      sort: 'name'
    };

    this.playerService.list(req).subscribe({
      next: (res: PagedResult<PlayerDto>) => {
        this.players = res.items;
        this.total.set(res.total);
        this.loading.set(false);
      },
      error: () => {
        this.notification.set('No se pudieron cargar los jugadores');
        setTimeout(() => this.notification.set(null), 3000);
        this.loading.set(false);
      }
    });
  }

  loadAllTeams() {
    this.teamService.list({ page: 1, size: 1000 }).subscribe({
      next: (res) => {
        this.allTeams = res.items;
      }
    });
  }

  onSearch() {
    this.page.set(1);
    this.loadPlayers();
  }

  onFilterTeam(teamId: string) {
    this.filterTeam.set(teamId ? Number(teamId) : null);
    this.page.set(1);
    this.loadPlayers();
  }

  nextPage() {
    if (this.page() * this.size < this.total()) {
      this.page.set(this.page() + 1);
      this.loadPlayers();
    }
  }

  prevPage() {
    if (this.page() > 1) {
      this.page.set(this.page() - 1);
      this.loadPlayers();
    }
  }

  editPlayer(id: number) {
    this.router.navigate(['/admin/players', id]);
  }

  deletePlayer(id: number) {
    if (confirm('¿Seguro que deseas eliminar este jugador?')) {
      this.playerService.delete(id).subscribe({
        next: () => {
          this.loadPlayers();
          this.notification.set('Jugador eliminado correctamente');
          setTimeout(() => this.notification.set(null), 3000);
        },
        error: () => {
          this.notification.set('No se pudo eliminar el jugador');
          setTimeout(() => this.notification.set(null), 3000);
        }
      });
    }
  }
}
