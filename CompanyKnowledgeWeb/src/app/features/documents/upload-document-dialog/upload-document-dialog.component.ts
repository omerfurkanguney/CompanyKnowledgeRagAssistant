import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';

export interface UploadDocumentDialogResult {
  file: File;
  documentType: string;
  department: string;
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

  protected readonly selectedFile = signal<File | null>(null);
  protected readonly documentType = new FormControl('Policy', {
    nonNullable: true,
    validators: [Validators.required],
  });
  protected readonly department = new FormControl('HumanResources', {
    nonNullable: true,
    validators: [Validators.required],
  });

  protected readonly documentTypes = [
    { value: 'Policy', label: 'Politika' },
    { value: 'Procedure', label: 'Prosedür' },
    { value: 'Guide', label: 'Kılavuz' },
    { value: 'Contract', label: 'Sözleşme' },
    { value: 'Cv', label: 'CV' },
    { value: 'Other', label: 'Diğer' },
  ];

  protected readonly departments = [
    { value: 'HumanResources', label: 'İnsan Kaynakları' },
    { value: 'Finance', label: 'Finans' },
    { value: 'InformationTechnology', label: 'Bilgi Teknolojileri' },
    { value: 'Operations', label: 'Operasyon' },
    { value: 'Legal', label: 'Hukuk' },
    { value: 'General', label: 'Genel' },
  ];

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
      documentType: this.documentType.value,
      department: this.department.value,
    });
  }
}
