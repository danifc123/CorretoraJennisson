import { Component, signal, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ChatService } from '../../services/chat.service';
import { ChatModal } from '../chat-modal/chat-modal';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-chat-button',
  standalone: true,
  imports: [CommonModule, ChatModal],
  templateUrl: './chat-button.html',
  styleUrls: ['./chat-button.scss']
})
export class ChatButton implements OnInit {
  @ViewChild(ChatModal) chatModal?: ChatModal;

  unreadCount = signal(0);
  hasMessages = signal(false);

  constructor(
    private router: Router,
    private authService: AuthService,
    private chatService: ChatService
  ) {}

  async ngOnInit() {
    // Verifica se está logado e se tem mensagens
    if (this.authService.isAuthenticated() && !this.authService.isAdmin()) {
      await this.checkHasMessages();
      await this.loadUnreadCount();

      // Atualiza periodicamente
      setInterval(() => {
        this.checkHasMessages();
        this.loadUnreadCount();
      }, 30000); // A cada 30 segundos
    }
  }

  private async checkHasMessages() {
    try {
      const messages = await firstValueFrom(this.chatService.getAll());
      if (messages && messages.length > 0) {
        const currentUser = this.authService.getCurrentUser();
        if (currentUser) {
          // Verifica se o usuário tem pelo menos uma mensagem (enviada por ele)
          const userMessages = messages.filter(m =>
            m.usuario_Id === currentUser.userId &&
            m.remetente_Tipo === 1 // Usuario
          );
          this.hasMessages.set(userMessages.length > 0);
        }
      } else {
        this.hasMessages.set(false);
      }
    } catch (error) {
      console.error('Erro ao verificar mensagens:', error);
      this.hasMessages.set(false);
    }
  }

  private async loadUnreadCount() {
    try {
      // Conta mensagens não lidas do usuário (respostas do admin)
      const messages = await firstValueFrom(this.chatService.getAll());
      if (messages) {
        const currentUser = this.authService.getCurrentUser();
        if (currentUser) {
          const unread = messages.filter(m =>
            !m.lida &&
            m.remetente_Tipo === 2 && // Administrador
            m.usuario_Id === currentUser.userId
          ).length;
          this.unreadCount.set(unread);
        }
      }
    } catch (error) {
      console.error('Erro ao carregar contagem de não lidas:', error);
    }
  }

  openChat() {
    // Sempre abre o modal de chat para clientes
    if (this.chatModal) {
      this.chatModal.open();
    }
  }

  shouldShow(): boolean {
    // Mostra sempre para usuários logados (não admin)
    return this.authService.isAuthenticated() && !this.authService.isAdmin();
  }
}

