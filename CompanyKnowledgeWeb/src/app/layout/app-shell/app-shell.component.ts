import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { SidebarComponent, SidebarNavItem } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';

@Component({
  selector: 'app-shell',
  imports: [
    RouterOutlet,
    MatSidenavModule,
    SidebarComponent,
    TopbarComponent,
  ],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShellComponent {
  protected isSidebarCollapsed = false;

  protected readonly navItems: SidebarNavItem[] = [
    { label: 'Genel Bakış', icon: 'home', route: '/home' },
    { label: 'Dokümanlar', icon: 'description', route: '/documents' },
    { label: 'Sohbet', icon: 'chat_bubble', route: '/chat' },
    { label: 'Kaynaklar', icon: 'storage', route: '/sources' },
    { label: 'Kullanıcılar', icon: 'groups', route: '/system' },
    { label: 'Ayarlar', icon: 'settings', route: '/system' },
  ];

  protected toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}
