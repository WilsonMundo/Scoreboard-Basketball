import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PlayerService, PlayerCreateDto, PlayerUpdateDto, PlayerDto } from '../../../services/player.service';
import { TeamService, TeamDto } from '../../../services/team.service';
import { NotificationComponent } from '../../../components/notification/notification.component';

@Component({
  selector: 'app-player-form',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationComponent],
  templateUrl: './player-form.component.html'
})
export class PlayerFormComponent implements OnInit {
  id: number | null = null;

  // campos del jugador
  fullName = '';
  number: number=0;
  position = '';
  heightMeters: number | null = null;
  age: number | null = null;
  nationality = '';
  teamId: number | null = null;

  // equipos disponibles para dropdown
  teams: TeamDto[] = [];

  // notificación
  notification = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private playerService: PlayerService,
    private teamService: TeamService
  ) {}

  ngOnInit(): void {
    // cargar equipos
    this.teamService.list().subscribe({
      next: (res) => (this.teams = res.items),
      error: () => this.showError('No se pudieron cargar los equipos')
    });

    // si es edición
    this.id = Number(this.route.snapshot.paramMap.get('id')) || null;
    if (this.id) {
      this.playerService.getById(this.id).subscribe({
        next: (player) => {
          this.fullName = player.fullName;
          this.number = player.number ?? 0;
          this.position = player.position ?? '';
          this.heightMeters = player.heightMeters ?? null;
          this.age = player.age ?? null;
          this.nationality = player.nationality ?? '';
          this.teamId = player.teamId ?? null;
        },
        error: () => this.showError('No se pudo cargar el jugador')
      });
    }
  }

  save() {
    if (!this.fullName.trim()) {
      this.showError('El nombre es obligatorio');
      return;
    }

    if (this.id) {
      // actualizar
      const dto: PlayerUpdateDto = {
        fullName: this.fullName,
        number: this. number?? 0,
        position: this.position,
        heightMeters: this.heightMeters,
        age: this.age ?? 0,
        nationality: this.nationality,
        teamId: this.teamId
      };

      this.playerService.update(this.id, dto).subscribe({
        next: () => {
          this.notification.set('Jugador actualizado correctamente');
          setTimeout(() => this.router.navigate(['/admin/players']), 2000);
        },
        error: (err) => this.showError(err?.error ?? 'No se pudo actualizar el jugador')
      });
    } else {
      // crear
      const dto: PlayerCreateDto = {
        fullName: this.fullName,
        number: this.number?? 0,
        position: this.position,
        heightMeters: this.heightMeters,
        age: this.age ?? 0,
        nationality: this.nationality,
        teamId: this.teamId
      };

      this.playerService.create(dto).subscribe({
        next: () => {
          this.notification.set('Jugador creado correctamente');
          setTimeout(() => this.router.navigate(['/admin/players']), 2000);
        },
        error: (err) => this.showError(err?.error ?? 'No se pudo crear el jugador')
      });
    }
  }

  private showError(message: string) {
    this.notification.set(message);
    setTimeout(() => this.notification.set(null), 3000);
  }
}
