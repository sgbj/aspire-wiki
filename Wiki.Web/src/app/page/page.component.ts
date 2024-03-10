import { HttpClient } from '@angular/common/http';
import { Component, effect, inject, input, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { combineLatest, switchMap, tap } from 'rxjs';
import { JsonPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MarkdownModule } from 'ngx-markdown';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Page } from './page';

@Component({
  selector: 'app-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    JsonPipe,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MarkdownModule,
  ],
  templateUrl: './page.component.html',
  styleUrl: './page.component.scss',
})
export class PageComponent {
  #http = inject(HttpClient);
  #fb = inject(FormBuilder);

  id = input.required<string>();
  edit = signal(false);
  page = toSignal(
    combineLatest([toObservable(this.id), toObservable(this.edit)]).pipe(
      switchMap(([id]) => this.#http.get<Page>(`/api/pages/${id}`)),
      tap((page) => this.form.patchValue(page))
    )
  );

  form = this.#fb.group({
    title: ['', Validators.required],
    content: [''],
  });

  onEdit() {
    this.edit.set(true);
  }

  onSave() {
    this.#http.put(`/api/pages/${this.id()}`, this.form.value).subscribe(() => {
      this.edit.set(false);
    });
  }

  onCancel() {
    this.edit.set(false);
  }
}
