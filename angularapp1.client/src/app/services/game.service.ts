import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateGameDto { homeName: string; awayName: string; quarterSeconds: number; }
export interface TeamDto {
  id: number;
  name: string;
  isHome: boolean;
  score: number;
  foulsTeam: number;
}
export interface GameDto {
  id: number;
  status: string;
  quarter: number;
  quarterSeconds: number;
  home?: TeamDto | null;   
  away?: TeamDto | null;   
}
export interface UpdateScoreDto { teamId: number; deltaPoints: number; }


@Injectable({ providedIn: 'root' })
export class GameService {
  private http = inject(HttpClient);
  private baseUrl = '/api/games'; // usando proxy

  create(dto: CreateGameDto): Observable<GameDto> {
    return this.http.post<GameDto>(this.baseUrl, dto);
  }
  getById(id: number): Observable<GameDto> {
    return this.http.get<GameDto>(`${this.baseUrl}/${id}`);
  }
  updateScore(gameId: number, dto: UpdateScoreDto): Observable<GameDto> {
    return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/score`, dto);
  }
}
