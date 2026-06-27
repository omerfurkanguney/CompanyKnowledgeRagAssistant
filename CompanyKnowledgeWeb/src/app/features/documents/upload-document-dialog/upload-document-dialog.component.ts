import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { DepartmentLookup, DocumentCategoryLookup } from '../../../core/api.models';

export interface UploadDocumentDialogResult {
  file: File;
  categoryId: string;
  departmentId: string;
}

export interface UploadDocumentDialogData {
  departments: DepartmentLookup[];
  categories: DocumentCategoryLookup[];
}

@Component({
  selector: 'app-upload-document-dialog',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
  ],
  templateUrl: './upload-document-dialog.component.html',
  styleUrl: './upload-document-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UploadDocumentDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<UploadDocumentDialogComponent, UploadDocumentDialogResult>);
  protected readonly data = inject<UploadDocumentDialogData>(MAT_DIALOG_DATA);

  protected readonly selectedFile = signal<File | null>(null);
  protected readonly documentType = new FormControl(this.data.categories[0]?.id ?? '', {
    nonNullable: true,
    validators: [Validators.required],
  });
  protected readonly department = new FormControl(this.data.departments[0]?.id ?? '', {
    nonNullable: true,
    validators: [Validators.required],
  });

  protected readonly documentTypes = this.data.categories;
  protected readonly departments = this.data.departments;

  selectFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile.set(input.files?.[0] ?? null);
  }

  submit(): void {
    const file = this.selectedFile();

    if (!file || this.documentType.invalid || this.department.invalid) {
      this.documentType.markAsTouched();
      this.department.markAsTouched();
      return;
    }

    this.dialogRef.close({
      file,
      categoryId: this.documentType.value,
      departmentId: this.department.value,
    });
  }
}
