import { ChangeDetectionStrategy, Component } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-home-page',
  imports: [
    DatePipe,
    DecimalPipe,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage {
  protected readonly now = new Date();
  protected readonly recentDocuments = [
    { name: '01-yillik-izin-politikasi.pdf', type: 'PDF', status: 'İndekslendi' },
    { name: '02-izin-ve-devamsizlik.docx', type: 'DOCX', status: 'İndekslendi' },
    { name: '03-performans-yonetimi.pdf', type: 'PDF', status: 'İndekslendi' },
    { name: '04-maas-politikasi.xlsx', type: 'XLSX', status: 'İşleniyor' },
    { name: '05-seyahat-politikasi.pdf', type: 'PDF', status: 'İndekslendi' },
  ];
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
}
