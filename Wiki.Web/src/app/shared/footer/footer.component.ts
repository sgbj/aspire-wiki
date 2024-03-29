import { Component } from '@angular/core';

const emojis = ['🧋', '☕', '😻', '❣️', '💻', '✨'];

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  emoji = emojis[Math.floor(Math.random() * emojis.length)];
}
