import { inject, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PushNotificationService {
  private connection: signalR.HubConnection | null = null;

  public notifications: { message: string; timestamp: string }[] = [];
  public unseenCount = 0;

  private authService = inject(AuthService);

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
    // this.connection.on('NewPropertyUploaded', (title: string, location: string, time: string) => {
    //   this.notifications.unshift({
    //     message: `New property posted!\n ${title} at ${location}`,
    //     timestamp: new Date(time).toLocaleString()
    //   });
    // });

    // this.connection.on('NewInquiryReceived', (propertyTitle: string, buyerName: string, time: string) => {
    //     this.notifications.unshift({
    //         message: `New inquiry!\n ${buyerName} contacted for ${propertyTitle}`,
    //         timestamp: new Date(time).toLocaleString()
    //     });
    // });
    this.connection.on('NewPropertyUploaded', (title: string, location: string, time: string) => {
      this.addNotification(`New property posted!\n${title} at ${location}`, time);
    });

    this.connection.on('NewInquiryReceived', (propertyTitle: string, buyerName: string, time: string) => {
      this.addNotification(`New inquiry!\n${buyerName} contacted for ${propertyTitle}`, time);
    });


    // Start connection
    this.connection.start()
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

  private addNotification(message: string, time: string): void {
    this.notifications.unshift({
      message,
      timestamp: new Date(time).toLocaleString()
    });
    this.unseenCount++;
  }


  getNotifications() {
    return this.notifications;
  }

  // removeNotification(index: number): void {
  //   this.notifications.splice(index, 1);
  // }
  removeNotification(index: number): void {
    if (index >= 0 && index < this.notifications.length) {
      this.notifications.splice(index, 1);
      if (this.unseenCount > 0) {
        this.unseenCount--;
      }
    }
  }

  markAllAsSeen(): void {
    this.unseenCount = 0;
  }

  clearAll(): void {
    this.notifications = [];
    this.unseenCount = 0;
  }

}
