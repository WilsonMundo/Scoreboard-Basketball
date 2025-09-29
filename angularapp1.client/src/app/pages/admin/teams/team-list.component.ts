import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TeamService, TeamDto, TeamListRequest, PagedResult } from '../../../services/team.service';
import { NotificationComponent } from '../../../components/notification/notification.component';

@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, NotificationComponent],
  templateUrl: './team-list.component.html'
})
export class TeamListComponent implements OnInit {
  teams: TeamDto[] = [];

  // signals
  loading = signal(false);
  notification = signal<string | null>(null);

  // búsqueda y paginación
  search = signal<string>('');         // texto libre (nombre/ciudad)
  filterCity = signal<string>('');     // filtro de ciudad
  page = signal<number>(1);
  size = 10;
  total = signal<number>(0);
  Math = Math;

  // ciudades disponibles para el filtro
  allCities: string[] = [];

  constructor(private teamService: TeamService, private router: Router) {}

  ngOnInit(): void {
    this.loadTeams();
    this.loadAllCities(); // cargar listado de ciudades únicas
  }

  loadTeams() {
    this.loading.set(true);

    const req: TeamListRequest = {
      q: this.search(),
      page: this.page(),
      city: this.filterCity(),
      size: this.size,
      sort: 'name'
    };

    this.teamService.list(req).subscribe({
      next: (res: PagedResult<TeamDto>) => {
        let items = res.items;

        // aplicar filtro de ciudad en el frontend
        if (this.filterCity()) {
          items = items.filter(t => t.city?.toLowerCase() === this.filterCity().toLowerCase());
        }

        this.teams = items;
        this.total.set(res.total);
        this.loading.set(false);
      },
      error: () => {
        this.notification.set('No se pudieron cargar los equipos');
        setTimeout(() => this.notification.set(null), 3000);
        this.loading.set(false);
      }
    });
  }

  loadAllCities() {
    // obtenemos todas las ciudades de todos los equipos (sin paginar)
    this.teamService.list({ page: 1, size: 1000 }).subscribe({
      next: (res) => {
        const cities = res.items
          .map(t => t.city)
          .filter((c): c is string => !!c);
        this.allCities = [...new Set(cities)]; // valores únicos
      }
    });
  }

  onSearch() {
    this.page.set(1);
    this.loadTeams();
  }

  onFilterCity(city: string) {
    this.filterCity.set(city);
    this.page.set(1);
    this.loadTeams();
  }

  nextPage() {
    if (this.page() * this.size < this.total()) {
      this.page.set(this.page() + 1);
      this.loadTeams();
    }
  }

  prevPage() {
    if (this.page() > 1) {
      this.page.set(this.page() - 1);
      this.loadTeams();
    }
  }

  editTeam(id: number) {
    this.router.navigate(['/admin/teams', id]);
  }

  deleteTeam(id: number) {
    if (confirm('¿Seguro que deseas eliminar este equipo?')) {
      this.teamService.delete(id).subscribe({
        next: () => {
          this.loadTeams();
          this.notification.set('Equipo eliminado correctamente');
          setTimeout(() => this.notification.set(null), 3000);
        },
        error: () => {
          this.notification.set('No se pudo eliminar el equipo');
          setTimeout(() => this.notification.set(null), 3000);
        }
      });
    }
  }
}
