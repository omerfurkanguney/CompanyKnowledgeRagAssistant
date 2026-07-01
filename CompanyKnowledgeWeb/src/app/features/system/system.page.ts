import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

interface SettingsCard {
  icon: string;
  title: string;
  description: string;
  items: Array<{
    label: string;
    value: string;
  }>;
}

@Component({
  selector: 'app-system-page',
  imports: [
    MatIconModule,
  ],
  templateUrl: './system.page.html',
  styleUrl: './system.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SystemPage {
  protected readonly settingsCards: SettingsCard[] = [
    {
      icon: 'psychology',
      title: 'AI Yapılandırması',
      description: 'Yanıt üretimi ve embedding işlemleri lokal Ollama üzerinden çalışır.',
      items: [
        { label: 'Sağlayıcı', value: 'Ollama' },
        { label: 'Chat modeli', value: 'qwen2.5:1.5b' },
        { label: 'Embedding modeli', value: 'bge-m3' },
        { label: 'Çalışma modu', value: 'Lokal / self-hosted' },
      ],
    },
    {
      icon: 'article',
      title: 'Doküman İşleme',
      description: 'PDF ve DOCX dosyaları metne çevrilir, chunklara ayrılır ve RAG için hazırlanır.',
      items: [
        { label: 'Desteklenen dosyalar', value: 'PDF, DOCX' },
        { label: 'Chunking', value: 'Clause-aware semantic chunking' },
        { label: 'Sayfa metadata', value: 'Aktif' },
        { label: 'Maksimum dosya', value: '25 MB' },
      ],
    },
    {
      icon: 'storage',
      title: 'Veritabanı ve Arama',
      description: 'Doküman, chunk, embedding ve sohbet geçmişi PostgreSQL üzerinde saklanır.',
      items: [
        { label: 'Veritabanı', value: 'PostgreSQL' },
        { label: 'Vektör arama', value: 'pgvector' },
        { label: 'Semantic search', value: 'Aktif' },
        { label: 'Sohbet geçmişi', value: 'Kalıcı' },
      ],
    },
    {
      icon: 'lock_open',
      title: 'Demo Modu',
      description: 'Bu MVP açık kaynak olarak auth olmadan çalışacak şekilde tasarlandı.',
      items: [
        { label: 'Kimlik doğrulama', value: 'Kapalı' },
        { label: 'Kullanıcı yönetimi', value: 'Kapalı' },
        { label: 'Deployment', value: 'Local veya sunucu' },
        { label: 'Kullanım amacı', value: 'RAG demo' },
      ],
    },
  ];
}

