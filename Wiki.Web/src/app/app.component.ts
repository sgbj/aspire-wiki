import { HttpClient } from '@angular/common/http';
import {
  Component,
  HostListener,
  TemplateRef,
  ViewChild,
  inject,
  viewChild,
} from '@angular/core';
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import {
  MatDialog,
  MatDialogModule,
  MatDialogRef,
  MatDialogState,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounce, debounceTime, of, switchMap } from 'rxjs';
import { ThemeService } from './shared/theme/theme.service';
import { FooterComponent } from './shared/footer/footer.component';
import { Page } from './shared/page/page.service';
import { SearchComponent } from './shared/search/search.component';
import { ThemePickerComponent } from './shared/theme-picker/theme-picker.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatSidenavModule,
    MatToolbarModule,
    MatTooltipModule,
    SearchComponent,
    ThemePickerComponent,
    FooterComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  #router = inject(Router);
  #http = inject(HttpClient);
  #dialog = inject(MatDialog);
  themeService = inject(ThemeService);

  addDialog = viewChild<TemplateRef<any>>('addDialog');

  onAdd() {
    this.#dialog
      .open(this.addDialog()!)
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.#http
            .post<Page>('/api/pages', {
              title: 'New page',
              content: '',
            })
            .subscribe((result) => {
              this.#router.navigate(['/page', result.id]);
            });
        }
      });
  }
}
