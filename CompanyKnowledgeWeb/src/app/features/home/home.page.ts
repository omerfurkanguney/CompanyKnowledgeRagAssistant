import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../core/api.service';
import { HomeSummary, RecentDocument } from '../../core/api.models';

@Component({
  selector: 'app-home-page',
  imports: [
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
  protected readonly categoryCounts = computed(() => this.summary()?.categoryDocumentCounts ?? []);
  protected readonly departmentCounts = computed(() => this.summary()?.departmentDocumentCounts ?? []);
  protected readonly sources = [
    { name: '01-yillik-izin-politikasi.pdf', score: 92, page: 8, section: 5 },
    { name: '02-izin-ve-devamsizlik.docx', score: 78, page: 12, section: 3 },
    { name: '05-seyahat-politikasi.pdf', score: 61, page: 5, section: 2 },
  ];
  protected readonly suggestedQuestions = [
    'Yıllık izin devredilebilir mi?',
    'İzin iptali nasıl yapılır?',
    'Ücretli izin hakları nelerdir?',
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
