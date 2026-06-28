import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

interface ReleaseNote {
  version: string;
  date: string;
  title: string;
  status: string;
  highlights: string[];
  tags: string[];
}

@Component({
  selector: 'app-release-notes-page',
  imports: [
    MatIconModule,
  ],
  templateUrl: './release-notes.page.html',
  styleUrl: './release-notes.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReleaseNotesPage {
  protected readonly currentVersion = 'v0.1.0';

  protected readonly notes: ReleaseNote[] = [
    {
      version: 'v0.1.0',
      date: 'Haziran 2026',
      title: 'MVP RAG deneyimi',
      status: 'Mevcut sürüm',
      highlights: [
        'Doküman yükleme, metin işleme, chunk üretimi ve embedding akışı eklendi.',
        'PostgreSQL + pgvector ile semantic search ve RAG cevap üretimi bağlandı.',
        'Ollama tabanlı lokal embedding ve chat modeli desteği hazırlandı.',
        'Angular arayüzünde Home, Dokümanlar, Sohbet, Kaynaklar ve Ayarlar ekranları oluşturuldu.',
        'Sohbet geçmişi ChatSession ve ChatMessage tabloları ile kalıcı hale getirildi.',
      ],
      tags: ['.NET 10', 'Angular 22', 'PostgreSQL', 'pgvector', 'Ollama'],
    },
    {
      version: 'v0.0.4',
      date: 'Haziran 2026',
      title: 'Doküman yönetimi ve UI iyileştirmeleri',
      status: 'Tamamlandı',
      highlights: [
        'Departman ve doküman kategorisi lookup endpointleri eklendi.',
        'Doküman filtreleri kategori, dosya türü ve durum bilgisine göre çalışır hale getirildi.',
        'Responsive layout düzenlemeleri yapıldı.',
      ],
      tags: ['Documents', 'Lookups', 'Responsive UI'],
    },
    {
      version: 'v0.0.3',
      date: 'Haziran 2026',
      title: 'RAG altyapısı',
      status: 'Tamamlandı',
      highlights: [
        'PDF ve DOCX metin çıkarma akışı hazırlandı.',
        'Chunking stratejisi ve embedding saklama modeli eklendi.',
        'Semantic search endpointi ve chat cevap üretimi bağlandı.',
      ],
      tags: ['Ingestion', 'Embeddings', 'Semantic Search'],
    },
  ];
}
