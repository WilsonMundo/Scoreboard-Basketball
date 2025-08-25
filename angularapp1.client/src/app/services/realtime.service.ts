import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private hub?: signalR.HubConnection;

  async connect(baseUrl: string) {
    if (this.hub?.state === signalR.HubConnectionState.Connected) return;
    this.hub = new signalR.HubConnectionBuilder()
       .configureLogging(signalR.LogLevel.Warning)
      .withUrl(`${baseUrl}/hubs/game`, { withCredentials: true }) // usa proxy o URL absoluta
      .withAutomaticReconnect()
      .build();
    await this.hub.start();
  }

  async disconnect() { await this.hub?.stop(); }

  async joinGame(gameId: number) { await this.hub?.invoke('JoinGame', gameId); }
  async leaveGame(gameId: number) { await this.hub?.invoke('LeaveGame', gameId); }

  onGameUpdated(handler: (dto: any) => void) {
    this.hub?.on('GameUpdated', handler);
  }

  offGameUpdated(handler: (dto: any) => void) {
    this.hub?.off('GameUpdated', handler);
  }

  state() { return this.hub?.state ?? signalR.HubConnectionState.Disconnected; }
}
