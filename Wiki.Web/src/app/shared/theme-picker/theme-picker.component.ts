import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Theme, ThemeService } from '../theme/theme.service';

@Component({
  selector: 'app-theme-picker',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatMenuModule, MatTooltipModule],
  templateUrl: './theme-picker.component.html',
  styleUrl: './theme-picker.component.scss',
})
export class ThemePickerComponent {
  themeService = inject(ThemeService);
  themes: Theme[] = ['system', 'light', 'dark'];
  themeConfigs = {
    system: {
      label: 'System',
      icon: 'contrast',
    },
    light: {
      label: 'Light',
      icon: 'light_mode',
    },
    dark: {
      label: 'Dark',
      icon: 'dark_mode',
    },
  };
}
