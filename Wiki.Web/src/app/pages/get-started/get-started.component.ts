import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PageFabComponent } from '../../shared/page-fab/page-fab.component';
import { Router } from '@angular/router';
import { PageService } from '../../shared/page/page.service';

@Component({
  selector: 'app-get-started',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, PageFabComponent],
  templateUrl: './get-started.component.html',
  styleUrl: './get-started.component.scss',
})
export class GetStartedComponent {
  #router = inject(Router);
  #pageService = inject(PageService);

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
}
