import {
  Component,
  HostListener,
  TemplateRef,
  inject,
  viewChild,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
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
import { BreakpointObserver } from '@angular/cdk/layout';
import { RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { debounceTime, switchMap } from 'rxjs';
import { PageService } from '../page/page.service';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    AsyncPipe,
    RouterLink,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent {
  #breakpointObserver = inject(BreakpointObserver);
  #dialog = inject(MatDialog);
  #pageService = inject(PageService);

  breakpoint$ = this.#breakpointObserver.observe('(max-width: 800px)');

  searchDialog = viewChild<TemplateRef<any>>('searchDialog');
  searchDialogRef: MatDialogRef<any, any> | null = null;
  searchControl = new FormControl('', { nonNullable: true });

  searchResults = toSignal(
    this.searchControl.valueChanges.pipe(
      debounceTime(100),
      switchMap((query) => this.#pageService.searchPages(query))
    ),
    { initialValue: [] }
  );

  @HostListener('window:keydown.control.k', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    event.preventDefault();
    this.onSearch();
  }

  onSearch() {
    if (this.searchDialogRef?.getState() === MatDialogState.OPEN) {
      this.searchDialogRef.close();
    } else {
      this.searchControl.setValue('');
      this.searchDialogRef = this.#dialog.open(this.searchDialog()!, {
        panelClass: 'search-dialog',
        width: '100%',
        height: '100%',
        maxWidth: '800px',
        maxHeight: '80vh',
      });
    }
  }
}
