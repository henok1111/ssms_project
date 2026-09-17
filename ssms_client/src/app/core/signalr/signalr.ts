import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalrService {
  private hubConnection?: signalR.HubConnection;
  readonly isConnected = signal(false);

  connect(): void {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl.replace('/api', '')}/hubs/chat`, {
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => this.isConnected.set(true))
      .catch(err => console.error('SignalR connection failed:', err));
  }

  joinListingConversation(conversationId: string): void {
    this.hubConnection?.invoke('JoinListingConversation', conversationId);
  }

  leaveListingConversation(conversationId: string): void {
    this.hubConnection?.invoke('LeaveListingConversation', conversationId);
  }

  onListingMessage(callback: (message: any) => void): void {
    this.hubConnection?.on('ReceiveListingMessage', callback);
  }

  offListingMessage(): void {
    this.hubConnection?.off('ReceiveListingMessage');
  }

joinJobGroup(jobId: string): void {
  this.hubConnection?.invoke('JoinJobGroup', jobId);
}

leaveJobGroup(jobId: string): void {
  this.hubConnection?.invoke('LeaveJobGroup', jobId);
}

onJobMessage(callback: (message: any) => void): void {
  this.hubConnection?.on('ReceiveMessage', callback);
}

offJobMessage(): void {
  this.hubConnection?.off('ReceiveMessage');
}

onNotification(callback: (notification: any) => void): void {
  this.hubConnection?.on('ReceiveNotification', callback);
}

offNotification(): void {
  this.hubConnection?.off('ReceiveNotification');
}
}