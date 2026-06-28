import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { AskQuestionResponse, ChatMessage, ChatSessionSummary } from '../../core/api.models';

@Component({
  selector: 'app-chat-page',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
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

  protected readonly suggestedQuestions = [
    'Yıllık izin devredilir mi?',
    'Yarım gün izin alınabilir mi?',
    'İzin iptali nasıl yapılır?',
    'Masraf iadesi için hangi belgeler gerekir?',
    'Uzaktan çalışma gün sınırı nedir?',
    'Onboarding süreci kaç gün sürer?',
    'Bilgi güvenliği ihlali nasıl bildirilir?',
  ];

  ngOnInit(): void {
    this.loadChatSessions();
  }

  loadChatSessions(): void {
    this.loadingSessions.set(true);
    this.api
      .listChatSessions()
      .pipe(finalize(() => this.loadingSessions.set(false)))
      .subscribe({
        next: (sessions) => this.chatSessions.set(sessions),
        error: () => this.snackBar.open('Sohbet geçmişi alınamadı.', 'Kapat', { duration: 4000 }),
      });
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
        this.currentMessages.set(detail.messages);
        this.restoreLastExchange(detail.messages);
      },
      error: () => this.snackBar.open('Sohbet detayı alınamadı.', 'Kapat', { duration: 4000 }),
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
}
