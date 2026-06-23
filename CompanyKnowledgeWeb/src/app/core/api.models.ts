export interface AskQuestionRequest {
  question: string;
  topK: number;
}

export interface AskQuestionResponse {
  answer: string;
  sources: AskQuestionSource[];
}

export interface AskQuestionSource {
  documentId: string;
  documentName: string;
  chunkId: string;
  content: string;
  pageNumber: number | null;
  chunkIndex: number;
  score: number;
}

export interface DocumentItem {
  id: string;
  fileName: string;
  contentType: string;
  sizeInBytes: number;
  status: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface UploadDocumentResponse {
  id: string;
  fileName: string;
  contentType: string;
  sizeInBytes: number;
  status: string;
  createdAt: string;
}

export interface ProcessDocumentResponse {
  documentId: string;
  status: string;
  chunkCount: number;
  failureReason: string | null;
}

export interface EmbedDocumentResponse {
  documentId: string;
  status: string;
  embeddedChunkCount: number;
  failureReason: string | null;
}

export interface ApiInfo {
  name: string;
  environment: string;
  version: string;
}

export interface SystemHealth {
  status: string;
  environment: string;
  checkedAtUtc: string;
}
