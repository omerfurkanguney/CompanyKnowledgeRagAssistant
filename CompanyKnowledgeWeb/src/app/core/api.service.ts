import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ApiInfo,
  AskQuestionRequest,
  AskQuestionResponse,
  DocumentItem,
  EmbedDocumentResponse,
  ProcessDocumentResponse,
  SystemHealth,
  UploadDocumentResponse,
} from './api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;
  private readonly apiUrl = `${this.baseUrl}/api`;

  askQuestion(request: AskQuestionRequest): Observable<AskQuestionResponse> {
    return this.http.post<AskQuestionResponse>(`${this.apiUrl}/chat/ask`, request);
  }

  listDocuments(): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(`${this.apiUrl}/documents`);
  }

  uploadDocument(file: File): Observable<UploadDocumentResponse> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<UploadDocumentResponse>(`${this.apiUrl}/documents`, formData);
  }

  processDocument(documentId: string): Observable<ProcessDocumentResponse> {
    return this.http.post<ProcessDocumentResponse>(`${this.apiUrl}/documents/${documentId}/process`, {});
  }

  embedDocument(documentId: string): Observable<EmbedDocumentResponse> {
    return this.http.post<EmbedDocumentResponse>(`${this.apiUrl}/documents/${documentId}/embed`, {});
  }

  deleteDocument(documentId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/documents/${documentId}`);
  }

  getApiInfo(): Observable<ApiInfo> {
    return this.http.get<ApiInfo>(`${this.apiUrl}/system/info`);
  }

  getSystemHealth(): Observable<SystemHealth> {
    return this.http.get<SystemHealth>(`${this.apiUrl}/system/health`);
  }

  getHealth(): Observable<string> {
    return this.http.get(`${this.baseUrl}/health`, { responseType: 'text' });
  }
}
