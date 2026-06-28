import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../core/api.service';
import { HomeSummary, RecentDocument } from '../../core/api.models';

@Component({
  selector: 'app-home-page',
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage implements OnInit {
  private readonly api = inject(ApiService);

  protected readonly summary = signal<HomeSummary | null>(null);
  protected readonly loading = signal(false);
  protected readonly recentDocuments = computed(() => this.summary()?.recentDocuments ?? []);
  protected readonly departmentCounts = computed(() => this.summary()?.departmentDocumentCounts ?? []);
  protected readonly suggestedQuestions = [
    'Yıllık izin devredilebilir mi?',
    'İzin iptali nasıl yapılır?',
    'Ücretli izin hakları nelerdir?',
    'Uzaktan çalışma kuralları nelerdir?',
    'Masraf iadesi için hangi belgeler gerekir?',
    'Bilgi güvenliği ihlali nasıl bildirilir?',
    'Onboarding süreci kaç gün sürer?',
  ];

  ngOnInit(): void {
    this.loadSummary();
  }

  loadSummary(): void {
    this.loading.set(true);
    this.api.getHomeSummary().subscribe({
      next: (summary) => {
        this.summary.set(summary);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  documentType(document: RecentDocument): string {
    if (document.fileName.toLowerCase().endsWith('.docx')) {
      return 'DOCX';
    }

    if (document.fileName.toLowerCase().endsWith('.xlsx')) {
      return 'XLSX';
    }

    return 'PDF';
  }

  statusLabel(status: string): string {
    const labels: Record<string, string> = {
      Uploaded: 'Yüklendi',
      Processing: 'İşleniyor',
      Processed: 'İşlendi',
      Embedding: 'Embedding',
      Indexed: 'İndekslendi',
      Failed: 'Hata',
      Deleted: 'Silindi',
    };

    return labels[status] ?? status;
  }
}
