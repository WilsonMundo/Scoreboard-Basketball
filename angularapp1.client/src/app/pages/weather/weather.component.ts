// pages/weather/weather.component.ts
import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';

interface WeatherForecast { date: string; temperatureC: number; temperatureF: number; summary: string; }

@Component({
  selector: 'app-weather',
  standalone: true,
  template: `
    <h1>Weather forecast</h1>
    @if (forecasts().length === 0) {
      <p><em>Loading...</em></p>
    } @else {
      <table>
        <thead>
        <tr><th>Date</th><th>Temp. (C)</th><th>Temp. (F)</th><th>Summary</th></tr>
        </thead>
        <tbody>
          @for (f of forecasts(); track f.date) {
            <tr>
              <td>{{ f.date }}</td>
              <td>{{ f.temperatureC }}</td>
              <td>{{ f.temperatureF }}</td>
              <td>{{ f.summary }}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `
})
export class WeatherComponent {
  private http = inject(HttpClient);
  forecasts = toSignal(
    this.http.get<WeatherForecast[]>('/weatherforecast').pipe(
      catchError(err => { console.error(err); return of([]); })
    ),
    { initialValue: [] }
  );
}
