import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
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
  protected readonly loading = signal(false);
  protected readonly uploading = signal(false);
  protected readonly workingDocumentId = signal<string | null>(null);
  protected readonly displayedColumns = ['fileName', 'status', 'size', 'createdAt', 'actions'];

  ngOnInit(): void {
    this.loadDocuments();
  }

  loadDocuments(): void {
    this.loading.set(true);
    this.api
      .listDocuments()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (documents) => this.documents.set(documents),
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
          this.snackBar.open('Doküman yüklendi.', 'Kapat', { duration: 3000 });
          this.loadDocuments();
        },
        error: () => this.snackBar.open('Upload başarısız.', 'Kapat', { duration: 4000 }),
      });
  }

  process(document: DocumentItem): void {
    this.runDocumentAction(document.id, () => this.api.processDocument(document.id), 'Doküman işlendi.');
  }

  embed(document: DocumentItem): void {
    this.runDocumentAction(document.id, () => this.api.embedDocument(document.id), 'Embedding tamamlandı.');
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
}
