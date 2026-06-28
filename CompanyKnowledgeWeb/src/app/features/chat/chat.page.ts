import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { AskQuestionResponse } from '../../core/api.models';

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
export class ChatPage {
  private readonly api = inject(ApiService);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly question = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(2000)],
  });
  protected readonly topK = signal(4);
  protected readonly loading = signal(false);
  protected readonly response = signal<AskQuestionResponse | null>(null);
  protected readonly lastQuestion = signal('Yıllık izin kaç gün önceden talep edilmelidir?');
  protected readonly now = new Date();
  protected readonly answerLines = computed(() => this.response()?.answer.split('\n') ?? [
    'Yıllık izin talepleri, planlanan başlangıç tarihinden en az 10 iş günü öncesinden yöneticinize iletilmelidir.',
    'Beş iş gününden kısa izin taleplerinde ise en az 5 iş günü önce başvuru yapılması yeterlidir.',
  ]);
  protected readonly confidence = computed(() => {
    const sources = this.response()?.sources ?? [];
    if (sources.length === 0) {
      return 94;
    }

    const average = sources.reduce((total, source) => total + source.score, 0) / sources.length;
    return Math.round(average * 100);
  });

  protected readonly chatHistory = [
    { title: 'Yıllık izin talebi nasıl yapılır?', time: '10:24', active: true },
    { title: 'Masraf iadesi için hangi belgeler gerekli?', time: '09:48', active: false },
    { title: 'Uzaktan çalışma gün sınırı nedir?', time: 'Dün', active: false },
    { title: 'Onboarding süreci kaç gün sürer?', time: 'Dün', active: false },
    { title: 'Bilgi güvenliği ihlali nasıl bildirilir?', time: '11 Haz', active: false },
  ];

  protected readonly suggestedQuestions = [
    'Yıllık izin devredilir mi?',
    'Yarım gün izin alınabilir mi?',
    'İzin iptali nasıl yapılır?',
    'Masraf iadesi için hangi belgeler gerekir?',
    'Uzaktan çalışma gün sınırı nedir?',
    'Onboarding süreci kaç gün sürer?',
    'Bilgi güvenliği ihlali nasıl bildirilir?',
  ];

  ask(): void {
    if (this.question.invalid || this.loading()) {
      this.question.markAsTouched();
      return;
    }

    const question = this.question.value.trim();
    this.loading.set(true);
    this.response.set(null);
    this.lastQuestion.set(question);

    this.api
      .askQuestion({
        question,
        topK: this.topK(),
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => this.response.set(response),
        error: () => this.snackBar.open('Cevap alınamadı. API ve Ollama durumunu kontrol et.', 'Kapat', { duration: 5000 }),
      });
  }

  setExample(text: string): void {
    this.question.setValue(text);
  }
}
