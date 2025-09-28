import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// DTOs del backend
export interface PlayerListRequest {
  q?: string;
  teamId?: number | null;
  page?: number;
  size?: number;
  sort?: string;
}

export interface PlayerDto {
  id: number;
  fullName: string;
  number?: number | null;
  position?: string | null;
  heightMeters?: number | null;
  age?: number | null;
  nationality?: string | null;
  teamId?: number | null;
  teamName?: string | null;
  createdAtUtc: string;
}

export interface PlayerCreateDto {
  fullName: string;
  number?: number | null;
  position?: string | null;
  heightMeters?: number | null;
  age?: number | null;
  nationality?: string | null;
  teamId?: number | null;
}

export interface PlayerUpdateDto {
  fullName: string;
  number?: number | null;
  position?: string | null;
  heightMeters?: number | null;
  age?: number | null;
  nationality?: string | null;
  teamId?: number | null;
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
export class PlayerService {
  private http = inject(HttpClient);
  private baseUrl = '/api/players'; // usando proxy

  // Listar jugadores con búsqueda, paginación y filtros
  list(req?: PlayerListRequest): Observable<PagedResult<PlayerDto>> {
  const params: any = {};

  if (req?.q) params['q'] = req.q;
  if (req?.teamId != null) params['teamId'] = req.teamId.toString();
  if (req?.page != null) params['page'] = req.page.toString();
  if (req?.size != null) params['size'] = req.size.toString();
  if (req?.sort) params['sort'] = req.sort;

  return this.http.get<PagedResult<PlayerDto>>(this.baseUrl, { params });
}

  // Obtener jugador por ID
  getById(id: number): Observable<PlayerDto> {
    return this.http.get<PlayerDto>(`${this.baseUrl}/${id}`);
  }

  // Crear jugador
  create(dto: PlayerCreateDto): Observable<PlayerDto> {
    return this.http.post<PlayerDto>(this.baseUrl, dto);
  }

  // Actualizar jugador
  update(id: number, dto: PlayerUpdateDto): Observable<PlayerDto> {
    return this.http.put<PlayerDto>(`${this.baseUrl}/${id}`, dto);
  }

  // Eliminar jugador
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
