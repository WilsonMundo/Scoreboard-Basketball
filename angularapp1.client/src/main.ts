// main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';


bootstrapApplication(AppComponent, appConfig)
  .catch(err => console.error(err));
