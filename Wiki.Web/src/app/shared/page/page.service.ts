import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Subject, finalize, startWith, switchMap } from 'rxjs';

export type Page = {
  id: string;
  title: string;
  content: string;
};

@Injectable({
  providedIn: 'root',
})
export class PageService {
  #http = inject(HttpClient);

  #updatePages$ = new Subject<void>();

  pages = toSignal(
    this.#updatePages$.pipe(
      startWith(undefined),
      switchMap(() => this.getPages())
    ),
    { initialValue: [] }
  );

  getPages() {
    return this.#http.get<Page[]>('/api/pages');
  }

  getPageById(id: string) {
    return this.#http.get<Page>(`/api/pages/${id}`);
  }

  createPage(page: Partial<Page>) {
    return this.#http
      .post<Page>('/api/pages', page)
      .pipe(finalize(() => this.#updatePages$.next()));
  }

  updatePage(page: Page) {
    return this.#http
      .put<Page>(`/api/pages/${page.id}`, page)
      .pipe(finalize(() => this.#updatePages$.next()));
  }

  deletePage(id: string) {
    return this.#http
      .delete<void>(`/api/pages/${id}`)
      .pipe(finalize(() => this.#updatePages$.next()));
  }

  searchPages(query: string) {
    return this.#http.get<Page[]>('/api/pages/search', {
      params: { query },
    });
  }
}
