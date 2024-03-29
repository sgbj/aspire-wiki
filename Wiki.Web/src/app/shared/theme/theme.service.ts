import { Injectable, effect, inject, signal } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';

export type Theme = 'light' | 'dark' | 'system';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  #mediaMatcher = inject(MediaMatcher);
  theme = signal<Theme>(localStorage['theme'] ?? 'system');

  constructor() {
    effect(() => {
      const theme = this.theme();
      localStorage.setItem('theme', theme);
      document.body.classList.toggle(
        'dark-theme',
        theme === 'dark' ||
          (theme === 'system' &&
            this.#mediaMatcher.matchMedia('(prefers-color-scheme: dark)')
              .matches)
      );
    });
  }
}
