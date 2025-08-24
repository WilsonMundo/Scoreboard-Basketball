import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface QuarterPointsDto {
  quarter: number;
  home: number;
  away: number;
  isOvertime: boolean;
}
export interface CreateGameDto { homeName: string; awayName: string; quarterSeconds: number; }
export interface TeamDto {
  id: number;
  name: string;
  isHome: boolean;
  score: number;
  foulsTeam: number;
  fouls: number;
  logoUrl?: string | null;
}
export interface GameDto {
  id: number;
  status: string;
  quarter: number;
  quarterSeconds: number;
  remainingSeconds: number;  
  isTimerRunning: boolean; 
  home?: TeamDto | null;   
  away?: TeamDto | null;
  box: QuarterPointsDto[];
}
export interface GameListItemDto {
  id: number;
  status: string;
  quarter: number;
  homeName?: string | null;
  awayName?: string | null;
  homeScore: number;
  awayScore: number;
  createdAtUtc: string;
}


export interface UpdateScoreDto { teamId: number; deltaPoints: number; }
export const API_BASE = 'https://localhost:7022';


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
  uploadTeamLogo(gameId: number, teamId: number, file: File): Observable<GameDto> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/teams/${teamId}/logo`, form);
  }
  startTimer(gameId: number) { return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/timer/start`, {}); }
  pauseTimer(gameId: number) { return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/timer/pause`, {}); }
  resetTimer(gameId: number, newQuarterSeconds?: number) {
    return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/timer/reset`, { quarterSeconds: newQuarterSeconds });
  }
  nextQuarter(gameId: number) {
    return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/quarter/next`, {});
  }
  prevQuarter(gameId: number) {
    return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/quarter/prev`, {});
  }
  isTie(g: GameDto): boolean {
    return (g.home?.score ?? 0) === (g.away?.score ?? 0);
  }

  isFinished(g: GameDto): boolean {
    const h = g.home?.score ?? 0, a = g.away?.score ?? 0;
    return g.status === 'Finished' || (g.quarter >= 4 && g.remainingSeconds === 0 && h !== a);
  }

  canGoNext(g: GameDto): boolean {
    if (this.isFinished(g)) return false;
    if (g.quarter < 4) return true;        // Q1..Q3
    return this.isTie(g);                   // Q4/OT solo si empate
  }

  finish(gameId: number) {
    return this.http.post<GameDto>(`${this.baseUrl}/${gameId}/finish`, {});
  }
  updateFouls(gameId: number, teamId: number, delta: number) {
    return this.http.post<GameDto>(`/api/fouls/${gameId}/teams/${teamId}/fouls`, delta);
  }
  getAll() {
    return this.http.get<GameListItemDto[]>(`${this.baseUrl}`);
  }
}
