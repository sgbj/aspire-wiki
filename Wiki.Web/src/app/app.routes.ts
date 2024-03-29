import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'pages',
    loadComponent: () =>
      import('./pages/pages.component').then((m) => m.PagesComponent),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/get-started/get-started.component').then(
            (m) => m.GetStartedComponent
          ),
      },
      {
        path: ':id',
        loadComponent: () =>
          import('./pages/page/page.component').then((m) => m.PageComponent),
      },
    ],
  },
  {
    path: 'chat',
    loadComponent: () =>
      import('./chat/chat.component').then((m) => m.ChatComponent),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
