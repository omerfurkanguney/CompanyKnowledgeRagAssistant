import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { forkJoin, finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiService } from '../../core/api.service';
import { ApiInfo, SystemHealth } from '../../core/api.models';

@Component({
  selector: 'app-system-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatSnackBarModule,
  ],
  templateUrl: './system.page.html',
  styleUrl: './system.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SystemPage implements OnInit {
  private readonly api = inject(ApiService);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly apiBaseUrl = environment.apiBaseUrl;
  protected readonly loading = signal(false);
  protected readonly info = signal<ApiInfo | null>(null);
  protected readonly health = signal<SystemHealth | null>(null);
  protected readonly rootHealth = signal<string | null>(null);

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);

    forkJoin({
      info: this.api.getApiInfo(),
      health: this.api.getSystemHealth(),
      rootHealth: this.api.getHealth(),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => {
          this.info.set(result.info);
          this.health.set(result.health);
          this.rootHealth.set(result.rootHealth);
        },
        error: () => this.snackBar.open('Sistem bilgileri alınamadı.', 'Kapat', { duration: 4000 }),
      });
  }
}
