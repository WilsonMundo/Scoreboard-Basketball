import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';

interface WeatherForecast { date: string; temperatureC: number; temperatureF: number; summary: string; }

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private http = inject(HttpClient);

  forecasts = toSignal(
    this.http.get<WeatherForecast[]>('/weatherforecast').pipe(
      catchError(err => { console.error(err); return of([]); })
    ),
    { initialValue: [] }
  );

  title = 'angularapp1.client';
}
