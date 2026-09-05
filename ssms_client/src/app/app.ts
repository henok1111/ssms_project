import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeToggleComponent } from './components/theme-toggle/theme-toggle.component';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,ThemeToggleComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})

export class App {
  
  title = 'ssms-client';
}
