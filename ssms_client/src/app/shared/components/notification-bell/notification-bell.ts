import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { NotificationService } from '../../../core/services/notification';
import { SignalrService } from '../../../core/signalr/signalr';
import { AuthService } from '../../../core/auth/auth';
import { NotificationResponse } from '../../../core/models/notification.model';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-bell.html',
  styleUrl: './notification-bell.scss'
})
export class NotificationBell implements OnInit, OnDestroy {
  private notificationService = inject(NotificationService);
  private signalr = inject(SignalrService);
  private authService = inject(AuthService);

  notifications = signal<NotificationResponse[]>([]);
  isOpen = signal(false);

  get unreadCount(): number {
    return this.notifications().filter(n => !n.isRead).length;
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) return;

    this.signalr.connect();
    this.notificationService.getMine().subscribe(list => this.notifications.set(list));

    this.signalr.onNotification((notification: NotificationResponse) => {
      this.notifications.update(list => [notification, ...list]);
    });
  }

  ngOnDestroy(): void {
    this.signalr.offNotification();
  }

  toggleOpen(): void {
    this.isOpen.update(v => !v);
  }

  onMarkAsRead(notification: NotificationResponse): void {
    if (notification.isRead) return;
    this.notificationService.markAsRead(notification.id).subscribe(() => {
      this.notifications.update(list =>
        list.map(n => n.id === notification.id ? { ...n, isRead: true } : n)
      );
    });
  }
}