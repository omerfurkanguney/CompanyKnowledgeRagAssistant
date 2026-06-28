import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { AskQuestionResponse, AskQuestionSource, ChatMessage, ChatSessionSummary } from '../../core/api.models';

interface SuggestedQuestionTopic {
  key: string;
  label: string;
  questions: string[];
}

type ChatHistoryPeriod = 'today' | 'week' | 'all';

@Component({
  selector: 'app-chat-page',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule,
  ],
  templateUrl: './chat.page.html',
  styleUrl: './chat.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChatPage implements OnInit {
  private readonly api = inject(ApiService);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly question = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(2000)],
  });
  protected readonly topK = signal(4);
  protected readonly loading = signal(false);
  protected readonly loadingSessions = signal(false);
  protected readonly response = signal<AskQuestionResponse | null>(null);
  protected readonly chatSessions = signal<ChatSessionSummary[]>([]);
  protected readonly currentSessionId = signal<string | null>(null);
  protected readonly currentMessages = signal<ChatMessage[]>([]);
  protected readonly selectedHistoryPeriod = signal<ChatHistoryPeriod>('today');
  protected readonly selectedQuestionTopic = signal('annualLeave');
  protected readonly now = new Date();
  protected readonly activeSessionTitle = computed(() =>
    this.chatSessions().find((session) => session.id === this.currentSessionId())?.title ?? 'Yeni Sohbet');
  protected readonly confidence = computed(() => {
    const sources = this.response()?.sources ?? [];
    if (sources.length === 0) {
      return 94;
    }

    const average = sources.reduce((total, source) => total + source.score, 0) / sources.length;
    return Math.round(average * 100);
  });

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
    this.loadChatSessions();
  }

  loadChatSessions(): void {
    this.loadingSessions.set(true);
    this.api
      .listChatSessions(this.selectedHistoryPeriod())
      .pipe(finalize(() => this.loadingSessions.set(false)))
      .subscribe({
        next: (sessions) => this.chatSessions.set(sessions),
        error: () => this.snackBar.open('Sohbet geçmişi alınamadı.', 'Kapat', { duration: 4000 }),
      });
  }

  changeHistoryPeriod(period: ChatHistoryPeriod): void {
    this.selectedHistoryPeriod.set(period);
    this.currentSessionId.set(null);
    this.currentMessages.set([]);
    this.response.set(null);
    this.loadChatSessions();
  }

  newChat(): void {
    this.currentSessionId.set(null);
    this.currentMessages.set([]);
    this.response.set(null);
    this.question.setValue('');
  }

  selectSession(session: ChatSessionSummary): void {
    this.currentSessionId.set(session.id);
    this.api.getChatSession(session.id).subscribe({
      next: (detail) => {
        const messages = this.sortMessages(detail.messages);
        this.currentMessages.set(messages);
        this.restoreLastExchange(messages);
      },
      error: () => this.snackBar.open('Sohbet detayı alınamadı.', 'Kapat', { duration: 4000 }),
    });
  }

  deleteSession(event: MouseEvent, session: ChatSessionSummary): void {
    event.stopPropagation();
    this.api.deleteChatSession(session.id).subscribe({
      next: () => {
        if (this.currentSessionId() === session.id) {
          this.newChat();
        }

        this.chatSessions.set(this.chatSessions().filter((item) => item.id !== session.id));
        this.snackBar.open('Sohbet silindi.', 'Kapat', { duration: 3000 });
      },
      error: () => this.snackBar.open('Sohbet silinemedi.', 'Kapat', { duration: 4000 }),
    });
  }

  ask(): void {
    if (this.question.invalid || this.loading()) {
      this.question.markAsTouched();
      return;
    }

    const question = this.question.value.trim();
    const userMessage: ChatMessage = {
      id: crypto.randomUUID(),
      role: 'user',
      content: question,
      sources: [],
      createdAt: new Date().toISOString(),
    };

    this.currentMessages.set([...this.currentMessages(), userMessage]);
    this.loading.set(true);
    this.response.set(null);
    this.question.setValue('');

    this.api
      .askQuestion({
        question,
        topK: this.topK(),
        sessionId: this.currentSessionId(),
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => {
          const assistantMessage: ChatMessage = {
            id: crypto.randomUUID(),
            role: 'assistant',
            content: response.answer,
            sources: response.sources,
            createdAt: new Date().toISOString(),
          };

          this.response.set(response);
          this.currentSessionId.set(response.sessionId);
          this.currentMessages.set([...this.currentMessages(), assistantMessage]);
          this.loadChatSessions();
        },
        error: () => this.snackBar.open('Cevap alınamadı. API ve Ollama durumunu kontrol et.', 'Kapat', { duration: 5000 }),
      });
  }

  setExample(text: string): void {
    this.question.setValue(text);
  }

  formatSessionTime(updatedAt: string): string {
    return new Date(updatedAt).toLocaleTimeString('tr-TR', {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  messageLines(content: string): string[] {
    return content.split('\n').filter((line) => line.trim().length > 0);
  }

  messageTime(createdAt: string): string {
    return new Date(createdAt).toLocaleTimeString('tr-TR', {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  sourceLocation(source: AskQuestionSource): string {
    const parts: string[] = [];

    if (source.clauseId) {
      parts.push(`Madde ${source.clauseId}`);
    }

    const pageRange = this.sourcePageRange(source);
    if (pageRange) {
      parts.push(`Sayfa ${pageRange}`);
    }

    return parts.length > 0 ? parts.join(' · ') : `Chunk ${source.chunkIndex}`;
  }

  private sourcePageRange(source: AskQuestionSource): string | null {
    if (source.startPageNumber && source.endPageNumber) {
      return source.startPageNumber === source.endPageNumber
        ? source.startPageNumber.toString()
        : `${source.startPageNumber}-${source.endPageNumber}`;
    }

    return null;
  }

  private restoreLastExchange(messages: ChatMessage[]): void {
    const lastAssistantMessage = [...messages].reverse().find((message) => message.role === 'assistant');

    if (lastAssistantMessage) {
      this.response.set({
        sessionId: this.currentSessionId() ?? '',
        answer: lastAssistantMessage.content,
        sources: lastAssistantMessage.sources,
      });
      return;
    }

    this.response.set(null);
  }

  private sortMessages(messages: ChatMessage[]): ChatMessage[] {
    return [...messages].sort((left, right) => {
      const dateCompare = new Date(left.createdAt).getTime() - new Date(right.createdAt).getTime();

      if (dateCompare !== 0) {
        return dateCompare;
      }

      return this.roleOrder(left.role) - this.roleOrder(right.role);
    });
  }

  private roleOrder(role: string): number {
    return role === 'user' ? 0 : 1;
  }
}
