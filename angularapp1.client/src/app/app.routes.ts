import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';

export const routes: Routes = [
  // Login sin layout
  { path: 'login', component: LoginComponent },

  // Rutas con layout principal
  {
    path: '',
    loadComponent: () =>
      import('./layout/layout.component').then(m => m.LayoutComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./pages/home/home.component').then(m => m.HomeComponent),
        canActivate: [AuthGuard]
      },
      {
        path: 'games/setup',
        loadComponent: () =>
          import('./components/game-setup/game-setup.component').then(
            m => m.GameSetupComponent
          ),
        canActivate: [AuthGuard]
      },
      {
        path: 'games/list',
        loadComponent: () =>
          import('./components/game-list/game-list.component').then(
            m => m.GameListComponent
          ),
        canActivate: [AuthGuard]
      },
      {
        path: 'games/:id',
        loadComponent: () =>
          import('./components/game/game.component').then(
            m => m.GameComponent
          ),
        canActivate: [AuthGuard]
      },

      // Rutas de administración (solo Admins)
      {
        path: 'admin',
        loadComponent: () =>
          import('./pages/admin/admin-layout.component').then(
            m => m.AdminLayoutComponent
          ),
        canActivate: [AdminGuard],
        children: [

          {
            path: '',
            pathMatch: 'full',
            loadComponent: () =>
              import('./pages/admin/admin-home.component').then(
                m => m.AdminHomeComponent
              ),
            canActivate: [AdminGuard]
          },
          {
            path: 'teams',
            loadComponent: () =>
              import('./pages/admin/teams/team-list.component').then(
                m => m.TeamListComponent
              ),
            canActivate: [AdminGuard]
          },
          {
            path: 'teams/new',
            loadComponent: () =>
              import('./pages/admin/teams/team-form.component').then(
                m => m.TeamFormComponent
              ),
            canActivate: [AdminGuard]
          },
          {
            path: 'teams/:id',
            loadComponent: () =>
              import('./pages/admin/teams/team-form.component').then(
                m => m.TeamFormComponent
              ),
            canActivate: [AdminGuard]
          },
          // ---------------- JUGADORES ----------------
          {
            path: 'players',
            loadComponent: () =>
              import('./pages/admin/players/player-list.component').then(
                m => m.PlayerListComponent
              ),
            canActivate: [AdminGuard]
          },
          {
            path: 'players/new',
            loadComponent: () =>
              import('./pages/admin/players/player-form.component').then(
                m => m.PlayerFormComponent
              ),
            canActivate: [AdminGuard]
          },
          {
            path: 'players/:id',
            loadComponent: () =>
              import('./pages/admin/players/player-form.component').then(
                m => m.PlayerFormComponent
              ),
            canActivate: [AdminGuard]
          },
          /* {
             path: 'matches',
             loadComponent: () =>
               import('./pages/admin/matches/match-list.component').then(
                 m => m.MatchListComponent
               ),
             canActivate: [AdminGuard]
           },*/
          { path: '', pathMatch: 'full', redirectTo: 'teams' }
        ]
      },

      // Fallback
      { path: '**', redirectTo: '' }
    ]
  }
];
