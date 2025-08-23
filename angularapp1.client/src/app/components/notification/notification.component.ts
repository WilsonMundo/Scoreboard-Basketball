import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';

  @Component({
    selector: 'app-notification',
    standalone: true,
    imports: [NgIf],
    templateUrl: './notification.component.html',
    styleUrls: ['./notification.component.css']
  })
export class NotificationComponent {
  @Input() message: string | null = null;
}
