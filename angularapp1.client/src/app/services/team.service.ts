import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


// DTOs del backend
export interface TeamListRequest {
  q?: string;   // coincide con el backend: TeamListRequest.Q
  page?: number;
  size?: number;
  sort?: string;
  city?: string; // nuevo filtro de ciudad  
}

export interface TeamDto {
  id: number;
  name: string;
  city: string;
  logoUrl?: string | null;
  createdAtUtc?: string;
}

export interface TeamCreateDto {
  name: string;
  city?: string;
}

export interface TeamUpdateDto {
  name: string;
  city?: string;
}

// Respuesta paginada del backend
export interface PagedResult<T> {
  page: number;
  size: number;
  total: number;
  items: T[];
}

export interface ResultAPI<T> {
  isSuccess: boolean;
  code: number;
  message: string;
  result: T;
}

@Injectable({ providedIn: 'root' })
export class TeamService {
  private http = inject(HttpClient);
  private baseUrl = '/api/teams'; // usando proxy

  // Listar equipos con búsqueda, paginación y orden
  list(req?: TeamListRequest): Observable<PagedResult<TeamDto>> {
    return this.http.get<PagedResult<TeamDto>>(this.baseUrl, {
      params: { ...req }
    });
  }

  // Obtener equipo por ID
  getById(id: number): Observable<TeamDto> {
    return this.http.get<TeamDto>(`${this.baseUrl}/${id}`);
  }

  // Crear equipo
  create(dto: TeamCreateDto): Observable<TeamDto> {
  return this.http.post<TeamDto>(this.baseUrl, dto);
     // ← extraemos solo el TeamDto
}

  // Actualizar equipo
  update(id: number, dto: TeamUpdateDto): Observable<TeamDto> {
    return this.http.put<TeamDto>(`${this.baseUrl}/${id}`, dto);
    
  }

  // Eliminar equipo
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  // Subir logo
  uploadLogo(id: number, file: File): Observable<TeamDto> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<ResultAPI<TeamDto>>(`${this.baseUrl}/${id}/logo`, form).pipe(
    map(res => res.result)
  );
  }
}
