import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { Observable, finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { DepartmentLookup, DocumentCategoryLookup, DocumentItem } from '../../core/api.models';
import {
  UploadDocumentDialogData,
  UploadDocumentDialogComponent,
  UploadDocumentDialogResult,
} from './upload-document-dialog/upload-document-dialog.component';

@Component({
  selector: 'app-documents-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatChipsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTableModule,
  ],
  templateUrl: './documents.page.html',
  styleUrl: './documents.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentsPage implements OnInit, OnDestroy {
  private readonly api = inject(ApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly documents = signal<DocumentItem[]>([]);
  protected readonly selectedDocument = signal<DocumentItem | null>(null);
  protected readonly loading = signal(false);
  protected readonly uploading = signal(false);
  protected readonly workingDocumentId = signal<string | null>(null);
  protected readonly bulkActionRunning = signal(false);
  protected readonly departments = signal<DepartmentLookup[]>([]);
  protected readonly categories = signal<DocumentCategoryLookup[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly selectedCategory = signal('');
  protected readonly selectedFileType = signal('');
  protected readonly selectedStatus = signal('');
  protected readonly pageIndex = signal(0);
  protected readonly pageSize = 10;
  protected readonly displayedColumns = ['fileName', 'type', 'createdAt', 'status', 'actions'];
  protected readonly indexedCount = computed(() => this.documents().filter((document) => document.status === 'Indexed').length);
  protected readonly processedCount = computed(() =>
    this.documents().filter((document) => document.status === 'Processed' || document.status === 'Indexed').length);
  protected readonly failedCount = computed(() => this.documents().filter((document) => document.status === 'Failed').length);
  protected readonly fileTypeOptions = computed(() =>
    Array.from(new Set(this.documents().map((document) => this.documentType(document)))).sort());
  protected readonly statusOptions = computed(() =>
    Array.from(new Set(this.documents().map((document) => document.status))).sort());
  protected readonly categoryOptions = computed(() => {
    const lookupOptions = this.categories().map((category) => category.name);
    const documentOptions = this.documents().map((document) => this.documentCategory(document));

    return Array.from(new Set([...lookupOptions, ...documentOptions])).sort();
  });
  protected readonly filteredDocuments = computed(() => {
    const searchTerm = this.searchTerm().trim().toLowerCase();
    const selectedCategory = this.selectedCategory();
    const selectedFileType = this.selectedFileType();
    const selectedStatus = this.selectedStatus();

    return this.documents().filter((document) => {
      const category = this.documentCategory(document);
      const department = this.documentDepartment(document);
      const fileType = this.documentType(document);
      const matchesSearch = !searchTerm
        || document.fileName.toLowerCase().includes(searchTerm)
        || category.toLowerCase().includes(searchTerm)
        || department.toLowerCase().includes(searchTerm);
      const matchesCategory = !selectedCategory || category === selectedCategory;
      const matchesFileType = !selectedFileType || fileType === selectedFileType;
      const matchesStatus = !selectedStatus || document.status === selectedStatus;

      return matchesSearch && matchesCategory && matchesFileType && matchesStatus;
    });
  });
  protected readonly pagedDocuments = computed(() => {
    const startIndex = this.pageIndex() * this.pageSize;

    return this.filteredDocuments().slice(startIndex, startIndex + this.pageSize);
  });

  private pollingTimerId: number | null = null;

  ngOnInit(): void {
    this.loadLookups();
    this.loadDocuments();
  }

  ngOnDestroy(): void {
    this.stopPolling();
  }

  loadLookups(): void {
    this.api.listDepartments().subscribe({
      next: (departments) => this.departments.set(departments),
      error: () => this.snackBar.open('Departman listesi alınamadı.', 'Kapat', { duration: 4000 }),
    });

    this.api.listDocumentCategories().subscribe({
      next: (categories) => this.categories.set(categories),
      error: () => this.snackBar.open('Doküman türleri alınamadı.', 'Kapat', { duration: 4000 }),
    });
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
          this.syncPolling();
        },
        error: () => this.snackBar.open('Doküman listesi alınamadı.', 'Kapat', { duration: 4000 }),
      });
  }

  openUploadDialog(): void {
    this.dialog
      .open<UploadDocumentDialogComponent, UploadDocumentDialogData, UploadDocumentDialogResult>(UploadDocumentDialogComponent, {
        width: '560px',
        autoFocus: false,
        data: {
          departments: this.departments(),
          categories: this.categories(),
        },
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.upload(result);
        }
      });
  }

  upload(result: UploadDocumentDialogResult): void {
    this.uploading.set(true);
    this.api
      .uploadDocument(result.file, result.departmentId, result.categoryId)
      .pipe(finalize(() => this.uploading.set(false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Doküman yüklendi. Metin işleme bekliyor.', 'Kapat', { duration: 3000 });
          this.loadDocuments();
        },
        error: (error: HttpErrorResponse) => {
          const message = error.status === 409
            ? 'Aynı dosya adına sahip bir doküman zaten var.'
            : 'Upload başarısız.';

          this.snackBar.open(message, 'Kapat', { duration: 4000 });
        },
      });
  }

  selectDocument(document: DocumentItem): void {
    this.selectedDocument.set(document);
  }

  updateSearchTerm(value: string): void {
    this.searchTerm.set(value);
    this.resetPagination();
  }

  updateSelectedCategory(value: string): void {
    this.selectedCategory.set(value);
    this.resetPagination();
  }

  updateSelectedFileType(value: string): void {
    this.selectedFileType.set(value);
    this.resetPagination();
  }

  updateSelectedStatus(value: string): void {
    this.selectedStatus.set(value);
    this.resetPagination();
  }

  updatePage(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
  }

  process(document: DocumentItem): void {
    this.runDocumentAction(
      document.id,
      () => this.api.processDocument(document.id),
      'Doküman metin işleme kuyruğuna alındı.',
      'ProcessingQueued',
      'Metin işleme kuyruğa alındı.');
  }

  embed(document: DocumentItem): void {
    this.runDocumentAction(
      document.id,
      () => this.api.embedDocument(document.id),
      'Embedding kuyruğuna alındı.',
      'EmbeddingQueued',
      'Embedding kuyruğa alındı. Bu işlem biraz sürebilir.');
  }

  retry(document: DocumentItem): void {
    this.runDocumentAction(
      document.id,
      () => this.api.retryDocument(document.id),
      'Doküman tekrar deneme kuyruğuna alındı.',
      document.chunkCount > 0 ? 'EmbeddingQueued' : 'ProcessingQueued',
      'Tekrar deneme kuyruğa alındı.');
  }

  bulkProcessPending(): void {
    this.runBulkAction('process', 'Bekleyen dokümanlar metin işleme kuyruğuna alındı.');
  }

  bulkEmbedPending(): void {
    this.runBulkAction('embed', 'Metni hazır dokümanlar embedding kuyruğuna alındı.');
  }

  bulkRetryFailed(): void {
    this.runBulkAction('retry', 'Hatalı dokümanlar tekrar deneme kuyruğuna alındı.');
  }

  delete(document: DocumentItem): void {
    this.runDocumentAction(document.id, () => this.api.deleteDocument(document.id), 'Doküman silindi.');
  }

  download(document: DocumentItem): void {
    const link = window.document.createElement('a');
    link.href = this.api.getDocumentDownloadUrl(document.id);
    link.download = document.fileName;
    link.rel = 'noopener';
    window.document.body.appendChild(link);
    link.click();
    link.remove();
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

    if (document.categoryName) {
      return document.categoryName;
    }

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

  documentDepartment(document: DocumentItem): string {
    return document.departmentName ?? 'Departmansız';
  }

  statusLabel(status: string): string {
    const labels: Record<string, string> = {
      Uploaded: 'Yüklendi',
      ProcessingQueued: 'İşlem Kuyruğunda',
      Processing: 'İşleniyor',
      Processed: 'İşlendi',
      EmbeddingQueued: 'Embed Kuyruğunda',
      Embedding: 'Embedding',
      Indexed: 'İndekslendi',
      Failed: 'Hata',
      Deleted: 'Silindi',
    };

    return labels[status] ?? status;
  }

  statusHint(status: string): string {
    const hints: Record<string, string> = {
      Uploaded: 'Metin işleme bekliyor',
      ProcessingQueued: 'Sırada bekliyor',
      Processing: 'Chunk üretiliyor',
      Processed: 'Embed bekliyor',
      EmbeddingQueued: 'Sırada bekliyor',
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
    return document.status === 'Uploaded' || document.status === 'Failed' || document.status === 'ProcessingQueued';
  }

  canEmbed(document: DocumentItem): boolean {
    return document.status === 'Processed'
      || document.status === 'EmbeddingQueued'
      || (document.status === 'Failed' && document.chunkCount > 0);
  }

  canRetry(document: DocumentItem): boolean {
    return document.status === 'Failed'
      || document.status === 'ProcessingQueued'
      || document.status === 'EmbeddingQueued';
  }

  private runDocumentAction<T>(
    documentId: string,
    action: () => Observable<T>,
    successMessage: string,
    optimisticStatus?: string,
    pendingMessage?: string): void {
    this.workingDocumentId.set(documentId);

    if (optimisticStatus) {
      this.updateDocumentStatus(documentId, optimisticStatus);
    }

    if (pendingMessage) {
      this.snackBar.open(pendingMessage, 'Kapat', { duration: 3000 });
    }

    action()
      .pipe(finalize(() => this.workingDocumentId.set(null)))
      .subscribe({
        next: () => {
          this.snackBar.open(successMessage, 'Kapat', { duration: 3000 });
          this.syncPolling();
          this.loadDocuments();
        },
        error: () => {
          this.snackBar.open('İşlem başarısız.', 'Kapat', { duration: 4000 });
          this.loadDocuments();
        },
      });
  }

  private updateDocumentStatus(documentId: string, status: string): void {
    const updatedDocuments = this.documents().map((document) =>
      document.id === documentId
        ? {
            ...document,
            status,
            failureReason: null,
            updatedAt: new Date().toISOString(),
          }
        : document);

    this.documents.set(updatedDocuments);

    const selectedDocument = this.selectedDocument();
    if (selectedDocument?.id === documentId) {
      this.selectedDocument.set(updatedDocuments.find((document) => document.id === documentId) ?? selectedDocument);
    }
  }

  private runBulkAction(action: 'process' | 'embed' | 'retry', successMessage: string): void {
    this.bulkActionRunning.set(true);
    this.api
      .queueDocumentJobs({ action, onlyPending: true })
      .pipe(finalize(() => this.bulkActionRunning.set(false)))
      .subscribe({
        next: (response) => {
          this.snackBar.open(`${successMessage} (${response.queuedCount})`, 'Kapat', { duration: 3500 });
          this.loadDocuments();
          this.syncPolling();
        },
        error: () => this.snackBar.open('Toplu işlem başarısız.', 'Kapat', { duration: 4000 }),
      });
  }

  private syncPolling(): void {
    const hasActiveJobs = this.documents().some((document) =>
      ['ProcessingQueued', 'Processing', 'EmbeddingQueued', 'Embedding'].includes(document.status));

    if (hasActiveJobs && this.pollingTimerId === null) {
      this.pollingTimerId = window.setInterval(() => this.loadDocuments(), 5000);
      return;
    }

    if (!hasActiveJobs) {
      this.stopPolling();
    }
  }

  private stopPolling(): void {
    if (this.pollingTimerId !== null) {
      window.clearInterval(this.pollingTimerId);
      this.pollingTimerId = null;
    }
  }

  private resetPagination(): void {
    this.pageIndex.set(0);
  }
}
