import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Login } from './feature/auth/login/login';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
//   protected title = 'OnlineAuctionFrontend';

//   toggle() {
//   this.toggleDarkMode();
// }

// toggleDarkMode() {
//   document.body.classList.toggle('dark');
// }



}
