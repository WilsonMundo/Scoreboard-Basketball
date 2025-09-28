import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
@Component({
  standalone: true,
  selector: 'app-admin-layout',
  imports: [CommonModule, RouterLink,RouterOutlet],
  templateUrl: './admin-layout.component.html',
  
})
export class AdminLayoutComponent {
    constructor(private auth: AuthService, private router: Router) {}

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']); 
  }
}