import { Routes } from '@angular/router';
import { ChatPage } from './features/chat/chat.page';
import { DocumentsPage } from './features/documents/documents.page';
import { SystemPage } from './features/system/system.page';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'chat',
  },
  {
    path: 'chat',
    component: ChatPage,
    title: 'Chat',
  },
  {
    path: 'documents',
    component: DocumentsPage,
    title: 'Documents',
  },
  {
    path: 'system',
    component: SystemPage,
    title: 'System',
  },
  {
    path: '**',
    redirectTo: 'chat',
  },
];
