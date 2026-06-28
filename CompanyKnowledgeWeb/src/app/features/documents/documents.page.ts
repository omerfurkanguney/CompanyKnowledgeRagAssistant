import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
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
    MatProgressBarModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTableModule,
  ],
  templateUrl: './documents.page.html',
  styleUrl: './documents.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentsPage implements OnInit {
  private readonly api = inject(ApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly documents = signal<DocumentItem[]>([]);
  protected readonly selectedDocument = signal<DocumentItem | null>(null);
  protected readonly loading = signal(false);
  protected readonly uploading = signal(false);
  protected readonly workingDocumentId = signal<string | null>(null);
  protected readonly departments = signal<DepartmentLookup[]>([]);
  protected readonly categories = signal<DocumentCategoryLookup[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly selectedCategory = signal('');
  protected readonly selectedFileType = signal('');
  protected readonly selectedStatus = signal('');
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

  ngOnInit(): void {
    this.loadLookups();
    this.loadDocuments();
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
      Uploaded: 'Metin işleme bekliyor',
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
    return document.status === 'Processed' || (document.status === 'Failed' && document.chunkCount > 0);
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
