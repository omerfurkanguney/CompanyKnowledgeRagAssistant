import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ApiInfo,
  AskQuestionRequest,
  AskQuestionResponse,
  BulkQueueDocumentsRequest,
  BulkQueueDocumentsResponse,
  ChatSessionDetail,
  ChatSessionSummary,
  DepartmentLookup,
  DocumentCategoryLookup,
  DocumentItem,
  HomeSummary,
  QueueDocumentJobResponse,
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

  listChatSessions(period: 'today' | 'week' | 'all' = 'all'): Observable<ChatSessionSummary[]> {
    return this.http.get<ChatSessionSummary[]>(`${this.apiUrl}/chat/sessions`, {
      params: { period },
    });
  }

  getChatSession(sessionId: string): Observable<ChatSessionDetail> {
    return this.http.get<ChatSessionDetail>(`${this.apiUrl}/chat/sessions/${sessionId}`);
  }

  deleteChatSession(sessionId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/chat/sessions/${sessionId}`);
  }

  listDocuments(): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(`${this.apiUrl}/documents`);
  }

  getHomeSummary(): Observable<HomeSummary> {
    return this.http.get<HomeSummary>(`${this.apiUrl}/home/summary`);
  }

  listDepartments(): Observable<DepartmentLookup[]> {
    return this.http.get<DepartmentLookup[]>(`${this.apiUrl}/lookups/departments`);
  }

  listDocumentCategories(): Observable<DocumentCategoryLookup[]> {
    return this.http.get<DocumentCategoryLookup[]>(`${this.apiUrl}/lookups/document-categories`);
  }

  uploadDocument(file: File, departmentId?: string | null, categoryId?: string | null): Observable<UploadDocumentResponse> {
    const formData = new FormData();
    formData.append('file', file);
    if (departmentId) {
      formData.append('departmentId', departmentId);
    }

    if (categoryId) {
      formData.append('categoryId', categoryId);
    }

    return this.http.post<UploadDocumentResponse>(`${this.apiUrl}/documents`, formData);
  }

  processDocument(documentId: string): Observable<QueueDocumentJobResponse> {
    return this.http.post<QueueDocumentJobResponse>(`${this.apiUrl}/documents/${documentId}/process`, {});
  }

  embedDocument(documentId: string): Observable<QueueDocumentJobResponse> {
    return this.http.post<QueueDocumentJobResponse>(`${this.apiUrl}/documents/${documentId}/embed`, {});
  }

  retryDocument(documentId: string): Observable<QueueDocumentJobResponse> {
    return this.http.post<QueueDocumentJobResponse>(`${this.apiUrl}/documents/${documentId}/retry`, {});
  }

  queueDocumentJobs(request: BulkQueueDocumentsRequest): Observable<BulkQueueDocumentsResponse> {
    return this.http.post<BulkQueueDocumentsResponse>(`${this.apiUrl}/documents/bulk-jobs`, request);
  }

  deleteDocument(documentId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/documents/${documentId}`);
  }

  getDocumentDownloadUrl(documentId: string): string {
    return `${this.apiUrl}/documents/${documentId}/download`;
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
