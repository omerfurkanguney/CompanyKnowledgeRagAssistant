import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { ApiService } from '../../core/api.service';
import { HomeSummary, RecentDocument } from '../../core/api.models';

interface SuggestedQuestionTopic {
  key: string;
  label: string;
  questions: string[];
}

@Component({
  selector: 'app-home-page',
  imports: [
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
  ],
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage implements OnInit {
  private readonly api = inject(ApiService);

  protected readonly summary = signal<HomeSummary | null>(null);
  protected readonly loading = signal(false);
  protected readonly selectedQuestionTopic = signal('annualLeave');
  protected readonly recentDocuments = computed(() => this.summary()?.recentDocuments ?? []);
  protected readonly departmentCounts = computed(() => this.summary()?.departmentDocumentCounts ?? []);
  protected readonly questionTopics: SuggestedQuestionTopic[] = [
    {
      key: 'annualLeave',
      label: 'Yıllık İzin Politikası',
      questions: [
        'Yıllık izin kaç gün önceden talep edilmelidir?',
        'Beş iş gününden kısa izinlerde kaç gün önce başvuru yapmak gerekir?',
        'Yıllık izin talebi hangi sistem üzerinden oluşturulur?',
        'Yıllık izin talebini kim onaylar?',
        'Hafta sonları yıllık izinden düşülür mü?',
      ],
    },
    {
      key: 'remoteWork',
      label: 'Uzaktan Çalışma Politikası',
      questions: [
        'Haftada kaç gün uzaktan çalışabilirim?',
        'Haftada kaç gün ofiste bulunmak gerekir?',
        'Uzaktan çalışma talebi kaç gün önce oluşturulmalıdır?',
        'Aynı gün uzaktan çalışma talebi oluşturulabilir mi?',
        'Uzaktan çalışma talebi hangi sistemden yapılır?',
      ],
    },
    {
      key: 'expense',
      label: 'Masraf İade Politikası',
      questions: [
        'Masraf iadesi için hangi belgeler gerekir?',
        'Masraf talebi kaç gün içinde yapılmalıdır?',
        'Seyahat masrafları kaç gün içinde bildirilmelidir?',
        'Masraf iadeleri ne zaman ödenir?',
        'İnternet desteği ne kadar?',
      ],
    },
    {
      key: 'security',
      label: 'Bilgi Güvenliği Politikası',
      questions: [
        'Şirket sistemlerine uzaktan erişirken VPN kullanmak zorunlu mu?',
        'Çok faktörlü kimlik doğrulama hangi sistemlerde zorunludur?',
        'Şüpheli e-posta alırsam ne yapmalıyım?',
        'Şirket verilerini kişisel e-posta adresime gönderebilir miyim?',
        'Şirket dokümanlarını kişisel bulut hesabıma yükleyebilir miyim?',
      ],
    },
    {
      key: 'equipment',
      label: 'Ekipman ve Zimmet Politikası',
      questions: [
        'Şirket laptopum kaybolursa ne yapmalıyım?',
        'Kayıp veya çalıntı cihaz kaç saat içinde bildirilmelidir?',
        'Şirket cihazını aile üyem kullanabilir mi?',
        'Zimmetli ekipmanı ne zaman iade etmeliyim?',
        'İşten ayrılırken ekipman teslim süresi nedir?',
      ],
    },
    {
      key: 'onboarding',
      label: 'Onboarding Politikası',
      questions: [
        'Yeni çalışan onboarding süreci kaç gün sürer?',
        'İlk gün hangi işlemler yapılır?',
        'Buddy sistemi nedir?',
        'Yeni çalışanın şirket hesapları ne zaman açılır?',
        'Deneme sürecinde değerlendirme ne zaman yapılır?',
      ],
    },
    {
      key: 'performance',
      label: 'Performans Değerlendirme Politikası',
      questions: [
        'Performans değerlendirmesi yılda kaç kez yapılır?',
        'Hedefler ne zaman belirlenir?',
        'Ara değerlendirme hangi ay yapılır?',
        'Yıl sonu değerlendirmesi hangi ay yapılır?',
        'Düşük performans durumunda ne yapılır?',
      ],
    },
    {
      key: 'handbook',
      label: 'Çalışan El Kitabı Genel Kurallar',
      questions: [
        'Şirketin standart çalışma saatleri nedir?',
        'Öğle arası hangi saatler arasındadır?',
        'Devamsızlık durumunda kime haber vermeliyim?',
        'Toplantılara geç kalırsam ne yapmalıyım?',
        'Şirket içi iletişimde hangi kanallar kullanılır?',
      ],
    },
  ];
  protected readonly suggestedQuestions = computed(() =>
    this.questionTopics.find((topic) => topic.key === this.selectedQuestionTopic())?.questions ?? []);

  ngOnInit(): void {
    this.loadSummary();
  }

  loadSummary(): void {
    this.loading.set(true);
    this.api.getHomeSummary().subscribe({
      next: (summary) => {
        this.summary.set(summary);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  documentType(document: RecentDocument): string {
    if (document.fileName.toLowerCase().endsWith('.docx')) {
      return 'DOCX';
    }

    if (document.fileName.toLowerCase().endsWith('.xlsx')) {
      return 'XLSX';
    }

    return 'PDF';
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
}
