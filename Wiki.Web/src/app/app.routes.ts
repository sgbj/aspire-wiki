import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'page/:id',
    loadComponent: () =>
      import('./page/page.component').then((m) => m.PageComponent),
  },
];
