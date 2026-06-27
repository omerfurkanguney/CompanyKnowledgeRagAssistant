import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';

interface KnowledgeSource {
  name: string;
  type: string;
  department: string;
  lastSync: string;
  documentCount: number;
  status: 'Aktif' | 'Senkronize Ediliyor' | 'Uyarı' | 'Hata';
  owner: string;
  description: string;
}

@Component({
  selector: 'app-sources-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatChipsModule,
    MatIconModule,
    MatProgressBarModule,
    MatTableModule,
  ],
  templateUrl: './sources.page.html',
  styleUrl: './sources.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SourcesPage {
  protected readonly displayedColumns = ['name', 'type', 'department', 'lastSync', 'documentCount', 'status', 'actions'];
  protected readonly sources = signal<KnowledgeSource[]>([
    {
      name: 'IK Politikaları Klasörü',
      type: 'SharePoint',
      department: 'İnsan Kaynakları',
      lastSync: '2026-06-12T14:25:00Z',
      documentCount: 124,
      status: 'Aktif',
      owner: 'Ayşe Demir',
      description: 'İnsan Kaynakları politika ve prosedür dokümanlarının bulunduğu kurumsal kaynak.',
    },
    {
      name: 'Finans Prosedürleri',
      type: 'Google Drive',
      department: 'Finans',
      lastSync: '2026-06-12T11:40:00Z',
      documentCount: 86,
      status: 'Aktif',
      owner: 'Mehmet Kaya',
      description: 'Finans prosedürleri, masraf, fatura ve bütçe dokümanları.',
    },
    {
      name: 'BT Güvenlik Arşivi',
      type: 'Confluence',
      department: 'Bilgi Teknolojileri',
      lastSync: '2026-06-11T18:05:00Z',
      documentCount: 203,
      status: 'Senkronize Ediliyor',
      owner: 'Can Arslan',
      description: 'Bilgi güvenliği, erişim ve operasyonel teknik kurallar.',
    },
    {
      name: 'Operasyon El Kitapları',
      type: 'Local Upload',
      department: 'Operasyon',
      lastSync: '2026-06-10T09:12:00Z',
      documentCount: 57,
      status: 'Aktif',
      owner: 'Zeynep Acar',
      description: 'Operasyon ekiplerinin kullandığı süreç ve kontrol listeleri.',
    },
    {
      name: 'Onboarding Belgeleri',
      type: 'OneDrive',
      department: 'İnsan Kaynakları',
      lastSync: '2026-06-09T16:28:00Z',
      documentCount: 42,
      status: 'Uyarı',
      owner: 'Ayşe Demir',
      description: 'Yeni çalışan başlangıç ve adaptasyon belgeleri.',
    },
    {
      name: 'Yasal Sözleşmeler',
      type: 'S3 Bucket',
      department: 'Hukuk',
      lastSync: '2026-06-08T13:02:00Z',
      documentCount: 311,
      status: 'Aktif',
      owner: 'Elif Yıldız',
      description: 'Sözleşme, taahhüt ve hukuki doküman arşivi.',
    },
    {
      name: 'Eski Arşiv Belgeleri',
      type: 'FTP',
      department: 'Arşiv',
      lastSync: '2026-06-07T08:55:00Z',
      documentCount: 425,
      status: 'Hata',
      owner: 'Burak Şen',
      description: 'Eski arşiv sistemi üzerinden aktarılan belgeler.',
    },
  ]);
  protected readonly selectedSource = signal<KnowledgeSource>(this.sources()[0]);
  protected readonly totalDocuments = computed(() => this.sources().reduce((total, source) => total + source.documentCount, 0));
  protected readonly activeSources = computed(() => this.sources().filter((source) => source.status === 'Aktif').length);
  protected readonly syncingSources = computed(() => this.sources().filter((source) => source.status === 'Senkronize Ediliyor').length);
  protected readonly sourceTypeCount = computed(() => new Set(this.sources().map((source) => source.type)).size);

  selectSource(source: KnowledgeSource): void {
    this.selectedSource.set(source);
  }

  statusClass(status: KnowledgeSource['status']): string {
    if (status === 'Senkronize Ediliyor') {
      return 'status-syncing';
    }

    if (status === 'Uyarı') {
      return 'status-warning';
    }

    if (status === 'Hata') {
      return 'status-error';
    }

    return 'status-active';
  }

  sourceIcon(type: string): string {
    const icons: Record<string, string> = {
      SharePoint: 'hub',
      'Google Drive': 'change_history',
      Confluence: 'all_inclusive',
      'Local Upload': 'cloud_upload',
      OneDrive: 'cloud',
      'S3 Bucket': 'deployed_code',
      FTP: 'folder_zip',
    };

    return icons[type] ?? 'database';
  }
}
