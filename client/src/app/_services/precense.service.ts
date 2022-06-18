import { Router } from '@angular/router';
import { filter, take } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

      this.hubConnection
        .start()
        .catch(error => console.log(error));

      // TODO: when user logs in it doesn't update you need to refresh to see that they're online. 
      this.hubConnection.on('UserIsOnline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
          this.onlineUserSource.next([...usernames, username])
        })
      })

      this.hubConnection.on('UserIsOffline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
          this.onlineUserSource.next([...usernames.filter(x => x !== username)])
        })
      })

      this.hubConnection.on("GetOnlineUsers", (usernames: string[]) => {
        this.onlineUserSource.next(usernames);
      })

      this.hubConnection.on("NewMessageReceived", ({username, knownAs}) => {
        this.toastr.info(knownAs + ' has sent you a new message!')
        .onTap
        .pipe(take(1)).subscribe(() => {
          this.router.navigateByUrl('/members/' + username + '?tab=3')
        })
      })
  }

  stopHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
  }

}
