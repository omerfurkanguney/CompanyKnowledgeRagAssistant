import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { AskQuestionResponse } from '../../core/api.models';

@Component({
  selector: 'app-chat-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatChipsModule,
    MatDividerModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSliderModule,
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
  protected readonly topK = signal(2);
  protected readonly loading = signal(false);
  protected readonly response = signal<AskQuestionResponse | null>(null);
  protected readonly answerLines = computed(() => this.response()?.answer.split('\n') ?? []);

  ask(): void {
    if (this.question.invalid || this.loading()) {
      this.question.markAsTouched();
      return;
    }

    this.loading.set(true);
    this.response.set(null);

    this.api
      .askQuestion({
        question: this.question.value,
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
