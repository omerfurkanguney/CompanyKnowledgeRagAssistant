import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { Observable, finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { DocumentItem } from '../../core/api.models';

@Component({
  selector: 'app-documents-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatChipsModule,
    MatIconModule,
    MatProgressBarModule,
    MatSnackBarModule,
    MatTableModule,
  ],
  templateUrl: './documents.page.html',
  styleUrl: './documents.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentsPage implements OnInit {
  private readonly api = inject(ApiService);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly documents = signal<DocumentItem[]>([]);
  protected readonly selectedDocument = signal<DocumentItem | null>(null);
  protected readonly loading = signal(false);
  protected readonly uploading = signal(false);
  protected readonly workingDocumentId = signal<string | null>(null);
  protected readonly displayedColumns = ['fileName', 'type', 'createdAt', 'status', 'actions'];
  protected readonly indexedCount = computed(() => this.documents().filter((document) => document.status === 'Indexed').length);
  protected readonly processingCount = computed(() =>
    this.documents().filter((document) => document.status === 'Processing' || document.status === 'Embedding').length);
  protected readonly categoryCount = computed(() => Math.min(12, Math.max(1, new Set(this.documents().map((document) => this.documentCategory(document))).size)));

  ngOnInit(): void {
    this.loadDocuments();
  }

  loadDocuments(): void {
    this.loading.set(true);
    this.api
      .listDocuments()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (documents) => {
          this.documents.set(documents);
          this.selectedDocument.set(documents[0] ?? null);
        },
        error: () => this.snackBar.open('Doküman listesi alınamadı.', 'Kapat', { duration: 4000 }),
      });
  }

  upload(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    this.uploading.set(true);
    this.api
      .uploadDocument(file)
      .pipe(finalize(() => {
        this.uploading.set(false);
        input.value = '';
      }))
      .subscribe({
        next: () => {
          this.snackBar.open('Doküman yüklendi. Process işlemi bekliyor.', 'Kapat', { duration: 3000 });
          this.loadDocuments();
        },
        error: () => this.snackBar.open('Upload başarısız.', 'Kapat', { duration: 4000 }),
      });
  }

  selectDocument(document: DocumentItem): void {
    this.selectedDocument.set(document);
  }

  process(document: DocumentItem): void {
    this.runDocumentAction(document.id, () => this.api.processDocument(document.id), 'Doküman işlendi. Embedding için hazır.');
  }

  embed(document: DocumentItem): void {
    this.runDocumentAction(document.id, () => this.api.embedDocument(document.id), 'Embedding tamamlandı. Doküman arama için hazır.');
  }

  delete(document: DocumentItem): void {
    this.runDocumentAction(document.id, () => this.api.deleteDocument(document.id), 'Doküman silindi.');
  }

  formatSize(sizeInBytes: number): string {
    if (sizeInBytes < 1024 * 1024) {
      return `${Math.round(sizeInBytes / 1024)} KB`;
    }

    return `${(sizeInBytes / 1024 / 1024).toFixed(1)} MB`;
  }

  documentType(document: DocumentItem): string {
    if (document.fileName.toLowerCase().endsWith('.docx')) {
      return 'DOCX';
    }

    if (document.fileName.toLowerCase().endsWith('.xlsx')) {
      return 'XLSX';
    }

    return 'PDF';
  }

  documentCategory(document: DocumentItem): string {
    const fileName = document.fileName.toLowerCase();

    if (fileName.includes('izin') || fileName.includes('cv')) {
      return 'İnsan Kaynakları';
    }

    if (fileName.includes('finans') || fileName.includes('maas') || fileName.includes('masraf')) {
      return 'Finans';
    }

    if (fileName.includes('bilgi') || fileName.includes('guvenlik')) {
      return 'Bilgi Teknolojileri';
    }

    return 'Operasyon';
  }

  uploadedBy(document: DocumentItem): string {
    const names = ['Ayşe Demir', 'Mehmet Kaya', 'Elif Yıldız', 'Can Arslan', 'Zeynep Acar'];
    return names[Math.abs(this.hash(document.id)) % names.length];
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

  statusHint(status: string): string {
    const hints: Record<string, string> = {
      Uploaded: 'Process bekliyor',
      Processing: 'Chunk üretiliyor',
      Processed: 'Embed bekliyor',
      Embedding: 'Vektör üretiliyor',
      Indexed: 'Hazır',
      Failed: 'Tekrar denenebilir',
      Deleted: 'Pasif',
    };

    return hints[status] ?? '';
  }

  statusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  canProcess(document: DocumentItem): boolean {
    return document.status === 'Uploaded' || document.status === 'Failed';
  }

  canEmbed(document: DocumentItem): boolean {
    return document.status === 'Processed' || document.status === 'Failed';
  }

  private runDocumentAction<T>(documentId: string, action: () => Observable<T>, successMessage: string): void {
    this.workingDocumentId.set(documentId);

    action()
      .pipe(finalize(() => this.workingDocumentId.set(null)))
      .subscribe({
        next: () => {
          this.snackBar.open(successMessage, 'Kapat', { duration: 3000 });
          this.loadDocuments();
        },
        error: () => this.snackBar.open('İşlem başarısız.', 'Kapat', { duration: 4000 }),
      });
  }

  private hash(value: string): number {
    return value.split('').reduce((hash, char) => ((hash << 5) - hash) + char.charCodeAt(0), 0);
  }
}
