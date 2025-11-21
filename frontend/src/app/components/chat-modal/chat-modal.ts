import { Component, OnInit, OnDestroy, signal, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService, Mensagem, RemetenteTipo } from '../../services/chat.service';
import { SignalRService } from '../../services/signalr.service';
import { AuthService } from '../../services/auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-chat-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-modal.html',
  styleUrls: ['./chat-modal.scss']
})
export class ChatModal implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesContainer') messagesContainer?: ElementRef<HTMLDivElement>;

  isOpen = signal(false);
  messages = signal<Mensagem[]>([]);
  newMessage = signal('');
  loading = signal(false);
  isConnected = signal(false);
  private shouldScroll = false;
  private unsubscribeMessage?: () => void;

  constructor(
    private chatService: ChatService,
    private signalRService: SignalRService,
    private authService: AuthService
  ) {}

  async ngOnInit() {
    // Inicializa chat quando o modal é criado
    await this.initializeChat();
  }

  ngOnDestroy() {
    if (this.unsubscribeMessage) {
      this.unsubscribeMessage();
    }
  }

  ngAfterViewChecked() {
    if (this.shouldScroll) {
      this.scrollToBottom();
      this.shouldScroll = false;
    }
  }

  open() {
    this.isOpen.set(true);
    this.loadMessages();
  }

  close() {
    this.isOpen.set(false);
  }

  private async initializeChat() {
    try {
      // Conecta SignalR
      if (!this.signalRService.isConnected()) {
        await this.signalRService.startConnection();
        await new Promise(resolve => setTimeout(resolve, 100));
      }
      this.isConnected.set(this.signalRService.isConnected());

      // Escuta novas mensagens do SignalR
      this.unsubscribeMessage = this.signalRService.onNewMessage((mensagem) => {
        const currentMessages = this.messages();
        const currentUser = this.authService.getCurrentUser();

        if (currentUser && mensagem.usuario_Id === currentUser.userId) {
          if (!currentMessages.find(m => m.id === mensagem.id)) {
            const updated = [...currentMessages, mensagem].sort((a, b) =>
              new Date(a.created_At).getTime() - new Date(b.created_At).getTime()
            );
            this.messages.set(updated);
            this.shouldScroll = true;
          }
        }
      });
    } catch (error) {
      console.error('Erro ao inicializar chat:', error);
    }
  }

  private async loadMessages() {
    try {
      this.loading.set(true);
      const allMessages = await firstValueFrom(this.chatService.getAll());
      if (allMessages && allMessages.length > 0) {
        const currentUser = this.authService.getCurrentUser();
        if (currentUser) {
          const userMessages = allMessages.filter(m => m.usuario_Id === currentUser.userId);
          const sorted = userMessages.sort((a, b) =>
            new Date(a.created_At).getTime() - new Date(b.created_At).getTime()
          );
          this.messages.set(sorted);
          this.shouldScroll = true;
        }
      }
    } catch (error) {
      console.error('Erro ao carregar mensagens:', error);
    } finally {
      this.loading.set(false);
    }
  }

  async sendMessage() {
    const conteudo = this.newMessage().trim();
    if (!conteudo) {
      return;
    }

    try {
      // Tenta enviar via SignalR se estiver conectado
      if (this.isConnected()) {
        await this.signalRService.sendMessage(conteudo);
        this.newMessage.set('');
        this.shouldScroll = true;
        await this.loadMessages();
      } else {
        // Se não estiver conectado, tenta conectar primeiro
        try {
          await this.signalRService.startConnection();
          await new Promise(resolve => setTimeout(resolve, 100));
          this.isConnected.set(this.signalRService.isConnected());

          if (this.isConnected()) {
            await this.signalRService.sendMessage(conteudo);
            this.newMessage.set('');
            this.shouldScroll = true;
            await this.loadMessages();
          } else {
            // Fallback para REST se SignalR não funcionar
            await firstValueFrom(this.chatService.create({ conteudo }));
            this.newMessage.set('');
            await this.loadMessages();
          }
        } catch (signalRError) {
          // Se SignalR falhar, usa REST
          await firstValueFrom(this.chatService.create({ conteudo }));
          this.newMessage.set('');
          await this.loadMessages();
        }
      }
    } catch (error) {
      console.error('Erro ao enviar mensagem:', error);
      // Tenta enviar via REST como último recurso
      try {
        await firstValueFrom(this.chatService.create({ conteudo }));
        this.newMessage.set('');
        await this.loadMessages();
      } catch (restError) {
        console.error('Erro ao enviar mensagem via REST:', restError);
        alert('Erro ao enviar mensagem. Tente novamente.');
      }
    }
  }

  isUsuarioMessage(mensagem: Mensagem): boolean {
    return mensagem.remetente_Tipo === RemetenteTipo.Usuario;
  }

  getSenderName(mensagem: Mensagem): string {
    if (mensagem.remetente_Tipo === RemetenteTipo.Usuario) {
      return 'Você';
    } else {
      return mensagem.administrador_Nome || 'Administrador';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const minutes = Math.floor(diff / 60000);

    if (minutes < 1) return 'Agora';
    if (minutes < 60) return `${minutes}min atrás`;
    if (minutes < 1440) return `${Math.floor(minutes / 60)}h atrás`;

    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getCleanMessageText(conteudo: string): string {
    if (!conteudo || !conteudo.trim()) {
      return conteudo;
    }

    const lines = conteudo.split('\n');
    const cleanLines: string[] = [];
    let foundContent = false;

    for (const line of lines) {
      const trimmedLine = line.trim();

      if (trimmedLine.startsWith('Assunto:') ||
          trimmedLine.startsWith('Contato:') ||
          trimmedLine.startsWith('E-mail:') ||
          trimmedLine.startsWith('Telefone:') ||
          trimmedLine === '---') {
        continue;
      }

      if (trimmedLine) {
        cleanLines.push(line);
        foundContent = true;
      }
    }

    if (foundContent && cleanLines.length > 0) {
      return cleanLines.join('\n').trim();
    }

    return conteudo.trim();
  }

  private scrollToBottom() {
    if (this.messagesContainer) {
      const element = this.messagesContainer.nativeElement;
      element.scrollTop = element.scrollHeight;
    }
  }
}

