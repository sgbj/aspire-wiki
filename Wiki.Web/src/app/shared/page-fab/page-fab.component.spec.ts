import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageFabComponent } from './page-fab.component';

describe('PageFabComponent', () => {
  let component: PageFabComponent;
  let fixture: ComponentFixture<PageFabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PageFabComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PageFabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
