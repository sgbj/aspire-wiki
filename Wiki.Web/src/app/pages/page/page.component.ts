import {
  Component,
  TemplateRef,
  computed,
  inject,
  input,
  signal,
  viewChild,
} from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { combineLatest, switchMap, tap } from 'rxjs';
import { JsonPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { EditorModule } from '@tinymce/tinymce-angular';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { PageService } from '../../shared/page/page.service';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { PageFabComponent } from '../../shared/page-fab/page-fab.component';

@Component({
  selector: 'app-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    JsonPipe,
    MatButtonModule,
    MatDialogModule,
    MatDividerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    EditorModule,
    PageFabComponent,
  ],
  templateUrl: './page.component.html',
  styleUrl: './page.component.scss',
})
export class PageComponent {
  #router = inject(Router);
  #sanitizer = inject(DomSanitizer);
  #dialog = inject(MatDialog);
  #pageService = inject(PageService);

  id = input.required<string>();

  deleteDialog = viewChild<TemplateRef<any>>('deleteDialog');

  edit = signal(false);
  page = toSignal(
    combineLatest([toObservable(this.id), toObservable(this.edit)]).pipe(
      switchMap(([id]) => this.#pageService.getPageById(id)),
      tap((page) => this.contentControl.setValue(page.content))
    )
  );
  safeContent = computed(() =>
    this.#sanitizer.bypassSecurityTrustHtml(this.page()?.content ?? '')
  );

  contentControl = new FormControl('', { nonNullable: true });

  onCreate() {
    this.#pageService
      .createPage({
        title: 'New page',
        content: `
          <h1>New page</h1>
          <p>Page content</p>
        `,
      })
      .subscribe((page) => {
        this.#router.navigate(['/pages', page.id]);
      });
  }

  onEdit() {
    this.edit.set(true);
  }

  onDelete() {
    this.#dialog
      .open(this.deleteDialog()!)
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.#pageService.deletePage(this.id()).subscribe(() => {
            this.#router.navigate(['/pages']);
          });
        }
      });
  }

  onSave() {
    const content = this.contentControl.value;
    const parser = new DOMParser().parseFromString(content, 'text/html');
    const title = parser.querySelector('h1')?.textContent ?? 'Untitled';

    this.#pageService
      .updatePage({
        id: this.id(),
        title,
        content,
      })
      .subscribe(() => {
        this.edit.set(false);
      });
  }

  onCancel() {
    this.edit.set(false);
  }
}
