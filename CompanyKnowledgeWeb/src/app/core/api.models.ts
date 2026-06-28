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
  departmentId: string | null;
  departmentName: string | null;
  categoryId: string | null;
  categoryName: string | null;
  status: string;
  chunkCount: number;
  failureReason: string | null;
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

export interface HomeSummary {
  totalDocumentCount: number;
  totalUserCount: number;
  todayQuestionCount: number;
  recentDocuments: RecentDocument[];
  departmentDocumentCounts: DepartmentDocumentCount[];
  categoryDocumentCounts: CategoryDocumentCount[];
}

export interface RecentDocument {
  id: string;
  fileName: string;
  contentType: string;
  sizeInBytes: number;
  status: string;
  departmentId: string | null;
  departmentName: string | null;
  categoryId: string | null;
  categoryName: string | null;
  createdAt: string;
}

export interface DepartmentDocumentCount {
  departmentId: string | null;
  departmentName: string;
  documentCount: number;
}

export interface CategoryDocumentCount {
  categoryId: string | null;
  categoryName: string;
  documentCount: number;
}

export interface DepartmentLookup {
  id: string;
  name: string;
  slug: string;
}

export interface DocumentCategoryLookup {
  id: string;
  name: string;
  slug: string;
}
