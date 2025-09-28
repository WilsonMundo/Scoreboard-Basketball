import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TeamService, TeamCreateDto, TeamUpdateDto, TeamDto } from '../../../services/team.service';
import { NotificationComponent } from '../../../components/notification/notification.component';

@Component({
  selector: 'app-team-form',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationComponent],
  templateUrl: './team-form.component.html'
})
export class TeamFormComponent implements OnInit {
  id: number | null = null;
  name = '';
  city = '';
  logoFile: File | null = null;
  notification = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private teamService: TeamService
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id')) || null;
    if (this.id) {
      this.teamService.getById(this.id).subscribe({
        next: team => {
          this.name = team.name;
          this.city = team.city ?? '';
        },
        error: () => {
          this.notification.set('No se pudo cargar el equipo');
          setTimeout(() => this.notification.set(null), 3000);
        }
      });
    }
  }

  onFileChange(event: any) {
    this.logoFile = event.target.files[0];
  }

  save() {
    if (!this.name.trim()) {
      this.notification.set('El nombre es obligatorio');
      setTimeout(() => this.notification.set(null), 3000);
      return;
    }

    if (this.id) {
      // -------------------------
      // EDITAR EQUIPO
      // -------------------------
      const dto: TeamUpdateDto = { name: this.name, city: this.city };
      this.teamService.update(this.id, dto).subscribe({
        next: () => {
          if (this.logoFile) {
            this.teamService.uploadLogo(this.id!, this.logoFile!).subscribe({
              next: () => {
                this.notification.set('Equipo actualizado correctamente');
                setTimeout(() => this.router.navigate(['/admin/teams']), 2000);
              },
              error: (err) => {
                this.showError(err?.error ?? 'No se pudo subir el logo');
              }
            });
          } else {
            this.notification.set('Equipo actualizado correctamente');
            setTimeout(() => this.router.navigate(['/admin/teams']), 2000);
          }
        },
        error: (err) => {
          // Muestra el mensaje real del backend (ej: "Ya existe un equipo con ese nombre")
          this.showError(err?.error ?? 'No se pudo actualizar el equipo');
        }
      });
    } else {
      // -------------------------
      // CREAR EQUIPO
      // -------------------------
      const dto: TeamCreateDto = { name: this.name, city: this.city };

      this.teamService.create(dto).subscribe({
        next: (team: TeamDto) => {
          console.log('DEBUG team devuelto:', team);
          if (this.logoFile && team?.id) {
            this.teamService.uploadLogo(team.id, this.logoFile!).subscribe({
              next: () => {
                this.notification.set('Equipo creado correctamente');
                setTimeout(() => this.router.navigate(['/admin/teams']), 2000);
              },
              error: (err) => {
                this.showError(err?.error ?? 'Equipo creado, pero no se pudo subir el logo');
              }
            });
          } else {
            this.notification.set('Equipo creado correctamente');
            setTimeout(() => this.router.navigate(['/admin/teams']), 2000);
          }
        },
        error: (err) => {
          // Muestra el mensaje real del backend (ej: "Ya existe un equipo con ese nombre")
          this.showError(err?.error ?? 'No se pudo crear el equipo');
        }
      });
    }
  }

  private showError(message: string) {
    this.notification.set(message);
    setTimeout(() => this.notification.set(null), 3000);
  }
}
