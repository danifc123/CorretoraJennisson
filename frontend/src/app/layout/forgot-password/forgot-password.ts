import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrls: ['./forgot-password.scss']
})
export class ForgotPassword {
  // Dados do formulário
  email = '';

  // Estados
  loading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  emailSent = signal(false);

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  /**
   * Envia link de recuperação de senha
   */
  onSubmit(): void {
    // Limpa mensagens anteriores
    this.errorMessage.set('');
    this.successMessage.set('');

    // Validações básicas
    if (!this.email) {
      this.errorMessage.set('Por favor, insira seu e-mail.');
      return;
    }

    if (!this.isValidEmail(this.email)) {
      this.errorMessage.set('Por favor, insira um e-mail válido.');
      return;
    }

    this.loading.set(true);

    this.authService.forgotPassword(this.email).subscribe({
      next: (response) => {
        console.log('Recuperação de senha solicitada para:', this.email, response);
        this.emailSent.set(true);
        this.successMessage.set(
          response?.message ||
            'Link de recuperação enviado! Verifique sua caixa de entrada e spam.'
        );
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao solicitar recuperação de senha:', error);
        this.errorMessage.set(
          error?.error ?? 'Ocorreu um erro ao solicitar a recuperação de senha.'
        );
        this.loading.set(false);
      }
    });
  }

  /**
   * Valida formato de e-mail
   */
  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  /**
   * Volta para o login
   */
  backToLogin(): void {
    this.router.navigate(['/login']);
  }

  /**
   * Tenta novamente (limpa o formulário)
   */
  tryAgain(): void {
    this.email = '';
    this.emailSent.set(false);
    this.successMessage.set('');
    this.errorMessage.set('');
  }
}

