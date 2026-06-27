import { Routes } from '@angular/router';
import { ChatPage } from './features/chat/chat.page';
import { DocumentsPage } from './features/documents/documents.page';
import { HomePage } from './features/home/home.page';
import { SourcesPage } from './features/sources/sources.page';
import { SystemPage } from './features/system/system.page';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'home',
  },
  {
    path: 'home',
    component: HomePage,
    title: 'Home',
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
    path: 'sources',
    component: SourcesPage,
    title: 'Sources',
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
