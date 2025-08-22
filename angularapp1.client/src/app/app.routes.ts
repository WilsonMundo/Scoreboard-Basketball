import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', pathMatch: 'full', loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) },
      {
        path: 'games/setup',
        loadComponent: () => import('./components/game-setup/game-setup.component').then(m => m.GameSetupComponent)
      },
      {
        path: 'games/:id',
        loadComponent: () => import('./components/game/game.component').then(m => m.GameComponent)
      },
      { path: '**', redirectTo: '' }
    ]
  }
];
