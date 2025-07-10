import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PushNotificationService {
  private connection: signalR.HubConnection | null = null;
  public notifications: { message: string; timestamp: string }[] = [];

  constructor(private authService: AuthService) {}

  startConnection(): void {
    // Prevent double connections
    if (this.connection && this.connection.state !== signalR.HubConnectionState.Disconnected) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiBaseUrl}/notificationHub`, {
        accessTokenFactory: async () => {
        try {
            if (this.authService.isAccessTokenExpired()) {
                await this.authService.refreshToken(); 
            }

            const token = this.authService.getAccessToken();

            if (!token) {
                console.error('[SignalR] Token missing after refresh!');
                throw new Error('Access token is missing');
            }

            return token;
        } catch (error) {
            console.error('[SignalR] Failed to refresh token for SignalR connection', error);
            throw error;
        }
        },
        withCredentials: false
      })
      .withAutomaticReconnect()
      .build();

    // Listen for incoming notifications
    this.connection.on('NewPropertyUploaded', (title: string, location: string, time: string) => {
      this.notifications.unshift({
        message: `New property posted!\n ${title} at ${location}`,
        timestamp: new Date(time).toLocaleString()
      });
    });

    this.connection.on('NewInquiryReceived', (propertyTitle: string, buyerName: string, time: string) => {
        this.notifications.unshift({
            message: `New inquiry!\n ${buyerName} contacted for ${propertyTitle}`,
            timestamp: new Date(time).toLocaleString()
        });
    });


    // Start connection
    this.connection
      .start()
      .then(() => console.log('SignalR connected to notificationHub'))
      .catch(err => console.error('SignalR connection error:', err));
  }

  stopConnection(): void {
    if (this.connection) {
      this.connection.stop().then(() => {
        console.log('SignalR connection stopped');
        this.connection = null;
      });
    }
  }

  getNotifications() {
    return this.notifications;
  }

  removeNotification(index: number): void {
    this.notifications.splice(index, 1);
  }

}
